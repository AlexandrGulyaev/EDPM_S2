using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Lab1
{
    class MyImage
    {
        Bitmap img; // Битмап imageGS
        public ushort[,] image; // Исходное изображение, над которым проводятся все манипуляции
        ushort[,] imageGS; // Изображение в шкале серости. К шкале серости преобразуем image только для отображения/сохранения
        double[,] fourier2D;
        double[,] fourierInv2D;
        public ushort[,] temp;
        ushort min = 255;
        ushort max = 0;
        int width;
        int height;
        Dictionary<ushort, int> histogram;
        int[] CDF;
        int[] TransferScale;
        int[] InverseCDF;
        double ColumnFreq = 0;
        public double[,] Re;
        public double[,] Im;
        public double[,] imageDouble;


        /// <summary>
        /// Загрузить изображение на основе битовой карты
        /// </summary>
        /// <param name="Image">Битовая карта</param>
        public MyImage(Bitmap Image)
        {
            this.img = Image;
            Load();
        }

        /// <summary>
        /// Загрузить изображение из файловой системы
        /// </summary>
        /// <param name="filename">Полный путь до файла с его именем</param>
        /// <param name="extension">Расширение файла</param>
        public MyImage(string filename, string extension = "jpg")
        {
            filename = filename + "." + extension;
            this.img = (Bitmap)Image.FromFile(filename);
            Load();
        }

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
        /// <param name="extension">Расширение файла</param>
        /// <param name="ToGS">Конвертировать в шкалу серости:
        ///                    true  - конвертировать;
        ///                    false - не конвертировать.</param>
        public MyImage(string filename, int width = 1024, int height = 1024, int startIndex = 2048, bool swapBytes = false, string extension = "xcr")
        {
            ImageXCR imageXCR = new ImageXCR(filename, width, height, startIndex, swapBytes, extension);
            this.height = imageXCR.Image.GetLength(0);
            this.width = imageXCR.Image.GetLength(1);
            this.min = imageXCR.min;
            this.max = imageXCR.max;
            this.image = imageXCR.Image;
        }

        /// <summary>
        /// Открыть изображение .dat
        /// </summary>
        /// <param name="filename">Имя файла с расширением!</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public MyImage(string filename, int width = 1024, int height = 1024)
        {
            //ImageXCR imageXCR = new ImageXCR(filename, width, height, startIndex, swapBytes, extension);
            //this.height = imageXCR.Image.GetLength(0);
            //this.width = imageXCR.Image.GetLength(0);
            //this.min = imageXCR.min;
            //this.max = imageXCR.max;
            //this.image = imageXCR.Image;

            byte[] buffer = File.ReadAllBytes(filename);
            ushort[,] tmp = new ushort[width, height];
            this.imageDouble = new double[width, height];
            int x = 0, y = 0;

            if (height == 1)
            {
                x = (width - buffer.Length / 4) / 2;
            }

            for (int i = 0; i < buffer.Length; i += 4)
            {
                byte[] data = new byte[4];
                data[0] = buffer[i];
                data[1] = buffer[i + 1];
                data[2] = buffer[i + 2];
                data[3] = buffer[i + 3];

                this.imageDouble[x, y] = BitConverter.ToSingle(data, 0);
                tmp[x, y] = (ushort)BitConverter.ToSingle(data, 0);
                x++;

                if (x == width)
                {
                    x = 0;
                    y++;
                    if (y == height)
                    {
                        break;
                    }
                }
            }

            this.width = height;
            this.height = width;
            this.image = tmp;
            setExtremums();
        }

        /// <summary>
        /// Загрузить изображение
        /// </summary>
        private void Load()
        {
            width = img.Height;
            height = img.Width;
            image = new ushort[img.Width, img.Height];
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    image[x, y] = img.GetPixel(x, y).R;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToGS">Привести к шкале серости</param>
        public void buildImage(bool ToGS = true)
        {
            height = image.GetLength(0);
            width = image.GetLength(1);
            Bitmap temp = new Bitmap(height, width);
            if (ToGS) ConvertToGrayscale();
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    byte color = ToGS ? (byte)(imageGS[x, y]) : image[x, y] > 255 ? (byte)255 : (byte)image[x, y];
                    temp.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }
            img = temp;
        }

        /// <summary>
        /// Показать изображение
        /// </summary>
        /// <param name="ImageName">Имя изображения</param>
        /// <param name="ToGS">Привести к шкале серости</param>
        public void Show(string ImageName = "", bool ToGS = true)
        {
            buildImage(ToGS);
            FormImage formImage = new FormImage(ConvertTypes.BitmapToByte2D(img), ImageName);
        }

        /// <summary>
        /// Сохранить изображение
        /// </summary>
        /// <param name="filename">Имя изображения</param>
        /// <param name="extension">Расширение (default jpg)</param>
        public void Save(string filename, string extension = "jpg")
        {
            buildImage();
            InOut.SaveImage(img, filename, extension);
        }

        /// <summary>
        /// Определить экстремумы изображения
        /// </summary>
        public void setExtremums()
        {
            min = image[0, 0];
            max = image[0, 0];
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    ushort c = image[x, y];
                    min = c < min ? c : min;
                    max = c > max ? c : max;
                }
            }
        }

        /// <summary>
        /// Преобразовать к шкале серости
        /// </summary>
        public void ConvertToGrayscale()
        {
            setExtremums();
            imageGS = new ushort[height, width];
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    imageGS[x, y] = (byte)((double)(image[x, y] - min) / (double)(max - min) * 255.0);
                }
            }
        }

        /// <summary>
        /// Изменить интенсивность цвета (сдвинуть цвет)
        /// </summary>
        /// <param name="shift">Величина сдвига</param>
        public void shiftImage(int shift)
        {
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    image[x, y] += (ushort)shift;
                }
            }
        }

        /// <summary>
        /// Изменить интенсивность цвета в несколько раз
        /// </summary>
        /// <param name="coeff">Коэффициент изменения</param>
        public void intensifyImage(double coeff)
        {
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    image[x, y] = (ushort)(image[x, y] * coeff);
                }
            }
        }

        /// <summary>
        /// Изменить размер изображения
        /// </summary>
        /// <param name="coeff">Коэффициент изменения размера</param>
        /// <param name="nearestNeighbor">Режим изменения размера изображения:
        ///                    true - метод ближайшего соседа;
        ///                    false - метод билинейной интерполяции.</param>
        public void Resize(double coeff, bool nearestNeighbor = true)
        {
            image = Model.ResizeImage(ConvertTypes.BitmapToByte2D(img), coeff, nearestNeighbor);
            height = image.GetLength(0);
            width = image.GetLength(1);
        } 
        
        /// <summary>
        /// Повернуть изображение
        /// </summary>
        /// <param name="cw">Режим поворота:
        ///                  true - по часовой стрелке (CW);
        ///                  false - против часовой стрелки (CCW).</param>
        /// <param name="angle">Угол поворота в градусах(кратно 90)</param>
        public void Rotate(bool cw = true, int angle = 90)
        {
            ushort[,] temp;
            int width = image.GetLength(0);
            int height = image.GetLength(1);
            angle %= 360;
            if (!cw) angle = 360 - angle;
            switch (angle)
            {
                case 90:
                    temp = new ushort[height, width];
                    // Вращение
                    for (int x = 0; x < image.GetLength(0); x++)
                    {
                        for (int y = 0; y < image.GetLength(1); y++)
                        {
                            temp[temp.GetLength(0) - y - 1, x] = image[x, y];
                        }
                    }
                    break;
                case 180:
                    temp = new ushort[width, height];
                    for (int x = 0; x < image.GetLength(0); x++)
                    {
                        for (int y = 0; y < image.GetLength(1); y++)
                        {
                            temp[y, x] = image[x, y];
                        }
                    }
                    break;
                case 270:
                    temp = new ushort[height, width];
                    for (int x = 0; x < image.GetLength(0); x++)
                    {
                        for (int y = 0; y < image.GetLength(1); y++)
                        {
                            temp[y, temp.GetLength(1) - x - 1] = image[x, y];
                        }
                    }
                    break;
                default:
                    temp = image;
                    break;
            }
            image = temp;
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

        /// <summary>
        /// Получить гистограму изображения
        /// </summary>
        public void GetHistogram()
        {
            histogram = Model.GetHistogram(image);
        }

        /// <summary>
        /// Отобразить гистограму изображения
        /// </summary>
        /// <param name="rebuildHistogram">Построить гистограму заново</param>
        public void ShowHistogram(bool rebuildHistogram = true, string name = "")
        {
            if (rebuildHistogram) GetHistogram();
            InOut.ShowHistogram(histogram, name);
        }

        /// <summary>
        /// Получить функцию распределения для изображения
        /// </summary>
        public void GetCDF()
        {
            CDF = Model.GetCDF(histogram);
        }

        /// <summary>
        /// Отобразить функцию распределения
        /// </summary>
        /// <param name="rebuildCDF">Построить функцию распределения заново</param>
        /// <param name="mode">0 - отобразить функцию распределения исходного изображения
        ///                    1 - отобразить функцию распределения эквализированного изображения
        ///                    2 - отобразить обратную функцию распределения</param>
        public void ShowCDF(bool rebuildCDF = true, byte mode = 0)
        {
            if (rebuildCDF) GetCDF();
            switch (mode)
            {
                case 0:
                    InOut.ShowCDF(CDF);
                    break;
                case 1:
                    InOut.ShowCDF(TransferScale, " передаточная шкала");
                    break;
                case 2:
                    InOut.ShowCDF(InverseCDF, "inverse");
                    break;
            }
        }

        /// <summary>
        /// Получить шкалу передачи (выполнить эквализацию гистограмы)
        /// </summary>
        public void GetTransferScale()
        {
            CDF = Model.GetCDF(histogram);
            TransferScale = Model.GetTransferScale(CDF);
        }

        /// <summary>
        /// Выполнить замену пикселей на основе функции распределения изображения
        /// </summary>
        /// <param name="mode">true  - эквализированная CDF
        ///                    false - обратная CDF</param>
        public void ReplacePixelsFromCDF(bool mode = true)
        {
            image = Model.ReplacePixelsFromCDF(image, mode ? TransferScale : InverseCDF);
        }

        /// <summary>
        /// Получить функцию, обратную функции распределения
        /// </summary>
        public void GetInverseCDF()
        {
            InverseCDF = Model.GetInverseFunction(TransferScale);
        }

        /// <summary>
        /// Обнаружить артефакты рентгеновской сетки
        /// </summary>
        public void DetectArtifacts()
        {
            ColumnFreq = 0;
            // Получить частоту колонок для подавления 
            {
                int strStep = (int)(width * 0.1);
                int index = 0;
                int counter = 0;

                while (index < width)
                {
                    // Извлекаем пиксельную строку
                    double[] str = new double[height];
                    for (int i = 0; i < height; i++)
                    {
                        str[i] = image[i, index];
                    }

                    // Получаем производную строки
                    double[] derivative = Model.GetDerivative(str);

                    // Строим АКФ производной
                    double[] AKF = new double[height];
                    for (int i = 0; i < height; i++)
                    {
                        AKF[i] = Analysis.AKF(derivative, i);
                    }

                    // Строим спектр АКФ производной
                    double[] AKFFourier = Model.GetAmplitudeSpectrum(AKF);

                    // Для первой строки построим спектр её производной и отобразим спектры производной и АКФ производной
                    if (index == 0)
                    {
                        // Строим спектр производной
                        double[] DerivativeFourier = Model.GetAmplitudeSpectrum(derivative);

                        // Отобразим спектры
                        FormChart ChartFourierDer = new FormChart(DerivativeFourier, 
                                                                  FormName: "Спектр производной", 
                                                                  size: DerivativeFourier.Count() / 2,
                                                                  step: 1.0 / (double)DerivativeFourier.Count());
                        FormChart ChartFourierAKF = new FormChart(AKFFourier, 
                                                                  FormName: "Спектр АКФ производной", 
                                                                  size: DerivativeFourier.Count() / 2,
                                                                  step: 1.0 / (double)DerivativeFourier.Count());

                    }

                    double[] halfAKFFourier = new double[AKFFourier.Count() / 2];
                    Array.Copy(AKFFourier, halfAKFFourier, AKFFourier.Count() / 2);
                    // Получаем частоту для дальнейшего подавления
                    ColumnFreq = Analysis.getMaxIndex(halfAKFFourier);

                    counter++;
                    index += strStep;
                }

                ColumnFreq = (double)ColumnFreq / (double)height;
            }
        }

        /// <summary>
        /// Подавить артефакты
        /// </summary>
        public void SuppressArtifacts()
        {
            DetectArtifacts();

            // Подавляем сетку
            double fc1 = ColumnFreq * 0.92;
            double fc2 = ColumnFreq * 1.08;
            double dt = 1;
            int m = 32;
            double[] BSF = Model.BSF(Model.LPF(m, fc1, dt), Model.LPF(m, fc2, dt));

            for (int i = 0; i < width; i++)
            {
                double[] str = new double[height];
                for (int j = 0; j < height; j++)
                {
                    str[j] = image[j, i];
                }

                double[] old_str = str;
                str = Model.Convolution(str, BSF);

                if (i == 0)
                {
                    double[] der = Model.GetDerivative(old_str);
                    FormChart chart = new FormChart(Model.GetAmplitudeSpectrum(der),
                                                    FormName: "Спектр производной старой строки",
                                                    size: der.Count() / 2,
                                                    step: 1.0 / (double)der.Count());
                    der = Model.GetDerivative(str);
                    FormChart chart2 = new FormChart(Model.GetAmplitudeSpectrum(der),
                                                    FormName: "Спектр производной новой строки",
                                                    size: der.Count() / 2,
                                                    step: 1.0 / (double)der.Count());
                }

                int startIndex = (str.Count() - height) / 2;
                for (int j = startIndex; j < height + startIndex; j++)
                {
                    image[j - startIndex, i] = (ushort)(str[j]);
                }
            }

            setExtremums();
        }

        /// <summary>
        /// Наложить шум на изображение
        /// </summary>
        /// <param name="type">Тип шума:
        ///                    0 - cоль-перец;
        ///                    1 - cлучайный шум;
        ///                    2 - комбинированный шум</param>
        /// <param name="scale">Точность случайного шума (количество знаков после запятой)</param>
        /// <param name="frequency">Частота шума соль-перец (от 0 до 1)</param>
        /// <param name="coeff">Коэффициент случайного шума</param>
        /// <returns>Сгенерированный шум</returns>
        public void AddNoise(byte type, int scale = 1, double frequency = 0.1, double coeff = 25)
        {
            setExtremums();
            double[] noise = Model.getNoise(height * width, type, scale, frequency, coeff);
            int index = 0;
            double value;
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    switch (type)
                    {
                        case 0:
                            image[x, y] = (ushort)(noise[index] == -10000 ? 255 : noise[index] == -20000 ? 0 : image[x, y]);
                            break;
                        case 1:
                            value = (double)image[x, y] + noise[index];
                            value = value > 255 ? 255 : value < 0 ? 0 : value;
                            image[x, y] = (ushort)value;
                            break;
                        case 2:
                            if (noise[index] == -10000 || noise[index] == -20000)
                            {
                                image[x, y] = (ushort)(noise[index] == -10000 ? 255 : noise[index] == -20000 ? 0 : image[x, y]);
                            }
                            else
                            {
                                value = (double)image[x, y] + noise[index];
                                value = value > 255 ? 255 : value < 0 ? 0 : value;
                                image[x, y] = (ushort)value;
                            }
                            break;
                    }
                    //image[x, y] = image[x, y] > max ? max : image[x, y];
                    index++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode">true - медианный фильтр</param>
        /// <param name="sizeMask"></param>
        public void SuppressNoise(bool mode = true, int sizeMask = 3)
        {
            ushort[,] new_image = new ushort[image.GetLength(0), image.GetLength(1)];
            for (int x = 1; x < image.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < image.GetLength(1) - 1; y++)
                {
                    List<ushort> lst = new List<ushort>();
                    if (sizeMask == 3)
                    { 
                        lst.Add(image[x - 1, y - 1]);
                        lst.Add(image[x - 1, y]);
                        lst.Add(image[x - 1, y + 1]);
                        lst.Add(image[x, y - 1]);
                        lst.Add(image[x, y]);
                        lst.Add(image[x, y + 1]);
                        lst.Add(image[x + 1, y - 1]);
                        lst.Add(image[x + 1, y]);
                        lst.Add(image[x + 1, y + 1]);
                    }
                    else
                    {
                        lst.Add(image[x - 1, y - 1]);
                        lst.Add(image[x - 1, y]);
                        lst.Add(image[x, y - 1]);
                        lst.Add(image[x, y]);
                    }
                    if (mode)
                    { 
                        lst.Sort();
                        new_image[x, y] = sizeMask == 3 ? lst[4] : (ushort)((lst[1] + lst[2]) / 2);
                    }
                    else
                    {
                        ushort mean = 0;
                        foreach (ushort val in lst)
                        {
                            mean += val;
                        }
                        mean = (ushort)(mean / lst.Count());

                        new_image[x, y] = mean;
                    }
                }
            }
            image = new_image;
        }

        public void getInverseFourier()
        {
            double[] str = new double[image.GetLength(1)];
            for (int i = 0; i < image.GetLength(1); i++)
            {
                str[i] = image[240, i];
            }
            FormChart chartStr = new FormChart(str, FormName: "Строка");

            double[] fourier = Model.getInverseFourier(str);
            FormChart chartFourier = new FormChart(fourier, FormName: "Фурье строки");

            double[] invFourier = Model.getInverseFourier(fourier);
            FormChart chartInvFourier = new FormChart(invFourier, FormName: "Обратный фурье строки");
        }


        /// <summary>
        /// Выполнить 2D преобразование Фурье
        /// </summary>
        /// <param name="inverse">Обратное преобразование</param>
        public void getFourier2D(bool inverse = false, bool need2D = true)
        {
            int size1 = inverse ? fourier2D.GetLength(1) : image.GetLength(1);
            int size2 = inverse ? fourier2D.GetLength(0) : image.GetLength(0);

            if (inverse)
            { 
                fourierInv2D = new double[size2, size1];
            }
            else
            {
                fourier2D = new double[size2, size1];
            }
            Re = new double[size2, size1];
            Im = new double[size2, size1];


            double[,] fourier2Dtemp = new double[size2, size1];

            // Преобразование Фурье 1D
            for (int y = 0; y < size2; y++)
            {
                double[] str = new double[size1];
                for (int x = 0; x < size1; x++)
                {
                    str[x] = inverse ? fourier2D[y, x] : image[y, x];
                }
                for (int x = 0; x < size1; x++)
                {
                    Re[y, x] = Model.Re(str, x);
                    Im[y, x] = Model.Im(str, x);
                }
                double[] fourier = Model.getInverseFourier(str);
                for (int x = 0; x < size1; x++)
                {
                    fourier2Dtemp[y, x] = fourier[x];
                }
            }

            // Преобразование Фурье 2D
            for (int x = 0; x < size1; x++)
            {
                double[] column = new double[size2];
                for (int y = 0; y < size2; y++)
                {
                    column[y] = fourier2Dtemp[y, x];
                }
                if (need2D)
                { 
                    for (int y = 0; y < size2; y++)
                    {
                        Re[y, x] = Model.Re(column, y);
                        Im[y, x] = Model.Im(column, y);
                    }
                }

                double[] fourier = Model.getInverseFourier(column);
                for (int y = 0; y < size2; y++)
                {
                    if (inverse)
                        fourierInv2D[y, x] = fourier[y];
                    else
                        fourier2D[y, x] = fourier[y];
                }
            }
        }

        public void ShowFourier2D(string ImageName = "Фурье 2D", bool ToGS = false)
        {
            temp = image;
            double maxf = Math.Abs(fourier2D[0, 0]);
            double minf = Math.Abs(fourier2D[0, 0]);
            
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    maxf = fourier2D[x, y] > maxf ? fourier2D[x, y] : maxf;
                    minf = fourier2D[x, y] < minf ? fourier2D[x, y] : minf;
                    //image[x, y] = (ushort)fourier2D[x, y];
                }
            }
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    maxf = Math.Abs(fourier2D[x, y]) > maxf ? Math.Abs(fourier2D[x, y]) : maxf;
                    minf = Math.Abs(fourier2D[x, y]) < minf ? Math.Abs(fourier2D[x, y]) : minf;
                    image[x, y] = (ushort)((Math.Abs(fourier2D[x, y]) - minf) / (maxf - minf) * 255);
                }
            }
            Show(ImageName, ToGS);
            image = temp;
        }

        public void ShowFourierInv2D(string ImageName = "Фурье обратный 2D")
        {
            temp = image;
            double maxf = Math.Abs(fourierInv2D[0, 0]);
            double minf = Math.Abs(fourierInv2D[0, 0]);
            ushort[,] temp_image = new ushort[fourierInv2D.GetLength(0), fourierInv2D.GetLength(1)];

            for (int x = 0; x < fourierInv2D.GetLength(0); x++)
            {
                for (int y = 0; y < fourierInv2D.GetLength(1); y++)
                {
                    maxf = fourierInv2D[x, y] > maxf ? fourierInv2D[x, y] : maxf;
                    minf = fourierInv2D[x, y] < minf ? fourierInv2D[x, y] : minf;
                    //image[x, y] = (ushort)fourier2D[x, y];
                }
            }
            for (int x = 0; x < fourierInv2D.GetLength(0); x++)
            {
                for (int y = 0; y < fourierInv2D.GetLength(1); y++)
                {
                    maxf = fourierInv2D[x, y] > maxf ? fourierInv2D[x, y] : maxf;
                    minf = fourierInv2D[x, y] < minf ? fourierInv2D[x, y] : minf;
                    temp_image[x, y] = (ushort)((fourierInv2D[x, y] - minf) / (maxf - minf) * 255);
                }
            }
            image = temp_image;
            Show(ImageName);
            image = temp;
            temp = temp_image;
        }

        public void ResizeFourier2D(double coeff)
        {
            int new_size1 = (int)(fourier2D.GetLength(0) * coeff);
            int new_size2 = (int)(fourier2D.GetLength(1) * coeff);
            int size1 = fourier2D.GetLength(0);
            int size2 = fourier2D.GetLength(1);
            double[,] new_fourier2D = new double[new_size1, new_size2];
            int indexX = 0, indexY = 0;
            for (int x = 0; x < new_size1; x++)
            {
                for (int y = 0; y < new_size2; y++)
                {
                    if ((x < size1 / 2 || x > (new_size1 - size1 / 2 - 1)) && (y < size2 / 2 || y > (new_size2 - size2 / 2 - 1)))
                    {
                        new_fourier2D[x, y] = fourier2D[indexX, indexY];
                        indexY++;
                    }
                }
                indexY = 0;
                if (x < size1 / 2 || x > (new_size1 - size1 / 2 - 1))
                {
                    indexX++;
                }
            }
            fourier2D = new_fourier2D;
        }

        /// <summary>
        /// Применить к изображению фильтр
        /// </summary>
        /// <param name="filter_type">Применяемый фильтр: 0 - ФНЧ; 1 - ФВЧ; 2 - ПФ; 3 - РФ</param>
        /// <param name="fc1">Частота среза 1</param>
        /// <param name="fc2">Частота среза 2 (для ПФ и РФ)</param>
        /// <param name="mode">Режим фильтрации: 0 - построчно; 1 - постолбцово; 2 - 2D-фильтрация</param>
        public void ApplyFilter(byte filter_type = 0, double fc1 = 0, double fc2 = 0, int m = 32, byte mode = 0)
        {
            double[] filter;
            double[] LPF1 = Model.LPF(m, fc1, 1);
            // TODO: сделать для ПФ и РФ
            switch (filter_type)
            {
                case 0:
                    filter = LPF1;
                    break;
                case 1:
                    filter = Model.HPF(LPF1);
                    break;
                default:
                    filter = LPF1;
                    break;
            }
            image = Model.applyFilter(image, filter, mode);
        }

        /// <summary>
        /// Пороговое преобразование
        /// </summary>
        /// <param name="threshold">Порог</param>
        /// <param name="toZero">Режим порогового преобразования:
        ///     false - бинаризация;
        ///     true - к нулю</param>
        /// <param name="invert">true - инвертированное; false - стандартное</param>
        public void ThresholdTransform(ushort threshold, bool invert = false, bool toZero = false)
        {
            if (!toZero)
            {
                int color1 = invert ? 0 : 255;
                int color2 = invert ? 255 : 0;
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    for (int y = 0; y < image.GetLength(1); y++)
                    {
                        image[x, y] = (ushort)(image[x, y] > threshold ? color1 : color2);
                    }
                }
            }
            else
            {
                for (int x = 0; x < image.GetLength(0); x++)
                {
                    for (int y = 0; y < image.GetLength(1); y++)
                    {
                        int color1 = invert ? image[x, y] : 0;
                        int color2 = invert ? 0 : image[x, y];
                        image[x, y] = (ushort)(image[x, y] > threshold ? color1 : color2);
                    }
                }
            }
        }

        /// <summary>
        /// Применить градиент к изображению
        /// </summary>
        /// <param name="gradient_type">
        ///    1 - оператор Превитта
        ///    2 - оператор Собеля
        ///    3 - оператор Щарра
        ///    4 - оператор Лапласа
        ///    5 - оператор Лапласа с диагоналями
        /// </param>
        /// <param name="direction">Направление:
        ///    0 - вдоль строк
        ///    1 - вдоль столбцов
        ///    2 - двумерное</param>
        public void ApplyGradient(byte gradient_type = 1, byte direction = 2)
        {
            int[,] maskX, maskY;
            switch (gradient_type)
            {
                case 1:
                    // Ядро Превитта
                    maskX = new int[3, 3]
                    {
                        { -1, -1, -1 },
                        {  0,  0,  0 },
                        {  1,  1,  1 }
                    };

                    maskY = new int[3, 3]
                    {
                        { -1, 0, 1 },
                        { -1, 0, 1 },
                        { -1, 0, 1 }
                    };
                    break;
                case 2:
                    // Ядро Собеля
                    maskX = new int[3, 3]
                    {
                        { -1, -2, -1 },
                        {  0,  0,  0 },
                        {  1,  2,  1 }
                    };

                    maskY = new int[3, 3]
                    {
                        { -1, 0, 1 },
                        { -2, 0, 2 },
                        { -1, 0, 1 }
                    };
                    break;
                case 3:
                    // Ядро Щарра
                    maskX = new int[3, 3]
                    {
                        {  3,  10,  3 },
                        {  0,   0,  0 },
                        { -3, -10, -3 }
                    };

                    maskY = new int[3, 3]
                    {
                        {  3, 0,  -3 },
                        { 10, 0, -10 },
                        {  3, 0,  -3 }
                    };
                    break;
                case 4:
                    // Ядро Лапласа
                    maskX = new int[3, 3]
                    {
                        { 0,  1, 0 },
                        { 1, -4, 1 },
                        { 0,  1, 0 }
                    };

                    maskY = maskX;
                    break;
                default:
                    // Ядро Лапласа с диагоналями
                    maskX = new int[3, 3]
                    {
                        {  1,  1, 1 },
                        {  1, -8, 1 },
                        {  1,  1, 1 }
                    };

                    maskY = maskX;
                    break;
            }

            image = Model.applyGradient(image, maskX, maskY, direction);
        }


        /// <summary>
        /// Применить дилатацию
        /// </summary>
        /// <param name="mask">Маска</param>
        /// <param name="threshold">Порог для бинаризации</param>
        public void ApplyDilatation(int[,] mask, ushort threshold = 128)
        {
            ThresholdTransform(threshold);
            image = Model.ApplyMorphologicalOperation(image, mask, true);
        }

        /// <summary>
        /// Применить дилатацию
        /// </summary>
        /// <param name="mask">Маска</param>
        /// <param name="threshold">Порог для бинаризации</param>
        public void ApplyErosion(int[,] mask, ushort threshold = 128)
        {
            ThresholdTransform(threshold, true);
            image = Model.ApplyMorphologicalOperation(image, mask, false);
        }

        public void swapHalfes()
        {
            int w = image.GetLength(0);
            int h = image.GetLength(1);
            ushort[,] tmp = new ushort[w, h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x <= w / 2)
                    {
                        tmp[x + w / 2, y] = image[x, y];
                    }
                    else
                    {
                        tmp[x - w / 2, y] = image[x, y];
                    }
                }
            }
            image = tmp;
        }
    }
}
