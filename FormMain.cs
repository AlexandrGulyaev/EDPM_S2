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
        string[] tasks = new string[14] {
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
            "\n     - hollywood.jpg.",

            // Лабораторная работа №5
            "1.Рассчитать и отобразить гистограммы изображений и соответствующие им функции распределения." +
            "\n2. Необходимо реализовать метод для эквализации гистограммы и применить его к изображениям в файлах. " +
            "\n3. Реализовать метод приведения гистограммы (вариант линеаризации функции распределения с помощью нахождения кривой обратной к функции распределения яркостей) и применить его к изображениям в файлах." +
            "\n Файлы: photo*.jpg , hollywood.jpg, *.xcr",

            // Лабораторная работа №6
            "Реализовать обнаружение и подавление артефактов противорассеивающих сеток в рентгеновских снимках.",

            // Лабораторная работа №7
            "Реализовать методы для зашумления модельного изображения следующими типами шумов:" +
            "\n  • соль и перец (salt & pepper)" +
            "\n  • случайный шум (random noise)" +
            "\n  • смесь двух типов шумов в разных пропорциях" +
            "\nНеобходимо подавить наложенные шумы следующими способами:" +
            "\n  • усредняющий фильтр – отклик фильтра равен среднему значению по маске" +
            "\n  • медианный фильтр – отклик фильтра равен медианному значению по маске" +
            "\nОтобразить изображения: исходное, зашумленные тремя указанными способами и обработанные изображения масками разного размера." +
            "\nФайл: model.jpg",

            // Лабораторная работа №8
            "Реализовать 1-D обратное и 2-D прямое и обратное преобразование Фурье. Требования:" +
            "\n  1. Реализовать обратное 1-D ПФ для модельной кардиограммы." +
            "\n  2. Реализовать прямое и обратное 2-D ПФ для изображения из файла." +
            "\n  3. Реализовать изменение размеров изображения из файла с помощью прямого ПФ, дополнения спектра нулями и обратного ПФ. " +
            "\n  4. Оценить качество всех “resizing”-методов с помощью вычитания и градационного преобразования разностного изображения." +
            "\nФайл: grace.jpg",

            // Лабораторная работа №9
            "Необходимо компенсировать искажения, произведенные известной искажающей 1-D функцией (смазывание в одном направлении). " +
            "В качестве входных данных предоставляется искаженное изображение и файл с искажающей функцией.",

            // Лабораторная работа №10
            "Выделить контуры объектов на изображении посредством применения ФНЧ и ФВЧ, а также пороговых преобразований и арифметических операций по необходимости." +
            "\nВыделить контуры на model.jpg, зашумленном смесью шумов и обработанном методами из ЛР №7.",

            // Лабораторная работа №11
            "Реализовать градиент (обычный и масками Собела по разным направлениям) и Лапласиан" +
            " и применить их к изображению для выделения контуров. " +
            "Изображения без шумов, зашумленных и обработанных методами из ЛР №7.",

            // Лабораторная работа №12
            "Реализовать операции эрозии и дилатации, применить их к изображению для выделения контуров." +
            "\nМинимальные требования: отобразить изображения с контурами. Выделить контуры морфологическими операциями на изображениях, зашумленных и восстановленных методами из ЛР №7." +
            "\nФайл: model.jpg",

            // Лабораторная работа №13
            "",

            // Лабораторная работа №14
            ""
        };

        public FormMain()
        {
            InitializeComponent();
            initFiles();
        }

        private void initFiles()
        {
            files = new FileData[12];

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
            files[3].degreeGT = 0.2;
            files[3].coeffLT = 73;
            files[3].degreeLT = 5;

            files[4] = new FileData();
            files[4].filename = "photo2";
            files[4].extension = "jpg";
            files[4].coeffGT = 146;
            files[4].degreeGT = 0.1;
            files[4].coeffLT = 106;
            files[4].degreeLT = 10;

            files[5] = new FileData();
            files[5].filename = "photo3";
            files[5].extension = "jpg";
            files[5].coeffGT = 2e-4;
            files[5].degreeGT = 2.5;
            files[5].coeffLT = -86;
            files[5].degreeLT = 0.1;

            files[6] = new FileData();
            files[6].filename = "photo4";
            files[6].extension = "jpg";
            files[6].coeffGT = 2.3e-10;
            files[6].degreeGT = 5;
            files[6].coeffLT = -15.94;
            files[6].degreeLT = 0.5;

            files[7] = new FileData();
            files[7].filename = "hollywood";
            files[7].extension = "jpg";
            files[7].coeffGT = 6e-2;
            files[7].degreeGT = 1.5;
            files[7].coeffLT = -1.36;
            files[7].degreeLT = 0.95;

            files[8] = new FileData();
            files[8].filename = "model";
            files[8].extension = "png";

            files[9] = new FileData();
            files[9].filename = "blur307x221D";
            files[9].extension = "dat";
            files[9].width = 307;
            files[9].height = 221;

            files[10] = new FileData();
            files[10].filename = "blur307x221D_N";
            files[10].extension = "dat";
            files[10].width = 307;
            files[10].height = 221;

            files[11] = new FileData();
            files[11].filename = "kernD76_f4";
            files[11].extension = "dat";
            files[11].width = 307;
            files[11].height = 1;
        }

        private void button_lab_Click(object sender, EventArgs e)
        {
            this.label_task.Visible = true;
            int AttestationNo = Convert.ToInt32(((Button)sender).Text.Substring(11, 1));
            int TaskNo = Convert.ToInt32(((Button)sender).Text.Substring(21));
            this.label_task.Text = tasks[TaskNo - 1 + (AttestationNo - 1) * 4];

            switch (AttestationNo * 10 + TaskNo)
            {
                case 11:
                    Lab1();
                    break;
                case 12:
                    Lab2();
                    break;
                case 13:
                    Lab3();
                    break;
                case 14:
                    Lab4();
                    break;
                case 21:
                    Lab5();
                    break;
                case 22:
                    Lab6();
                    break;
                case 23:
                    Lab7();
                    break;
                case 24:
                    Lab8();
                    break;
                case 31:
                    Lab9();
                    break;
                case 32:
                    Lab10();
                    break;
                case 33:
                    Lab11();
                    break;
                case 34:
                    Lab12();
                    break;
                case 41:
                    Lab13();
                    break;
                case 42:
                    Lab14();
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
            MyImage image = new MyImage(CurrFile.filename, CurrFile.extension);
            image.Show(CurrFile.filename);

            image.shiftImage(30);
            image.buildImage();
            image.Show(CurrFile.filename + "2");
            image.Save(CurrFile.filename + "2");

            image.intensifyImage(1.3);
            image.buildImage();
            image.Show(CurrFile.filename + "3");
            image.Save(CurrFile.filename + "3");

            image.ConvertToGrayscale();
            image.buildImage();
            image.Show(CurrFile.filename + "4");
            image.Save(CurrFile.filename + "4");
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
            imageXCR.Show(CurrFile.filename + ".xcr, исходник в GS", false);

            // Поворот изображения на 90 градусов CCW
            imageXCR.Rotate(false, 90);
            imageXCR.Show(CurrFile.filename + ".xcr, исходник в GS с 90CCW", false);

            // Сохранить полученное изображение
            imageXCR.Save(CurrFile.filename, "jpg");
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
                    if (CurrFile.filename == "u0")
                    {
                        ImageXCR imageXCR = new ImageXCR(CurrFile.filename, CurrFile.width, CurrFile.height, CurrFile.startIndex, true);
                        ImageXCR imageXCR2 = new ImageXCR(CurrFile.filename, CurrFile.width, CurrFile.height, CurrFile.startIndex, true);

                        // Отобразить исходное изображение
                        //imageXCR.ShowImage(CurrFile.filename + ".xcr, исходник в GS");

                        // Поворот изображения на 90 градусов CCW
                        imageXCR.Rotate(false, 90);
                        imageXCR2.Rotate(false, 90);
                        //imageXCR.ShowImage(CurrFile.filename + ".xcr, исходник в GS с 90CCW");

                        // Метод ближайшего соседа
                        imageXCR.Resize(0.4);
                        imageXCR.Show(CurrFile.filename + " resize NN", true);
                        // Метод билинейной интерполяции
                        imageXCR2.Resize(0.4, false);
                        imageXCR2.Show(CurrFile.filename + " resize BI", true);

                        imageXCR.Save(CurrFile.filename + " resize NN", "jpg", true);
                        imageXCR2.Save(CurrFile.filename + " resize BI", "jpg", true);
                    }
                }
                else if (CurrFile.filename == "grace")
                {
                    // Для метода ближайшего соседа
                    MyImage imageJPEG = new MyImage(CurrFile.filename, CurrFile.extension);
                    // Для метода 4-смежной билинейной интерполяции
                    MyImage imageJPEG2 = new MyImage(CurrFile.filename, CurrFile.extension);
                    imageJPEG.Resize(1.3);
                    imageJPEG.Show(CurrFile.filename + " resize NN");
                    imageJPEG2.Resize(1.3, false);
                    imageJPEG2.Show(CurrFile.filename + " resize BI");

                    imageJPEG.Save(CurrFile.filename + " resize NN");
                    imageJPEG2.Save(CurrFile.filename + " resize BI");
                }
            }
        }

        /// <summary>
        /// Градационные преобразования
        /// </summary>
        private void Lab4()
        {
            foreach (FileData CurrFile in files)
            {
                if (CurrFile.filename == "grace")
                {
                    MyImage imageJPEG = new MyImage(CurrFile.filename, CurrFile.extension);
                    //imageJPEG.ShowImage(CurrFile.filename);
                    imageJPEG.GradationTransformation(0, 256);
                    //imageJPEG.ShowImage(CurrFile.filename + " negative");
                    imageJPEG.Save(CurrFile.filename + " negative");
                }
                else if (CurrFile.extension == "jpg")
                {
                    // Логарифмическое преобразование
                    MyImage imageJPEGLT = new MyImage(CurrFile.filename, CurrFile.extension);
                    imageJPEGLT.GradationTransformation(1, CurrFile.coeffLT, CurrFile.degreeLT);
                    // Гамма-преобразование
                    MyImage imageJPEGGT = new MyImage(CurrFile.filename, CurrFile.extension);
                    imageJPEGGT.GradationTransformation(2, CurrFile.coeffGT, CurrFile.degreeGT);

                    //imageJPEGLT.ShowImage(CurrFile.filename + " LT");
                    imageJPEGLT.Save(CurrFile.filename + " LT");
                    //imageJPEGGT.ShowImage(CurrFile.filename + " GT");
                    imageJPEGGT.Save(CurrFile.filename + " GT");
                }
                else
                {
                    ImageXCR imageXCR = new ImageXCR(CurrFile.filename, CurrFile.width, CurrFile.height, CurrFile.startIndex, true);
                    imageXCR.Rotate(false, 90);
                    //imageXCR.ShowImage(CurrFile.filename, false);
                    imageXCR.GradationTransformation(0, 256);
                    //imageXCR.ShowImage(CurrFile.filename + " negative", false);
                    imageXCR.Save(CurrFile.filename + " negative", "jpg", false);
                }
            }
        }

        /// <summary>
        /// Гистограммы изображений и соответствующие им функции распределения
        /// </summary>
        private void Lab5()
        {
            FileData currFile = files[1];
            MyImage image;
            if (currFile.extension != "xcr" && currFile.extension != "xgs")
            { 
                image = new MyImage(currFile.filename, currFile.extension);
            }
            else
            {
                image = new MyImage(currFile.filename, currFile.width, currFile.height, currFile.startIndex, true, currFile.extension);
                image.Rotate(false);
            }
            string fullname = currFile.filename + "." + currFile.extension;

            image.Show(fullname);
            image.Save(fullname);

            // Гистограмма изображения
            image.ShowHistogram(true, "до эквализации");
            // Функция распределения
            image.ShowCDF();

            // Передаточная шкала
            image.GetTransferScale();
            image.ShowCDF(false, 1);

            // Эквализация
            image.ReplacePixelsFromCDF();
            //image.Save("Эквализация");
            image.Show(fullname + " эквализация");
            image.Save(fullname + " эквализация");
            image.ShowHistogram(true, "после эквализации");

            // Обратная CDF
            image.GetInverseCDF();
            image.ReplacePixelsFromCDF(false);
            image.Show(fullname + " обратная");
            image.Save(fullname + " обратная");

            image.ShowCDF(false, 2);

        }

        /// <summary>
        /// Реализовать обнаружение и подавление артефактов противорассеивающих сеток в рентгеновских снимках.
        /// </summary>
        private void Lab6()
        {
            FileData currFile = files[0];
            MyImage image = new MyImage(currFile.filename, currFile.width, currFile.height, currFile.startIndex, true, currFile.extension);
            image.Rotate(false);

            image.SuppressArtifacts();

            image.Save(currFile.filename + " - подавление артефактов");

            image.SuppressArtifacts();
            image.SuppressArtifacts();
            image.Save(currFile.filename + " - повторное подавление артефактов");
        }

        /// <summary>
        /// Работа с шумами
        /// </summary>
        private void Lab7()
        {
            FileData currFile = files[8];
            MyImage image = new MyImage(currFile.filename, currFile.extension);
            image.Show("Без шума", false);

            // С медианным фильтром
            //image.AddNoise(0, frequency: 0.05);
            //image.Show("Наложение шума соль-перец, 5% от всего изображения", false);
            //image.ShowHistogram(name: "Гистограмма изображения с шумом соль-перец");
            //image.SuppressNoise();
            //image.Show("Подавление шума соль-перец медианным фильтром", false);

            //image = new MyImage(currFile.filename, currFile.extension);
            //image.AddNoise(1);
            //image.Show("Наложение случайного шума", false);
            //image.ShowHistogram(name: "Гистограмма изображения со случайным шумом");
            //image.SuppressNoise();
            //image.Show("Подавление случайного шума медианным фильтром", false);

            image = new MyImage(currFile.filename, currFile.extension);
            image.AddNoise(2);
            image.Save("Model с комбинированным шумом");
            image.Show("Наложение комбинированного шума", false);
            //image.ShowHistogram(name: "Гистограмма изображения с комбинированным");
            image.SuppressNoise();
            image.Show("Подавление комбинированного шума медианным фильтром", false);
            image.Save("Model с подавленным медианным фильтром шумом");

            // С усредняющим фильтром
            //image.AddNoise(0, frequency: 0.05);
            //image.SuppressNoise(false);
            //image.Show("Подавление шума соль-перец усредняющим фильтром", false);

            //image = new MyImage(currFile.filename, currFile.extension);
            //image.AddNoise(1);
            //image.SuppressNoise(false);
            //image.Show("Подавление случайного шума усредняющим фильтром", false);

            image = new MyImage(currFile.filename, currFile.extension);
            image.AddNoise(2);
            image.SuppressNoise(false);
            image.Show("Подавление комбинированного шума усредняющим фильтром", false);
            image.Save("Model с подавленным усредняющим фильтром шумом");


            //image = new MyImage(currFile.filename, currFile.extension);
            //image.AddNoise(2);
            //image.SuppressNoise(false, 2);
            //image.Show("Подавление комбинированного шума усредняющим фильтром 2x2", false);

            //image = new MyImage(currFile.filename, currFile.extension);
            //image.AddNoise(2);
            //image.SuppressNoise(true, 2);
            //image.Show("Подавление комбинированного шума медианным фильтром 2x2", false);
        }

        /// <summary>
        /// Обратные 1-D и 2-D преобразования Фурье
        /// </summary>
        private void Lab8()
        {
            FileData currFile = files[2];
            // Часть первая

            //MyImage image = new MyImage(currFile.filename, currFile.extension);
            //image.getInverseFourier();

            //image.getFourier2D();
            //image.ShowFourier2D();

            ////image.getFourier2D(true);
            ////image.ShowFourierInv2D();

            //image.ResizeFourier2D(1.5);
            //image.getFourier2D(true);
            //image.ShowFourierInv2D();

            //MyImage new_image = new MyImage(ConvertTypes.Ushort2DToBitmap(image.temp));
            //new_image.Save("ResizeFourier2D");


            // Часть вторая
            MyImage image = new MyImage(currFile.filename, currFile.extension);

            MyImage imageFur = new MyImage("ResizeFourier2D", "jpg"); // Фурье
            imageFur.Show("Ресайз Фурье");

            MyImage imageNN = new MyImage("NN", "jpg"); // Ближайший сосед
            imageNN.Show("Ближайший сосед");

            MyImage imageBI = new MyImage("BI", "jpg"); // Билиненая интерполяция
            imageBI.Show("Билинейная интерполяция");

            short[,] DiffFurAndOrig = Model.getDiffImage(imageFur.image, image.image); // Разность между Фурье и оригиналом
            short[,] DiffBIAndOrig = Model.getDiffImage(imageBI.image, image.image); // Разность между Билинейной интерполяцией и оригиналом
            short[,] DiffNNAndOrig = Model.getDiffImage(imageNN.image, image.image); // Разность между Ближайшим соседом и оригиналом

            // Выполним градационное логарифмическое преобразование
            ushort[,] UDiffFurAndOrig = Model.LogarithmicTransformation(DiffFurAndOrig, 51, 3);
            MyImage new_image = new MyImage(ConvertTypes.Ushort2DToBitmap(UDiffFurAndOrig));
            new_image.Show("Фурье vs Оригинал");

            ushort[,] UDiffBIAndOrig = Model.LogarithmicTransformation(DiffBIAndOrig, 51, 3);
            MyImage new_image2 = new MyImage(ConvertTypes.Ushort2DToBitmap(UDiffBIAndOrig));
            new_image2.Show("BI vs Оригинал");

            ushort[,] UDiffNNAndOrig = Model.LogarithmicTransformation(DiffNNAndOrig, 51, 3);
            MyImage new_image3 = new MyImage(ConvertTypes.Ushort2DToBitmap(UDiffNNAndOrig));
            new_image3.Show("NN vs Оригинал");
        }


        /// <summary>
        /// Восстановить смазанное изображение
        /// </summary>
        private void Lab9()
        {
            //// Файл без шума
            //FileData currFile = files[9];
            //MyImage image = new MyImage(filename: currFile.filename + "." + currFile.extension,
            //                            width: currFile.width,
            //                            height: currFile.height);
            //image.Show(currFile.filename + "." + currFile.extension);

            //// Искажающая функция
            //FileData currFileDist = files[11];
            //MyImage imageDist = new MyImage(filename: currFileDist.filename + "." + currFileDist.extension,
            //                            width: currFileDist.width,
            //                            height: currFileDist.height);

            //// Фурье 1Д файла без шума
            //double[,] imageFourier, ArrRe, ArrIm;
            //Model.getFourier(image.imageDouble, out imageFourier, out ArrRe, out ArrIm, false);

            //// Фурье 1Д искажающей функции
            //double[,] imageDistFourier, ArrReDist, ArrImDist;
            //Model.getFourier(imageDist.imageDouble, out imageDistFourier, out ArrReDist, out ArrImDist, false);

            //// Удаляем искажения
            //imageFourier = Model.SuppressDistorsion(ArrRe, ArrIm, ArrReDist, ArrImDist);

            //// Преобразуем Фурье обратно в изображение
            //double[,] newImage;
            //Model.getFourier(imageFourier, out newImage, out ArrRe, out ArrIm, false);

            //image.image = ConvertTypes.Double2DToUshort2D(newImage);
            //image.swapHalfes();
            //image.Show();



            // Файл с шумом
            FileData currFileNoise = files[10];
            MyImage imageNoise = new MyImage(filename: currFileNoise.filename + "." + currFileNoise.extension,
                                        width: currFileNoise.width,
                                        height: currFileNoise.height);
            imageNoise.Show(currFileNoise.filename + "." + currFileNoise.extension);

            // Искажающая функция
            FileData currFileDist = files[11];
            MyImage imageDist = new MyImage(filename: currFileDist.filename + "." + currFileDist.extension,
                                        width: currFileDist.width,
                                        height: currFileDist.height);

            // Фурье 1Д файла с шумом
            double[,] imageFourier, ArrRe, ArrIm;
            Model.getFourier(imageNoise.imageDouble, out imageFourier, out ArrRe, out ArrIm, false);

            // Фурье 1Д искажающей функции
            double[,] imageDistFourier, ArrReDist, ArrImDist;
            Model.getFourier(imageDist.imageDouble, out imageDistFourier, out ArrReDist, out ArrImDist, false);

            // Удаляем искажения
            imageFourier = Model.SuppressDistorsion(ArrRe, ArrIm, ArrReDist, ArrImDist, true);

            // Преобразуем Фурье обратно в изображение
            double[,] newImage;
            Model.getFourier(imageFourier, out newImage, out ArrRe, out ArrIm, false);

            imageNoise.image = ConvertTypes.Double2DToUshort2D(newImage);
            imageNoise.swapHalfes();
            imageNoise.Show();
        }

        /// <summary>
        /// Выделить границы с помощью НЧФ и ВЧФ и пороговых преобразований (при необходимости)
        /// Выделить границы на зашумлённом смесью шумов изображении.
        /// Выделить границы на обработанном от шумов изображении.
        /// Файл: model.jpg
        /// </summary>
        private void Lab10()
        {
            //FileData currFile = files[8];
            //MyImage image = new MyImage(currFile.filename, currFile.extension);
            //MyImage image2 = new MyImage(currFile.filename, currFile.extension);
            //image.Show("Исходное изображение", false);

            //image.ApplyFilter(filter_type: 1, fc1: 0.05, m: 32, mode: 2);

            ////Вычитание для ФВЧ не выполняется
            ////image.image = ConvertTypes.ShortToUshort2D(Model.getDiffImage(image.image, image2.image));

            ////ФНЧ: 210, 177 для одномерного фильтра; 173 для двумерного
            ////ФВЧ: 105, 125 для одномерного фильтра; 131 для двумерного
            //image.ThresholdTransform(131);
            //image.Show("Контур", false);


            /*
             * ****************************
             * ************ ФНЧ ***********
             * ****************************
             */

            //MyImage imageWithNoise = new MyImage("Model с комбинированным шумом", "jpg");
            //MyImage imageWithNoise2 = new MyImage("Model с комбинированным шумом", "jpg");
            //imageWithNoise.ApplyFilter(filter_type: 0, fc1: 0.05, m: 32, mode: 2);
            //imageWithNoise.image = ConvertTypes.ShortToUshort2D(Model.getDiffImage(imageWithNoise.image, imageWithNoise2.image));
            //imageWithNoise.ThresholdTransform(150);
            //imageWithNoise.Show("Контур", false);

            //MyImage imageWithSuppAVGNoise = new MyImage("Model с подавленным усредняющим фильтром шумом", "jpg");
            //MyImage imageWithSuppAVGNoise2 = new MyImage("Model с подавленным усредняющим фильтром шумом", "jpg");
            //imageWithSuppAVGNoise.ApplyFilter(filter_type: 0, fc1: 0.05, m: 32, mode: 2);
            //imageWithSuppAVGNoise.image = ConvertTypes.ShortToUshort2D(Model.getDiffImage(imageWithSuppAVGNoise.image, imageWithSuppAVGNoise2.image));
            //imageWithSuppAVGNoise.ThresholdTransform(173);
            //imageWithSuppAVGNoise.Show("Контур", false);


            //MyImage imageWithSuppMedNoise = new MyImage("Model с подавленным медианным фильтром шумом", "jpg");
            //MyImage imageWithSuppMedNoise2 = new MyImage("Model с подавленным медианным фильтром шумом", "jpg");
            //imageWithSuppMedNoise.ApplyFilter(filter_type: 0, fc1: 0.05, m: 32, mode: 2);
            //imageWithSuppMedNoise.image = ConvertTypes.ShortToUshort2D(Model.getDiffImage(imageWithSuppMedNoise.image, imageWithSuppMedNoise2.image));
            //imageWithSuppMedNoise.ThresholdTransform(185);
            //imageWithSuppMedNoise.Show("Контур", false);

            /*
             * ****************************
             * ************ ФВЧ ***********
             * ****************************
             */

            //MyImage imageWithNoise = new MyImage("Model с комбинированным шумом", "jpg");
            //imageWithNoise.ApplyFilter(filter_type: 1, fc1: 0.05, m: 32, mode: 2);
            //imageWithNoise.ThresholdTransform(180);
            //imageWithNoise.Show("Контур", false);

            //MyImage imageWithSuppAVGNoise = new MyImage("Model с подавленным усредняющим фильтром шумом", "jpg");
            //imageWithSuppAVGNoise.ApplyFilter(filter_type: 1, fc1: 0.05, m: 32, mode: 2);
            //imageWithSuppAVGNoise.ThresholdTransform(173);
            //imageWithSuppAVGNoise.Show("Контур", false);


            MyImage imageWithSuppMedNoise = new MyImage("Model с подавленным медианным фильтром шумом", "jpg");
            imageWithSuppMedNoise.ApplyFilter(filter_type: 1, fc1: 0.05, m: 32, mode: 2);
            imageWithSuppMedNoise.ThresholdTransform(120);
            imageWithSuppMedNoise.Show("Контур", false);

        }

        /// <summary>
        /// Градиент и лапласиан 
        /// </summary>
        private void Lab11()
        {
            for (byte i = 1; i < 6; i++)
            {
                for (byte j = 0; j < 3; j++)
                {
                    string name = "Model ";
                    switch (j)
                    {
                        case 0:
                            name += "с комбинированным шумом";
                            break;
                        case 1:
                            name += "с подавленным усредняющим фильтром шумом";
                            break;
                        default:
                            name += "с подавленным медианным фильтром шумом";
                            break;
                    }
                    MyImage image = new MyImage(name, "jpg");
                    image.ApplyGradient(i, 2);
                    switch (i)
                    {
                        case 1:
                            name = "Превитт " + name;
                            break;
                        case 2:
                            name = "Собель " + name;
                            break;
                        case 3:
                            name = "Щарр " + name;
                            break;
                        case 4:
                            name = "Лаплас " + name;
                            break;
                        default:
                            name = "Лаплас c диагоналями " + name;
                            break;
                    }
                    image.Show(name);
                }
            }

            //for (byte i = 1; i < 6; i++)
            //{
            //    for (byte j = 0; j < 3; j++)
            //    {
            //        MyImage image = new MyImage("CHGK", "jpg");
            //        image.ApplyGradient(i, j);
            //        string name = "";
            //        switch (i)
            //        {
            //            case 1:
            //                name += "Превитт";
            //                break;
            //            case 2:
            //                name += "Собель";
            //                break;
            //            case 3:
            //                name += "Щарр";
            //                break;
            //            case 4:
            //                name += "Лаплас";
            //                break;
            //            default:
            //                name += "Лаплас с диагоналями";
            //                break;
            //        }
            //        switch (j)
            //        {
            //            case 0:
            //                name += " по строкам";
            //                break;
            //            case 1:
            //                name += " по столбцам";
            //                break;
            //            default:
            //                name += " двумерно";
            //                image.Save(name);
            //                break;
            //        }
            //        image.Show(name);
            //    }
            //}

            //MyImage image = new MyImage("StarDestroyer", "jpg");
            //image.ApplyGradient(0, 2);
            //image.Save("StarDestroyer Превитт");
            //image = new MyImage("StarDestroyer", "jpg");
            //image.ApplyGradient(3, 2);
            //image.Save("StarDestroyer Лаплас");
        }

        /// <summary>
        /// Эррозия и дилатация
        /// </summary>
        private void Lab12()
        {
            FileData currFile = files[8];
            //MyImage image = new MyImage(currFile.filename, currFile.extension);
            //MyImage image = new MyImage("Model с комбинированным шумом", "jpg");
            //MyImage image = new MyImage("Model с подавленным усредняющим фильтром шумом", "jpg");
            MyImage image = new MyImage("Model с подавленным медианным фильтром шумом", "jpg");

            int[,] mask = new int[3, 3]
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };

            // ДИЛАТАЦИЯ И ЭРОЗИЯ ИСХОДНОГО
            image.ApplyDilatation(mask, 200);
            image.Show("Дилатация");


            // Нужно пересоздавать, потому что си шарп передаёт ссылку на массив, а не сам массив! Злостный сишарп! Фу таким быть!
            mask = new int[3, 3]
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 1, 1, 1 }
            };

            //image = new MyImage(currFile.filename, currFile.extension);
            //image = new MyImage("Model с комбинированным шумом", "jpg");
            //image = new MyImage("Model с подавленным усредняющим фильтром шумом", "jpg");
            image = new MyImage("Model с подавленным медианным фильтром шумом", "jpg");
            image.ApplyErosion(mask, 200);
            image.Show("Эрозия");

            // ДИЛАТАЦИЯ И ЭРОЗИЯ ЗАШУМЛЕННОГО
            //image = new MyImage(currFile.filename, currFile.extension);
            //image.ApplyErosion(mask, 200);
            //image.Show("Эрозия");



        }

        private void Lab13()
        {
        }

        private void Lab14()
        {

        }
    }
}
