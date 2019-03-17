using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    /// <summary>
    /// Composite integrator for approximating \int_a^b f(x) dx using Newton-Cotes formulae of order 1,2,3 or 4.
    /// </summary>
    public class CompositeIntegrator
    {
            private int newtonCotesOrder;
            const int maxOrder = 4;
            const int maxOrderLength = 5; // internal variable for the maximum number of weights we will need
            private int numOfEvals;

            // This is are the pre-computed weights for different Newton Cotes formulae
            private static double[,] weights = new double[maxOrder, maxOrderLength] {
        {0.5, 0.5, 0, 0, 0},                    // Trapezium rule
        {1.0/6.0, 4.0/6.0, 1.0/6.0, 0, 0},      // Simpson's rule 
        {1.0/8.0, 3.0/8.0, 3.0/8.0, 1.0/8.0,0},
        {7.0/90.0, 32.0/90.0, 12.0/90.0, 32.0/90.0, 7.0/90.0} };
            private double[] quadraturePoints;
            private double[] quadraturePointsFVal;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:CompositeIntegrator"/> class.
            /// </summary>
            public CompositeIntegrator()
            {
                newtonCotesOrder = 1;
                quadraturePoints = new double[newtonCotesOrder + 1];
                quadraturePointsFVal = new double[newtonCotesOrder + 1];
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:CompositeIntegrator"/> class as a copy.
            /// </summary>
            /// <param name="integrator">The Integrator instance to copy.</param>
            public CompositeIntegrator(CompositeIntegrator integrator)
            {
                newtonCotesOrder = integrator.newtonCotesOrder;
                quadraturePoints = new double[newtonCotesOrder + 1];
                quadraturePointsFVal = new double[newtonCotesOrder + 1];
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:CompositeIntegrator"/> class.
            /// </summary>
            /// <param name="newtonCotesOrder">Desired Newton cotes order.</param>
            public CompositeIntegrator(int newtonCotesOrder)
            {
                if (newtonCotesOrder <= 0 || newtonCotesOrder > maxOrder)
                    throw new System.ArgumentException("Only order 1,2,3,4 are supported");
                this.newtonCotesOrder = newtonCotesOrder;
                quadraturePoints = new double[newtonCotesOrder + 1];
                quadraturePointsFVal = new double[newtonCotesOrder + 1];
            }


            private void updateQuadraturePointsAndFvals(double a, double h, Func<double, double> f)
            {
                double delta = h / newtonCotesOrder;
                for (int i = 0; i <= newtonCotesOrder; i++)
                {
                    quadraturePoints[i] = a + i * delta;
                    quadraturePointsFVal[i] = f(quadraturePoints[i]);
                    numOfEvals++; // for counting number of function evaluations needed
                }
            }

            /// <summary>
            /// Integrate (approximately) the specified f over the interval [a, b] given N subdivisions.
            /// </summary>
            /// <param name="f">The integrand.</param>
            /// <param name="a">The lower bound.</param>
            /// <param name="b">The upper bound.</param>
            /// <param name="N">Number of subdivisions.</param>
            public double Integrate(Func<double, double> f, double a, double b, int N)
            {
                numOfEvals = 0; // for counting number of function evaluations needed

                if (N <= 0)
                    throw new System.ArgumentException("Number of partitions must be an integer > 0.");
                double integral = 0;
                double h = (b - a) / N;
                for (int i = 0; i < N; i++)
                {
                    updateQuadraturePointsAndFvals(a + i * h, h, f);
                    double stepIncrement = 0.0;
                    for (int j = 0; j <= newtonCotesOrder; j++)
                    {
                        stepIncrement += weights[newtonCotesOrder - 1, j] * quadraturePointsFVal[j];
                    }
                    integral += stepIncrement * h;
                }
                return integral;
            }


            public int GetNumEvals() { return numOfEvals; }
    }
}
