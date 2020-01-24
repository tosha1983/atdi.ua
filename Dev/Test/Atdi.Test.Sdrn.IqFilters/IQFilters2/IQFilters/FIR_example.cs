using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQFilters
{
    class FIR_example
    {
        //public static readonly 
        const byte RECTANGULAR = 0;// Rectangular window function
        const byte BARTLETT = 1;// Bartlett (triangular) window
        const byte HANNING = 2;// Hanning window
        const byte HAMMING = 3;// Hamming window
        const byte BLACKMAN = 4;// Blackman window
        const byte BLACKMAN_HARRIS = 5;// Blackman-Harris window
        const byte BLACKMAN_NUTTAL = 6;// Blackman-Nuttal window
        const byte NUTTAL = 7;// Nuttal window
                                               //
        const byte OFF = 0;
        const byte LOWPASS = 1;
        const byte HIGHPASS = 2;
        const byte BANDSTOP = 3;// NOTCH
        const byte BANDPASS = 4;
        //
        const int GRIDDENSITY = 4;//16
        const int MAXITERATIONS = 40;
        //
        private short[] m_delay;
        public double[] m_fir;

        //
        public FIR_example()
        {
        }

        ///
        /// <summary>получить коэффициент заданного типа окна</summary>
        /// <param name="i">индекс</param>
        ///@param n- размер окна
        ///@param type- тип окна
        ///<returns>коэффициент</returns>
        ///
        static double getWindow(int i, int n, byte window)
        {
            if (window == BARTLETT)
            {// устраняем нулевые значения
                double a = i - (n - 1) / 2.0;
                if (a < 0)
                    a = -a;
                return 2.0 / n * (n / 2.0 - a);
            }
            else if (window == HANNING)// устраняем нулевые значения
                return 0.5 - 0.5 * Math.Cos(Math.PI / n * (1.0 + 2.0 * i));
            if (window == BLACKMAN)
            {// устраняем нулевые значения
                double a = Math.PI / n * (1.0 + 2.0 * i);
                return 0.5 * (1.0 - 0.16 - Math.Cos(a) + 0.16 * Math.Cos(2.0 * a));
            }
            else
            {
                double a = 2.0 * Math.PI * i / (n - 1);
                if (window == HAMMING)
                    return 0.54 - 0.46 * Math.Cos(a);
                else if (window == BLACKMAN_HARRIS)
                    return 0.35875 - 0.48829 * Math.Cos(a) + 0.14128 * Math.Cos(2.0 * a) - 0.01168 * Math.Cos(3.0 * a);
                else if (window == BLACKMAN_NUTTAL)
                    return 0.35819 - 0.4891775 * Math.Cos(a) + 0.1365995 * Math.Cos(2.0 * a)
                            - 0.0106411 * Math.Cos(3.0 * a);
                else if (window == NUTTAL)
                    return 0.355768 - 0.487396 * Math.Cos(a) + 0.144232 * Math.Cos(2.0 * a) - 0.012604 * Math.Cos(3.0 * a);
            }
            // if( type == RECTANGULAR )
            return 1.0;
        }

        /**
         * инициализация параметров
         * 
         * @param type
         *            - тип фильтра
         * @param window
         *            - окно
         * @param order
         *            - порядок фильтра
         * @param f1
         *            - частота ФНЧ и ФВЧ фильтра
         * @param f2
         *            - верхняя частота для полосового и режекторного фильтра
         * @param sampleRate
         *            - частота дискретизации
         */
        public void init(byte type, byte window, short order, int f1, int f2, int sampleRate)
        {
            m_fir = new double[order];
            m_delay = new short[order];// создаём и обнуляем буфер данных
            if (order == 1)
            {
                m_fir[0] = 1.0;
                return;
            }
            int n2 = order / 2;
            double w = 2.0 * Math.PI * (double)f1 / (double)sampleRate;
            double sum = 0;
            /*
             * расчёт симметричной характеристики для ФНЧ double c = (order - 1) / 2.0; if(
             * (order&1) != 0 ) { m_fir[n2] = w * getWindow( n2, order, BLACKMAN ); sum +=
             * m_fir[n2]; } for( int i = 0; i < n2; i++ ) { double d = (double)i - c;
             * m_fir[i] = Math.Sin(w * d) / d * getWindow( i, order, window ); sum +=
             * m_fir[i]; sum += m_fir[i]; } // нормализация if( (order&1) != 0 ) m_fir[n2]
             * /= sum; for( int i = 0; i < n2; i++ ) m_fir[i] = m_fir[order - i - 1] =
             * m_fir[i] / sum;
             */
            for (int i = 0; i < order; i++)
            {
                int d = i - n2;
                m_fir[i] = ((d == 0) ? w : Math.Sin(w * d) / d) * getWindow(i, order, window);
                sum += m_fir[i];
            }
            // нормализация
            for (int i = 0; i < order; i++)
            {
                m_fir[i] /= sum;
            }
            //
            if (type == LOWPASS)
                return;
            else if (type == HIGHPASS)
            {
                for (int i = 0; i < order; i++)
                {
                    m_fir[i] = -m_fir[i];
                }
                m_fir[n2] += 1.0;
                return;
            }
            else
            {// если полосовой или режекторный фильтр
             // расчитываем верхнюю частоту
                double[] hf = new double[order];
                w = 2.0 * Math.PI * (double)f2 / (double)sampleRate;
                sum = 0;
                for (int i = 0; i < order; i++)
                {
                    int d = i - n2;
                    hf[i] = ((d == 0) ? w : Math.Sin(w * d) / d) * getWindow(i, order, window);
                    sum += hf[i];
                }
                // нормализация
                for (int i = 0; i < order; i++)
                {
                    hf[i] /= sum;
                }
                // инвертируем и объединяем с ФНЧ
                for (int i = 0; i < order; i++)
                {
                    m_fir[i] -= hf[i];
                }
                m_fir[n2] += 1.0;
                if (type == BANDSTOP)
                    return;
                else if (type == BANDPASS)
                {
                    for (int i = 0; i < order; i++)
                    {
                        m_fir[i] = -m_fir[i];
                    }
                    m_fir[n2] += 1.0;
                }
            }
        }

        /**
         * инициализация параметров
         * 
         * @param env
         *            - огибающая фильтра, коэффициенты передачи. длина массива должна
         *            быть степенью числа 2
         * @param order
         *            - порядок фильтра, меньше удвоенной длины массива огибающей
         * @param window
         *            - тип окна
         */
        public bool init(double[] env, short order, byte window)
        {
            if (order >= (env.Length << 1))
            {
                //System.out.println("filter order must be lower than filter envelope length");
                return false;
            }
            if ((env.Length & (env.Length - 1)) != 0)
            {
                //System.out.println("filter envelope length must be power of 2");
                return false;
            }
            // расчитываем импульсную характеристику
            double[] buf = new double[env.Length << 2];
            // обнуляем фазу и центральный элемент (в java буфер уже обнулён)
            // for( int i = 1; i < (env.Length << 2); i += 2 ) { buf[i] = 0.0; }
            // buf[env.Length << 1] = 0.0;
            // готовим симметричный буфер
            buf[0] = env[0];
            for (int i = 1; i < env.Length; i++)
            {
                buf[i << 1] = buf[(env.Length << 2) - (i << 1)] = env[i];
            }
            // обратное БПФ
            int power = 0;
            for (int i = 1; i < (env.Length << 2); power++)
            {
                i = i << 1;
            }
            //		FFT.realFastFourierTransform(env, power, true);
            // забираем импульсную характеристику и применяем оконную функцию
            m_fir = new double[order];
            for (int i = 0; i < order; i++)
            {
                int m = (i - (order >> 1)) << 1;
                m_fir[i] = buf[m & ((env.Length << 2) - 1)] * getWindow(i, order, window);
            }
            // создаём и обнуляем буфер данных
            m_delay = new short[order];// в java уже обнулён
                                       // for( int i = 0; i < order; i++ ) { m_delay[i] = 0; }
            return true;
        }

        /**************************************************************************
         * Алгоритм Паркса-Макклеллана для расчёта КИХ фильтра (версия на Си) Copyright
         * (C) 1995 Jake Janovetz (janovetz@coewl.cen.uiuc.edu) Converted to Java by
         * Iain A Robin, June 1998
         *************************************************************************/
        /**
         * оцениваем необходимый порядок фильтра с помощью формулы Кайзера
         * 
         * @param trband
         *            - нормализованная переходная полоса фильтра
         * @param atten_dB
         *            - степень подавления в дБ.
         * @param ripple_dB
         *            - пульсации в полосе пропускания дБ
         */
        public static int estimatedOrder(double trband, double atten_dB, double ripple_dB)
        {
            double deltaP = 0.5 * (1.0 - Math.Pow(10.0, -0.05 * ripple_dB));
            double deltaS = Math.Pow(10.0, -0.05 * atten_dB);
            double test = Math.Log10(deltaP * deltaS);
            double order = Math.Round((-10.0 * Math.Log10(deltaP * deltaS) - 13) / (14.6 * trband));//Math.Round((float)((-10.0 * Math.Log10(deltaP * deltaS) - 13) / (14.6 * trband)))
            return (int)order;
        }

        /*******************
         * createDenseGrid ================= Создёт плотную сетку частот из указанных
         * диапазонов. Также создаёт функцию Требуемого Частотного Отклика (d[]) и
         * Весовую функцию (w[]) этой сетки
         *
         * @param r
         *            - число коэффициентов фильтра / 2
         * @param numtaps
         *            - число коэффициентов в результирующем фильтре
         * @param numband
         *            - число диапазонов, заданных пользователем
         * @param bands
         *            - число границ диапазонов, заданных пользователем [2*numband]
         * @param des
         *            - желаемый отклик в каждом диапазоне [numband]
         * @param weight
         *            - вес в каждом диапазоне [numband]
         * @param symmetry
         *            - симметрия фильтра - используется для проверки сетки
         * @param gridSize
         *            - число элементов в плотной сетке частот
         *
         * @return grid - частоты (от 0 до 0.5) плотной сетки [gridSize]
         * @return d - желаемый отклик плотной сетки [gridSize]
         * @return w - весовая функция плотной сетки [gridSize]
         *******************/
        private static void createDenseGrid(int r, int numtaps, int numband, double[] bands, double[] des, double[] weight,
                int gridSize, double[] grid, double[] d, double[] w, bool is_symmetry)
        {
            double delf = 0.5 / (GRIDDENSITY * r);

            // Для дифференциатора, гильберта,
            // симметрия является нечётной и grid[0] = max( delf, band[0] )
            //if (!is_symmetry && (delf > bands[0]))
            //    bands[0] = delf;

            //for (int band = 0, j = 0; band < numband; band++)
            //{
            //    int band2 = band << 1;
            //    grid[j] = bands[band2];
            //    double lowf = bands[band2];
            //    double highf = bands[++band2];
            //    int k = (int)Math.Round((highf - lowf) / delf);
            //    for (int i = 0; i < k; i++)
            //    {
            //        d[j] = des[band];
            //        w[j] = weight[band];
            //        grid[j] = lowf;
            //        lowf += delf;
            //        j++;
            //    }
            //    if (j > 0)  grid[j - 1] = highf;
            //}

            if (!is_symmetry && (delf > bands[0]))
                bands[0] = delf;

            for (int band = 0, j = 0; band < numband; band++)
            {
                grid[j] = bands[band*2];
                double lowf = bands[band*2];
                double highf = bands[band*2 + 1];
                int k = (int)Math.Ceiling((highf - lowf) / delf);
                for (int i = 0; i < k; i++)
                {
                    d[j] = des[band];
                    w[j] = weight[band];
                    grid[j] = lowf;
                    lowf += delf;
                    j++;
                }
                grid[j - 1] = highf;
            }


            // Аналогично вышесказанному, если симметрия нечётна,
            // последний указатель сетки не может быть .5
            // - но, если число отводов чётное, оставляем указатель сетки в .5
            if (!is_symmetry && (grid[gridSize - 1] > (0.5 - delf)) && ((numtaps & 1) != 0))
            {
                grid[gridSize - 1] = 0.5 - delf;
            }
        }

        /********************
         * initialGuess ============== Размещает Экстремальные Частоты равномерно по
         * всей плотной сетке.
         *
         * @param r
         *            - число коэффициентов фильтра / 2
         * @param gridSize
         *            - число элементов в плотной сетке частот
         *
         * @return ext[] - указатели на экстремумы в плотной сетке частот [r+1]
         ********************/
        private static void initialGuess(int r, int[] ext, int gridSize)
        {
            for (int i = 0; i <= r; i++)
                ext[i] = i * (gridSize - 1) / r;
        }

        /***********************
         * calcParms ===========
         *
         * @param r
         *            - число коэффициентов фильтра / 2
         * @param ext
         *            - Указатели на экстремумы в плотной сетке частот [r+1]
         * @param grid
         *            - частоты (от 0 до 0.5) плотной сетки [gridSize]
         * @param d
         *            - желаемый отклик плотной сетки [gridSize]
         * @param w
         *            - весовая функция плотной сетки [gridSize]
         *
         * @return ad - 'b' в книге Оппенгейма & Шафера [r+1]
         * @return x - [r+1]
         * @return y - 'C' в книге Оппенгейма & Шафера [r+1]
         ***********************/
        private static void calcParms(int r, int[] ext, double[] grid, double[] d, double[] w, double[] ad, double[] x,
                double[] y)
        {
            double denom, numer;

            // Расчитываем x[]
            for (int i = 0; i <= r; i++)
                x[i] = Math.Cos(2.0 * Math.PI * grid[ext[i]]);

            // Расчитываем ad[] - уравнение 7.132 в книге Оппенгейма & Шафера
            int ld = (r - 1) / 15 + 1;// Переход вокруг, чтобы избежать ошибок округления
            for (int i = 0; i <= r; i++)
            {
                denom = 1.0;
                double xi = x[i];
                for (int j = 0; j < ld; j++)
                {
                    for (int k = j; k <= r; k += ld)
                        if (k != i)
                            denom *= 2.0 * (xi - x[k]);
                }
                if (Math.Abs(denom) < 0.00001)
                    denom = 0.00001;
                ad[i] = 1.0 / denom;
            }

            // Расчитываем delta - уравнение 7.131 в книге Оппенгейма & Шафера
            numer = denom = 0;
            double sign = 1;
            for (int i = 0; i <= r; i++)
            {
                numer += ad[i] * d[ext[i]];
                denom += sign * ad[i] / w[ext[i]];
                sign = -sign;
            }
            double delta = numer / denom;
            sign = 1;

            // Расчитываем y[] - уравнение 7.133b в книге Оппенгейма & Шафера
            for (int i = 0; i <= r; i++)
            {
                y[i] = d[ext[i]] - sign * delta / w[ext[i]];
                sign = -sign;
            }
        }

        /*********************
         * computeA ========== Используя значения, расчитанные в CalcParms, ComputeA
         * расчитывает действительный отклик фильтра на заданной частоте (freq).
         * Используется уравнение 7.133a из книги Оппенгейма & Шафера.
         *
         * @param freq
         *            - частота (от 0 до 0.5) для которой расчитывается A
         * @param r
         *            - число коэффициентов фильтра / 2
         * @param ad
         *            - 'b' в книге Оппенгейма & Шафера [r+1]
         * @param x
         *            - [r+1]
         * @param y
         *            - 'C' в книге Оппенгейма & Шафера [r+1]
         *
         * @return возвращает значение в виде double для A[freq]
         *********************/
        private static double computeA(double freq, int r, double[] ad, double[] x, double[] y)
        {
            double numer = 0;
            double denom = 0;
            double xc = Math.Cos(2.0 * Math.PI * freq);
            for (int i = 0; i <= r; i++)
            {
                double c = xc - x[i];
                if (Math.Abs(c) < 1.0e-7)
                {
                    numer = y[i];
                    denom = 1;
                    break;
                }
                c = ad[i] / c;
                denom += c;
                numer += c * y[i];
            }
            return numer / denom;
        }

        /************************
         * calcError =========== Расчитывает функцию Ошибки исходя из желаемого
         * частотного отклика плотной сетки (d[]), весовой функции плотной сетки (w[]),
         * и имеющегося расчитанного отклика (A[])
         *
         * @param r
         *            - число коэффициентов фильтра / 2
         * @param ad
         *            - [r+1]
         * @param x
         *            - [r+1]
         * @param y
         *            - [r+1]
         * @param gridSize
         *            - число элементов в плотной сетке частот
         * @param grid
         *            - частоты (от 0 до 0.5) плотной сетки [gridSize]
         * @param d
         *            - желаемый отклик плотной сетки [gridSize]
         * @param w
         *            - весовая функция плотной сетки [gridSize]
         *
         * @return e - функция ошибки плотной сетки [gridSize]
         ************************/
        private static void calcError(int r, double[] ad, double[] x, double[] y, int gridSize, double[] grid, double[] d,
                double[] w, double[] e)
        {
            for (int i = 0; i < gridSize; i++)
            {
                double A = computeA(grid[i], r, ad, x, y);
                e[i] = w[i] * (d[i] - A);
            }
        }

        /************************
         * search ======== Ищет максимумы/минимумы огибающей ошибки. Если найден более
         * чем r+1 экстремум, используется следующая эвристика (спасибо Крису Хансону):
         * 1) Сначала удаляются прилегающие, не меняющиеся экстремумы. 2) Если есть
         * более одного лишнего экстремума, удаляем имеющий наименьшую ошибку. Это
         * позволит создать условия нечередования, которое устраняется п. 1). 3) Если
         * есть только один лишний экстремум, удаляем меньший их первого/последнего
         * экстремума
         *
         * @param r
         *            - число коэффициентов фильтра / 2
         * @param ext
         *            - указатели на grid[] экстремальных частот [r+1]
         * @param gridSize
         *            - число элементов в плотной сетке частот
         * @param e
         *            - массив значений ошибки. [gridSize]
         *
         * @return ext - новые указатели на экстремальные частоты [r+1]
         ************************/
        private static void search(int r, int[] ext, int gridSize, double[] e)
        {
            int[] foundExt = new int[gridSize];// Массив найденных экстремумов
            int k = 0;
            // Проверка на экстремум в 0.
            if (((e[0] > 0.0) && (e[0] > e[1])) || ((e[0] < 0.0) && (e[0] < e[1])))
                foundExt[k++] = 0;

            // Проверка на экстремум внутри плотной сетки
            for (int i = 1; i < gridSize - 1; i++)
            {
                if (((e[i] >= e[i - 1]) && (e[i] > e[i + 1]) && (e[i] > 0.0))
                        || ((e[i] <= e[i - 1]) && (e[i] < e[i + 1]) && (e[i] < 0.0)))
                    foundExt[k++] = i;
            }

            // Проверка на экстремум в 0.5
            int j = gridSize - 1;
            if (((e[j] > 0.0) && (e[j] > e[j - 1])) || ((e[j] < 0.0) && (e[j] < e[j - 1])))
                foundExt[k++] = j;

            // Удаление лишних экстремумов
            for (int extra = k - (r + 1); extra > 0; extra--)
            {
                bool up = e[foundExt[0]] > 0.0;
                // up = true --> первый это максимум
                // up = false --> первый это минимум
                int l = 0;
                bool alt = true;
                for (j = 1; j < k; j++)
                {
                    if (Math.Abs(e[foundExt[j]]) < Math.Abs(e[foundExt[l]]))
                        l = j;// новая наименьшая ошибка.
                    if (up && (e[foundExt[j]] < 0.0))
                        up = false;// переключение на поиск минимума
                    else if (!up && (e[foundExt[j]] > 0.0))
                        up = true;// переключение на поиск максимума
                    else
                    {
                        alt = false;
                        break;// упс, найдено 2 не попеременных
                    } // экстремум. Удаляем наиментший из них
                } // когда цикл закончен, все экстремумы являются попеременными

                // Если есть только доин лишний экстремум, а все другие попеременные,
                // удаляем наименьший из первого/последнего экстремума.
                if (alt && (extra == 1))
                {
                    if (Math.Abs(e[foundExt[k - 1]]) < Math.Abs(e[foundExt[0]]))
                        l = foundExt[k - 1];// удаляем последний экстремум
                    else
                        l = foundExt[0];// удаляем первый экстремум
                }

                for (j = l; j < k; j++)// цикл удаления
                    foundExt[j] = foundExt[j + 1];
                k--;
            }
            // копируем найденные экстремумы в ext[]
            for (int i = 0; i <= r; i++)
                ext[i] = foundExt[i];
        }

        /*******************
         * isDone ======== Проверяет, достаточно ли мала функция ошибок, чтобы считать,
         * что результат сошёлся.
         *
         * @param r
         *            - число коэффициентов фильтра / 2
         * @param ext
         *            - указатели на экстремальные частоты [r+1]
         * @param e
         *            - функция ошибки плотной сетки [gridSize]
         *
         * @return возвращает true, если результат сошёлся, false, если результат не
         *         сошёлся
         ********************/
        private static bool isDone(int r, int[] ext, double[] e)
        {
            double min, max;
            min = max = Math.Abs(e[ext[0]]);
            for (int i = 1; i <= r; i++)
            {
                double current = Math.Abs(e[ext[i]]);
                if (current < min)
                    min = current;
                if (current > max)
                    max = current;
            }
            if (((max - min) / max) < 0.0001)
                return true;
            return false;
        }

        /*********************
         * freqSample ============ Простой алгоритм частотной дискретизации для
         * определения импульсного отклика h[] из значений A, найденных в ComputeA
         *
         * @param A
         *            - примерные точки желаемого отклика [N/2]
         * @param is_symm
         *            - симметричность желаемого фильтра
         * @param N
         *            - число коэффициентов фильтра
         *
         * @return h - импульсный отклик финального фильтра [N]
         *********************/
        private static void freqSample(double[] A, bool is_symm, int N, double[] h)
        {
            double x, val;
            double M = (N - 1.0) / 2.0;
            if (is_symm)
            {
                if ((N & 1) != 0)
                {
                    for (int n = 0; n < N; n++)
                    {
                        val = A[0];
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (int k = 1; k <= M; k++)
                            val += 2.0 * A[k] * Math.Cos(x * k);
                        h[n] = val / N;
                    }
                }
                else
                {
                    for (int n = 0; n < N; n++)
                    {
                        val = A[0];
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (int k = 1; k <= (N / 2 - 1); k++)
                            val += 2.0 * A[k] * Math.Cos(x * k);
                        h[n] = val / N;
                    }
                }
            }
            else
            {
                if ((N & 1) != 0)
                {
                    for (int n = 0; n < N; n++)
                    {
                        val = 0;
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (int k = 1; k <= M; k++)
                            val += 2.0 * A[k] * Math.Sin(x * k);
                        h[n] = val / N;
                    }
                }
                else
                {
                    for (int n = 0; n < N; n++)
                    {
                        val = A[N / 2] * Math.Sin(Math.PI * (n - M));
                        x = 2.0 * Math.PI * (n - M) / N;
                        for (int k = 1; k <= (N / 2 - 1); k++)
                            val += 2.0 * A[k] * Math.Sin(x * k);
                        h[n] = val / N;
                    }
                }
            }
        }

        /********************
         * remez ======= Расчёт оптимального (в смысле Чебышева/минимаксного)
         * импульсного отклика КИХ фильтра, заданного набором границ диапазонов,
         * желаемого отлика в этих диапазонах и веса ошибки в этих диапазонах.
         *
         * @param numtaps
         *            - число коэффициентов фильтра
         * @param numband
         *            - число диапазонов в указанном фильтре
         * @param bands
         *            - число границ диапазонов, заданных пользователем [2 * numband]
         * @param des
         *            - заданный пользователем отклик в каждом диапазоне [numband]
         * @param weight
         *            - заданные пользователем веса ошибок [numband]
         * @param type
         *            - тип фильтра
         *
         * @return h - импульсный отклик финального фильтра [numtaps]
         ********************/
        private static void remez(int numtaps, double[] bands, double[] des, double[] weight, int type, double[] h)
        {
            bool is_symmetry = (type == BANDPASS);
            int r = numtaps >> 1;// число экстремумов
            if (((numtaps & 1) != 0) && is_symmetry)
                r++;

            // Заранее предсказываем размер плотной сетки для размеров массивов
            int gridSize__ = 0;
            int gridSize = 0;
            int numband = des.Length;
            //int numband = bands.Length;
            gridSize = (int)Math.Round(2.0 * r * GRIDDENSITY);

            for (int i = 0; i < numband; i++)
            {
                gridSize__ += (int)Math.Round(2.0 * r * GRIDDENSITY * (bands[2 * i + 1] - bands[2 * i]));
                
            }
            if (!is_symmetry)
                gridSize--;
            double[] grid = new double[gridSize];
            double[] d = new double[gridSize];
            double[] w = new double[gridSize];
            double[] e = new double[gridSize];
            double[] x = new double[r + 1];
            double[] y = new double[r + 1];
            double[] ad = new double[r + 1];
            double[] taps = new double[r + 1];
            int[] ext = new int[r + 1];

            // Создаём плотную сетку частот
            createDenseGrid(r, numtaps, numband, bands, des, weight, gridSize, grid, d, w, is_symmetry);
            initialGuess(r, ext, gridSize);

            // Для Дифференциатора: (правим сетку)
            // if( type == DIFFERENTIATOR ) {// не используется
            // for( int i = 0; i < gridSize; i++ ) {
            // if( d[i] > 0.0001 ) w[i] /= grid[i];
            // }
            // }

            // Для нечётных или несимметричных фильтров, меняем
            // d[] и w[] в соответствии с алгоритмом Паркса-Маклеллана
            if (is_symmetry)
            {
                if ((numtaps & 1) == 0)
                {
                    for (int i = 0; i < gridSize; i++)
                    {
                        double c = Math.Cos(Math.PI * grid[i]);
                        d[i] /= c;
                        w[i] *= c;
                    }
                }
            }
            else
            {
                if ((numtaps & 1) != 0)
                {
                    for (int i = 0; i < gridSize; i++)
                    {
                        double c = Math.Sin(2.0 * Math.PI * grid[i]);
                        d[i] /= c;
                        w[i] *= c;
                    }
                }
                else
                {
                    for (int i = 0; i < gridSize; i++)
                    {
                        double c = Math.Sin(Math.PI * grid[i]);
                        d[i] /= c;
                        w[i] *= c;
                    }
                }
            }
            // Выполняем алгоритм обмена Ремеза
            int iter = 0;
            for (; iter < MAXITERATIONS; iter++)
            {
                calcParms(r, ext, grid, d, w, ad, x, y);
                calcError(r, ad, x, y, gridSize, grid, d, w, e);
                search(r, ext, gridSize, e);
                if (isDone(r, ext, e))
                    break;
            }
            if (iter == MAXITERATIONS)
            {
                //System.out.println("Reached maximum iteration count.");
                //System.out.println("Results may be bad.");
            }

            calcParms(r, ext, grid, d, w, ad, x, y);

            // Ищем 'отводы' фиьтра для использования частотной дискретизации
            // Для нечётных или несимметричных фильтров корректируем отводы
            // в соответствии с алгоритмом Паркса-Маклеллана
            for (int i = 0; i <= numtaps / 2; i++)
            {
                double c;
                if (is_symmetry)
                {
                    if ((numtaps & 1) != 0)
                        c = 1;
                    else
                        c = Math.Cos(Math.PI * (double)i / numtaps);
                }
                else
                {
                    if ((numtaps & 1) != 0)
                        c = Math.Sin(2.0 * Math.PI * (double)i / numtaps);
                    else
                        c = Math.Sin(Math.PI * (double)i / numtaps);
                }
                taps[i] = computeA((double)i / numtaps, r, ad, x, y) * c;
            }
            // Расчёт частотной дискретизации используя расчитанные 'отводы'
            freqSample(taps, is_symmetry, numtaps, h);
        }

        /**
         * Инициализация параметров для расчёта методом Паркса-Маклеллана
         * 
         * @param type
         *            - тип фильтра
         * @param order
         *            - порядок фильтра
         * @param f1
         *            - частота среза ФНЧ и ФВЧ фильтра
         * @param f2
         *            - верхняя частота для полосового и режекторного фильтра
         * @param trband
         *            - нормализованная переходная полоса фильтра
         * @param sampleRate
         *            - частота дискретизации
         * @param atten_dB
         *            - степень подавления в дБ.
         * @param ripple_dB
         *            - пульсации в полосе пропускания дБ
         */
        public void init(byte type, int order, float f1, float f2, float trband, float sampleRate, float atten_dB,
                float ripple_dB)
        {
            f1 /= sampleRate;
            f2 /= sampleRate;
            trband /= sampleRate;
            int numBands = 2;
            double deltaP = 0.5 * (1.0 - Math.Pow(10.0, -0.05 * ripple_dB));
            double deltaS = (float)Math.Pow(10.0, -0.05 * atten_dB);
            double rippleRatio = deltaP / deltaS;
            // оцениваем необходимый порядок фильтра с помощью формулы Кайзера
            if (order < 1) order = estimatedOrder(trband, atten_dB, ripple_dB);
            int numTaps = order + 1;
            if (order < 1)
                order = estimatedOrder(trband, atten_dB, ripple_dB) + 1;
            //int numTaps = order;
            if (type == BANDPASS || type == BANDSTOP)
                numBands = 3;
            double[] desired = new double[numBands];
            double[] bands = new double[numBands << 1];
            double[] weights = new double[numBands];
            switch (type)
            {
                case LOWPASS:
                    desired[0] = 1.0;
                    desired[1] = 0.0;
                    bands[0] = 0.0;
                    bands[1] = f1;
                    bands[2] = f1 + trband;
                    bands[3] = 1.0;// 0.5;
                    weights[0] = 1.0;
                    weights[1] = rippleRatio;
                    break;
                case HIGHPASS:
                    desired[0] = 0.0;
                    desired[1] = 1.0;
                    bands[0] = 0.0;
                    bands[1] = f1 - trband;
                    bands[2] = f1;
                    bands[3] = 0.5;
                    weights[0] = rippleRatio;
                    weights[1] = 1.0;
                    break;
                case BANDPASS:
                    desired[0] = 0.0;
                    desired[1] = 1.0;
                    desired[2] = 0.0;
                    bands[0] = 0.0;
                    bands[1] = f1 - trband;
                    bands[2] = f1;
                    bands[3] = f2;
                    bands[4] = f2 + trband;
                    bands[5] = 1.0;// 0.5;
                    weights[0] = rippleRatio;
                    weights[1] = 1.0;
                    weights[2] = rippleRatio;
                    break;
                case BANDSTOP:
                    desired[0] = 1.0;
                    desired[1] = 0.0;
                    desired[2] = 1.0;
                    bands[0] = 0.0;
                    bands[1] = f1 - trband;
                    bands[2] = f1;
                    bands[3] = f2;
                    bands[4] = f2 + trband;
                    bands[5] = 0.5;
                    weights[0] = 1;
                    weights[1] = rippleRatio;
                    weights[2] = 1;
                    break;
            }
            // BANDPASS противоположен DIFFERENTIATOR или HILBERT !
            m_fir = new double[numTaps];
            remez(numTaps, bands, desired, weights, type == LOWPASS ? BANDPASS : type, m_fir);
            m_delay = new short[order];// создаём и обнуляем буфер данных
        }

        /**
         * фильтрация
         * 
         * @param sample
         *            - отсчёт звука
         * @return результат фильтрации
         */
        public float proc(float sample)
        {
            // линия задержки
            for (int i = m_delay.Length; --i > 0;)
                m_delay[i] = m_delay[i - 1];
            m_delay[0] = (short)sample;
            // расчёт отклика
            double response = 0;
            for (int i = 0; i < m_delay.Length; i++)
                response += m_delay[i] * m_fir[i];
            // ограничение амплитуды
            if (response > 0)
            {
                response += 0.5;
                if (response > 32767)
                    response = 32767;
            }
            else
            {
                response -= 0.5;
                if (response < -32768)
                    response = -32768;
            }
            return (float)response;
        }
        
    }
}
