using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sem2Lab1
{
    public partial class FormImage : Form
    {
        public Bitmap img;

        public FormImage(ushort[,] data, string formName = "")
        {
            InitializeComponent();
            this.Show();
            this.Text = formName + "; size: " + data.GetLength(0) + "x" + data.GetLength(1);
            img = new Bitmap(ConvertTypes.Ushort2DToBitmap(data));
        }

        public FormImage(byte[,] data, string formName = "")
        {
            InitializeComponent();
            this.Show();
            this.Text = formName + "; size: " + data.GetLength(0) + "x" + data.GetLength(1);
            img = new Bitmap(ConvertTypes.Byte2DToBitmap(data));
        }

        private void panel_image_Paint(object sender, PaintEventArgs e)
        {
            if (img == null)
                return;

            panel_image.Width = img.Width;
            panel_image.Height = img.Height;

            this.Width = img.Width + 30;
            this.Height = img.Height + 30;

            e.Graphics.DrawImage(
                img,
                panel_image.AutoScrollPosition.X,
                panel_image.AutoScrollPosition.Y,
                img.Size.Width,
                img.Size.Height
            );
        }
    }
}
