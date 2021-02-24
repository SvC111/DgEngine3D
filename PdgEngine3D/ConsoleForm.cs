using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace PdgEngine3D
{
    public partial class ConsoleForm : Form
    {
        public ConsoleForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            
            ConsoleForm_Load(this, null);
        }


        Vector3D vec3d;
        Thread thread;
        Graphics g1;
        Graphics g2;
        Graphics g3;
        Bitmap btm;
        Engine eng = new Engine();

        bool drawing = true;

        int margin = 30;
        int spread = 1000;
        int initialRadius = 50;

        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            PointF img = new PointF(0, 0);
            Task.Run(() => eng.Start(this.pictureBox1));
        }

        private void ImportFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                eng.ChangeObject(ofd.FileName);
            }
        }

    }
}
