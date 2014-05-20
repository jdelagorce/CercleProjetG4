using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//include supplémentaire
using System.Runtime.InteropServices;
using System.Management;
using System.IO;


namespace formationCP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DetectUSB detectUSB = new DetectUSB();
            detectUSB.DetectionUSB();
        }

        private string Parseur_fichier(string chemin)
        {
            string[] fichier = chemin.Split('\\');
            return fichier[fichier.Length - 1];
        }

        private void Parcourir_Click(object sender, EventArgs e)
        {
            champ_fichier.Clear();
            listFiles.Clear();

            System.Windows.Forms.OpenFileDialog Parcourir_file = new System.Windows.Forms.OpenFileDialog();

            Parcourir_file.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = Parcourir_file.ShowDialog();

            // Process input if the user clicked OK.
            if (result == DialogResult.OK)
            {
                int i = 0;
                foreach(string filename in Parcourir_file.FileNames)
                {
                    
                champ_fichier.Text += "'";
                champ_fichier.Text += filename+ "'" + Environment.NewLine;
                listFiles.Items.Add(Parseur_fichier(filename), i);
                }
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.Value = 50;
            progressBar1.BackColor = Color.Black;
            progressBar1.ForeColor = Color.Red;

        }
        
        //drag and drop



    }
    /*public class NewProgressBar : ProgressBar
    {
        public NewProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // None... Helps control the flicker.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            const int inset = 2; // A single inset value to control teh sizing of the inner rect.

            using (Image offscreenImage = new Bitmap(this.Width, this.Height))
            {
                using (Graphics offscreen = Graphics.FromImage(offscreenImage))
                {
                    Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

                    if (ProgressBarRenderer.IsSupported)
                        ProgressBarRenderer.DrawHorizontalBar(offscreen, rect);

                    rect.Inflate(new Size(-inset, -inset)); // Deflate inner rect.
                    rect.Width = (int)(rect.Width * ((double)this.Value / this.Maximum));
                    if (rect.Width == 0) rect.Width = 1; // Can't draw rec with width of 0.

                    LinearGradientBrush brush = new LinearGradientBrush(rect, this.BackColor, this.ForeColor, LinearGradientMode.Vertical);
                    offscreen.FillRectangle(brush, inset, inset, rect.Width, rect.Height);

                    e.Graphics.DrawImage(offscreenImage, 0, 0);
                    offscreenImage.Dispose();
                }
            }
        }
    }*/
    
}
