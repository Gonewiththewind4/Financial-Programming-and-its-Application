using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HestonModel
{
    public partial class GraphicInUse : Form
    {
        //数据接口
        private double Kappa;
        private double Theta;
        private double sigma;
        private double Rho;
        private double V0;
        private double S;
        private double K;
        private double r;
        private string[] XAxisText = new string[] { "1", "10", "100", "1000" };
        private string[] YAxisText = new string[] { "1", "10", "100", "1000" };
        private string[] AxisTextColor = new string[] { "1", "10", "100", "1000" };
        //private GraphPainter m_graphPainter = new GraphPainter();
        public GraphicInUse(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {
            InitializeComponent();
            using (Graphics g = this.CreateGraphics())
            {
                //m_graphPainter.InitGraphPositions(g, this.ClientSize);
            }

            this.DoubleBuffered = true;
            this.Kappa = Kappa;
            this.Theta = Theta;
            this.sigma = sigma;
            this.Rho = Rho;
            this.V0 = V0;
            this.S = S;
            this.K = K;
            this.r = r;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bt = new Bitmap(800, 800);// (panel1.Width, panel1.Height);
            Graphics g = Graphics.FromImage(bt);
            Pen p = new Pen(Color.Black, 1);
            g = panel1.CreateGraphics();
            // g.DrawRectangle(new Pen(Color.Black, 1), 0, 0, panel1.Width, panel1.Height);
            g.FillRectangle(new SolidBrush(Color.White), 1, 1, panel1.Width - 2, panel1.Height - 2);

            //画坐标轴
            g.DrawLine(p, new Point(50, 20), new Point(50, panel1.Height - 20));//Y
            g.DrawLine(p, new Point(50, panel1.Height - 20), new Point(panel1.Width - 20, panel1.Height - 20));//X
            //初始化轴线说明文字
            //SetAxisText(ref g
            Font font = new System.Drawing.Font("Arial", 9, FontStyle.Regular);

            String[] n = { "0.1", " 1", " 10", " 100", " 1000" };
            int x = 50;
            for (int i = 0; i < 5; i++)
            {
                g.DrawString(n[i].ToString(), font, Brushes.Black, x, panel1.Height - 20); //设置文字内容及输出位置
                                                                                           /*
                                                                                                         for (int j = 1; j < 11; j++)
                                                                                                            {
                                                                                                              Math.Log10(j * Math.Pow(10, i - 1));
                                                                                                             g.DrawLine(p, new Point(80, panel1.Height - 22), new Point(80, panel1.Height - 20));//画刻度
                                                                                                             }
                                                                                                                                                                                       */

                x = x + (panel1.Width - 40 - 50) / 4;
            }
            int y = panel1.Height - 40;
            for (int i = 0; i < 5; i++)
            {
                g.DrawString(n[i].ToString(), font, Brushes.Black, 10, y); //设置文字内容及输出位置

                y = y - (panel1.Height - 20 - 40) / 4;
            }
            double[,] data = GetData(Kappa, Theta, sigma, Rho, V0, S, K, r);

            //补上画刻度和点，并用curve连线


        }
        public double[,] GetData(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {

            double T1 = 1;
            double T2 = 2;
            double T3 = 3;
            double T4 = 4;
            double T5 = 15;
            double C1, C2, C3, C4, C5, D1, D2, D3, D4, D5;

            HestonModelFormula Task2 = new HestonModelFormula(Kappa, Theta, sigma, Rho, V0);// (1.5768, 0.0398, 0.5751, -0.5711, 0.0175);//(1.5757,0.15656,0.57829,-0.57717,0.01843);//(1.5768, 0.0398, 0.5751, -0.5711, 0.0175);
            HestonModelMonteCarlo Task3 = new HestonModelMonteCarlo(Kappa, Theta, sigma, Rho, V0);// (2, 0.06, 0.4, 0.5, 0.04);

            C1 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T1);
            C2 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T2);
            C3 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T3);
            C4 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T4);
            C5 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T5);
            D1 = Task3.CalculateEuropeanCallOption(S, K, T1, 10000, 365, r);
            D2 = Task3.CalculateEuropeanCallOption(S, K, T2, 10000, 365, r);
            D3 = Task3.CalculateEuropeanCallOption(S, K, T3, 10000, 365, r);
            D4 = Task3.CalculateEuropeanCallOption(S, K, T4, 10000, 365, r);
            D5 = Task3.CalculateEuropeanCallOption(S, K, T5, 10000, 365, r);

            double[,] data = new double[2, 5];
            double[] dataC = new double[] { C1, C2, C3, C4, C5 };
            double[] dataD = new double[] { D1, D2, D3, D4, D5 };
            for (int i = 0; i < 5; i++)
            {
                data[0, i] = dataC[i];
                data[1, i] = dataD[i];
            }
            return data;

        }

    }
}
