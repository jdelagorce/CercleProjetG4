using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formationCP
{
    class DetectUSB : Form1
    {
        public DetectUSB()
        {
               
        }
            Form1 form_princ = new Form1();
        
        public void DetectionUSB()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    form_princ.champ_fichier.Text += drive.Name;
                }
            }

        }

    }
}
