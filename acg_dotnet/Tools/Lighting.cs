using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace acg_dotnet.Tools
{
    static class Lighting
    {

        private static int cnt = 0;
        public static Brush PhongLighting(Model model, double[] a, double[] b, double[] c, double[] p, 
            double[] vn1, double[] vn2, double[] vn3, double[] vt1, double[] vt2, double[] vt3) {

            double[] texture = VectorOperations.InterpolateNormal(a, b, c, p, vt1, vt2, vt3);

            double[] normal = VectorOperations.NormalizeArray(VectorOperations.InterpolateNormal(a, b, c, p, vn1, vn2, vn3));
            double[] light = new double[] {
                Constants.LIGHT_VIEWPORT[0],
                Constants.LIGHT_VIEWPORT[1],
                Constants.LIGHT_VIEWPORT[2]
            };
           
            light = VectorOperations.SubstractArrays(p, light);            
            light = VectorOperations.NormalizeArray(light);
            light = VectorOperations.ArrayOnNumberProduct(light, -1);
            
            double[] eye = new double[] {
                Constants.EYE_VIEWPORT[0],
                Constants.EYE_VIEWPORT[1],
                Constants.EYE_VIEWPORT[2]
            };

            if (cnt == 0) {
                //Console.WriteLine(eye[0] + " " + eye[1] + " " + eye[2]);
                cnt += 1;
            }
            eye = VectorOperations.NormalizeArray(VectorOperations.SubstractArrays(p, eye));
            //eye = VectorOperations.ArrayOnNumberProduct(eye, -1);

            int[] RGB = model.GetDiffuseColor(texture[0], texture[1]);          
            double[] I_ambient = AmbientLighting(RGB);
            double[] I_diffuse = DiffuseLighting(RGB, normal, light);
            double[] I_specular = SpecularLighting(RGB, normal, light, eye);

            double[] I_result = new double[] {
                I_ambient[0] + I_diffuse[0] + I_specular[0],
                I_ambient[1] + I_diffuse[1] + I_specular[1],
                I_ambient[2] + I_diffuse[2] + I_specular[2]
            };

            for (int i = 0; i < I_result.Length; i++) {
                if (I_result[i] > 255) {
                    I_result[i] = 255;
                }
                else if (I_result[i] < 0) {
                    I_result[i] = 0;
                }
            }

            Color color = Color.FromArgb(
                Convert.ToInt32(Math.Round(I_result[0])),
                Convert.ToInt32(Math.Round(I_result[1])),
                Convert.ToInt32(Math.Round(I_result[2]))
            );
            return new SolidBrush(color);
        }

        private static double[] AmbientLighting(int[] RGB, double k = Constants.k_a) {
            return new double[] {
                k * RGB[0],
                k * RGB[1],
                k * RGB[2]
            };
        }

        private static double[] DiffuseLighting(int[] RGB, double[] normal, double[] light, double k = Constants.k_d) {

            double cos = VectorOperations.ArraysScalarProduct(
                normal,
                light
            );

            cos = cos < 0 ? 0 : cos;

            return new double[] {
                k * cos * RGB[0],
                k * cos * RGB[1],
                k * cos * RGB[2]
            };
        }

        private static double[] SpecularLighting(int[] RGB, double[] normal, double[] light, double[] v,
            double k = Constants.k_s, int alpha = Constants.alpha) {

            double[] r = VectorOperations.SubstractArrays(
                    light,
                    VectorOperations.ArrayOnNumberProduct(
                            normal,
                            2 * VectorOperations.ArraysScalarProduct(light, normal)
                        )
                );

            r = VectorOperations.NormalizeArray(r);
            v = VectorOperations.NormalizeArray(v);

            double coef = k * Math.Pow(VectorOperations.ArraysScalarProduct(r, v), alpha);
            return new double[] {
                coef * RGB[0],
                coef * RGB[1],
                coef * RGB[2]
            };
        }

        public static Brush FlatLighting(int[] RGB, 
            double[] v1, double[] v2, double[] v3, double[] vn1, double[] vn2, double[] vn3) {

            int R = RGB[0];
            int G = RGB[1];
            int B = RGB[2];

            double[] light = new double[] {
                Constants.LIGHT_VIEWPORT[0],
                Constants.LIGHT_VIEWPORT[1],
                Constants.LIGHT_VIEWPORT[2]
            };

            double[] light_v1 = VectorOperations.NormalizeArray(VectorOperations.SubstractArrays(v1, light));
            double[] light_v2 = VectorOperations.NormalizeArray(VectorOperations.SubstractArrays(v1, light));
            double[] light_v3 = VectorOperations.NormalizeArray(VectorOperations.SubstractArrays(v1, light));

            double[] normal_v1 = VectorOperations.NormalizeArray(vn1);
            double[] normal_v2 = VectorOperations.NormalizeArray(vn2);
            double[] normal_v3 = VectorOperations.NormalizeArray(vn3);


            double cos1 = VectorOperations.ArraysScalarProduct(
                normal_v1,
                VectorOperations.ArrayOnNumberProduct(light_v1, -1)
            );
            double cos2 = VectorOperations.ArraysScalarProduct(
                normal_v2,
                VectorOperations.ArrayOnNumberProduct(light_v2, -1)
            );
            double cos3 = VectorOperations.ArraysScalarProduct(
                normal_v3,
                VectorOperations.ArrayOnNumberProduct(light_v3, -1)
            );

            cos1 = cos1 < 0 ? 0 : cos1;
            cos2 = cos2 < 0 ? 0 : cos2;
            cos3 = cos3 < 0 ? 0 : cos3;

            Color color = Color.FromArgb(
                Convert.ToInt32(Math.Round(Math.Sqrt(
                    ((R * cos1) * (R * cos1) + (R * cos2) * (R * cos2) + (R * cos3) * (R * cos3)) / 3
                ))),
                Convert.ToInt32(Math.Round(Math.Sqrt(
                    ((G * cos1) * (G * cos1) + (G * cos2) * (G * cos2) + (G * cos3) * (G * cos3)) / 3
                ))),
                Convert.ToInt32(Math.Round(Math.Sqrt(
                    ((B * cos1) * (B * cos1) + (B * cos2) * (B * cos2) + (B * cos3) * (B * cos3)) / 3
                )))
            );
            return new SolidBrush(color);
        }
    }
}
