using System;
using System.Drawing;
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

        // Make form snap to screen edge
        // https://stackoverflow.com/a/591734
        private const int SnapDist = 25;
        private bool DoSnap(int pos, int edge)
        {
            int delta = pos - edge;
            return (delta < 0) || (delta > 0 && delta <= SnapDist);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            Screen scn = Screen.FromPoint(this.Location);
            if (DoSnap(this.Left, scn.WorkingArea.Left)) this.Left = scn.WorkingArea.Left;
            if (DoSnap(this.Top, scn.WorkingArea.Top)) this.Top = scn.WorkingArea.Top;
            if (DoSnap(scn.WorkingArea.Right, this.Right)) this.Left = scn.WorkingArea.Right - this.Width;
            if (DoSnap(scn.WorkingArea.Bottom, this.Bottom)) this.Top = scn.WorkingArea.Bottom - this.Height;
        }

        private Bitmap getBackground()
        {
            Bitmap[] bg = { Properties.Resources.ARCADE, Properties.Resources.ARGYLE,
                            Properties.Resources.CASTLE, Properties.Resources.EGYPT,
                            Properties.Resources.REDBRICK, Properties.Resources.RIVETS,
                            Properties.Resources.SQUARES, Properties.Resources.THATCH,
                            Properties.Resources.ZIGZAG };

            Random rnd = new Random();
            int index = rnd.Next(bg.Length);
            return bg[index];
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

        private void setUpForm()
        {
            var style = GetWindowLongPtr(this.Handle, GWL.GWL_EXSTYLE);
            style = new IntPtr(style.ToInt64() | WS_EX_TOOLWINDOW);
            SetWindowLongPtr(this.Handle, GWL.GWL_EXSTYLE, style);

            this.MinimumSize = new Size(10, 10);
            this.AutoSize = false;

            this.TopMost = true;
        }

        public floatii()
        {
            InitializeComponent();

            setUpForm();

            this.MouseDown += Form1_MouseDown;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackgroundImage = getBackground();
            this.Size = this.BackgroundImage.Size;
            this.MouseWheel += scrollWheel;
        }

        private void scrollWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                changeOpacity(0.05);
            }
            else
            {
                changeOpacity(-0.05);
            }
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
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                Size foo = this.BackgroundImage.Size;
                if (foo.Width > 100 || foo.Height > 100)
                {
                    foo.Width /= 10;
                    foo.Height /= 10;
                }
                this.Size = foo;
            }
            else
            {
                this.BackgroundImage = getBackground();
                this.FormBorderStyle = FormBorderStyle.None;
                this.BackgroundImageLayout = ImageLayout.Tile;
                this.Size = this.BackgroundImage.Size;
            }
            setUpForm();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.BringToFront();
        }

        private void tmrBringFront_Tick(object sender, EventArgs e)
        {
            this.BringToFront();
        }


        /*
        private void upToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            changeOpacity(0.07);
        }

        private void downToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            changeOpacity(-0.07);
        }
        */

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
