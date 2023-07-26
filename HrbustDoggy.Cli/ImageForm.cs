using System.Drawing;
using System.Windows.Forms;

namespace HrbustDoggy.Cli
{
    internal class ImageForm : Form
    {

        public ImageForm(Image image)
        {
            InitializeComponent();
            BackgroundImage = image;
            BackgroundImageLayout = ImageLayout.Center;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ImageForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(100, 50);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImageForm";
            this.ShowInTaskbar = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ResumeLayout(false);

        }
    }
}
