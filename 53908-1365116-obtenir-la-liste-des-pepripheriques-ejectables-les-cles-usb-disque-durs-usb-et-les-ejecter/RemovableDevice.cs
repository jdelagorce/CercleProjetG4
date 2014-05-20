using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace prjEject
{
  public class RemovableDevice
  {
    private Device m_Device;
    public Device Device {
      get { return m_Device; }
    }
    private Device[] m_StorageDevices;
    public Device[] StorageDevices {
      get { return m_StorageDevices; }
    }
    private Device[] m_VolumeDevices;
    public Device[] VolumeDevices {
      get { return m_VolumeDevices; }
    }

    private Device[] GetChildren(Device device, string cls)
    {
      List<Device> ret = new List<Device>();
      if (device.DeviceCLASS == cls) {
        ret.Add(device);
      }
      foreach (var child in device.ChildDevices) {
        ret.AddRange(GetChildren(child, cls));
      }
      return ret.ToArray();
    }

    internal RemovableDevice(Device device)
    {
      this.m_Device = device;
      this.m_StorageDevices = GetChildren(device, "DiskDrive");
      this.m_VolumeDevices = GetChildren(device, "Volume");
    }
  }
}

