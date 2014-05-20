using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace prjEject
{
    internal class frmChild : System.Windows.Forms.Form
    {
        #region Code généré par le Concepteur Windows Form
        public frmChild(Device dev)
        {
            //Cet appel est requis par le Concepteur Windows Form.
            InitializeComponent();

            InitList();
            FillLV(dev);
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
        public System.Windows.Forms.ListView LV;
        //REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
        //Il peut être modifié à l'aide du Concepteur Windows Form.
        //Ne pas le modifier à l'aide de l'éditeur de code.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.LV = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            //
            //LV
            //
            this.LV.Location = new System.Drawing.Point(0, 0);
            this.LV.Name = "LV";
            this.LV.Size = new System.Drawing.Size(544, 403);
            this.LV.TabIndex = 0;
            this.LV.UseCompatibleStateImageBehavior = false;
            this.LV.View = System.Windows.Forms.View.Details;
            //
            //frmChild
            //
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(548, 407);
            this.Controls.Add(this.LV);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Location = new System.Drawing.Point(4, 30);
            this.Name = "frmChild";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Child Devices";
            this.ResumeLayout(false);

        }
        #endregion

        private void InitList()
        {
            LV.Columns.Clear();
            LV.Columns.Add("FRIENDLYNAME");
            LV.Columns.Add("DEVICEDESC");
            LV.Columns.Add("SERVICE");
            LV.Columns.Add("ENUMERATOR_NAME");
            LV.Columns.Add("DRIVER");
            ;
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
        }

        private void FillLV(Device dev)
        {
            ListViewItem li;

            foreach (Device d in dev.ChildDevices)
            {
                li = LV.Items.Add(d.FRIENDLYNAME);
                li.SubItems.Add(d.DEVICEDESC);
                li.SubItems.Add(d.SERVICE);
                li.SubItems.Add(d.ENUMERATOR_NAME);
                li.SubItems.Add(d.DRIVER);
                li.SubItems.Add(d.GetDeviceTypeString());
                li.SubItems.Add(d.GetDeviceCapabilitiesString());
                li.SubItems.Add(d.CHARACTERISTICS.ToString());
                li.SubItems.Add(d.GetDeviceConfigFlagString());
                li.SubItems.Add(d.MFG);
                li.SubItems.Add(d.LOCATION_INFORMATION);
                li.SubItems.Add(d.ADDRESS.ToString());
                li.SubItems.Add(d.BUSNUMBER.ToString());
                li.SubItems.Add(d.DeviceCLASS);
                li.SubItems.Add(d.ClassGuid.ToString());
                li.SubItems.Add(d.COMPATIBLEIDS);
                li.SubItems.Add(d.Exclusive.ToString());
                li.SubItems.Add(d.HARDWAREID);
                li.SubItems.Add(d.PHYSICAL_DEVICE_OBJECT_NAME);
                li.SubItems.Add(d.LOWERFILTERS);
                li.SubItems.Add(d.UPPERFILTERS);
                li.SubItems.Add(d.UI_NUMBER.ToString());
                li.SubItems.Add(d.UI_NUMBER_DESC_FORMAT);
                li.SubItems.Add(d.SECURITY_SDS);
                FillLV(d);
            }
        }
    }
}

