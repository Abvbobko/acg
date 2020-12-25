using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace acg_dotnet.Tools
{
    static class VectorOperations
    {
        public static double[] InterpolateNormal(double[] a, double[] b, double[] c, double[] p, 
            double[] v1, double[] v2, double[] v3) {
            double[] bar = GetBarycentricCoordinates(a, b, c, p);
            double[] normal = AddArrays(
                    ArrayOnNumberProduct(v1, bar[0]),
                    AddArrays(
                        ArrayOnNumberProduct(v2, bar[1]),
                        ArrayOnNumberProduct(v3, bar[2])
                    )
                );
            return NormalizeArray(normal);
        }

        public static double[] NormalizeArray(double[] vector) {
            double sqr_sum = 0;
            for (int i = 0; i < vector.Length; i++) {
                sqr_sum += vector[i] * vector[i];
            }
            sqr_sum = Math.Sqrt(sqr_sum);
            for (int i = 0; i < vector.Length; i++) {
                vector[i] /= sqr_sum * 1.0;
            }
            return vector;
        }

        public static double[] SubstractArrays(double[] a, double[] b) {
            if (a.Length != b.Length) {
                throw new Exception("a and b must be the same length.");
            }

            double[] c = new double[a.Length];
            for (int i = 0; i < a.Length; i++) {
                c[i] = a[i] - b[i];
            }

            return c;
        }

        public static double[] AddArrays(double[] a, double[] b) {
            if (a.Length != b.Length) {
                throw new Exception("a and b must be the same length.");
            }

            double[] c = new double[a.Length];
            for (int i = 0; i < a.Length; i++) {
                c[i] = a[i] + b[i];
            }

            return c;
        }

        public static double[] Arrays3CrossProduct(double[] a, double[] b) {
            return new double[] {
                a[1]*b[2] - a[2]*b[1],
                - (a[0]*b[2] - a[2]*b[0]),
                a[0]*b[1] - a[1]*b[0]
            };
        }

        public static double ArraysScalarProduct(double[] a, double[] b) {
            double result = 0;
            for (int i = 0; i < a.Length; i++) {
                result += a[i] * b[i];
            }
            return result;
        }

        public static double[] ArrayOnNumberProduct(double[] a, double coef) {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++) {
                result[i] = a[i] * coef;
            }
            return result;
        }

        public static double[] GetBarycentricCoordinates(double[] a, double[] b, double[] c, double[] p) {

            /*double abc, cap, abp, bcp;

            abc = GetTriangleArea(a, b, c);
            cap = GetTriangleArea(c, a, p);
            abp = GetTriangleArea(a, b, p);
            bcp = GetTriangleArea(b, c, p);
            
            double[] bar = new double[] {
                bcp / abc,
                cap / abc,
                abp / abc
            };*/


            double[] ab, ac, pa;
            ab = SubstractArrays(b, a);

            ac = SubstractArrays(c, a);

            pa = SubstractArrays(a, p);

            double[] sx = new double[] { ab[0], ac[0], pa[0] };
            sx = ArrayOnNumberProduct(sx, -1);

            double[] sy = new double[] { ab[1], ac[1], pa[1] };

            double[] u = Arrays3CrossProduct(sx, sy);

            double[] bar = new double[] {
                1 - (u[0] + u[1])/u[2],
                u[1]/u[2],
                u[0]/u[2]
            };


            bool print = false;
            if (print) {
                Console.WriteLine("a " + a[0] + " " + a[1] + " " + a[2]);
                Console.WriteLine("b " + b[0] + " " + b[1] + " " + b[2]);
                Console.WriteLine("c " + c[0] + " " + c[1] + " " + c[2]);
                Console.WriteLine("p " + p[0] + " " + p[1] + " " + p[2]);

                Console.WriteLine("ab " + ab[0] + " " + ab[1] + " " + ab[2]);
                Console.WriteLine("ac " + ac[0] + " " + ac[1] + " " + ac[2]);
                Console.WriteLine("pa " + pa[0] + " " + pa[1] + " " + pa[2]);
                Console.WriteLine("sx " + sx[0] + " " + sx[1] + " " + sx[2]);
                Console.WriteLine("sy " + sy[0] + " " + sy[1] + " " + sy[2]);
                Console.WriteLine("u " + u[0] + " " + u[1] + " " + u[2]);
            }

            //Console.WriteLine(bar[0] + " " + bar[1] + " " + bar[2]);
            return bar;
        }

        private static double GetTriangleArea(double[] a, double[] b, double[] c) {
            return ArraysScalarProduct(SubstractArrays(b, a), SubstractArrays(c, a));
        }

        public static double GetNormal(double[] v1, double[] v2, double[] v3) {
            return (v2[0] - v1[0]) * (v3[1] - v1[1]) - (v3[0] - v1[0]) * (v2[1] - v1[1]);
        }
    }
}
