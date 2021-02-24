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

        Engine engine;

        //initialize
        private void ConsoleForm_Load(object sender, EventArgs e)
        {
            engine = new Engine();
            Task.Run(() => engine.Start(this.pictureBox1));
        }


        //Event called on import file button
        private void ImportFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Task.Run(() =>
                {
                    engine.ChangeObject(ofd.FileName);
                });
            }
        }

    }
}
