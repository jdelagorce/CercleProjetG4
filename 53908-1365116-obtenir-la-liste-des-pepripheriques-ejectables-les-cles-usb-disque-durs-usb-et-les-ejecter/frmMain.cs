using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace prjEject
{
    internal class frmMain : System.Windows.Forms.Form
    {
        #region Code généré par le Concepteur Windows Form
        public frmMain()
        {
            //Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent();
        }
        //La méthode substituée Dispose du formulaire pour nettoyer la liste des composants.
        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(Disposing);
        }
        //Requis par le Concepteur Windows Form
        private System.ComponentModel.IContainer components;
        public System.Windows.Forms.ToolTip ToolTip1;
        public System.Windows.Forms.ListBox lstEject;
        public System.Windows.Forms.TabPage _SSTab1_TabPage0;
        public System.Windows.Forms.ListView LV;
        public System.Windows.Forms.TabPage _SSTab1_TabPage1;
        public System.Windows.Forms.TabControl SSTab1;
        public System.Windows.Forms.CheckBox chkSurprise;
        public System.Windows.Forms.Button cmdRefresh;
        public System.Windows.Forms.Button cmdEject;
        //REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
        //Il peut être modifié à l'aide du Concepteur Windows Form.
        //Ne pas le modifier à l'aide de l'éditeur de code.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SSTab1 = new System.Windows.Forms.TabControl();
            this._SSTab1_TabPage0 = new System.Windows.Forms.TabPage();
            this.lstEject = new System.Windows.Forms.ListBox();
            this._SSTab1_TabPage1 = new System.Windows.Forms.TabPage();
            this.LV = new System.Windows.Forms.ListView();
            this.chkSurprise = new System.Windows.Forms.CheckBox();
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.cmdEject = new System.Windows.Forms.Button();
            this.SSTab1.SuspendLayout();
            this._SSTab1_TabPage0.SuspendLayout();
            this._SSTab1_TabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SSTab1
            // 
            this.SSTab1.Controls.Add(this._SSTab1_TabPage0);
            this.SSTab1.Controls.Add(this._SSTab1_TabPage1);
            this.SSTab1.ItemSize = new System.Drawing.Size(42, 18);
            this.SSTab1.Location = new System.Drawing.Point(0, 0);
            this.SSTab1.Name = "SSTab1";
            this.SSTab1.SelectedIndex = 0;
            this.SSTab1.Size = new System.Drawing.Size(545, 401);
            this.SSTab1.TabIndex = 3;
            // 
            // _SSTab1_TabPage0
            // 
            this._SSTab1_TabPage0.Controls.Add(this.lstEject);
            this._SSTab1_TabPage0.Location = new System.Drawing.Point(4, 22);
            this._SSTab1_TabPage0.Name = "_SSTab1_TabPage0";
            this._SSTab1_TabPage0.Size = new System.Drawing.Size(537, 375);
            this._SSTab1_TabPage0.TabIndex = 0;
            this._SSTab1_TabPage0.Text = "Vue simple";
            // 
            // lstEject
            // 
            this.lstEject.BackColor = System.Drawing.SystemColors.Window;
            this.lstEject.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstEject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEject.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstEject.Location = new System.Drawing.Point(0, 0);
            this.lstEject.Name = "lstEject";
            this.lstEject.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lstEject.Size = new System.Drawing.Size(537, 375);
            this.lstEject.TabIndex = 5;
            // 
            // _SSTab1_TabPage1
            // 
            this._SSTab1_TabPage1.Controls.Add(this.LV);
            this._SSTab1_TabPage1.Location = new System.Drawing.Point(4, 22);
            this._SSTab1_TabPage1.Name = "_SSTab1_TabPage1";
            this._SSTab1_TabPage1.Size = new System.Drawing.Size(537, 375);
            this._SSTab1_TabPage1.TabIndex = 1;
            this._SSTab1_TabPage1.Text = "Détails";
            // 
            // LV
            // 
            this.LV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV.Location = new System.Drawing.Point(0, 0);
            this.LV.Name = "LV";
            this.LV.Size = new System.Drawing.Size(537, 375);
            this.LV.TabIndex = 4;
            this.LV.UseCompatibleStateImageBehavior = false;
            this.LV.View = System.Windows.Forms.View.Details;
            this.LV.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LV_MouseDown);
            // 
            // chkSurprise
            // 
            this.chkSurprise.BackColor = System.Drawing.SystemColors.Control;
            this.chkSurprise.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkSurprise.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkSurprise.Location = new System.Drawing.Point(0, 480);
            this.chkSurprise.Name = "chkSurprise";
            this.chkSurprise.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSurprise.Size = new System.Drawing.Size(544, 19);
            this.chkSurprise.TabIndex = 2;
            this.chkSurprise.Text = "N\'afficher que les périphériques qui gère le débranchement surprise (Surprise Rem" +
                "oval)";
            this.chkSurprise.UseVisualStyleBackColor = false;
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.BackColor = System.Drawing.SystemColors.Control;
            this.cmdRefresh.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdRefresh.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdRefresh.Location = new System.Drawing.Point(0, 444);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdRefresh.Size = new System.Drawing.Size(544, 34);
            this.cmdRefresh.TabIndex = 1;
            this.cmdRefresh.Text = "Mettre à jour la liste";
            this.cmdRefresh.UseVisualStyleBackColor = false;
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // cmdEject
            // 
            this.cmdEject.BackColor = System.Drawing.SystemColors.Control;
            this.cmdEject.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdEject.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdEject.Location = new System.Drawing.Point(0, 405);
            this.cmdEject.Name = "cmdEject";
            this.cmdEject.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdEject.Size = new System.Drawing.Size(544, 37);
            this.cmdEject.TabIndex = 0;
            this.cmdEject.Text = "Eject Device";
            this.cmdEject.UseVisualStyleBackColor = false;
            this.cmdEject.Click += new System.EventHandler(this.cmdEject_Click);
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(546, 500);
            this.Controls.Add(this.SSTab1);
            this.Controls.Add(this.chkSurprise);
            this.Controls.Add(this.cmdRefresh);
            this.Controls.Add(this.cmdEject);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Location = new System.Drawing.Point(4, 30);
            this.Name = "frmMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Ejectable Device";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.SSTab1.ResumeLayout(false);
            this._SSTab1_TabPage0.ResumeLayout(false);
            this._SSTab1_TabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private DevicesList lDevices;

        private class DeviceDisplay
        {
            private Device m_Device;
            private string m_Display;

            public DeviceDisplay(Device device, string display)
            {
                this.m_Device = device;
                this.m_Display = display;
            }
            public string Display
            {
                get { return m_Display; }
            }

            public Device Device
            {
                get { return m_Device; }
            }

            public override string ToString()
            {
                return m_Display;
            }
        }

        private void InitList()
        {
            LV.Columns.Clear();
            LV.Columns.Add("FRIENDLYNAME");
            LV.Columns.Add("DEVICEDESC");
            LV.Columns.Add("SERVICE");
            LV.Columns.Add("ENUMERATOR_NAME");
            LV.Columns.Add("DRIVER");

            LV.Columns.Add("DevType");
            LV.Columns.Add("CAPABILITIES");
            LV.Columns.Add("CHARACTERISTICS");
            LV.Columns.Add("CONFIGFLAGS");
            LV.Columns.Add("MFG");
            LV.Columns.Add("LOCATION_INFORMATION");
            LV.Columns.Add("ADDRESS");
            LV.Columns.Add("BUSNUMBER");
            LV.Columns.Add("BUSTYPEGUID");
            LV.Columns.Add("CLASS");
            LV.Columns.Add("CLASSGUID");
            LV.Columns.Add("COMPATIBLEIDS");
            LV.Columns.Add("Exclusive");
            LV.Columns.Add("HARDWAREID");
            LV.Columns.Add("PHYSICAL_DEVICE_OBJECT_NAME");
            LV.Columns.Add("LOWERFILTERS");
            LV.Columns.Add("UPPERFILTERS");
            LV.Columns.Add("UI_NUMBER");
            LV.Columns.Add("UI_NUMBER_DESC_FORMAT");
            LV.Columns.Add("SECURITY_SDS");

            LV.SmallImageList = lDevices.ImageList;
        }

        private void FillList(RemovableDevice rdev)
        {
            var dev = rdev.Device;
            string dn;
            dn = dev.DOSDriveName;
            if (!string.IsNullOrEmpty(dn))
            {
                lstEject.Items.Add(new DeviceDisplay(dev, Convert.ToString((!string.IsNullOrEmpty(dev.FRIENDLYNAME) ? dev.FRIENDLYNAME : dev.DEVICEDESC)) + " on " + Convert.ToString(dev.LOCATION_INFORMATION) + " (" + dn + ")"));
            }
            if (rdev.VolumeDevices.Length > 0)
            {
                dn = string.Empty;
                string ddn;
                foreach (var d in rdev.VolumeDevices)
                {
                    ddn = d.DOSDriveName;
                    if (!string.IsNullOrEmpty(ddn))
                    {
                        if (string.IsNullOrEmpty(dn))
                        {
                            dn = ddn;
                        }
                        else
                        {
                            dn += "," + ddn;
                        }
                    }
                }
                var cd = rdev.StorageDevices[0];
                if (!string.IsNullOrEmpty(dn))
                {
                    lstEject.Items.Add(new DeviceDisplay(dev, Convert.ToString((!string.IsNullOrEmpty(cd.FRIENDLYNAME) ? cd.FRIENDLYNAME : cd.DEVICEDESC)) + " on " + Convert.ToString(dev.LOCATION_INFORMATION) + " (" + dn + ")"));
                }
            }
        }

        //private void FillList(Device dev)
        //{
        //    string dn = null;
        //    dn = dev.DOSDriveName;
        //    if (dn.Length > 0)
        //    {
        //        lstEject.Items.Add(new DeviceDisplay(dev, Convert.ToString((!string.IsNullOrEmpty(dev.FRIENDLYNAME) ? dev.FRIENDLYNAME : dev.DEVICEDESC)) + " on " + Convert.ToString(dev.LOCATION_INFORMATION) + " (" + dn + ")"));
        //    }
        //    if (dev.ChildDevices.Count == 1)
        //    {
        //        dn = string.Empty;
        //        foreach (var sdev in dev.ChildDevices[0].ChildDevices)
        //        {
        //            if (string.IsNullOrEmpty(dn))
        //            {
        //                dn = sdev.DOSDriveName;
        //            }
        //            else
        //            {
        //                dn += "," + sdev.DOSDriveName;
        //            }
        //        }
        //        if (dn.Length > 0)
        //        {
        //            lstEject.Items.Add(new DeviceDisplay(dev, Convert.ToString((!string.IsNullOrEmpty(dev.FRIENDLYNAME) ? dev.FRIENDLYNAME : dev.DEVICEDESC)) + " on " + Convert.ToString(dev.LOCATION_INFORMATION) + " (" + dn + ")"));
        //        }
        //    }
        //}

        private void FillLV()
        {
            ListViewItem li;
            int x;
            int dwMask;

            LV.Items.Clear();
            lstEject.Items.Clear();

            dwMask = (int)((int)Device.DevCapabilities.CM_DEVCAP_REMOVABLE + (chkSurprise.Checked ? (int)Device.DevCapabilities.CM_DEVCAP_SURPRISEREMOVALOK : 0));
            foreach (Device dev in lDevices)
            {
                if (((int)dev.CAPABILITIES & dwMask) == dwMask)
                {
                    li = LV.Items.Add(dev.FRIENDLYNAME, dev.ImageIndex);
                    li.SubItems.Add(dev.DEVICEDESC);
                    li.SubItems.Add(dev.SERVICE);
                    li.SubItems.Add(dev.ENUMERATOR_NAME);
                    li.SubItems.Add(dev.DRIVER);
                    li.SubItems.Add(dev.GetDeviceTypeString());
                    li.SubItems.Add(dev.GetDeviceCapabilitiesString());
                    li.SubItems.Add(dev.CHARACTERISTICS.ToString());
                    li.SubItems.Add(dev.GetDeviceConfigFlagString());
                    li.SubItems.Add(dev.MFG);
                    li.SubItems.Add(dev.LOCATION_INFORMATION);
                    li.SubItems.Add(dev.ADDRESS.ToString());
                    li.SubItems.Add(dev.BUSNUMBER.ToString());
                    li.SubItems.Add(String.Empty);
                    li.SubItems.Add(dev.DeviceCLASS);
                    li.SubItems.Add(dev.ClassGuid.ToString());
                    li.SubItems.Add(dev.COMPATIBLEIDS);
                    li.SubItems.Add(dev.Exclusive.ToString());
                    li.SubItems.Add(dev.HARDWAREID);
                    li.SubItems.Add(dev.PHYSICAL_DEVICE_OBJECT_NAME);
                    li.SubItems.Add(dev.LOWERFILTERS);
                    li.SubItems.Add(dev.UPPERFILTERS);
                    li.SubItems.Add(dev.UI_NUMBER.ToString());
                    li.SubItems.Add(dev.UI_NUMBER_DESC_FORMAT);
                    li.SubItems.Add(dev.SECURITY_SDS);
                    li.Tag = dev;
                }
            }
            foreach (var d in lDevices.RemovableDevices)
                if (((int)d.Device.CAPABILITIES & dwMask) == dwMask)
                    FillList(d);
        }

        private void cmdEject_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            int ret;
            DeviceDisplay dev;
            if (SSTab1.SelectedIndex == 0)
            {
                if (lstEject.SelectedIndex == -1)
                {
                    MessageBox.Show("No device selected", "No device", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return;
                }

                if (MessageBox.Show("Do you really want to eject this Device ?", "Eject?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    dev = (DeviceDisplay)lstEject.SelectedItem;
                    prjEject.Device.PNP_VETO_TYPE veto = 0;
                    ret = dev.Device.EjectDevice(ref veto);
                    if (ret == 0)
                    {
                        lstEject.Items.Remove(dev);
                        for (int i = 0; i <= LV.Items.Count - 1; i++)
                        {
                            if (object.ReferenceEquals(LV.Items[i].Tag, dev.Device))
                            {
                                LV.Items.RemoveAt(i);
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }
            }
            else
            {
                if (LV.SelectedItems.Count == 0)
                {
                    MessageBox.Show("No device selected", "No device", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return;
                }

                if (MessageBox.Show("Do you really want to eject this Device ?", "Eject", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Device d = (Device)LV.SelectedItems[0].Tag;
                    prjEject.Device.PNP_VETO_TYPE veto = 0;
                    ret = d.EjectDevice(ref veto);
                    if (ret == 0)
                    {
                        LV.Items.Remove(LV.SelectedItems[0]);
                        for (int i = 0; i <= lstEject.Items.Count - 1; i++)
                        {
                            if (object.ReferenceEquals(((DeviceDisplay)lstEject.Items[i]).Device, d))
                            {
                                lstEject.Items.RemoveAt(i);
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }
                    }
                }
            }
        }

        private void cmdRefresh_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            lDevices.EnumDevices();
            FillLV();
        }

        private void frmMain_Load(System.Object eventSender, System.EventArgs eventArgs)
        {
            lDevices = new DevicesList(this.components);
            InitList();
            FillLV();
            //Dim l As Object = lDevices.GetDeviceIDList()
            //Dim l As Object = lDevices.GetInterfaceData(DevicesList.InterfaceGUID.GUID_DEVINTERFACE_VOLUME)
        }

        private void LV_MouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Device dev;
            ListViewHitTestInfo Item;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Item = LV.HitTest(e.X, e.Y);
                if (Item.Item != null)
                {
                    dev = (Device)Item.Item.Tag;
                    frmChild f = new frmChild(dev);
                    f.ShowDialog();
                }
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((IDisposable)lDevices).Dispose();
        }
    }
}

