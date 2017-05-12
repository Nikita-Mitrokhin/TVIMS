using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Tv
{
    public partial class Form1 : Form
    {
        private Random rand = new Random();
        double[] probabilityValue;

        double significanceLevel = 0;
        float r;
        int numberOfIntervals = 0;
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load_1(object sender, EventArgs e)
        {

            chart1.Series[0].Points.AddY(0.0);
            chart1.Series[1].Points.AddY(0.0);
            chart1.ChartAreas[0].AxisY.ScaleView.Zoom(0, 1);
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
        }
        float GammaFunction(float value) //Гамма - функция
        {
            float result = 1;
            if (value == 0)
            {
                return 1;
            }
            if (value == (int)value)
            {
                for (int i = 2; i < value; i++)
                {
                    result *= i;
                }
            }
            else if (value != (int)value)
            {
                result = (float)Math.Pow(Math.PI, 0.5);
                for (float i = (1f / 2f); i < value; i++)
                {
                    result *= i;
                }
            }
            return result;
        }

        float DistributionFunction1(float x) //Плотность распределения хи квадрат с r степенями свободы
        {
            float result = 0f;
            if (x <= 0)
            {
                return result;
            }
            else if (x > 0)
            {
                result = (float)(Math.Pow(2f, -1f * (float)((float)r / 2f)) * Math.Pow(GammaFunction((float)r / 2f), -1f) * Math.Pow((float)(x), (float)(((float)r / 2f) - 1f)) * Math.Pow(Math.E, (float)((float)(-x) / 2f)));
            }
            return result;
        }

        float MethodOfTrapezoids(float a, float b) //Метод трапеций
        {
            float result = 0;
            int n = 1000;
            for (int i = 1; i <= n; i++)
            {
                result += (float)(DistributionFunction1(a + (float)(b - a) * ((float)(i - 1) / (float)n)) + DistributionFunction1(a + (float)(b - a) * ((float)i / (float)n))) * (float)((float)b - (float)a) / (2f * (float)n);
            }
            return result;
        }

        double DistributionFunction(double p, int i) //рассчитываем вероятность 
        {
            double res = ((1 - p) * Math.Pow((p), i));
            return res;
        }

        int povtor(int m, int[] ArrayRandomValue, int N) //выбираем повторяющиеся элементы
        {
            int count = 0;
            for (int i = 0; i < N; i++)
            {
                if (ArrayRandomValue[i] == m)
                    count++;
            }
            return count;
        }

        int RandVal(double p) //моделируем случайную величину
        {

            int curr = 0;
            ToStart:
            double val = Convert.ToDouble(rand.Next(100)) / 100;
            if (val < p)
            {
                curr++;
                goto ToStart;
            }
            return curr;
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            double p = Convert.ToDouble(textBoxProbability.Text);
            int N = Convert.ToInt32(textBoxAmount.Text);
            if ((N < 1) || (p >= 1) || (p < 0))
            {
                MessageBox.Show("Введены не корректные значения.", "Ошибка!");
                goto ToEnd;
            }

            int[] ArrayRandomValue = new int[N];
            for (int i = 0; i < N; i++)
            {
                ArrayRandomValue[i] = RandVal(p);
            }
            Array.Sort(ArrayRandomValue);


            int[] ArrayNoRepeat = new int[N];
            ArrayNoRepeat = ArrayRandomValue.Distinct().ToArray();
            int Num = ArrayNoRepeat.Length;


            dataGridView1.RowCount = 3;
            dataGridView1.ColumnCount = Num;
            for (int i = 0; i < dataGridView1.ColumnCount; i++) //Таблица к первой части
            {
                dataGridView1.Columns[i].HeaderText = Convert.ToString(ArrayNoRepeat[i]);
            }
            dataGridView1.Rows[0].HeaderCell.Value = "Встретилось(количество)(Ni)";
            dataGridView1.Rows[1].HeaderCell.Value = "Вероятность(P({η = yj}) ";
            dataGridView1.Rows[2].HeaderCell.Value = "Ni/N ";

            probabilityValue = new double[Num];
            double[] randomValueFrequency = new double[Num];
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                probabilityValue[i] = DistributionFunction(p, i);
                randomValueFrequency[i] = ((double)(double)povtor(ArrayNoRepeat[i], ArrayRandomValue, N) / (double)N);
                dataGridView1.Rows[0].Cells[i].Value = Convert.ToString(povtor(ArrayNoRepeat[i], ArrayRandomValue, N));
                dataGridView1.Rows[1].Cells[i].Value = Convert.ToString(probabilityValue[i]);
                dataGridView1.Rows[2].Cells[i].Value = Convert.ToString(Math.Round(randomValueFrequency[i], 2));


            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            dataGridView2.RowCount = 11;
            dataGridView2.ColumnCount = 2;
            for (int i = 0; i <= 10; i++) //Таблица ко второй части(характеристики)
            {
                dataGridView2[1, i].Value = "-";
            }
            dataGridView2[0, 0].Value = "Eη";
            dataGridView2[0, 1].Value = "Xcp";
            dataGridView2[0, 2].Value = "|Eη − x|";
            dataGridView2[0, 3].Value = "Dη";
            dataGridView2[0, 4].Value = "S^2";
            dataGridView2[0, 5].Value = "|Dη − S^2|";
            dataGridView2[0, 6].Value = "Me";
            dataGridView2[0, 7].Value = "R(Размах выборки)";
            dataGridView2[0, 8].Value = "Макс.отклонение";
            dataGridView2[0, 9].Value = "D";
            dataGridView2[0, 10].Value = "F(R0)";

            double expectedValue = 1 / p;
            dataGridView2[1, 0].Value = Convert.ToString(Math.Round(expectedValue, 2));

            double dispersion = (1 - p) / (p * p);
            dataGridView2[1, 3].Value = Convert.ToString(Math.Round(dispersion, 2));

            double Xcp = 0;
            for (int i = 0; i < Num; i++)
            {
                Xcp += ArrayNoRepeat[i] * povtor(ArrayNoRepeat[i], ArrayRandomValue, N);
            }
            Xcp = (float)((float)Xcp / (float)Num);
            dataGridView2[1, 1].Value = Convert.ToString(Math.Round(Xcp, 2));

            double S2 = 0;
            for (int i = 0; i < Num; i++)
            {
                S2 += (ArrayNoRepeat[i] - Xcp) * (ArrayNoRepeat[i] - Xcp);
            }
            S2 = S2 / Num;
            dataGridView2[1, 4].Value = Convert.ToString(Math.Round(S2, 2));

            int rangeOfSample = ArrayNoRepeat[Num - 1] - ArrayNoRepeat[0];
            dataGridView2[1, 7].Value = Convert.ToString(rangeOfSample);

            int median = 0;
            if (Num % 2 == 1)
            {
                median = ArrayNoRepeat[(Num - 1) / 2];
            }
            else
            {
                median = (ArrayNoRepeat[Num / 2] + ArrayNoRepeat[(Num / 2) - 1]) / 2;
            }
            dataGridView2[1, 6].Value = Convert.ToString(median);

            double absExpectedValueXcp = Math.Abs(expectedValue - Xcp);
            dataGridView2[1, 2].Value = Convert.ToString(Math.Round(absExpectedValueXcp, 2));

            double absDispersionS2 = Math.Abs(dispersion - S2);
            dataGridView2[1, 5].Value = Convert.ToString(Math.Round(absDispersionS2, 2));

            double maximumDeviation = Math.Abs(randomValueFrequency[0] - probabilityValue[0]);
            for (int i = 0; i < Num; i++)
            {
                if (Math.Abs(randomValueFrequency[i] - probabilityValue[i]) > maximumDeviation)
                {
                    maximumDeviation = Math.Abs(randomValueFrequency[i] - probabilityValue[i]);
                }
            }
            dataGridView2[1, 8].Value = Convert.ToString(Math.Round(maximumDeviation, 2));
            
            //Строим графики
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            double x = probabilityValue[0];
            double y = randomValueFrequency[0];
            double[] scheduleRobabilityValue = new double[Num];
            double[] scheduleRandomValueRequency = new double[Num];
            chart1.Series[0].Points.AddXY(0, 0);
            chart1.Series[1].Points.AddXY(0, 0);
            chart1.Series[0].Points.AddXY(0, x);
            chart1.Series[1].Points.AddXY(0, y);
            for (int i = 1; i < Num; i++)
            {

                x += probabilityValue[i];
                y += randomValueFrequency[i];
                scheduleRobabilityValue[i] = x;
                scheduleRandomValueRequency[i] = y;
                chart1.Series[0].Points.AddXY(Convert.ToDouble(i), x);
                chart1.Series[1].Points.AddXY(Convert.ToDouble(i), y);
            }
            chart1.Series[0].Points.AddXY(Convert.ToDouble(Num), scheduleRobabilityValue[Num - 1]);
            chart1.Series[1].Points.AddXY(Convert.ToDouble(Num), 1.0);


            double D = Math.Abs(scheduleRandomValueRequency[0] - scheduleRobabilityValue[0]);
            for (int i = 0; i < Num; i++)
            {
                if (Math.Abs(scheduleRandomValueRequency[i] - scheduleRobabilityValue[i]) > D)
                {
                    D = Math.Abs(scheduleRandomValueRequency[i] - scheduleRobabilityValue[i]);
                }
            }
            dataGridView2[1, 9].Value = Convert.ToString(Math.Round(D, 2));

            this.провркаГипотезыToolStripMenuItem.Visible = true;


            ToEnd:;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
        }



        private void текстЗадачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("14 Вариант\nСтуденту на зачёте задаются вопросы, которые прекращаются, если студент на заданный вопрос не ответит. Вероятность ответа на каждый вопрос независимо от других равна p. C.в. η — число полученных ответов.", "Текст задания");
        }

        private void включитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MaximumSize = new System.Drawing.Size(1583, 653);
            this.MinimumSize = new System.Drawing.Size(1583, 653);
            this.Size = new System.Drawing.Size(1572, 662);
        }

        private void выключитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MaximumSize = new System.Drawing.Size(1200, 600);
            this.MinimumSize = new System.Drawing.Size(1200, 600);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.label4.Visible = true;
            this.dataGridView4.Visible = true;
            this.button3.Visible = true;
            this.label5.Visible = true;
            this.dataGridView5.Visible = true;
            this.label15.Visible = true;

            numberOfIntervals = Convert.ToInt32(CountNumberOfIntervals.Text);
            dataGridView4.RowCount = numberOfIntervals - 1;
            dataGridView4.ColumnCount = 2;
            r = numberOfIntervals - 1;
            for (int i = 0; i < numberOfIntervals - 1; i++)
            {
                dataGridView4[0, i].Value = "z" + Convert.ToString(i + 1);
                dataGridView4[1, i].Value = "0";
            }

            dataGridView5.RowCount = numberOfIntervals;
            dataGridView5.ColumnCount = 2;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                dataGridView5[0, i].Value = "q" + Convert.ToString(i + 1);
                dataGridView5[1, i].Value = "-";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double p = Convert.ToDouble(textBoxProbability.Text);
            int N = Convert.ToInt32(textBoxAmount.Text);
            int[] ArrayRandomValue = new int[N];
            for (int i = 0; i < N; i++)
            {
                ArrayRandomValue[i] = RandVal(p);
            }
            Array.Sort(ArrayRandomValue);

            int[] zi = new int[numberOfIntervals + 1];
            double[] qi = new double[numberOfIntervals];
            int[] numberOfOccurrences = new int[numberOfIntervals];
            significanceLevel = Convert.ToDouble(CountSignificanceLevel.Text);
            double[] probabilityValue = new double[N];

            zi[0] = -99999999;
            for (int i = 1; i < numberOfIntervals; i++)
            {
                zi[i] = Convert.ToInt32(dataGridView4[1, i - 1].Value);
            }
            zi[numberOfIntervals] = 99999999;

            for (int i = 0; i < N - 1; i++)
            { probabilityValue[i] = DistributionFunction(p, i); }

            for (int i = 0; i < N - 1; i++)
            { ArrayRandomValue[i] = i; }

            int tmpI = 0;

            for (int j = 0; j < N - 1; j++)

            {
                if ((ArrayRandomValue[j] >= zi[tmpI]) && (ArrayRandomValue[j] < zi[tmpI + 1]))
                {
                    qi[tmpI] += probabilityValue[j];
                    numberOfOccurrences[tmpI]++;
                }
                else
                {
                    tmpI++;
                    j--;
                }

            }

            qi[numberOfIntervals - 1] = 1;
            for (int i = 0; i < numberOfIntervals - 1; i++)
            {
                qi[numberOfIntervals - 1] = qi[numberOfIntervals - 1] - qi[i];
            }


            if (Convert.ToString(dataGridView4[1, numberOfIntervals - 2].Value) != dataGridView1.Columns[dataGridView1.ColumnCount - 1].HeaderText)
            {
                for (int i = 0; i < numberOfIntervals; i++)
                {
                    if (numberOfOccurrences[i] == 0)
                    {
                        MessageBox.Show("Некоректные границы интервалов.Интервалы следует выбирать так, чтобы каждый содержал хотя бы одну точку, а лучше несколько.", "Ошибка!");
                        goto toEnd;
                    }
                }
            }
            for (int i = 0; i < numberOfIntervals; i++)
            {
                dataGridView5[1, i].Value = Convert.ToString(qi[i]);
            }

            int Num = dataGridView1.ColumnCount;
            float R0 = 0;
            for (int i = 0; i < numberOfIntervals; i++)
            {
                R0 += (float)((Math.Pow((double)(numberOfOccurrences[i] - (Num) * qi[i]), 2)) / ((Num) * qi[i]));
            }
            float functionOfR0 = 1f - MethodOfTrapezoids(0f, (float)R0);
            dataGridView2[1, 10].Value = Convert.ToString(functionOfR0);

            if (significanceLevel < functionOfR0) //Проверка гипотезы
            {

                this.label15.Location = new System.Drawing.Point(751, 499);
                label15.Text = "Гипотеза верна";
                pictureBox1.Image = Image.FromFile(@"source.gif");
            }
            else if (((float)significanceLevel >= functionOfR0))
            {

                this.label15.Location = new System.Drawing.Point(729, 499);
                label15.Text = "Гипотеза не верна";
                pictureBox1.Image = Image.FromFile(@"sourcetmp.gif");
            }
            toEnd:;
        }
    }

}
