using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
namespace prjEject
{
  public class DevicesList : IEnumerable<Device>, IDisposable
  {
    //contient l'imagelist des icones des classes de périphériques
    private struct SP_CLASSIMAGELIST_DATA
    {
      public int cbSize;
      public IntPtr ImageList;
      public int Reserved;
    }
    private SP_CLASSIMAGELIST_DATA structClassesImageList;
    private List<DeviceInterface> m_VolumeInterfaces;
    private List<DeviceInterface> m_DiskInterfaces;
    private ImageList m_ImageList;

    public ImageList ImageList {
      get { return m_ImageList; }
    }
    public IEnumerable<DeviceInterface> VolumeInterfaces {
      get { return m_VolumeInterfaces; }
    }
    public IEnumerable<DeviceInterface> DiskInterfaces {
      get { return m_DiskInterfaces; }
    }

#region Image List
    //renvoie l'index dans l'imagelist de l'icone correspondant à la classe de périphérique
    [DllImport("setupapi.dll")]
    private static extern int SetupDiGetClassImageIndex(ref SP_CLASSIMAGELIST_DATA ClassImageListData, ref Guid ClassGuid, ref int ImageIndex);
    //renvoie un descripteur d'un imagelist contenant les icones des classes de périphériques
    [DllImport("setupapi.dll")]
    private static extern int SetupDiGetClassImageList(ref SP_CLASSIMAGELIST_DATA ClassImageListData);
    //detruit l'imagelist des icones de classes de périphériques
    [DllImport("setupapi.dll")]
    private static extern int SetupDiDestroyClassImageList(ref SP_CLASSIMAGELIST_DATA ClassImageListData);
    //renvoie le nombre d'icones dans un imagelist
    [DllImport("comctl32.dll")]
    private static extern int ImageList_GetImageCount(IntPtr himl);


    //dessine l'icone normalement
    private const int ILD_NORMAL = 0x0;
    //renvoie un HICON d'une icone de l'imagelist
    [DllImport("comctl32.dll")]
    private static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, int flags);
    //libère un HICON
    [DllImport("user32.dll")]
    private static extern int DestroyIcon(IntPtr hIcon);

    /// <summary>
    /// rempli un imagelist avec les icones des classes de périphériques
    /// </summary>
    /// <param name="container">structure contenant un descripteur d'un imagelist contenant les icones de classes de périphérique</param>
    /// <returns>ImageList à remplir (BackGround = 0x80000000)</returns>
    /// <remarks></remarks>
    private ImageList GetClassImageList(System.ComponentModel.IContainer container)
    {
      int numImage;
      int x;
      IntPtr hIcon;

      ImageList ret = new ImageList(container);

      ReadClassImageList();

      //la méthode suivante est la plus simple pour ajouter les icones des classes à l'Imagelist du treeview ou listview

      //on demande le nombre d'icones dans le ClassImageList
      numImage = ImageList_GetImageCount(structClassesImageList.ImageList);
      //pour chaque icone
      for (x = 0; x <= numImage - 1; x++) {
        //on demande un HICON de l'icone x
        hIcon = ImageList_GetIcon(structClassesImageList.ImageList, x, ILD_NORMAL);
        //on ajoute l'image du picturebox à l'imagelist
        ret.Images.Add(System.Drawing.Icon.FromHandle(hIcon));
        //on libère l'icone
        DestroyIcon(hIcon);
      }
      return ret;
    }

    /// <summary>
    /// renvoie l'imagelist contenant les icones de classes de périphériques
    /// </summary>
    /// <remarks></remarks>
    private void ReadClassImageList()
    {
      //enregistre la taille de la structure dans la structure
      structClassesImageList.cbSize = Marshal.SizeOf(structClassesImageList);
      //demande l'imagelist
      SetupDiGetClassImageList(ref structClassesImageList);
    }

    /// <summary>
    /// détruit un imagelist d'icones de classes de périphériques alloué avec GetImageList
    /// </summary>
    /// <remarks></remarks>
    private void DestroyClassImageList()
    {
      SetupDiDestroyClassImageList(ref structClassesImageList);
    }

    /// <summary>
    /// renvoie l'index d'une classe spécifiée dans l'imagelist contenant les icones des classes de périphériques
    /// </summary>
    /// <param name="ClassGuid">GUID de la classe pour laquelle on doit trouver l'index</param>
    /// <returns>structure contenant un descripteur d'un imagelist contenant les icones de classes de périphérique</returns>
    /// <remarks></remarks>
    internal int GetClassImageListIndex(Guid ClassGuid)
    {
      int ret = 0;
      //demande l'index de la classe spécifiée
      SetupDiGetClassImageIndex(ref structClassesImageList, ref ClassGuid, ref ret);
      return ret;
    }
#endregion

#region Devices
    //permet d'enumérer les Devices présent sur le système
    [DllImport("setupapi.dll")]
    private static extern int SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref prjEject.Device.SP_DEVINFO_DATA DeviceInfoData);
    //permet de supprimer le buffer alloué par l'api précédente
    [DllImport("setupapi.dll")]
    private static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
    //permet de trouver tous les périphériques
    [DllImport("setupapi.dll", EntryPoint = "SetupDiGetClassDevsA")]
    private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr HwndParent, int flags);
    //constante pour SetupDiGetClassDevs
    private const short DIGCF_DEFAULT = 0x1;
    private const short DIGCF_PRESENT = 0x2;
    private const short DIGCF_ALLCLASSES = 0x4;
    private const short DIGCF_PROFILE = 0x8;
    private const short DIGCF_DEVICEINTERFACE = 0x10;

    private System.Collections.Generic.List<Device> m_Devices = new System.Collections.Generic.List<Device>();
    public IEnumerable<Device> Devices {
      get { return this.m_Devices; }
    }

    /// <summary>
    /// énumère la liste de tous les devices présents sur le système
    /// </summary>
    /// <remarks></remarks>
    public void EnumDevices()
    {
      int dwInstance;
      IntPtr hDevInfo;
      Device.SP_DEVINFO_DATA DevInfo = new Device.SP_DEVINFO_DATA();

      this.m_VolumeInterfaces = this.GetInterfaceData(InterfaceGUID.GUID_DEVINTERFACE_VOLUME);
      this.m_DiskInterfaces = this.GetInterfaceData(InterfaceGUID.GUID_DEVINTERFACE_DISK);
      this.m_Devices.Clear();

      //on demande un Device Information Set
      Guid g = Guid.Empty;
      hDevInfo = SetupDiGetClassDevs(ref g, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);

      //on enregistre la taille de la structure dans la structure
      DevInfo.cbSize = Marshal.SizeOf(DevInfo);

      //si erreur dans demande de Device Information Set on quitte la fonction
      if (hDevInfo == new IntPtr(-1)) {
        return;
      }

      dwInstance = 0;
      do {
        //on enumére tous les devices
        if (SetupDiEnumDeviceInfo(hDevInfo, dwInstance, ref DevInfo) == 0) {
          //si erreur ou plus de devices
          //on libère le Device Information Set
          SetupDiDestroyDeviceInfoList(hDevInfo);
          // TODO: might not be correct. Was : Exit Do
          break; // TODO: might not be correct. Was : Exit Do
        }

        //on ajoute les infos à la liste
        //si NT/2000/XP...
        if (Device.IsWindowsNT() == true) {
          //on utilise les fonctions SetupDiXXX
          //si 9x/ME
          this.m_Devices.Add(new Device(this, hDevInfo, DevInfo));
        } else {
          //on utilise les fonctions CM_XXX
          this.m_Devices.Add(new Device(this, DevInfo.DevInst));
        }
        dwInstance += 1;
      } while (true);

      foreach (Device dev in this.Devices) {
        dev.EnumRelations();
      }

      this.EnumRemovableDevices();
    }
#endregion

#region IEnumerable
    System.Collections.Generic.IEnumerator<Device> System.Collections.Generic.IEnumerable<Device>.GetEnumerator()
    {
      return this.Devices.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.Devices.GetEnumerator();
    }
#endregion

    public DevicesList(System.ComponentModel.IContainer container)
    {
      this.ReadClassImageList();
      this.EnumDevices();
      this.m_ImageList = this.GetClassImageList(container);
    }

    private bool disposedValue = false;

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposedValue) {
        // TODO: free managed resources when explicitly called
        if (disposing) {
        }

        // TODO: free shared unmanaged resources
        DestroyClassImageList();
      }
      this.disposedValue = true;
    }

#region IDisposable Support
    // This code added by Visual Basic to correctly implement the disposable pattern.
    void IDisposable.Dispose()
    {
      // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Dispose(true);
      GC.SuppressFinalize(this);
    }
#endregion

#region Device ID
    //renvoie une chaine contenant l'ID du device
    [DllImport("cfgmgr32.dll")]
    private static extern int CM_Get_Device_ID(int dnDevInst, IntPtr pBuffer, int BufferLen, int ulFlags);
    [DllImport("cfgmgr32.dll", SetLastError = true)]
    private static extern int CM_Get_Device_ID_Size(ref int pulLen, int dnDevInst, int ulFlags);
    //renvoie une liste d'ID de device
    [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_Device_ID_ListA")]
    private static extern int CM_Get_Device_ID_List(string pszFilter, IntPtr pBuffer, int BufferLen, int ulFlags);
    //renvoie la taille de la liste renvoyé par CM_Get_Device_ID_List
    [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_Device_ID_List_SizeA")]
    private static extern int CM_Get_Device_ID_List_Size(ref int pulLen, string pszFilter, int ulFlags);

    public enum CM_GETIDLIST_FILTERS
    {
      CM_GETIDLIST_FILTER_NONE = (0x0),
      CM_GETIDLIST_FILTER_ENUMERATOR = (0x1),
      CM_GETIDLIST_FILTER_SERVICE = (0x2),
      CM_GETIDLIST_FILTER_EJECTRELATIONS = (0x4),
      CM_GETIDLIST_FILTER_REMOVALRELATIONS = (0x8),
      CM_GETIDLIST_FILTER_POWERRELATIONS = (0x10),
      CM_GETIDLIST_FILTER_BUSRELATIONS = (0x20),
      CM_GETIDLIST_DONOTGENERATE = (0x10000040),
      CM_GETIDLIST_FILTER_BITS = (0x1000007f)
    }

    /// <summary>
    /// récupère de DeviceId à partir d'un handle de device
    /// </summary>
    /// <param name="devInst"></param>
    /// <returns></returns>
    public static string GetDeviceId(int devInst)
    {
      int deviceIdLen = 0;
      CM_Get_Device_ID_Size(ref deviceIdLen, devInst, 0);
      deviceIdLen += 1;
      IntPtr buffDeviceId = Marshal.AllocHGlobal(deviceIdLen);
      CM_Get_Device_ID(devInst, buffDeviceId, deviceIdLen, 0);
      string deviceId = Marshal.PtrToStringAnsi(buffDeviceId);
      Marshal.FreeHGlobal(buffDeviceId);

      return deviceId;
    }

    /// <summary>
    /// fonction permettant de renvoyer la liste des IDs de device qui ont une relation donnée (Eject, removal...) avec le device donné
    /// </summary>
    /// <returns>renvoie un tableau de chaines de caractere contenant chacune un ID</returns>
    /// <remarks></remarks>
    public static List<string> GetDeviceIDList(string deviceId, CM_GETIDLIST_FILTERS filter)
    {
      int x;
      int uLen = 0;
      int Incr;
      IntPtr buff;
      string str1;
      List<string> tmp = new List<string>();

      //on demande la longueur de la liste des IDs
      CM_Get_Device_ID_List_Size(ref uLen, deviceId, (int)filter);
      //on redimmensionne le buffer à la taille de la liste
      buff = Marshal.AllocHGlobal(uLen);
      //on demande la liste des IDs
      CM_Get_Device_ID_List(deviceId, buff, uLen, (int)filter);
      //on prépare les compteurs pour l'énumération
      x = 0;
      Incr = 0;
      //le buffer contient des chaines de caracteres ascii terminée par un NULL + un NULL pour indiquer la fin de la liste
      IntPtr ptr = buff;
      while (!(uLen == 0)) {
        str1 = Marshal.PtrToStringAnsi(ptr);
        //on ajoute l'ID à la liste
        if (!string.IsNullOrEmpty(str1)) {
          tmp.Add(str1);
        }
        //on passe au suivant
        ptr = new IntPtr(ptr.ToInt32() + str1.Length + 1);
        uLen -= str1.Length + 1;
      }
      Marshal.FreeHGlobal(buff);
      //on renvoie la liste
      return tmp;
    }
    /// <summary>
    /// fonction permettant de renvoyer la liste des IDs de device
    /// </summary>
    /// <returns>renvoie un tableau de chaines de caractere contenant chacune un ID</returns>
    /// <remarks></remarks>
    public static List<string> GetDeviceIDList()
    {
      int x;
      int uLen = 0;
      int Incr;
      IntPtr buff;
      string str1;
      List<string> tmp = new List<string>();

      //on demande la longueur de la liste des IDs
      CM_Get_Device_ID_List_Size(ref uLen, null, 0);
      //on redimmensionne le buffer à la taille de la liste
      buff = Marshal.AllocHGlobal(uLen);
      //on demande la liste des IDs
      CM_Get_Device_ID_List(null, buff, uLen, 0);
      //on prépare les compteurs pour l'énumération
      x = 0;
      Incr = 0;
      //le buffer contient des chaines de caracteres ascii terminée par un NULL + un NULL pour indiquer la fin de la liste
      IntPtr ptr = buff;
      while (!(uLen == 0)) {
        str1 = Marshal.PtrToStringAnsi(ptr);
        //on ajoute l'ID à la liste
        tmp.Add(str1);
        //on passe au suivant
        ptr = new IntPtr(ptr.ToInt32() + str1.Length + 1);
        uLen -= str1.Length + 1;
      }
      Marshal.FreeHGlobal(buff);
      //on renvoie la liste
      return tmp;
    }
#endregion

#region Device Interfaces
    //structure permettant de contenir des informations sur les interfaces exposées par le pilote
    public struct DeviceInterface
    {
      public Guid InterfaceClassGuid;
      public int flags;
      public string DevicePath;
      public Device DevInfo;
    }
    //structure permettant de contenir le nom du l'interface accesible par les fonctions Win32 comme CreateFile
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
      public int cbSize;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
      public string DevicePath;
    }
    //permet de stocker des informations sur une interface exposée par un pilote
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SP_DEVICE_INTERFACE_DATA
    {
      public int cbSize;
      public Guid InterfaceClassGuid;
      public int flags;
      public IntPtr Reserved;
    }
    //enumére les interfaces disponibles
    public enum InterfaceGUID
    {
      GUID_DEVINTERFACE_DISK,
      GUID_DEVINTERFACE_CDROM,
      GUID_DEVINTERFACE_PARTITION,
      GUID_DEVINTERFACE_TAPE,
      GUID_DEVINTERFACE_WRITEONCEDISK,
      GUID_DEVINTERFACE_VOLUME,
      GUID_DEVINTERFACE_MEDIUMCHANGER,
      GUID_DEVINTERFACE_FLOPPY,
      GUID_DEVINTERFACE_CDCHANGER,
      GUID_DEVINTERFACE_STORAGEPORT,
      GUID_DEVINTERFACE_COMPORT,
      GUID_DEVINTERFACE_SERENUM_BUS_ENUMERATOR
    }
    //enumére  les interfaces exposées par un device
    [DllImport("setupapi.dll")]
    private static extern int SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, int DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);
    //permet d'obtenir des informations détaillées sur une interface exposée par un pilote
    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceInterfaceDetailW")]
    private static extern int SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);
    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceInterfaceDetailW")]
    private static extern int SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);
    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceInterfaceDetailW")]
    private static extern int SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, ref prjEject.Device.SP_DEVINFO_DATA DeviceInfoData);

    /// <summary>
    /// renvoie les interfaces du pilote spécifé
    /// </summary>
    /// <param name="InterGUID">GUID de classe de l'interface du device à enumémrer</param>
    /// <returns>renvoie une structure contenant des infos sur l'interface du device</returns>
    /// <remarks></remarks>
    public List<DeviceInterface> GetInterfaceData(InterfaceGUID InterGUID)
    {
      SP_DEVICE_INTERFACE_DATA interfaceData = new SP_DEVICE_INTERFACE_DATA();
      SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
      Device.SP_DEVINFO_DATA DevInfo = new Device.SP_DEVINFO_DATA();
      int status;
      int Index = 0;
      IntPtr hIntDevInfo;
      int interfaceDetailDataSize;
      int reqSize = 0;
      Guid ID;
      List<DeviceInterface> tmp = new List<DeviceInterface>();

      ID = GetInterfaceGUID(InterGUID);

      //on demande un Device Information Set
      hIntDevInfo = SetupDiGetClassDevs(ref ID, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

      //si erreur on quitte
      if (hIntDevInfo == new IntPtr(-1)) {
        return tmp;
      }

      //on enregistre la taille de la structure dans la structure
      DevInfo.cbSize = Marshal.SizeOf(DevInfo);
      interfaceData.cbSize = Marshal.SizeOf(interfaceData);

      do {
        //on enumère les interfaces
        status = SetupDiEnumDeviceInterfaces(hIntDevInfo, 0, ref ID, Index, ref interfaceData);

        if (status == 0) {
          //on libère le Device Information Set
          SetupDiDestroyDeviceInfoList(hIntDevInfo);
          //on quitte
          // TODO: might not be correct. Was : Exit Do
          break; // TODO: might not be correct. Was : Exit Do
        }
        //
        // Find out required buffer size, so pass NULL
        //

        status = SetupDiGetDeviceInterfaceDetail(hIntDevInfo, ref interfaceData, IntPtr.Zero, 0, ref reqSize, IntPtr.Zero);

        //
        // This call returns ERROR_INSUFFICIENT_BUFFER with reqSize
        // set to the required buffer size. Ignore the above error and
        // pass a bigger buffer to get the detail data
        //

        if (reqSize == 0) {
          SetupDiDestroyDeviceInfoList(hIntDevInfo);
          // TODO: might not be correct. Was : Exit Do
          break; // TODO: might not be correct. Was : Exit Do
        }

        interfaceDetailDataSize = reqSize;
        interfaceDetailData.cbSize = IntPtr.Size == 8 ? 8 : 6;

        //on demande des infos détaillées sur l'interface
        status = SetupDiGetDeviceInterfaceDetail(hIntDevInfo, ref interfaceData, ref interfaceDetailData, interfaceDetailDataSize, ref reqSize, ref DevInfo);

        if (status == 0) {
          //si erreur
          //on libère le Device Information Set
          SetupDiDestroyDeviceInfoList(hIntDevInfo);
          //on quitte
          // TODO: might not be correct. Was : Exit Do
          break; // TODO: might not be correct. Was : Exit Do
        }
        //on ajoute une entrée à la liste
        DeviceInterface devi = new DeviceInterface();
        //on ajoute les infos à la liste
        devi.DevicePath = interfaceDetailData.DevicePath;
        devi.flags = interfaceData.flags;
        devi.InterfaceClassGuid = interfaceData.InterfaceClassGuid;
        devi.DevInfo = new Device(this, hIntDevInfo, DevInfo);


        tmp.Add(devi);

        //on passe au suivant
        Index = Index + 1;
      } while (true);
      //on renvoie la liste
      return tmp;
    }

    /// <summary>
    /// renvoie le GUID de la classe d'interface désirée
    /// </summary>
    /// <param name="Interf">classe de l'interface</param>
    /// <returns>renvoie le GUID correspondant</returns>
    /// <remarks></remarks>
    private Guid GetInterfaceGUID(InterfaceGUID Interf)
    {
      switch (Interf) {
        case InterfaceGUID.GUID_DEVINTERFACE_DISK:
          return new Guid(0x53f56307, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_CDROM:
          return new Guid(0x53f56308, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_PARTITION:
          return new Guid(0x53f5630a, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_TAPE:
          return new Guid(0x53f5630b, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_WRITEONCEDISK:
          return new Guid(0x53f5630c, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_VOLUME:
          return new Guid(0x53f5630d, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_MEDIUMCHANGER:
          return new Guid(0x53f56310, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_FLOPPY:
          return new Guid(0x53f56311, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_CDCHANGER:
          return new Guid(0x53f56312, 0xb6bf, 0x11d0, 0x94, 0xf2, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_STORAGEPORT:
          return new Guid(0x2accfe60, 0xc130, 0x11d2, 0xb0, 0x82, 0x0, 0xa0, 0xc9, 0x1e, 0xfb,
          0x8b);
        case InterfaceGUID.GUID_DEVINTERFACE_COMPORT:
          return new Guid(0x86e0d1e0u, 0x8089, 0x11d0, 0x9c, 0xe4, 0x8, 0x0, 0x3e, 0x30, 0x1f,
          0x73);
        case InterfaceGUID.GUID_DEVINTERFACE_SERENUM_BUS_ENUMERATOR:
          return new Guid(0x4d36e978, 0xe325, 0x11ce, 0xbf, 0xc1, 0x8, 0x0, 0x2b, 0xe1, 0x3,
          0x18);
        default:
          return Guid.Empty;
      }
    }
#endregion

    private List<RemovableDevice> m_RemovableDevices = new List<RemovableDevice>();
    public IEnumerable<RemovableDevice> RemovableDevices {
      get { return this.m_RemovableDevices; }
    }
    private void EnumRemovableDevices()
    {
      this.m_RemovableDevices.Clear();
      foreach (var d in this.m_Devices) {
        if ((d.CAPABILITIES & Device.DevCapabilities.CM_DEVCAP_REMOVABLE) == Device.DevCapabilities.CM_DEVCAP_REMOVABLE) {
          m_RemovableDevices.Add(new RemovableDevice(d));
        }
      }
    }
  }
}

