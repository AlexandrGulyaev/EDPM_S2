using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Lab1
{
    class ImageJPEG
    {
        Bitmap img;
        int min = 255;
        int max = 0;
        int height;
        int width;
        ushort[,] image;

        public ImageJPEG(Bitmap Image)
        {
            this.img = Image;
            LoadImage();
        }

        public ImageJPEG(string filename, string extension = "jpg")
        {
            filename = filename + "." + extension;
            this.img = (Bitmap)Image.FromFile(filename);
            LoadImage();
        }

        private void LoadImage()
        {
            height = img.Height;
            width = img.Width;
            image = new ushort[img.Width, img.Height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] = img.GetPixel(x, y).R;
                }
            }
        }

        public void ShowImage(string ImageName = "")
        {
            buildImage();
            FormImage formImage = new FormImage(image, ImageName);
        }

        public void setExtremums()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int c = image[x, y];
                    min = c < min ? c : min;
                    max = c > max ? c : max;
                }
            }
        }

        public void ConvertToGrayscale()
        {
            setExtremums();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] = (byte)((double)(image[x, y] - min) / (double)(max - min) * 255.0);
                }
            }
        }

        public void shiftImage(int shift)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] += (ushort)shift;
                }
            }
        }

        public void intensifyImage(double coeff)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image[x, y] = (ushort)(image[x, y] * coeff);
                }
            }
        }

        public void buildImage()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte color = (byte)(image[x, y] > 255 ? 255 : image[x, y]);
                    
                    img.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }
        }

        public void SaveImage(string filename)
        {
            buildImage();
            InOut.SaveImage(img, filename, "jpeg");
        }

        public void ResizeImage(double coeff, bool nearestNeighbor = true)
        {
            ushort[,] temp = Model.ResizeImage(ConvertTypes.BitmapToByte2D(img), coeff, nearestNeighbor);
        }

        /// <summary>
        /// Градационные преобразования изображения
        /// </summary>
        /// <param name="mode">Тип градационного преобразования:
        ///     0 - линейное преобразование (негатив)
        ///     1 - логарифмическое преобразование
        ///     2 - гамма-преобразование (степенное)</param>
        /// <param name="c">Коэффициент при логарифмическом или степенном преобразовании или предельное значение (целое) для линейного преобразования</param>
        /// <param name="degree">Степень логарифмического или степенного преобразования</param>
        /// <returns></returns>
        public void GradationTransformation(byte mode = 0, double c = 1, double degree = 1)
        {
            image = Model.GradationTransformation(image, mode, c, degree);
            //ConvertToGrayscale();
        }
    }
}
