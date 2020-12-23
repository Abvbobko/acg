using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace acg_dotnet.Tools.Transformations
{
    /*struct Point
    {
        public double x, y, z;
        public Point(double x, double y, double z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }*/

    static class TransformationMatrices {

        public static Matrix<double> GetEye4() {
            return DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            });
        }

        public static double[,] ListToArray<T>(List<List<double>> vertex_list) {
            double[,] vertices_matrix = new double[vertex_list.Count, vertex_list[0].Count];
            for (int i = 0; i < vertex_list.Count; i++) {
                for (int j = 0; j < vertex_list[0].Count; j++) {
                    vertices_matrix[i, j] = vertex_list[i][j];
                }
            }
            return vertices_matrix;
        }

        public static double[,] ListToArray<T>(List<List<int>> vertex_list) {
            double[,] vertices_matrix = new double[vertex_list.Count, vertex_list[0].Count];
            for (int i = 0; i < vertex_list.Count; i++) {
                for (int j = 0; j < vertex_list[0].Count; j++) {
                    vertices_matrix[i, j] = vertex_list[i][j];
                }
            }
            return vertices_matrix;
        }


        public static Matrix<double> TransformMatrix(Point x_axis, Point y_axis, Point z_axis, Point translation) {
            /*
                Общее преобразование в матричной форме

                | XAxis.x YAxis.x ZAxis.x Translation.x |
                | XAxis.y YAxis.y ZAxis.y Translation.y |
                | XAxis.z YAxis.z ZAxis.z Translation.z |
                | 0       0       0       1             |

            */

            double[,] matrix = new double[,] { 
                { x_axis.x, y_axis.x, z_axis.x, translation.x },
                { x_axis.y, y_axis.y, z_axis.y, translation.y },
                { x_axis.z, y_axis.z, z_axis.z, translation.z },
                { 0, 0, 0, 1 }
            };
            return DenseMatrix.OfArray(matrix);
        }

        // model to world

        public static Matrix<double> MovingMatrix(Point translation) {
            /*
                Матрица перемещения
                
                | 1 0 0 Translation.x |
                | 0 1 0 Translation.y |
                | 0 0 1 Translation.z |
                | 0 0 0             1 |
            
            */
            return TransformMatrix(
                new Point(1, 0, 0),
                new Point(0, 1, 0),
                new Point(0, 0, 1),
                translation
            );            
        }

        public static Matrix<double> ScaleMatrix(Point scale) {
            /*
                Матрица масштаба
                
                | Scale.x 0       0       0 |
                | 0       Scale.y 0       0 |
                | 0       0       Scale.z 0 |
                | 0       0       0       1 |
            
            */
            return TransformMatrix(
                new Point(scale.x, 0, 0),
                new Point(0, scale.y, 0),
                new Point(0, 0, scale.z),
                new Point(0, 0, 0)
            );
        }

        public static Matrix<double> XRotationMatrix(double theta) {
            /*
                Матрица поворота вокруг оси X
                
                | 1 0          0           0 |
                | 0 cos(theta) -sin(theta) 0 |
                | 0 sin(theta) cos(theta)  0 |
                | 0 0          0           1 |
            
            */            

            return TransformMatrix(
                new Point(1, 0, 0),
                new Point(0, Math.Cos(theta), Math.Sin(theta)),
                new Point(0, -Math.Sin(theta), Math.Cos(theta)),
                new Point(0, 0, 0)
            );
        }

        public static Matrix<double> YRotationMatrix(double theta) {
            /*
                Матрица поворота вокруг оси Y
                
                | cos(theta)  0  sin(theta) 0 |
                | 0           1  0          0 |
                | -sin(theta) 0  cos(theta) 0 |
                | 0           0  0          1 |
            
            */            

            return TransformMatrix(
                new Point(Math.Cos(theta), 0, -Math.Sin(theta)),
                new Point(0, 1, 0),
                new Point(Math.Sin(theta), 0, Math.Cos(theta)),
                new Point(0, 0, 0)
            );
        }

        public static Matrix<double> ZRotationMatrix(double theta) {
            /*
                Матрица поворота вокруг оси Z
                
                | cos(theta) -sin(theta) 0 0 |
                | sin(theta) cos(theta)  0 0 |
                | 0          0           1 0 |
                | 0          0           0 1 |
            
            */
            
            return TransformMatrix(
                new Point(Math.Cos(theta), Math.Sin(theta), 0),
                new Point(-Math.Sin(theta), Math.Cos(theta), 0),
                new Point(0, 0, 1),
                new Point(0, 0, 0)
            );
        }

        // world to observer        

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

            double abc, cap, abp, bcp;

            abc = GetTriangleArea(a, b, c);
            cap = GetTriangleArea(c, a, p);
            abp = GetTriangleArea(a, b, p);
            bcp = GetTriangleArea(b, c, p);
            
            double[] bar = new double[] {
                bcp / abc,
                cap / abc,
                abp / abc
            };
            return bar;
        }

        private static double GetTriangleArea(double[] a, double[] b, double[] c) {
            return ArraysScalarProduct(SubstractArrays(b, a), SubstractArrays(c, a));
        }

        public static Matrix<double> WorldToObserver(double[] eye, double[] target, double[] up) {
            /*
                Возвращает матрицу трансформации, с помощью которой
                все объекты сцены получат новые координаты в пространстве,
                центром которого является позиция камеры. 

                | XAxis.x XAxis.y XAxis.z -(XAxis*eye) |
                | YAxis.x YAxis.y YAxis.z -(YAxis*eye) |
                | ZAxis.x ZAxis.y ZAxis.z -(ZAxis*eye) |
                | 0       0       0       1            |

                eye - позиция камеры в мировом пространстве
                target - позиция цели, на которую направлена камера
                up - вектор, направленный вертикально вверх с точки зрения камеры
            */

            Vector<double> v_eye = DenseVector.OfArray(eye);

            Vector<double> z_axis = DenseVector.OfArray(NormalizeArray(SubstractArrays(eye, target)));
            Vector<double> x_axis = DenseVector.OfArray(NormalizeArray(Arrays3CrossProduct(up, z_axis.ToArray())));
            Vector<double> y_axis = DenseVector.OfArray(up);
            
            return TransformMatrix(
                new Point(x_axis[0], y_axis[0], z_axis[0]),
                new Point(x_axis[1], y_axis[1], z_axis[1]),
                new Point(x_axis[2], y_axis[2], z_axis[2]),
                new Point(-x_axis.DotProduct(v_eye), -y_axis.DotProduct(v_eye), -z_axis.DotProduct(v_eye))
            );
        }


        // observer to projection
        public static Matrix<double> orthographicMatrix(int width, int height, double z_near, int z_far) {
            /*
                Матрица преобразует векторы из пространства наблюдателя
                в пространство ортографической проекции
                и предполагает правостороннюю систему координат

                | 2/width 0        0                  0                     |
                | 0       2/height 0                  0                     |
                | 0       0        1/(Z_near - Z_far) Z_near/(Z_near-Z_far) |
                | 0       0        0                  1                     |

            */


            return TransformMatrix(
                new Point(2.0 / width, 0, 0),
                new Point(0, 2.0 / height, 0),
                new Point(0, 0, 1.0 / (z_near - z_far)),
                new Point(0, 0, 1.0 * z_near / (z_near - z_far))
            );
        }

        public static Matrix<double> perspectiveMatrix(int width, int height, double z_near, int z_far, 
            double FOV = Constants.FOV, double aspect = Constants.ASPECT) {
            /*
                Матрица преобразует векторы из пространства наблюдателя
                в пространство перспективной проекции    

                | 2*Z_near/width 0               0                      0                           |
                | 0              2*Z_near/height 0                      0                           |
                | 0              0               Z_far/(Z_near - Z_far) Z_near*Z_far/(Z_near-Z_far) |
                | 0              0               -1                     0                           |         

            */

            /* Matrix<double> tmp_matrix = DenseMatrix.OfArray(new double[,] {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 1, 0 }
            }); 

            return TransformMatrix(
                new Point(2.0 * z_near / width, 0, 0),
                new Point(0, 2.0 * z_near / height, 0),
                new Point(0, 0, 1.0*z_far / (z_near - z_far)),
                new Point(0, 0, 1.0 * z_near * z_far / (z_near - z_far))
            ) - tmp_matrix;*/

            double[,] matrix = new double[,] {
                { 1/(aspect*Math.Tan(FOV/2)), 0, 0, 0 },
                { 0, 1/Math.Tan(FOV/2) , 0, 0 },
                { 0, 0, (double)z_far / (z_near - z_far), (double)z_near * z_far / (z_near - z_far) },
                { 0, 0, -1, 0 }
            };
            return DenseMatrix.OfArray(matrix);
        }

        //projection to viewport
        public static Matrix<double> viewportMatrix(int width, int height, int x_min, int y_min) {
            /*
                Чтобы адаптировать размеры проекции под размеры окна просмотра,
                учесть направление оси Y экрана и переместить начало координат в центр окна просмотра,
                необходимо воспользоваться следующей матрицей

                | width/2 0         0  x_min+(width/2)  |
                | 0       -height/2 0  y_min+(height/2) |
                | 0       0         1  0                |
                | 0       0         0  1                |

            */
            
            return TransformMatrix(
                new Point(width / 2.0, 0, 0),
                new Point(0, -height / 2.0, 0),
                new Point(0, 0, 1),
                new Point(x_min + (width / 2.0), y_min + (height / 2.0), 0)
            );
        }

    }

    
}
