using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace prjEject
{
  public class InstallableDriver
  {
    //permet de contenir des informations sur un pilote
    internal struct SP_DRVINFO_DATA
    {
      public int cbSize;
      public int DriverType;
      public int Reserved;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
      public string Description;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
      public string MfgName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
      public string ProviderName;
      public System.Runtime.InteropServices.ComTypes.FILETIME DriverDate;
      public decimal DriverVersion;
    }

    private SP_DRVINFO_DATA m_DriveInfo;

    internal InstallableDriver(SP_DRVINFO_DATA DrvInfo)
    {
      m_DriveInfo = DrvInfo;
    }

    public string ProviderName {
      get { return m_DriveInfo.ProviderName; }
    }
    public string MfgName {
      get { return m_DriveInfo.MfgName; }
    }
    public string Description {
      get { return m_DriveInfo.Description; }
    }
    public int DriverType {
      get { return m_DriveInfo.DriverType; }
    }

    public string DriverVersion {
      get { return m_DriveInfo.DriverVersion.ToString(); }
    }
    public System.DateTime DriverDate {
      get { return new System.DateTime(m_DriveInfo.DriverDate.dwLowDateTime + m_DriveInfo.DriverDate.dwHighDateTime << 32); }
    }
  }
}

