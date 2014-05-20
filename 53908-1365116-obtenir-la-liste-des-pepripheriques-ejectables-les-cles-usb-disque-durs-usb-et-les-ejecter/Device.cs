using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
namespace prjEject
{
  public class Device
  {
    private DevicesList m_DevicesList;
    private static Dictionary<string, string> s_MountedDevices;

    //permet de supprimer le buffer alloué par l'api précédente
    [DllImport("setupapi.dll")]
    private static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    //permet de trouver le nom de périphérique interne d'une lettre de lecteur
    [DllImport("KERNEL32.dll", EntryPoint = "QueryDosDeviceA")]
    private static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

    //renvoie un handle vers la clé du device
    [DllImport("setupapi.dll")]
    private static extern IntPtr SetupDiOpenDevRegKey(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, int Scope, int HwProfile, int KeyType, int samDesired);

    //constante pour SetupDiOpenDevRegKey
    private const int DICS_FLAG_GLOBAL = 0x1;
    private const int DIREG_DEV = 0x1;
    private const int KEY_QUERY_VALUE = 0x1;

    //fonctions d'acces au registre
    //ferme une clé du registre
    [DllImport("advapi32.dll")]
    private static extern int RegCloseKey(IntPtr hKey);
    //renvoie la valeur de la sous-clé
    [DllImport("advapi32.dll", EntryPoint = "RegQueryValueExA")]
    private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, ref int lpType, IntPtr lpData, ref int lpcbData);
    [DllImport("advapi32.dll", EntryPoint = "RegQueryValueExA")]
    private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, ref int lpType, StringBuilder lpData, ref int lpcbData);

    //ouvre la clé de registre du périphérique
    [DllImport("cfgmgr32.dll")]
    private static extern int CM_Open_DevNode_Key(int dnDevNode, int samDesired, int ulHardwareProfile, int Disposition, ref IntPtr phkDevice, int ulFlags);

    //constantes pour CM_Open_DevNode_Key
    private const int CM_REGISTRY_HARDWARE = 0;
    private const int OPEN_EXISTING = 3;

    private const int READ_CONTROL = 0x20000;
    private const int STANDARD_RIGHTS_READ = (READ_CONTROL);
    private const int KEY_ENUMERATE_SUB_KEYS = 0x8;
    private const int KEY_NOTIFY = 0x10;
    private const int SYNCHRONIZE = 0x100000;
    private const int KEY_READ = ((STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY) & (~SYNCHRONIZE));

    //renvoie le contenu d'un paramètre utilisateur
    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetCustomDevicePropertyW")]
    private static extern int SetupDiGetCustomDeviceProperty(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, string CustomPropertyName, int flags, ref int PropertyRegDataType, ref int PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);
    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetCustomDevicePropertyW")]
    private static extern int SetupDiGetCustomDeviceProperty(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, string CustomPropertyName, int flags, ref int PropertyRegDataType, IntPtr PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);
    [DllImport("setupapi.dll", CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetCustomDevicePropertyW")]
    private static extern int SetupDiGetCustomDeviceProperty(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, string CustomPropertyName, int flags, ref int PropertyRegDataType, StringBuilder PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);

    //permet de lire les propriétés d'un device
    [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryPropertyW")]
    private static extern int SetupDiGetDeviceRegistryProperty(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, int PropertyName, ref int PropertyRegDataType, ref int PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);
    [DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceRegistryPropertyW")]
    private static extern int SetupDiGetDeviceRegistryProperty(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, int PropertyName, ref int PropertyRegDataType, [MarshalAs(UnmanagedType.LPWStr)]
StringBuilder PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);

    //constante pour Property de SetupDiGetDeviceRegistryProperty
    private const int SPDRP_ADDRESS = (0x1c);
    private const int SPDRP_BUSNUMBER = (0x15);
    private const int SPDRP_BUSTYPEGUID = (0x13);
    private const int SPDRP_CAPABILITIES = (0xf);
    private const int SPDRP_CHARACTERISTICS = (0x1b);
    private const int SPDRP_CLASS = (0x7);
    private const int SPDRP_CLASSGUID = (0x8);
    private const int SPDRP_COMPATIBLEIDS = (0x2);
    private const int SPDRP_CONFIGFLAGS = (0xa);
    private const int SPDRP_DEVICEDESC = 0x0;
    private const int SPDRP_DEVTYPE = (0x19);
    private const int SPDRP_DRIVER = (0x9);
    private const int SPDRP_ENUMERATOR_NAME = (0x16);
    private const int SPDRP_EXCLUSIVE = (0x1a);
    private const int SPDRP_FRIENDLYNAME = (0xc);
    private const int SPDRP_HARDWAREID = (0x1);
    private const int SPDRP_LEGACYBUSTYPE = (0x14);
    private const int SPDRP_LOCATION_INFORMATION = (0xd);
    private const int SPDRP_LOWERFILTERS = (0x12);
    private const int SPDRP_MAXIMUM_PROPERTY = (0x1c);
    private const int SPDRP_MFG = (0xb);
    private const int SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = (0xe);
    private const int SPDRP_SECURITY = (0x17);
    private const int SPDRP_SECURITY_SDS = (0x18);
    private const int SPDRP_SERVICE = (0x4);
    private const int SPDRP_UI_NUMBER = (0x10);
    private const int SPDRP_UI_NUMBER_DESC_FORMAT = (0x1e);
    private const int SPDRP_UNUSED0 = (0x3);
    private const int SPDRP_UNUSED1 = (0x5);
    private const int SPDRP_UNUSED2 = (0x6);
    private const int SPDRP_UPPERFILTERS = (0x11);

    //permet de trouver tous les périphériques
    [DllImport("setupapi.dll", EntryPoint = "SetupDiGetClassDevsA")]
    private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, int Enumerator, IntPtr HwndParent, int flags);
    //constante pour SetupDiGetClassDevs
    private const short DIGCF_DEFAULT = 0x1;
    private const short DIGCF_PRESENT = 0x2;
    private const short DIGCF_ALLCLASSES = 0x4;
    private const short DIGCF_PROFILE = 0x8;
    private const short DIGCF_DEVICEINTERFACE = 0x10;

    //structure permettant de contenir des informations sur des périphériques
    public struct SP_DEVINFO_DATA
    {
      public int cbSize;
      public Guid ClassGuid;
      public int DevInst;
      public IntPtr Reserved;
    }

    [DllImport("cfgmgr32.dll")]
    private static extern int CM_Get_Parent(ref int pdnDevInst, int dnDevInst, int ulFlags);
    [DllImport("cfgmgr32.dll")]
    private static extern int CM_Get_Child(ref int pdnDevInst, int dnDevInst, int ulFlags);
    //renvoie un DevInstance d'un device de même niveau
    [DllImport("cfgmgr32.dll")]
    private static extern int CM_Get_Sibling(ref int pdnDevInst, int DevInst, int ulFlags);
    //renvoie les propriétés d'un device par son DevInstance
    [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_DevNode_Registry_PropertyA")]
    private static extern int CM_Get_DevNode_Registry_Property(int dnDevInst, int ulProperty, ref int pulRegDataType, ref int Buffer, ref int pulLength, int ulFlags);
    [DllImport("cfgmgr32.dll", EntryPoint = "CM_Get_DevNode_Registry_PropertyA")]
    private static extern int CM_Get_DevNode_Registry_Property(int dnDevInst, int ulProperty, ref int pulRegDataType, StringBuilder Buffer, ref int pulLength, int ulFlags);

    //constantes pour CM_Get_DevNode_Registry_Property
    private const int CM_DRP_DEVICEDESC = 0x1;
    private const int CM_DRP_HARDWAREID = 0x2;
    private const int CM_DRP_COMPATIBLEIDS = 0x3;
    private const int CM_DRP_UNUSED0 = 0x4;
    private const int CM_DRP_SERVICE = 0x5;
    private const int CM_DRP_UNUSED1 = 0x6;
    private const int CM_DRP_UNUSED2 = 0x7;
    private const int CM_DRP_CLASS = 0x8;
    private const int CM_DRP_CLASSGUID = 0x9;
    private const int CM_DRP_DRIVER = 0xa;
    private const int CM_DRP_CONFIGFLAGS = 0xb;
    private const int CM_DRP_MFG = 0xc;
    private const int CM_DRP_FRIENDLYNAME = 0xd;
    private const int CM_DRP_LOCATION_INFORMATION = 0xe;
    private const int CM_DRP_PHYSICAL_DEVICE_OBJECT_NAME = 0xf;
    private const int CM_DRP_CAPABILITIES = 0x10;
    private const int CM_DRP_UI_NUMBER = 0x11;
    private const int CM_DRP_UPPERFILTERS = 0x12;
    private const int CM_DRP_LOWERFILTERS = 0x13;
    private const int CM_DRP_BUSTYPEGUID = 0x14;
    private const int CM_DRP_LEGACYBUSTYPE = 0x15;
    private const int CM_DRP_BUSNUMBER = 0x16;
    private const int CM_DRP_ENUMERATOR_NAME = 0x17;
    private const int CM_DRP_SECURITY = 0x18;
    private const int CM_CRP_SECURITY = CM_DRP_SECURITY;
    private const int CM_DRP_SECURITY_SDS = 0x19;
    private const int CM_CRP_SECURITY_SDS = CM_DRP_SECURITY_SDS;
    private const int CM_DRP_DEVTYPE = 0x1a;
    private const int CM_CRP_DEVTYPE = CM_DRP_DEVTYPE;
    private const int CM_DRP_EXCLUSIVE = 0x1b;
    private const int CM_CRP_EXCLUSIVE = CM_DRP_EXCLUSIVE;
    private const int CM_DRP_CHARACTERISTICS = 0x1c;
    private const int CM_CRP_CHARACTERISTICS = CM_DRP_CHARACTERISTICS;
    private const int CM_DRP_ADDRESS = 0x1d;
    private const int CM_DRP_UI_NUMBER_DESC_FORMAT = 0x1e;
    private const int CM_DRP_DEVICE_POWER_DATA = 0x1f;
    private const int CM_DRP_REMOVAL_POLICY = 0x20;
    private const int CM_DRP_REMOVAL_POLICY_HW_DEFAULT = 0x21;
    private const int CM_DRP_REMOVAL_POLICY_OVERRIDE = 0x22;
    private const int CM_DRP_INSTALL_STATE = 0x23;
    private const int CM_DRP_MIN = 0x1;
    private const int CM_CRP_MIN = CM_DRP_MIN;
    private const int CM_DRP_MAX = 0x23;
    private const int CM_CRP_MAX = CM_DRP_MAX;
    //enumére les causes d'un refus d'ejection de device
    public enum PNP_VETO_TYPE
    {
      PNP_VetoTypeUnknown,
      PNP_VetoLegacyDevice,
      PNP_VetoPendingClose,
      PNP_VetoWindowsApp,
      PNP_VetoWindowsService,
      PNP_VetoOutstandingOpen,
      PNP_VetoDevice,
      PNP_VetoDriver,
      PNP_VetoIllegalDeviceRequest,
      PNP_VetoInsufficientPower,
      PNP_VetoNonDisableable,
      PNP_VetoLegacyDriver
    }

    //permet de demander l'ejection d'un device sous NT/2000/XP
    [DllImport("setupapi.dll", EntryPoint = "CM_Request_Device_EjectW")]
    private static extern int CM_Request_Device_Eject(int dnDevInst, ref PNP_VETO_TYPE pVetoType, IntPtr pszVetoName, int ulNameLength, int ulFlags);

    //permet de demander si l'ejection du device est supporté par celui-ci sous 9x
    [DllImport("cfgmgr32.dll")]
    private static extern int CM_Query_Remove_SubTree(int dnDevInst, int uFlags);
    //permet de demander l'ejection d'un device sous 9x
    [DllImport("cfgmgr32.dll")]
    private static extern int CM_Remove_SubTree(int dnDevInst, int uFlags);

    //constantes pour Capabilities
    [Flags()]
    public enum DevCapabilities
    {
      CM_DEVCAP_LOCKSUPPORTED = 0x1,
      CM_DEVCAP_EJECTSUPPORTED = 0x2,
      CM_DEVCAP_REMOVABLE = 0x4,
      CM_DEVCAP_DOCKDEVICE = 0x8,
      CM_DEVCAP_UNIQUEID = 0x10,
      CM_DEVCAP_SILENTINSTALL = 0x20,
      CM_DEVCAP_RAWDEVICEOK = 0x40,
      CM_DEVCAP_SURPRISEREMOVALOK = 0x80,
      CM_DEVCAP_HARDWAREDISABLED = 0x100,
      CM_DEVCAP_NONDYNAMIC = 0x200
    }

    //constantes pour ConfigFlags
    [Flags()]
    public enum DevConfig
    {
      CONFIGFLAG_DISABLED = 0x1,
      CONFIGFLAG_REMOVED = 0x2,
      CONFIGFLAG_MANUAL_INSTALL = 0x4,
      CONFIGFLAG_IGNORE_BOOT_LC = 0x8,
      CONFIGFLAG_NET_BOOT = 0x10,
      CONFIGFLAG_REINSTALL = 0x20,
      CONFIGFLAG_FAILEDINSTALL = 0x40,
      CONFIGFLAG_CANTSTOPACHILD = 0x80,
      CONFIGFLAG_OKREMOVEROM = 0x100,
      CONFIGFLAG_NOREMOVEEXIT = 0x200,
      CONFIGFLAG_FINISH_INSTALL = 0x400,
      CONFIGFLAG_NEEDS_FORCED_CONFIG = 0x800,
      CONFIGFLAG_NETBOOT_CARD = 0x1000,
      CONFIGFLAG_PARTIAL_LOG_CONF = 0x2000,
      CONFIGFLAG_SUPPRESS_SURPRISE = 0x4000,
      CONFIGFLAG_VERIFY_HARDWARE = 0x8000,
      CSCONFIGFLAG_BITS = 0x7,
      CSCONFIGFLAG_DISABLED = 0x1,
      CSCONFIGFLAG_DO_NOT_CREATE = 0x2,
      CSCONFIGFLAG_DO_NOT_START = 0x4
    }

    public enum DevFileType
    {
      FILE_DEVICE_8042_PORT = 0x27,
      FILE_DEVICE_ACPI = 0x32,
      FILE_DEVICE_BATTERY = 0x29,
      FILE_DEVICE_BEEP = 0x1,
      FILE_DEVICE_BUS_EXTENDER = 0x2a,
      FILE_DEVICE_CD_ROM = 0x2,
      FILE_DEVICE_CD_ROM_FILE_SYSTEM = 0x3,
      FILE_DEVICE_CHANGER = 0x30,
      FILE_DEVICE_CONTROLLER = 0x4,
      FILE_DEVICE_DATALINK = 0x5,
      FILE_DEVICE_DFS = 0x6,
      FILE_DEVICE_DFS_FILE_SYSTEM = 0x35,
      FILE_DEVICE_DFS_VOLUME = 0x36,
      FILE_DEVICE_DISK = 0x7,
      FILE_DEVICE_DISK_FILE_SYSTEM = 0x8,
      FILE_DEVICE_DVD = 0x33,
      FILE_DEVICE_FILE_SYSTEM = 0x9,
      FILE_DEVICE_FIPS = 0x3a,
      FILE_DEVICE_FULLSCREEN_VIDEO = 0x34,
      FILE_DEVICE_INPORT_PORT = 0xa,
      FILE_DEVICE_KEYBOARD = 0xb,
      FILE_DEVICE_KS = 0x2f,
      FILE_DEVICE_KSEC = 0x39,
      FILE_DEVICE_MAILSLOT = 0xc,
      FILE_DEVICE_MASS_STORAGE = 0x2d,
      FILE_DEVICE_MIDI_IN = 0xd,
      FILE_DEVICE_MIDI_OUT = 0xe,
      FILE_DEVICE_MODEM = 0x2b,
      FILE_DEVICE_MOUSE = 0xf,
      FILE_DEVICE_MULTI_UNC_PROVIDER = 0x10,
      FILE_DEVICE_NAMED_PIPE = 0x11,
      FILE_DEVICE_NETWORK = 0x12,
      FILE_DEVICE_NETWORK_BROWSER = 0x13,
      FILE_DEVICE_NETWORK_FILE_SYSTEM = 0x14,
      FILE_DEVICE_NETWORK_REDIRECTOR = 0x28,
      FILE_DEVICE_NULL = 0x15,
      FILE_DEVICE_PARALLEL_PORT = 0x16,
      FILE_DEVICE_PHYSICAL_NETCARD = 0x17,
      FILE_DEVICE_PRINTER = 0x18,
      FILE_DEVICE_SCANNER = 0x19,
      FILE_DEVICE_SCREEN = 0x1c,
      FILE_DEVICE_SERENUM = 0x37,
      FILE_DEVICE_SERIAL_MOUSE_PORT = 0x1a,
      FILE_DEVICE_SERIAL_PORT = 0x1b,
      FILE_DEVICE_SMARTCARD = 0x31,
      FILE_DEVICE_SMB = 0x2e,
      FILE_DEVICE_SOUND = 0x1d,
      FILE_DEVICE_STREAMS = 0x1e,
      FILE_DEVICE_TAPE = 0x1f,
      FILE_DEVICE_TAPE_FILE_SYSTEM = 0x20,
      FILE_DEVICE_TERMSRV = 0x38,
      FILE_DEVICE_TRANSPORT = 0x21,
      FILE_DEVICE_UNKNOWN = 0x22,
      FILE_DEVICE_VDM = 0x2c,
      FILE_DEVICE_VIDEO = 0x23,
      FILE_DEVICE_VIRTUAL_DISK = 0x24,
      FILE_DEVICE_WAVE_IN = 0x25,
      FILE_DEVICE_WAVE_OUT = 0x26
    }

    private List<Device> m_ChildDevices = new List<Device>();
    public IEnumerable<Device> ChildDevices {
      get { return m_ChildDevices; }
    }

    private int DevInst;
    public Guid ClassGuid;
    public int ADDRESS;
    public int BUSNUMBER;
    public Guid BUSTYPEGUID;
    public DevCapabilities CAPABILITIES;
    public int CHARACTERISTICS;
    public string DeviceCLASS;
    public string COMPATIBLEIDS;
    public DevConfig CONFIGFLAGS;
    public string DEVICEDESC;
    public DevFileType DevType;
    public string DRIVER;
    public string ENUMERATOR_NAME;
    public int Exclusive;
    public string FRIENDLYNAME;
    public string HARDWAREID;
    public string LOCATION_INFORMATION;
    public string LOWERFILTERS;
    public string MFG;
    public string PHYSICAL_DEVICE_OBJECT_NAME;
    public int REMOVAL_POLICY;
    public int REMOVAL_POLICY_HW_DEFAULT;
    public int REMOVAL_POLICY_OVERRIDE;
    public string SECURITY_SDS;
    public string SERVICE;
    public int UI_NUMBER;
    public string UI_NUMBER_DESC_FORMAT;
    public string UPPERFILTERS;
    private int m_ImageIndex;
    public int ImageIndex {
      get { return m_ImageIndex; }
    }
    private int GetDevicePropertyInteger(IntPtr hDevInfo, ref SP_DEVINFO_DATA DevInfoData, int iCustomPropertyName)
    {
      int ret = 0;
      int dwReqLen = 0;
      int regDataType = 0;
      SetupDiGetDeviceRegistryProperty(hDevInfo, ref DevInfoData, iCustomPropertyName, ref regDataType, ref ret, 4, ref dwReqLen);
      return ret;
    }
    private string GetDevicePropertyString(IntPtr hDevInfo, ref SP_DEVINFO_DATA DevInfoData, int iCustomPropertyName)
    {
      StringBuilder sbBuff;
      int dwReqLen = 0;
      int regDataType = 0;
      int dummy = 0;
      SetupDiGetDeviceRegistryProperty(hDevInfo, ref DevInfoData, iCustomPropertyName, ref regDataType, ref dummy, 0, ref dwReqLen);
      sbBuff = new StringBuilder(dwReqLen);
      SetupDiGetDeviceRegistryProperty(hDevInfo, ref DevInfoData, iCustomPropertyName, ref regDataType, sbBuff, dwReqLen, ref dwReqLen);
      return sbBuff.ToString();
    }
    private void ReadDeviceProperties(IntPtr hDevInfo, SP_DEVINFO_DATA DevInfoData)
    {
      this.DevInst = DevInfoData.DevInst;
      this.ADDRESS = GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_ADDRESS);
      this.BUSNUMBER = GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_BUSNUMBER);
      this.CAPABILITIES = (DevCapabilities)GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_CAPABILITIES);
      this.CHARACTERISTICS = GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_CHARACTERISTICS);
      this.DeviceCLASS = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_CLASS);
      var s = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_CLASSGUID);
      if (!string.IsNullOrEmpty(s))
        this.ClassGuid = new Guid(s);
      this.COMPATIBLEIDS = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_COMPATIBLEIDS);
      this.CONFIGFLAGS = (DevConfig)GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_CONFIGFLAGS);
      this.DEVICEDESC = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_DEVICEDESC);
      this.DevType = (DevFileType)GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_DEVTYPE);
      this.DRIVER = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_DRIVER);
      this.ENUMERATOR_NAME = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_ENUMERATOR_NAME);
      this.Exclusive = GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_EXCLUSIVE);
      this.FRIENDLYNAME = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_FRIENDLYNAME);
      this.HARDWAREID = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_HARDWAREID);
      this.LOCATION_INFORMATION = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_LOCATION_INFORMATION);
      this.LOWERFILTERS = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_LOWERFILTERS);
      this.MFG = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_MFG);
      this.PHYSICAL_DEVICE_OBJECT_NAME = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_PHYSICAL_DEVICE_OBJECT_NAME);
      this.SECURITY_SDS = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_SECURITY_SDS);
      this.SERVICE = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_SERVICE);
      this.UI_NUMBER = GetDevicePropertyInteger(hDevInfo, ref DevInfoData, SPDRP_UI_NUMBER);
      this.UI_NUMBER_DESC_FORMAT = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_UI_NUMBER_DESC_FORMAT);
      this.UPPERFILTERS = GetDevicePropertyString(hDevInfo, ref DevInfoData, SPDRP_UPPERFILTERS);
    }
    public string GetDeviceCapabilitiesString()
    {
      StringBuilder sb = new StringBuilder();
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_LOCKSUPPORTED) == DevCapabilities.CM_DEVCAP_LOCKSUPPORTED) {
        sb.Append("Lock Supported - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_EJECTSUPPORTED) == DevCapabilities.CM_DEVCAP_EJECTSUPPORTED) {
        sb.Append("Eject Supported - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_REMOVABLE) == DevCapabilities.CM_DEVCAP_REMOVABLE) {
        sb.Append("Removable - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_DOCKDEVICE) == DevCapabilities.CM_DEVCAP_DOCKDEVICE) {
        sb.Append("Dock Device - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_UNIQUEID) == DevCapabilities.CM_DEVCAP_UNIQUEID) {
        sb.Append("Unique ID - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_SILENTINSTALL) == DevCapabilities.CM_DEVCAP_SILENTINSTALL) {
        sb.Append("Silent Install - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_RAWDEVICEOK) == DevCapabilities.CM_DEVCAP_RAWDEVICEOK) {
        sb.Append("Raw Device OK - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_SURPRISEREMOVALOK) == DevCapabilities.CM_DEVCAP_SURPRISEREMOVALOK) {
        sb.Append("Surprise Removal OK - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_HARDWAREDISABLED) == DevCapabilities.CM_DEVCAP_HARDWAREDISABLED) {
        sb.Append("Hardware Disabled - ");
      }
      if ((this.CAPABILITIES & DevCapabilities.CM_DEVCAP_NONDYNAMIC) == DevCapabilities.CM_DEVCAP_NONDYNAMIC) {
        sb.Append("Non dynamic - ");
      }
      return sb.ToString();
    }
    public string GetDeviceConfigFlagString()
    {
      StringBuilder sb = new StringBuilder();
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_DISABLED) == DevConfig.CONFIGFLAG_DISABLED) {
        sb.Append("Disabled - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_REMOVED) == DevConfig.CONFIGFLAG_REMOVED) {
        sb.Append("Removed - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_MANUAL_INSTALL) == DevConfig.CONFIGFLAG_MANUAL_INSTALL) {
        sb.Append("Manual Install - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_IGNORE_BOOT_LC) == DevConfig.CONFIGFLAG_IGNORE_BOOT_LC) {
        sb.Append("Ignore Boot LC - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_NET_BOOT) == DevConfig.CONFIGFLAG_NET_BOOT) {
        sb.Append("NET Boot - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_REINSTALL) == DevConfig.CONFIGFLAG_REINSTALL) {
        sb.Append("Reinstall - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_FAILEDINSTALL) == DevConfig.CONFIGFLAG_FAILEDINSTALL) {
        sb.Append("Failed Install - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_CANTSTOPACHILD) == DevConfig.CONFIGFLAG_CANTSTOPACHILD) {
        sb.Append("Can't stop a child - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_OKREMOVEROM) == DevConfig.CONFIGFLAG_OKREMOVEROM) {
        sb.Append("OK remove ROM - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_NOREMOVEEXIT) == DevConfig.CONFIGFLAG_NOREMOVEEXIT) {
        sb.Append("No Remove Exit - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_FINISH_INSTALL) == DevConfig.CONFIGFLAG_FINISH_INSTALL) {
        sb.Append("Finish Install - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_NEEDS_FORCED_CONFIG) == DevConfig.CONFIGFLAG_NEEDS_FORCED_CONFIG) {
        sb.Append("Need Forced Config - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_NETBOOT_CARD) == DevConfig.CONFIGFLAG_NETBOOT_CARD) {
        sb.Append("NetBoot Card - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_PARTIAL_LOG_CONF) == DevConfig.CONFIGFLAG_PARTIAL_LOG_CONF) {
        sb.Append("Partial Log Config - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_SUPPRESS_SURPRISE) == DevConfig.CONFIGFLAG_SUPPRESS_SURPRISE) {
        sb.Append("Suppress Surprise - ");
      }
      if ((this.CONFIGFLAGS & DevConfig.CONFIGFLAG_VERIFY_HARDWARE) == DevConfig.CONFIGFLAG_VERIFY_HARDWARE) {
        sb.Append("Verify Hardware - ");
      }
      return sb.ToString();
    }
    public string GetDeviceTypeString()
    {
      switch (this.DevType) {
        case DevFileType.FILE_DEVICE_8042_PORT:
          return "8042 Port";
        case DevFileType.FILE_DEVICE_ACPI:
          return "Acpi";
        case DevFileType.FILE_DEVICE_BATTERY:
          return "Battery";
        case DevFileType.FILE_DEVICE_BEEP:
          return "Beep";
        case DevFileType.FILE_DEVICE_BUS_EXTENDER:
          return "Bus Extender";
        case DevFileType.FILE_DEVICE_CD_ROM:
          return "CDROM";
        case DevFileType.FILE_DEVICE_CD_ROM_FILE_SYSTEM:
          return "CDROM File System";
        case DevFileType.FILE_DEVICE_CHANGER:
          return "Changer";
        case DevFileType.FILE_DEVICE_CONTROLLER:
          return "Controller";
        case DevFileType.FILE_DEVICE_DATALINK:
          return "DataLink";
        case DevFileType.FILE_DEVICE_DFS:
          return "DFS";
        case DevFileType.FILE_DEVICE_DFS_FILE_SYSTEM:
          return "DFS File System";
        case DevFileType.FILE_DEVICE_DFS_VOLUME:
          return "DFS Volume";
        case DevFileType.FILE_DEVICE_DISK:
          return "Disk";
        case DevFileType.FILE_DEVICE_DISK_FILE_SYSTEM:
          return "Disk File System";
        case DevFileType.FILE_DEVICE_DVD:
          return "DVD";
        case DevFileType.FILE_DEVICE_FILE_SYSTEM:
          return "File System";
        case DevFileType.FILE_DEVICE_FIPS:
          return "FIPS";
        case DevFileType.FILE_DEVICE_FULLSCREEN_VIDEO:
          return "FullScreen Video";
        case DevFileType.FILE_DEVICE_INPORT_PORT:
          return "InPort Port";
        case DevFileType.FILE_DEVICE_KEYBOARD:
          return "Keyboard";
        case DevFileType.FILE_DEVICE_KS:
          return "KS";
        case DevFileType.FILE_DEVICE_KSEC:
          return "KSEC";
        case DevFileType.FILE_DEVICE_MAILSLOT:
          return "Mailslot";
        case DevFileType.FILE_DEVICE_MASS_STORAGE:
          return "Mass Storage";
        case DevFileType.FILE_DEVICE_MIDI_IN:
          return "MIDI In";
        case DevFileType.FILE_DEVICE_MIDI_OUT:
          return "MIDI Out";
        case DevFileType.FILE_DEVICE_MODEM:
          return "Modem";
        case DevFileType.FILE_DEVICE_MOUSE:
          return "Mouse";
        case DevFileType.FILE_DEVICE_MULTI_UNC_PROVIDER:
          return "Multi UNC Provider";
        case DevFileType.FILE_DEVICE_NAMED_PIPE:
          return "Named Pipe";
        case DevFileType.FILE_DEVICE_NETWORK:
          return "Network";
        case DevFileType.FILE_DEVICE_NETWORK_BROWSER:
          return "Network Browser";
        case DevFileType.FILE_DEVICE_NETWORK_FILE_SYSTEM:
          return "Network File System";
        case DevFileType.FILE_DEVICE_NETWORK_REDIRECTOR:
          return "Network Redirector";
        case DevFileType.FILE_DEVICE_NULL:
          return "Null";
        case DevFileType.FILE_DEVICE_PARALLEL_PORT:
          return "Parallel Port";
        case DevFileType.FILE_DEVICE_PHYSICAL_NETCARD:
          return "Physical Netcard";
        case DevFileType.FILE_DEVICE_PRINTER:
          return "Printer";
        case DevFileType.FILE_DEVICE_SCANNER:
          return "Scanner";
        case DevFileType.FILE_DEVICE_SCREEN:
          return "Screen";
        case DevFileType.FILE_DEVICE_SERENUM:
          return "Serenum";
        case DevFileType.FILE_DEVICE_SERIAL_MOUSE_PORT:
          return "Serial Mouse Port";
        case DevFileType.FILE_DEVICE_SERIAL_PORT:
          return "Serial Port";
        case DevFileType.FILE_DEVICE_SMARTCARD:
          return "Smart Card";
        case DevFileType.FILE_DEVICE_SMB:
          return "SMB";
        case DevFileType.FILE_DEVICE_SOUND:
          return "Sound";
        case DevFileType.FILE_DEVICE_STREAMS:
          return "Streams";
        case DevFileType.FILE_DEVICE_TAPE:
          return "Tape";
        case DevFileType.FILE_DEVICE_TAPE_FILE_SYSTEM:
          return "Tape File System";
        case DevFileType.FILE_DEVICE_TERMSRV:
          return "Terminal Server";
        case DevFileType.FILE_DEVICE_TRANSPORT:
          return "Transport";
        case DevFileType.FILE_DEVICE_UNKNOWN:
          return "Unknown";
        case DevFileType.FILE_DEVICE_VDM:
          return "VDM";
        case DevFileType.FILE_DEVICE_VIDEO:
          return "VIDEO";
        case DevFileType.FILE_DEVICE_VIRTUAL_DISK:
          return "Virtual Disk";
        case DevFileType.FILE_DEVICE_WAVE_IN:
          return "Wave In";
        case DevFileType.FILE_DEVICE_WAVE_OUT:
          return "Wave Out";
        default:
          return string.Empty;
      }
    }

    public string DeviceId {
      get { return m_DeviceId; }
      private set { m_DeviceId = value; }
    }
    private string m_DeviceId;

    /// <summary>
    /// Enumère les devices qui ont une relations 'Removal' (enfants virtuels)
    /// </summary>
    public void EnumRelations()
    {
      bool bAdded = false;
      foreach (string devId in DevicesList.GetDeviceIDList(this.DeviceId, DevicesList.CM_GETIDLIST_FILTERS.CM_GETIDLIST_FILTER_REMOVALRELATIONS)) {
        if ((from dev in this.ChildDevices where dev.DeviceId.ToLower() == devId.ToLower() select dev).FirstOrDefault() == null) {
          this.m_ChildDevices.AddRange(from dev in this.m_DevicesList.Devices where dev.DeviceId.ToLower() == devId.ToLower() select dev);
          bAdded = true;
        }
      }

      if (!bAdded) {
        int status = 0;
        int pDevInstF = 0;
        List<int> l = new List<int>();
        status = CM_Get_Child(ref pDevInstF, DevInst, 0);
        if (status != 0 || pDevInstF == 0) {
          goto en;
        }
        l.Add(pDevInstF);
        do {
          status = CM_Get_Sibling(ref pDevInstF, pDevInstF, 0);
          if (status != 0 || pDevInstF == 0) {
            goto en;
          }
          l.Add(pDevInstF);
        } while (true);
        en:

        foreach (int d in l) {
          this.m_ChildDevices.AddRange(from dev in this.m_DevicesList.Devices where dev.DevInst == d select dev);
        }
      }

      this.m_ChildDevices = this.m_ChildDevices.Distinct().ToList();
      foreach (Device dev in this.ChildDevices) {
        dev.EnumRelations();
      }

      //'If Me.ChildDevices.Count = 0 Then
      //If True Then
      //    Dim pDevInstF As Integer = 0
      //    Dim status = CM_Get_Parent(pDevInstF, DevInst, 0)
      //    Dim parent = (From dev In Me.m_DevicesList.Devices Where dev.DevInst = pDevInstF Select dev).FirstOrDefault()
      //    If parent IsNot Nothing Then
      //        If Not parent.ChildDevices.Contains(Me) Then
      //            parent.ChildDevices.Add(Me)
      //        End If
      //    End If
      //End If
    }

    /// <summary>
    /// Enumère les devices enfants
    /// </summary>
    /// <param name="DevInst"></param>
    public void EnumChildDevices(int DevInst)
    {
      this.DeviceId = DevicesList.GetDeviceId(DevInst);

      int status = 0;
      int pDevInstF = 0;
      List<int> l = new List<int>();
      status = CM_Get_Child(ref pDevInstF, DevInst, 0);
      if (status != 0 || pDevInstF == 0) {
        goto en;
      }
      l.Add(pDevInstF);
      do {
        status = CM_Get_Sibling(ref pDevInstF, pDevInstF, 0);
        if (status != 0 || pDevInstF == 0) {
          goto en;
        }
        l.Add(pDevInstF);
      } while (true);
      en:

      foreach (int dev in l) {
        this.m_ChildDevices.Add(new Device(this.m_DevicesList, dev));
      }
    }
    public int EjectDevice(ref PNP_VETO_TYPE lVeto)
    {
      if (IsWindowsNT() == true) {
        return CM_Request_Device_Eject(this.DevInst, ref lVeto, IntPtr.Zero, 0, 0);
      } else {
        int ret = CM_Query_Remove_SubTree(this.DevInst, 0);
        if (ret == 0) {
          CM_Remove_SubTree(DevInst, 0);
        }
        return ret;
      }
    }
    private int CM_ReadDevicePropertyInteger(int DevInst, int iCustomPropertyName)
    {
      int ret = 0;
      int dwReqLen = 4;
      int regDataType = 0;
      CM_Get_DevNode_Registry_Property(DevInst, iCustomPropertyName, ref regDataType, ref ret, ref dwReqLen, 0);
      return ret;
    }
    private string CM_ReadDevicePropertiyString(int DevInst, int iCustomPropertyName)
    {
      StringBuilder sbBuff;
      int dwReqLen = 0;
      int regDataType = 0;
      dwReqLen = 0;
      int dummy = 0;
      CM_Get_DevNode_Registry_Property(DevInst, iCustomPropertyName, ref regDataType, ref dummy, ref dwReqLen, 0);
      sbBuff = new StringBuilder(dwReqLen);
      CM_Get_DevNode_Registry_Property(DevInst, iCustomPropertyName, ref regDataType, sbBuff, ref dwReqLen, 0);
      return sbBuff.ToString();
    }
    private void CM_ReadDeviceProperties(int DevInst)
    {
      this.DevInst = DevInst;
      this.ADDRESS = CM_ReadDevicePropertyInteger(DevInst, CM_DRP_ADDRESS);
      this.BUSNUMBER = CM_ReadDevicePropertyInteger(DevInst, CM_DRP_BUSNUMBER);
      this.CAPABILITIES = (DevCapabilities)CM_ReadDevicePropertyInteger(DevInst, CM_DRP_CAPABILITIES);
      this.CHARACTERISTICS = CM_ReadDevicePropertyInteger(DevInst, CM_DRP_CHARACTERISTICS);
      this.DeviceCLASS = CM_ReadDevicePropertiyString(DevInst, CM_DRP_CLASS);
      var s = CM_ReadDevicePropertiyString(DevInst, CM_DRP_CLASSGUID);
      if (!string.IsNullOrEmpty(s))
        this.ClassGuid = new Guid(s);
      this.COMPATIBLEIDS = CM_ReadDevicePropertiyString(DevInst, CM_DRP_COMPATIBLEIDS);
      this.CONFIGFLAGS = (DevConfig)CM_ReadDevicePropertyInteger(DevInst, CM_DRP_CONFIGFLAGS);
      this.DEVICEDESC = CM_ReadDevicePropertiyString(DevInst, CM_DRP_DEVICEDESC);
      this.DevType = (DevFileType)CM_ReadDevicePropertyInteger(DevInst, CM_DRP_DEVTYPE);
      this.DRIVER = CM_ReadDevicePropertiyString(DevInst, CM_DRP_DRIVER);
      this.ENUMERATOR_NAME = CM_ReadDevicePropertiyString(DevInst, CM_DRP_ENUMERATOR_NAME);
      this.Exclusive = CM_ReadDevicePropertyInteger(DevInst, CM_DRP_EXCLUSIVE);
      this.FRIENDLYNAME = CM_ReadDevicePropertiyString(DevInst, CM_DRP_FRIENDLYNAME);
      this.HARDWAREID = CM_ReadDevicePropertiyString(DevInst, CM_DRP_HARDWAREID);
      this.LOCATION_INFORMATION = CM_ReadDevicePropertiyString(DevInst, CM_DRP_LOCATION_INFORMATION);
      this.LOWERFILTERS = CM_ReadDevicePropertiyString(DevInst, CM_DRP_LOWERFILTERS);
      this.MFG = CM_ReadDevicePropertiyString(DevInst, CM_DRP_MFG);
      this.PHYSICAL_DEVICE_OBJECT_NAME = CM_ReadDevicePropertiyString(DevInst, CM_DRP_PHYSICAL_DEVICE_OBJECT_NAME);
      this.SECURITY_SDS = CM_ReadDevicePropertiyString(DevInst, CM_DRP_SECURITY_SDS);
      this.SERVICE = CM_ReadDevicePropertiyString(DevInst, CM_DRP_SERVICE);
      this.UI_NUMBER = CM_ReadDevicePropertyInteger(DevInst, CM_DRP_UI_NUMBER);
      this.UI_NUMBER_DESC_FORMAT = CM_ReadDevicePropertiyString(DevInst, CM_DRP_UI_NUMBER_DESC_FORMAT);
      this.UPPERFILTERS = CM_ReadDevicePropertiyString(DevInst, CM_DRP_UPPERFILTERS);
    }
    private int GetDeviceCustomProperties(SP_DEVINFO_DATA DeviceInfoData, string PropertyName, ref StringBuilder Buffer, ref int retBufferLen, ref int RegDataType)
    {
      IntPtr hDevInfoSet = IntPtr.Zero;
      int reqSize = 0;
      Guid dummy = Guid.Empty;
      hDevInfoSet = SetupDiGetClassDevs(ref dummy, 0, IntPtr.Zero, DIGCF_ALLCLASSES | DIGCF_PRESENT);
      if (hDevInfoSet == new IntPtr(-1)) {
        return Marshal.GetLastWin32Error();
      }
      SetupDiGetCustomDeviceProperty(hDevInfoSet, ref DeviceInfoData, PropertyName, 0, ref RegDataType, IntPtr.Zero, reqSize, ref retBufferLen);
      Buffer = new StringBuilder(retBufferLen);
      int ret = SetupDiGetCustomDeviceProperty(hDevInfoSet, ref DeviceInfoData, PropertyName, 0, ref RegDataType, Buffer, retBufferLen, ref retBufferLen);
      SetupDiDestroyDeviceInfoList(hDevInfoSet);
      return ret;
    }
    private string GetMSDOSDriveName(string lpReqDevice)
    {
      string lpDrive;
      StringBuilder lpDevice;
      int nDeviceLength;
      if (IsWindowsNT() == false) {
        lpReqDevice = lpReqDevice.ToUpper();
      }
      foreach (string letter in Environment.GetLogicalDrives()) {
        lpDrive = letter.Substring(0, 2);
        if (IsWindowsNT() == false) {
          lpDrive = lpDrive.ToUpper();
        }
        nDeviceLength = 256;
        lpDevice = new StringBuilder(nDeviceLength);
        QueryDosDevice(lpDrive, lpDevice, nDeviceLength);
        if (lpDevice.ToString() == lpReqDevice) {
          return lpDrive;
        }
      }
      foreach (DevicesList.DeviceInterface devint in this.m_DevicesList.VolumeInterfaces) {
        if (devint.DevInfo.PHYSICAL_DEVICE_OBJECT_NAME.Equals(lpReqDevice, StringComparison.InvariantCultureIgnoreCase)) {
          string k = devint.DevicePath.ToLower();
          k = "\\??\\" + k.Substring(4);
          if (s_MountedDevices.ContainsKey(k)) {
            return s_MountedDevices[k];
          }
        }
      }
      //For Each devint As DevicesList.DeviceInterface In Me.m_DevicesList.DiskInterfaces
      //    If devint.DevInfo.PHYSICAL_DEVICE_OBJECT_NAME.Equals(lpReqDevice, StringComparison.InvariantCultureIgnoreCase) Then
      //        Dim k As String = devint.DevicePath.ToLower()
      //        k = k.Substring(4)
      //        If s_MountedDevices.ContainsKey(k) Then
      //            Return s_MountedDevices(k)
      //        End If
      //    End If
      //Next
      return string.Empty;
    }
    private int GetDeviceCustomProp(IntPtr hDevInfoSet, SP_DEVINFO_DATA DeviceInfoData, string PropertyName, ref StringBuilder Buffer, ref int retBufferLen)
    {
      IntPtr lDeviceKey;
      int ret = 0;
      int lpType = 0;
      lDeviceKey = SetupDiOpenDevRegKey(hDevInfoSet, ref DeviceInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_QUERY_VALUE);
      retBufferLen = 0;
      ret = RegQueryValueEx(lDeviceKey, PropertyName, 0, ref lpType, IntPtr.Zero, ref retBufferLen);
      if (retBufferLen > 0) {
        Buffer = new StringBuilder(retBufferLen);
        ret = RegQueryValueEx(lDeviceKey, PropertyName, 0, ref lpType, Buffer, ref retBufferLen);
      }
      RegCloseKey(lDeviceKey);
      return ret;
    }
    private int GetDeviceCustomProp_old(int DevInst, string PropertyName, ref StringBuilder Buffer, ref int retBufferLen)
    {
      IntPtr lDeviceKey = IntPtr.Zero;
      int ret = 0;
      int lpType = 0;
      ret = CM_Open_DevNode_Key(DevInst, KEY_QUERY_VALUE, 0, 0, ref lDeviceKey, CM_REGISTRY_HARDWARE);
      if (ret > 0) {
        return ret;
      }
      retBufferLen = 0;
      ret = RegQueryValueEx(lDeviceKey, PropertyName, 0, ref lpType, IntPtr.Zero, ref retBufferLen);
      if (retBufferLen > 0) {
        Buffer = new StringBuilder(retBufferLen);
        int dummy = 0;
        ret = RegQueryValueEx(lDeviceKey, PropertyName, 0, ref dummy, Buffer, ref retBufferLen);
      }
      RegCloseKey(lDeviceKey);
      return ret;
    }
    private Device(DevicesList devicesList)
    {
      this.m_DevicesList = devicesList;
      if (s_MountedDevices == null) {
        s_MountedDevices = new Dictionary<string, string>();
        RegistryKey hMountedDevices = Registry.LocalMachine.OpenSubKey("SYSTEM\\MountedDevices");
        if (hMountedDevices != null) {
          foreach (string k in hMountedDevices.GetValueNames()) {
            if (k.StartsWith("\\DosDevices\\")) {
              string v = System.Text.Encoding.Unicode.GetString((byte[])hMountedDevices.GetValue(k));
              s_MountedDevices.Add(v.ToLower(), k.Substring(12));
            }
          }
          hMountedDevices.Close();
        }
      }
    }
    internal Device(DevicesList devicesList, int pDevInstF)
        : this(devicesList)
    {
      StringBuilder b = null;
      CM_ReadDeviceProperties(pDevInstF);
      this.DevInst = pDevInstF;
      var s = "CurrentDriveLetterAssignment";
      var dummy = 0;
      if (GetDeviceCustomProp_old(pDevInstF, s, ref b, ref dummy) == 0)
      {
        this.PHYSICAL_DEVICE_OBJECT_NAME = b.ToString().Substring(0, 1) + ":";
      }
      this.m_ImageIndex = devicesList.GetClassImageListIndex(this.ClassGuid);
      this.EnumChildDevices(this.DevInst);
    }
    public string DOSDriveName {
      get { return this.GetMSDOSDriveName(this.PHYSICAL_DEVICE_OBJECT_NAME); }
    }
    internal Device(DevicesList devicesList, IntPtr hDevInfo, SP_DEVINFO_DATA DevInfo) : this(devicesList)
    {
      ReadDeviceProperties(hDevInfo, DevInfo);
      this.ClassGuid = DevInfo.ClassGuid;
      this.m_ImageIndex = devicesList.GetClassImageListIndex(this.ClassGuid);
      this.EnumChildDevices(this.DevInst);
    }
    public static bool IsWindowsNT()
    {
      return (Environment.OSVersion.Platform == PlatformID.Win32NT);
    }

    public override string ToString()
    {
      return string.Format("{0} ({1})", (string.IsNullOrEmpty(this.FRIENDLYNAME) ? this.DEVICEDESC : this.FRIENDLYNAME), this.DOSDriveName);
    }

    public override int GetHashCode()
    {
      return this.DeviceId.ToLower().GetHashCode();
    }
    public override bool Equals(object obj)
    {
      if (!(obj is Device))
        return false;
      return string.Equals(this.DeviceId, ((Device)obj).DeviceId, StringComparison.InvariantCultureIgnoreCase);
    }
  }
}

