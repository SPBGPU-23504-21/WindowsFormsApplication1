using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        const int SIZE_PROBLEM = 950;
        float[] y = new float[SIZE_PROBLEM];
        double[] x1trace = new double[SIZE_PROBLEM];
        double[] y2trace = new double[SIZE_PROBLEM];
        const float x_min = -5f, x_max = 5f, y_min = -5f, y_max = 5f;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            y = caclDots();
            richTextBox1.Clear();
            for (int i = 0; i != SIZE_PROBLEM; i++)
            {
                richTextBox1.Text += "(" + i + ", " + y[i].ToString() + ")" + "\n";
            }
            MessageBox.Show("Вычисления закончены");
        }
        private void button2_Click(object sender, EventArgs e)
        {//график функции
            DrawGraphic();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CalcMNGO();
            MessageBox.Show("Вычисления закончены");
        }
        public static float[] caclDots()
        {
            float h = 0.2f, xt = 20;
            float[] z1 = new float[SIZE_PROBLEM];
            float[] z2 = new float[SIZE_PROBLEM];
            float[] y = new float[SIZE_PROBLEM];
            z1[0] = 0; z2[0] = 0;
            for (int i = 1; i != SIZE_PROBLEM; i++)
            {
                z1[i] = z1[i - 1] + h * z2[i - 1];
                z2[i] = z2[i - 1] + h * ((xt - z1[i - 1] - 0.4f * z2[i - 1]) / 4);
                y[i] = 2 * z1[i - 1] - 3 * (xt - z1[i - 1] - 0.4f * z2[i - 1]);
            }
            return y;
        }
        public void DrawGraphic()
        {//трушная функиця с автоматическим скейлом координат
            Bitmap bmp = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            Graphics grBack = Graphics.FromImage(bmp);
            grBack.ScaleTransform(bmp.Width / (1000f - 0f), bmp.Height / (60f - (-60f)));
            grBack.TranslateTransform(-x_min, -y_min, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            grBack.DrawLine(Pens.Red, 0, 0, 0 + 1, y[1]);
            for (int i = 1; i != SIZE_PROBLEM - 1; i++)
            {
                if (i % 5 == 0)
                {
                    grBack.DrawRectangle(Pens.Purple, 0 + i - 2, y[i], 1, 1);
                }
                grBack.DrawLine(Pens.Red, i, y[i], (i + 1), y[i + 1]);
            }
            pictureBox1.Image = bmp;
        }
        public void CalcMNGO()
        {
            richTextBox2.Clear();
            int a = 2, b = 4, k = 2, kmax = 500, i = 2;
            float h = 0.1f, d = 0.01f;
            double gr1, gr2, x1 = 2, y1 = 3;

            while (k < kmax)
            {
                gr1 = 2 * x1 / Math.Pow(a, 2);
                x1 = x1 - h * gr1;
                x1trace[i] = x1;
                y2trace[i] = y1;
                i = i + 1;
                gr2 = 2 * y1 / Math.Pow(b, 2);
                y1 = y1 - h * gr2;
                x1trace[i] = x1;
                y2trace[i] = y1;
                i = i + 1;
                if (Math.Sqrt(Math.Pow(gr1, 2) + Math.Pow(gr2, 2)) <= d)
                {
                    break;
                }
                richTextBox2.Text += "(" + x1trace[k].ToString() + ", " + y2trace[k].ToString() + ")" + "\n";
                k = k + 1;
            }
        }
        public void DrawMNGO()
        {

            Bitmap bmp = new Bitmap(pictureBox2.ClientSize.Width, pictureBox2.ClientSize.Height);
            Graphics grBack = Graphics.FromImage(bmp);
            grBack.ScaleTransform(bmp.Width/(x_max - x_min), bmp.Height/(y_max - y_min));
            grBack.TranslateTransform(-x_min, -y_min, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            for (int LevelCurves = -3; LevelCurves <= 25; LevelCurves++)
                PlotLevelCurve(grBack, Convert.ToSingle(LevelCurves / 4), -4f, 4f, -4f, 4f, 0.05f, 1f, 1f, 0.002f);
           for (int i = 0; i != 290; i++)
            {
             if(i % 10 == 0)  grBack.DrawLine(Pens.Brown, (float)x1trace[i], (float)y2trace[i],
                    (float)x1trace[i + 1], (float)y2trace[i + 1]);
            }
           pictureBox2.Image = bmp;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            DrawMNGO();
        }
        public double func(double x, double y) 
        {//функция для расчета
            double f, a = 2, b = 4;
            f = Math.Pow((x / a), 2) + Math.Pow((y / b), 2);
            return f;
        }
        private double df_dx(double x, double y)
        {
            double dxdf, a = 2;
            dxdf = 2 * x / Math.Pow(a, 2); 
            return dxdf;
        }
        private double df_dy(double x, double y)
        {
            double dfdy, b = 4;
            dfdy = 2 * y/ Math.Pow(b, 2);
            return dfdy;
        }
        private void FindPointOnISO(ref double x, ref double y, double LevelCurves, double start_x, double start_y, double tolerance)
        {
            double dx = 0, dy = 0, dz, delta, f_xy;
            int direction = 0;
            x = start_x; y = start_y; delta = 0.01;
            int i = 0;
            do
            {
                f_xy = func(x, y); dz = LevelCurves - f_xy;
                if (Math.Abs(dz) < tolerance) break;
                //Анализируем направление:
                if (Math.Sign(dz) != direction)
                {
                    //Изменяем направление. Уменьшаем delta:
                    delta = delta / 2;
                    direction = Math.Sign(dz);
                }

                //Рассчитываем градиент:
                Gradient(x, y, ref dx, ref dy);
                if ((Math.Abs(dx) + Math.Abs(dy)) < 0.001) break;
                //Перемещаемся направо:
                x = x + dx * delta * (float)direction;
                y = y + dy * delta * (float)direction;
            }
            while (i < 1);
        }
        private void Gradient(double x, double y,ref double dx, ref double dy)
        {

            float dist = 0;
            dx = df_dx(x, y); dy = df_dy(x, y);
            dist = Convert.ToSingle(Math.Sqrt(dx * dx + dy * dy));
            if (Math.Abs(dist) < 0.0001)
            {
                dx = 0; dy = 0;
            }
            else
            {
                dx = dx / dist; dy = dy / dist;
            }
        }
        private void PlotLevelCurve(Graphics g, double LevelCurves, float x_min, float x_max, 
            float y_min,double y_max, double step_size, float start_x, float start_y, float tolerance)
        {

            int num_points = 0;
            double x0 = 0, y0 = 0, x1, y1, x2, y2, dx = 0, dy = 0;
            //Находим точку (x0, y0) на линии уровня LevelCurves:
            FindPointOnISO(ref x0, ref y0, LevelCurves,
            start_x, start_y, tolerance);
            //Начало:
            num_points = 1;
            //Следующая линия уровня LevelCurves:
            x2 = x0; y2 = y0;
            //В бесконечном цикле do-while выходим через break:
            int i = 0;
            do
            {
                x1 = x2; y1 = y2;
                //Находим следующую точку на линии:
                Gradient(x2, y2, ref dx, ref dy);
                if ((Math.Abs(dx) + Math.Abs(dy)) < 0.001) break;
                x2 = x2 + dy * step_size;
                y2 = y2 - dx * step_size;
                FindPointOnISO(ref x2, ref y2,
                LevelCurves, x2, y2, tolerance);
                //Рисуем до этой точки:
                g.DrawLine(Pens.Purple, (float)x1, (float)y1, (float)x2, (float)y2);
                num_points = num_points + 1;
                //Смотрим,находится ли точка 
                //вне области рисования:
                if (x2 < x_min && y2 < y_min)
                {
                    break;
                }
                else if (x2 > x_max && y2 > y_max)
                {
                    break;
                }
                //Если мы ушли более чем на 4 точки, то смотрим
                //не пришли ли мы в начало:
                if (num_points >= 4)
                {
                    if (Math.Sqrt((x0 - x2) * (x0 - x2) +
                    (y0 - y2) * (y0 - y2)) <= step_size * 1.1)
                    {
                  //      g.DrawLine(Pens.Blue, (float)x2, (float)y2, (float)x0, (float)y0);
                        break;
                    }
                }

            }
            while (i < 1);
        }
    }
}