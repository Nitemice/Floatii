using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Floatii
{
    public partial class floatii : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        const long WS_EX_TOPMOST = 0x00000008L;
        const long WS_EX_TOOLWINDOW = 0x00000080L;

        public enum GWL : int
        {
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12)
        }

        // Make form draggable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public floatii()
        {
            InitializeComponent();

            setUpForm();

            this.MouseDown += Form1_MouseDown;
        }

        private void setUpForm()
        {
            var style = GetWindowLongPtr(this.Handle, GWL.GWL_EXSTYLE);
            style = new IntPtr(style.ToInt64() | WS_EX_TOOLWINDOW);
            SetWindowLongPtr(this.Handle, GWL.GWL_EXSTYLE, style);

            //this.SetStyle(ControlStyles.)

            this.TopMost = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = this.BackgroundImage.Size;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void changeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dlgOpenFile.Filter = "Images|*.jpg; *.jpeg; *.gif; *.bmp; *.png;";
            if (this.dlgOpenFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.BackgroundImage = Bitmap.FromFile(this.dlgOpenFile.FileName);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            } else
            {
                this.BackgroundImage = new Bitmap(Properties.Resources.EGYPT);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.BackgroundImageLayout = ImageLayout.Tile;
                this.Size = this.BackgroundImage.Size;
            }
            setUpForm();
        }

        private void changeOpacity(double amount)
        {
            double op = this.Opacity;
            op += amount;
            if (op < 0.05)
            {
                op = 0.05;
            }
            else if (op > 1.0)
            {
                op = 1;
            }
            this.Opacity = op;
        }

        private void upToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            changeOpacity(0.07);
        }

        private void downToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            changeOpacity(-0.07);
        }

        /*
        private void changeSize(int amount)
        {
            Size resizer = this.Size;
            resizer.Height += amount;
            resizer.Width += amount;
            this.Size = resizer;
            setUpForm();
        }

        private void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeSize(5);
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeSize(-5);
        }
        */
    }
}
