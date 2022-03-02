using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sem2Lab1
{
    public partial class FormMain : Form
    {
        struct FileData
        {
            public string filename;
            public string extension;
            public int width;
            public int height;
            public int startIndex;
            public double coeffLT;
            public double coeffGT;
            public double degreeLT;
            public double degreeGT;
        }
        FileData[] files;

        Image img;
        string[] tasks = new string[4] {
            // Лабораторная работа №1
            "Реализовать методы загрузки, отображения, преобразования и сохранения файлов с изображением. Реализовать метод для приведения данных изображения к шкале серости." +
            "\nВ результате приложение должно иметь возможность:" +
            "\n  • загрузить изображение;" +
            "\n  • отобразить изображение на экране; " +
            "\n  • прибавить к изображению константу (30); " +
            "\n  • умножить изображение на константу (1.3);" +
            "\n  • отмасштабировать изображение (привести к шкале серости)." +
            "\nФайл - grace.jpg",
            // Лабораторная работа №2
            "Необходимо реализовать методы, аналогичные методам в лабораторной работе №1 для файлов с расширением .xcr: " +
            "\n  • открыть файл с изображением .xcr; " +
            "\n  • осуществить перестановку байтов; " +
            "\n  • пересчитать в шкалу серости;" +
            "\n  • отобразить изображение." +
            "\n  • сохранить *.xgs; " +
            "\n  • реализовать повороты изображения на 90 градусов." +
            "\n" +
            "\nФайлы:" +
            "\n  • c12-85v.xcr, размер 1024х1024;" +
            "\n  • u0.xcr, размер 2048х2500." +
            "\n" +
            "\nПримечания:" +
            "\n  • Файлы .xcr представляют собой бинарные файлы, в которых на одно значение отводится 2 байта; " +
            "\n  • Значения являются целочисленными, беззнаковыми;" +
            "\n  • Отображать изображение можно только после пересчёта в шкалу серости;" +
            "\n  • Файл содержит header (2048 байт) и хвост. При работе с изображением их учитывать не нужно.",
            // Лабораторная работа №3
            "Произвести изменение размеров изображений, используя два разных способа:" +
            "\n  • метод ближайшего соседа" +
            "\n  • метод билинейной интерполяции" +
            "\n" +
            "\nФайлы:" +
            "\n  • grace.jpg – увеличить в 1.3 раза;" +
            "\n  • c12-85v.xcr, u0.xcr – уменьшить изображения в 0.6 раз с поворотом на 90 градусов CCW (против часовой стрелки), чтобы вертикальный размер изображения полностью помещался в размер экрана.",
            // Лабораторная работа №4
            "Необходимо реализовать методы градационных преобразований изображения: логарифмическое преобразование, негатив, гамма-преобразование." +
            "\n Выполнить следующие преобразования и отобразить результаты: " +
            "\n  • негатив файлов: grace.jpg, *.xcr; " +
            "\n  • гамма-преобразование и логарифмическое преобразование файлов: " +
            "\n     - photo1.jpg, " +
            "\n     - photo2.jpg, " +
            "\n     - photo3.jpg, " +
            "\n     - photo4.jpg, " +
            "\n     - hollywood.jpg."
        };

        public FormMain()
        {
            InitializeComponent();
            initFiles();
        }

        private void initFiles()
        {
            files = new FileData[8];
            files[0] = new FileData();
            files[0].filename = "c12-85v";
            files[0].extension = "xcr";
            files[0].width = 1024;
            files[0].height = 1024;
            files[0].startIndex = 2048;

            files[1] = new FileData();
            files[1].filename = "u0";
            files[1].extension = "xcr";
            files[1].width = 2500;
            files[1].height = 2048;
            files[1].startIndex = 2048;

            files[2] = new FileData();
            files[2].filename = "grace";
            files[2].extension = "jpg";

            files[3] = new FileData();
            files[3].filename = "photo1";
            files[3].extension = "jpg";
            files[3].coeffGT = 84;
            files[3].coeffLT = 73;
            files[3].degreeGT = 0.2;
            files[3].degreeLT = 5;

            files[4] = new FileData();
            files[4].filename = "photo2";
            files[4].extension = "jpg";
            files[4].coeffGT = 146;
            files[4].coeffLT = 106;
            files[4].degreeGT = 0.1;
            files[4].degreeLT = 10;

            files[5] = new FileData();
            files[5].filename = "photo3";
            files[5].extension = "jpg";
            files[5].coeffGT = 1;
            files[5].coeffLT = 1;
            files[5].degreeGT = 1;
            files[5].degreeLT = 1;

            files[6] = new FileData();
            files[6].filename = "photo4";
            files[6].extension = "jpg";
            files[6].coeffGT = 1;
            files[6].coeffLT = 1;
            files[6].degreeGT = 1;
            files[6].degreeLT = 1;

            files[7] = new FileData();
            files[7].filename = "hollywood";
            files[7].extension = "jpg";
            files[7].coeffGT = 1;
            files[7].coeffLT = 1;
            files[7].degreeGT = 1;
            files[7].degreeLT = 1;
        }

        private void button_lab_Click(object sender, EventArgs e)
        {
            this.label_task.Visible = true;
            int TaskNo = Convert.ToInt32(((Button)sender).Text.Substring(21));
            this.label_task.Text = tasks[TaskNo - 1];

            switch (TaskNo)
            {
                case 1:
                    Lab1();
                    break;
                case 2:
                    Lab2();
                    break;
                case 3:
                    Lab3();
                    break;
                case 4:
                    Lab4();
                    break;
                default:
                    break;
            }
            //panel_image.AutoScroll = true;
            //panel_image.AutoScrollMinSize = img.Size;
            //panel_image.Invalidate();
        }

        private void Lab1()
        {
            FileData CurrFile = files[2];
            ImageJPEG image = new ImageJPEG(CurrFile.filename, CurrFile.extension);

            image.shiftImage(30);
            image.buildImage();
            image.SaveImage(CurrFile.filename + "2.jpg");

            image.intensifyImage(1.3);
            image.buildImage();
            image.SaveImage(CurrFile.filename + "3.jpg");

            image.ConvertToGrayscale();
            image.buildImage();
            image.SaveImage(CurrFile.filename + "4.jpg");
        }

        /// <summary>
        /// c12-85v.xcr, размер 1024х1024;
        /// u0.xcr, размер 2048х2500.
        /// </summary>
        private void Lab2()
        {
            FileData CurrFile = files[0];
            ImageXCR imageXCR = new ImageXCR(CurrFile.filename, CurrFile.width, CurrFile.height, CurrFile.startIndex, true);

            // Отобразить исходное изображение
            imageXCR.ShowImage(CurrFile.filename + ".xcr, исходник в GS", false);

            // Поворот изображения на 90 градусов CCW
            imageXCR.RotateImage(false, 90);
            imageXCR.ShowImage(CurrFile.filename + ".xcr, исходник в GS с 90CCW", false);

            // Сохранить полученное изображение
            imageXCR.SaveImage(CurrFile.filename, "jpg");
        }

        /// <summary>
        ///  Файлы:
        /// • grace.jpg – увеличить в 1.3 раза;
        /// • c12-85v.xcr, u0.xcr – уменьшить изображения в 0.6 раз с поворотом на 90 градусов CCW,
        /// </summary>
        private void Lab3()
        {
            foreach (FileData CurrFile in files)
            {
                if (CurrFile.extension == "xcr")
                {
                    ImageXCR imageXCR = new ImageXCR(CurrFile.filename, CurrFile.width, CurrFile.height, CurrFile.startIndex, true);
                    ImageXCR imageXCR2 = new ImageXCR(CurrFile.filename, CurrFile.width, CurrFile.height, CurrFile.startIndex, true);

                    // Отобразить исходное изображение
                    //imageXCR.ShowImage(CurrFile.filename + ".xcr, исходник в GS");

                    // Поворот изображения на 90 градусов CCW
                    imageXCR.RotateImage(false, 90);
                    imageXCR2.RotateImage(false, 90);
                    //imageXCR.ShowImage(CurrFile.filename + ".xcr, исходник в GS с 90CCW");

                    // Метод ближайшего соседа
                    imageXCR.ResizeImage(0.4);
                    imageXCR.ShowImage(CurrFile.filename + " resize NN", true);
                    // Метод билинейной интерполяции
                    imageXCR2.ResizeImage(0.4, false);
                    imageXCR2.ShowImage(CurrFile.filename + " resize BI", true);
                }
                else if (CurrFile.filename == "grace")
                {
                    // Для метода ближайшего соседа
                    ImageJPEG imageJPEG = new ImageJPEG(CurrFile.filename, CurrFile.extension);
                    // Для метода 4-смежной билинейной интерполяции
                    ImageJPEG imageJPEG2 = new ImageJPEG(CurrFile.filename, CurrFile.extension);
                    imageJPEG.ResizeImage(1.3);
                    imageJPEG.SaveImage(CurrFile.filename + " resize NN");
                    imageJPEG2.ResizeImage(1.3, false);
                    imageJPEG2.SaveImage(CurrFile.filename + " resize BI");
                }
            }
        }

        /// <summary>
        /// Градационные преобразования
        /// </summary>
        private void Lab4()
        {
            int cnt = 0;
            foreach (FileData CurrFile in files)
            {
                cnt++;
                if (CurrFile.filename == "grace")
                {
                    ImageJPEG imageJPEG = new ImageJPEG(CurrFile.filename, CurrFile.extension);
                    imageJPEG.ShowImage(CurrFile.filename);
                    imageJPEG.GradationTransformation(0, 256);
                    imageJPEG.ShowImage(CurrFile.filename + " negative");
                }
                else if (CurrFile.extension == "jpg")
                {
                    if (cnt > 5) break;
                    // Логарифмическое преобразование
                    ImageJPEG imageJPEGLT = new ImageJPEG(CurrFile.filename, CurrFile.extension);
                    imageJPEGLT.GradationTransformation(1, CurrFile.coeffLT, CurrFile.degreeLT);
                    // Гамма-преобразование
                    ImageJPEG imageJPEGGT = new ImageJPEG(CurrFile.filename, CurrFile.extension);
                    imageJPEGGT.GradationTransformation(2, CurrFile.coeffGT, CurrFile.degreeGT);

                    imageJPEGLT.ShowImage(CurrFile.filename + " LT");
                    imageJPEGGT.ShowImage(CurrFile.filename + " GT");
                }
                else
                {
                    ImageXCR imageXCR = new ImageXCR(CurrFile.filename, CurrFile.width, CurrFile.height, CurrFile.startIndex, true);
                    imageXCR.RotateImage(false, 90);
                    imageXCR.ShowImage(CurrFile.filename, false);
                    imageXCR.GradationTransformation(0, 256);
                    imageXCR.ShowImage(CurrFile.filename + " negative", false);
                    imageXCR.SaveImage(CurrFile.filename + " negative", "jpg", false);
                }
            }
        }
    }
}
