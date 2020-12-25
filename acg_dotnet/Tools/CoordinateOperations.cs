using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace acg_dotnet.Tools
{
    static class CoordinateOperations
    {
        public static List<int> RejectFaces(List<List<int>> faces, Matrix<double> vertices) {
            List<int> visible_faces_indexes = new List<int>();
            int index = 0;
            foreach (List<int> face in faces) {
                double x1 = vertices.At(0, face[0] - 1);
                double y1 = vertices.At(1, face[0] - 1);

                double x2 = vertices.At(0, face[1] - 1);
                double y2 = vertices.At(1, face[1] - 1);

                double x3 = vertices.At(0, face[2] - 1);
                double y3 = vertices.At(1, face[2] - 1);

                double[] v1 = new double[] { x1, y1 };
                double[] v2 = new double[] { x2, y2 };
                double[] v3 = new double[] { x3, y3 };

                bool sign = VectorOperations.GetNormal(v1, v2, v3) < 0;

                if (sign) {
                    visible_faces_indexes.Add(index);
                }
                index += 1;
            }

            return visible_faces_indexes;
        }

        public static bool notOnOneLine(double[] a, double[] b, double[] c) {
            double x1, x2, x3, y1, y2, y3;

            if ((a[0] != b[0]) && (a[1] != b[1])) {
                x1 = a[0];
                x2 = b[0];
                x3 = c[0];
                y1 = a[1];
                y2 = b[1];
                y3 = c[1];
            }
            else if ((c[0] != b[0]) && (c[1] != b[1])) {
                x1 = c[0];
                x2 = b[0];
                x3 = a[0];
                y1 = c[1];
                y2 = b[1];
                y3 = a[1];
            }
            else if ((c[0] != a[0]) && (c[1] != a[1])) {
                x1 = c[0];
                x2 = a[0];
                x3 = b[0];
                y1 = c[1];
                y2 = a[1];
                y3 = b[1];
            }
            else {
                return false;
            }

            if ((x3 - x1) / (x2 - x1) == (y3 - y1) / (y2 - y1)) {
                return false;
            }

            return true;
        }

        public static List<double[]> DDA_Line(double x1, double x2, double y1, double y2, double z1, double z2) {
            double L = Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
            double dx = (x2 - x1) / L;
            double dy = (y2 - y1) / L;

            List<double[]> xy = new List<double[]>();
            int i = 0;
            xy.Add(new double[] { x1, y1, z1 });
            i += 1;

            while (i < L) {
                xy.Add(new double[] {
                    xy[i - 1][0] + dx,
                    xy[i - 1][1] + dy,
                    z1 + (z2 - z1)*(xy[i - 1][0] + dx - x1)/(x2-x1) // linear interpolation
                });
                i += 1;
            }

            xy.Add(new double[] { x2, y2, z2 });

            return xy;//new Tuple<List<double>, List<double>>(x, y);
        }
    }
}
