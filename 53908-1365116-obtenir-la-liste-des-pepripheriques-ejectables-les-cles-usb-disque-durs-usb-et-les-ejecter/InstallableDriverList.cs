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
    public class InstallableDriverList
    {
        [DllImport("setupapi.dll", EntryPoint = "SetupDiGetClassDevsA")]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, int Enumerator, IntPtr HwndParent, int flags);
        //contruit une liste de pilote compatibles avec le périphérique spécifié ou la liste complète des pilotes installable
        [DllImport("setupapi.dll")]
        private static extern int SetupDiBuildDriverInfoList(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, int DriverType);
        //permet de supprimer le buffer alloué par l'api précédente
        [DllImport("setupapi.dll")]
        private static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        //permet de détruire le buffer aloué par la fonction précédente
        [DllImport("setupapi.dll")]
        private static extern int SetupDiDestroyDriverInfoList(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, int DriverType);
        //permet d'énumérer les pilote du système (présent ou installable)
        [DllImport("setupapi.dll", EntryPoint = "SetupDiEnumDriverInfoW")]
        private static extern int SetupDiEnumDriverInfo(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, int DriverType, int MemberIndex, ref prjEject.InstallableDriver.SP_DRVINFO_DATA DriverInfoData);

        private System.Collections.Generic.List<InstallableDriver> m_DriversList = new System.Collections.Generic.List<InstallableDriver>();

        public InstallableDriverList()
        {
            ReadInstallableDriverList();
        }

        /// <summary>
        /// liste les pilotes installables
        /// </summary>
        /// <remarks></remarks>
        public void ReadInstallableDriverList()
        {
            IntPtr hDevInfoSet;
            int x;
            InstallableDriver.SP_DRVINFO_DATA DrvInfo = new InstallableDriver.SP_DRVINFO_DATA();


            var g = Guid.Empty;
            //on demande une liste Device Information Set
            hDevInfoSet = SetupDiGetClassDevs(ref g, 0, IntPtr.Zero, 4);
            //on demande une liste des pilotes installables
            SetupDiBuildDriverInfoList(hDevInfoSet, IntPtr.Zero, 1);
            //on enregistre la longueur de la structure
            DrvInfo.cbSize = Marshal.SizeOf(DrvInfo);

            x = 0;
            //on enumére le spilotes
            while (SetupDiEnumDriverInfo(hDevInfoSet, IntPtr.Zero, 1, x, ref DrvInfo) == 1)
            {
                //on ajoute le pilote à la liste
                m_DriversList.Add(new InstallableDriver(DrvInfo));
                //on passe au suivant
                x = x + 1;
            }
            //on libère la liste des pilotes
            SetupDiDestroyDriverInfoList(hDevInfoSet, IntPtr.Zero, 1);
            //on libère le Device Information Set
            SetupDiDestroyDeviceInfoList(hDevInfoSet);
        }

        public IEnumerable<InstallableDriver> DriversList
        {
            get { return m_DriversList; }
        }
    }
}

