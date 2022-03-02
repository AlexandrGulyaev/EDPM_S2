using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Lab1
{
    class ImageXCR
    {
        ushort[,] Image;
        public ushort[,] ImageGS;
        ushort min;
        ushort max;

        /// <summary>
        /// Открыть изображение XCR
        /// </summary>
        /// <param name="filename">Имя файла или его полный путь без расширения</param>
        /// <param name="width">Ширина изображения</param>
        /// <param name="height">Высота изображения</param>
        /// <param name="startIndex">Начальный индекс считывания изображения</param>
        /// <param name="swapBytes">Осуществлять ли перестановку байтов:
        ///                         true - осуществлять;
        ///                         false - не осуществлять.</param>
        public ImageXCR(string filename, int width = 1024, int height = 1024, int startIndex = 2048, bool swapBytes = false)
        {
            byte[] buffer = File.ReadAllBytes(filename + ".xcr");
            this.Image = new ushort[height, width];
            int x = 0, y = 0;
            for (int i = startIndex; i < buffer.Length; i++)
            {
                // Перестановка байтов
                if (swapBytes)
                {
                    if (i % 2 != 0)
                    {
                        this.Image[x, y] = (ushort)(buffer[i - 1] << 8 | buffer[i]);
                        x++;
                    }
                }
                else
                {
                    this.Image[x, y] = (ushort)(buffer[i] << 8 | buffer[i - 1]);
                    x++;
                }
                if (x == height)
                {
                    x = 0;
                    y++;
                    if (y == width)
                    {
                        break;
                    }
                }
            }
            ConvertToGrayscale();
            // Поскольку изображение перевернутое, его нужно отразить по вертикали
            FlipImage();
        }

        /// <summary>
        /// Отобразить изображение.
        /// Перед отображением приводим к шкале серости!!!
        /// </summary>
        /// <param name="ImageName">Наименование формы с изображением</param>
        /// <param name="ToGS">Привести к шкале серости перед сохранением (true)</param>
        public void ShowImage(string ImageName = "", bool ToGS = true)
        {
            if (ToGS) ConvertToGrayscale();
            FormImage formImage = new FormImage(ImageGS, ImageName);
        }

        /// <summary>
        /// Перевод к шкале серости
        /// </summary>
        public void ConvertToGrayscale()
        {
            // Получаем экстремумы
            getExtremums();
            this.ImageGS = new ushort[Image.GetLength(0), Image.GetLength(1)];
            for (int i = 0; i < Image.GetLength(0); i++)
            {
                for (int j = 0; j < Image.GetLength(1); j++)
                {
                    this.ImageGS[i, j] = (byte)((double)(Image[i, j] - min) / (double)(max - min) * 255.0);
                }
            }
        }

        /// <summary>
        /// Получить минимум и максимум
        /// </summary>
        public void getExtremums()
        {
            min = Image[0, 0];
            max = Image[0, 0];
            for (int i = 0; i < Image.GetLength(0); i++)
            {
                for (int j = 0; j < Image.GetLength(1); j++)
                {
                    min = Image[i, j] < min ? Image[i, j] : min;
                    max = Image[i, j] > max ? Image[i, j] : max;
                }
            }
        }

        /// <summary>
        /// Отразить изображение
        /// </summary>
        /// <param name="Image">Изображение</param>
        /// <param name="mode">Режим отражения:
        ///                 0 - по горизонтали; 
        ///                 1 - по вертикали; 
        ///                 2 - и по вертикали, и по горизонтали</param>
        /// <returns></returns>
        public void FlipImage(byte mode = 1)
        {
            ushort[,] temp = new ushort[ImageGS.GetLength(0), ImageGS.GetLength(1)];

            for (int x = 0; x < ImageGS.GetLength(0); x++)
            {
                for (int y = 0; y < ImageGS.GetLength(1); y++)
                {
                    switch (mode)
                    {
                        case 0:
                            temp[x, y] = ImageGS[ImageGS.GetLength(0) - x - 1, y];
                            break;
                        case 1:
                            temp[x, y] = ImageGS[x, ImageGS.GetLength(1) - y - 1];
                            break;
                        default:
                            temp[x, y] = ImageGS[ImageGS.GetLength(0) - x - 1, ImageGS.GetLength(1) - y - 1];
                            break;
                    }
                }
            }
            ImageGS = temp;
        }

        /// <summary>
        /// Повернуть изображение
        /// </summary>
        /// <param name="cw">Режим поворота:
        ///                  true - по часовой стрелке (CW);
        ///                  false - против часовой стрелки (CCW).</param>
        /// <param name="angle">Угол поворота в градусах(кратно 90)</param>
        public void RotateImage(bool cw = true, int angle = 90)
        {
            ushort[,] temp;
            int width = ImageGS.GetLength(0);
            int height = ImageGS.GetLength(1);
            angle %= 360;
            if (!cw) angle = 360 - angle;
            switch (angle)
            {
                case 90:
                    temp = new ushort[height, width];
                    // Вращение
                    for (int x = 0; x < ImageGS.GetLength(0); x++)
                    {
                        for (int y = 0; y < ImageGS.GetLength(1); y++)
                        {
                            temp[temp.GetLength(0) - y - 1, x] = ImageGS[x, y];
                        }
                    }
                    break;
                case 180:
                    temp = new ushort[width, height];
                    for (int x = 0; x < ImageGS.GetLength(0); x++)
                    {
                        for (int y = 0; y < ImageGS.GetLength(1); y++)
                        {
                            temp[y, x] = ImageGS[x, y];
                        }
                    }
                    break;
                case 270:
                    temp = new ushort[height, width];
                    for (int x = 0; x < ImageGS.GetLength(0); x++)
                    {
                        for (int y = 0; y < ImageGS.GetLength(1); y++)
                        {
                            temp[y, temp.GetLength(1) - x - 1] = ImageGS[x, y];
                        }
                    }
                    break;
                default:
                    temp = ImageGS;
                    break;
            }
            ImageGS = temp;
        }

        /// <summary>
        /// Сохранить изображение
        /// </summary>
        /// <param name="imageName">Имя файла</param>
        /// <param name="extension">Расширение файла</param>
        /// <param name="ToGS">Привести к шкале серости перед сохранением (true)</param>
        public void SaveImage(string imageName, string extension = "xgs", bool ToGS = true)
        {
            if (ToGS) ConvertToGrayscale();
            InOut.SaveImage(ConvertTypes.Ushort2DToBitmap(this.ImageGS), imageName, extension);
        }

        /// <summary>
        /// Изменить размер изображения
        /// </summary>
        /// <param name="coeff">Коэффициент масштабирования</param>
        /// <param name="nearestNeighbor"></param>
        public void ResizeImage(double coeff, bool nearestNeighbor = true)
        {
            this.Image = Model.ResizeImage(ConvertTypes.Ushort2DToByte2D(this.ImageGS), coeff, nearestNeighbor);
            //ConvertToGrayscale();
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
            ImageGS = Model.GradationTransformation(ImageGS, mode, c, degree);
            //ConvertToGrayscale();
        }
    }
}
