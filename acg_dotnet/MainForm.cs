using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using acg_dotnet.Tools;
using acg_dotnet.Tools.Transformations;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


namespace acg_dotnet
{
    struct Point
    {
        public double x, y, z;
        public Point(double x, double y, double z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public partial class MainForm : Form {
        Model model;
        ZBuffer zBuffer;

        public MainForm() {
            InitializeComponent();
            
            model = new Model();            
        }

        protected override void OnPaint(PaintEventArgs pea) {
            DrawModel(pea);
        }

        private void DrawModel(PaintEventArgs pea) {

            // Defines pen 
            Pen pen = new Pen(ForeColor);
            Brush brush = Brushes.Black;

            Matrix<double> vertices = model.TransformVertices();
            Matrix<double> vertices_n = model.TransformVerticesNormals();

            // возвращать только индексы нужных faces
            List<int> indexes = RejectFaces(model.FacesV, vertices);
            List<List<int>> faces = model.FacesV;
            List<List<int>> faces_vn = model.FacesVn;

            zBuffer = new ZBuffer(Size.Width, Size.Height);

            //int face_index = 0;
            
            foreach (int index in indexes) {
                List<double[]> polygonPoints = new List<double[]>();
                double x1, x2, y1, y2, z1, z2;               
                x1 = vertices.At(0, faces[index][0] - 1);
                y1 = vertices.At(1, faces[index][0] - 1);
                z1 = vertices.At(2, faces[index][0] - 1);

                x2 = vertices.At(0, faces[index].Last() - 1);
                y2 = vertices.At(1, faces[index].Last() - 1);
                z2 = vertices.At(2, faces[index].Last() - 1);                
                            
                double x1_ = vertices.At(0, faces[index][0] - 1), y1_ = vertices.At(1, faces[index][0] - 1), z1_ = vertices.At(2, faces[index][0] - 1);
                double x2_ = vertices.At(0, faces[index][1] - 1), y2_ = vertices.At(1, faces[index][1] - 1), z2_ = vertices.At(2, faces[index][1] - 1);
                double x3_ = vertices.At(0, faces[index][2] - 1), y3_ = vertices.At(1, faces[index][2] - 1), z3_ = vertices.At(2, faces[index][2] - 1);
                
                double[] vn1 = new double[] {
                    vertices_n.At(0, faces_vn[index][0] - 1),
                    vertices_n.At(1, faces_vn[index][0] - 1),
                    vertices_n.At(2, faces_vn[index][0] - 1)
                };

                double[] vn2 = new double[] {
                    vertices_n.At(0, faces_vn[index][1] - 1),
                    vertices_n.At(1, faces_vn[index][1] - 1),
                    vertices_n.At(2, faces_vn[index][1] - 1)
                };

                double[] vn3 = new double[] {
                    vertices_n.At(0, faces_vn[index][2] - 1),
                    vertices_n.At(1, faces_vn[index][2] - 1),
                    vertices_n.At(2, faces_vn[index][2] - 1)
                };

                Brush polygon_brush = GetBrush(
                    new double[] { x1_, y1_, z1_ },
                    new double[] { x2_, y2_, z2_ },
                    new double[] { x3_, y3_, z3_ },
                    vn1, vn2, vn3                    
                );               

                FillPolygon(
                    pea,
                    polygon_brush,
                    Convert.ToInt32(Math.Round(x1_)), Convert.ToInt32(Math.Round(y1_)), z1_,
                    Convert.ToInt32(Math.Round(x2_)), Convert.ToInt32(Math.Round(y2_)), z2_,
                    Convert.ToInt32(Math.Round(x3_)), Convert.ToInt32(Math.Round(y3_)), z3_                
                );                
            }            
            
        }

        private Brush PhongShadingBrush(double[] a, double[] b, double[] c, double[] p) {
            int R = 0;
            int G = 255;
            int B = 0;

            double[] bar = TransformationMatrices.GetBarycentricCoordinates(a, b, c, p);
            double[] normal = TransformationMatrices.AddArrays(
                    TransformationMatrices.ArrayOnNumberProduct(a, bar[0]),
                    TransformationMatrices.AddArrays(
                        TransformationMatrices.ArrayOnNumberProduct(b, bar[1]),
                        TransformationMatrices.ArrayOnNumberProduct(c, bar[2])
                    )
                );

            double[] light = new double[] {
                Constants.REVERSE_LIGHT_VIEWPORT_NORM[0],
                Constants.REVERSE_LIGHT_VIEWPORT_NORM[1],
                Constants.REVERSE_LIGHT_VIEWPORT_NORM[2]
            };

            light = TransformationMatrices.NormalizeArray(TransformationMatrices.SubstractArrays(p, light));
            normal = TransformationMatrices.NormalizeArray(normal);
            double cos = TransformationMatrices.ArraysScalarProduct(
                normal,
                TransformationMatrices.ArrayOnNumberProduct(light, -1)
            );

            cos = cos < 0 ? 0 : cos;
            Color color = Color.FromArgb(
                Convert.ToInt32(Math.Round(R * cos)),
                Convert.ToInt32(Math.Round(G * cos)),
                Convert.ToInt32(Math.Round(B * cos))
            );
            return new SolidBrush(color);

        }

        private Brush GetBrush(double[] v1, double[] v2, double[] v3, double[] vn1, double[] vn2, double[] vn3) {
            int R = 0;
            int G = 255;
            int B = 0;
        
            double[] light = new double[] {
                Constants.REVERSE_LIGHT_VIEWPORT_NORM[0],
                Constants.REVERSE_LIGHT_VIEWPORT_NORM[1],
                Constants.REVERSE_LIGHT_VIEWPORT_NORM[2]
            };

            double[] light_v1 = TransformationMatrices.NormalizeArray(TransformationMatrices.SubstractArrays(v1, light));
            double[] light_v2 = TransformationMatrices.NormalizeArray(TransformationMatrices.SubstractArrays(v1, light));
            double[] light_v3 = TransformationMatrices.NormalizeArray(TransformationMatrices.SubstractArrays(v1, light));
                        
            double[] normal_v1 = TransformationMatrices.NormalizeArray(vn1);
            double[] normal_v2 = TransformationMatrices.NormalizeArray(vn2);
            double[] normal_v3 = TransformationMatrices.NormalizeArray(vn3);

            
            double cos1 = TransformationMatrices.ArraysScalarProduct(
                normal_v1, 
                TransformationMatrices.ArrayOnNumberProduct(light_v1, -1)
            );
            double cos2 = TransformationMatrices.ArraysScalarProduct(
                normal_v2,
                TransformationMatrices.ArrayOnNumberProduct(light_v2, -1)
            );
            double cos3 = TransformationMatrices.ArraysScalarProduct(
                normal_v3,
                TransformationMatrices.ArrayOnNumberProduct(light_v3, -1)
            );

            cos1 = cos1 < 0 ? 0 : cos1;
            cos2 = cos2 < 0 ? 0 : cos2;
            cos3 = cos3 < 0 ? 0 : cos3;

            Color color = Color.FromArgb(
                Convert.ToInt32(Math.Round(Math.Sqrt(((R*cos1)*(R * cos1) + (R * cos2) * (R * cos2) + (R * cos3) * (R * cos3)) /3))),
                Convert.ToInt32(Math.Round(Math.Sqrt(((G * cos1) * (G * cos1) + (G * cos2) * (G * cos2) + (G * cos3) * (G * cos3)) / 3))),
                Convert.ToInt32(Math.Round(Math.Sqrt(((B * cos1) * (B * cos1) + (B * cos2) * (B * cos2) + (B * cos3) * (B * cos3)) / 3)))
            );
            //NewColor = sqrt((R1 ^ 2 + R2 ^ 2) / 2),sqrt((G1 ^ 2 + G2 ^ 2) / 2),sqrt((B1 ^ 2 + B2 ^ 2) / 2)

            return new SolidBrush(color);
        }

        private double[] Swap(double a, double b) {
            return new double[] { b, a };
        }

        private int[] SwapInt(int a, int b) {
            return new int[] { b, a };
        }

        private bool filterCoordinates(double[] a, double[] b, double[] c) {
            /*if ((a[0] == b[0]) && (b[0] == c[0])) {
                return false;
            }

            if ((a[1] == b[1]) && (b[1] == c[1])) {
                return false;
            }*/


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

        private void FillPolygon(PaintEventArgs pea, Brush brush,
            int x0, int y0, double z0, int x1, int y1, double z1, int x2, int y2, double z2) {

            double[] a = new double[] { x0, y0, z0 };
            double[] b = new double[] { x1, y1, z1 };
            double[] c = new double[] { x2, y2, z2 };

            if (!filterCoordinates(a, b, c)) {
                return ;
            }
            
            //Console.WriteLine(z0 + " " + z1 + " " + z2);
            if (y0 > y1) {
                int[] tmp = SwapInt(y0, y1);
                y0 = tmp[0];
                y1 = tmp[1];

                tmp = SwapInt(x0, x1);
                x0 = tmp[0];
                x1 = tmp[1];

                double[] tmp_d = Swap(z0, z1);
                z0 = tmp_d[0];
                z1 = tmp_d[1];

            }

            if (y0 > y2) {
                int[] tmp = SwapInt(y0, y2);
                y0 = tmp[0];
                y2 = tmp[1];

                tmp = SwapInt(x0, x2);
                x0 = tmp[0];
                x2 = tmp[1];

                double[] tmp_d = Swap(z0, z2);
                z0 = tmp_d[0];
                z2 = tmp_d[1];
            }

            if (y1 > y2) {
                int[] tmp = SwapInt(y1, y2);
                y1 = tmp[0];
                y2 = tmp[1];

                tmp = SwapInt(x1, x2);
                x1 = tmp[0];
                x2 = tmp[1];

                double[] tmp_d = Swap(z1, z2);
                z1 = tmp_d[0];
                z2 = tmp_d[1];

            }

            double total_height = y2 - y0;
            for (int i = 0; i < total_height; i++) {
                bool second_half = i > y1 - y0 || y1 == y0;
                double segment_height = second_half ? y2 - y1 : y1 - y0;
                double alpha = i / total_height;
                double beta = (i - (second_half ? y1 - y0 : 0)) / segment_height;
                double Ax = x0 + (x2 - x0) * alpha;
                double Ay = y0 + (y2 - y0) * alpha;
                double Az = z0 + (z2 - z0) * alpha;

                double Bx = second_half ? x1 + (x2 - x1) * beta : x0 + (x1 - x0) * beta;
                double By = second_half ? y1 + (y2 - y1) * beta : y0 + (y1 - y0) * beta;
                double Bz = second_half ? z1 + (z2 - z1) * beta : z0 + (z1 - z0) * beta;

                if (Ax > Bx) {                    
                    double tmp = Ax;
                    Ax = Bx;
                    Bx = tmp;

                    tmp = Ay;
                    Ay = By;
                    By = tmp;

                    tmp = Az;
                    Az = Bz;
                    Bz = tmp;
                }


                for (int j = Convert.ToInt32(Ax); j <= Bx; j++) {                 
                    double phi = Bx == Ax ? 1 : (j - Ax) / (Bx - Ax);
                    double Pz = Az + (Bz - Az) * phi;
                    
                    if (Pz < zBuffer[j, y0 + i]) {                        
                        zBuffer[j, y0 + i] = Pz;
                        //Brush brush = PhongShadingBrush(a, b, c, new double[] { j, y0 + i, Pz });
                        pea.Graphics.FillRectangle(brush, j, y0 + i, 1, 1);
                    }                    
                }
            }            

        }

        private double GetNormal(double[] v1, double[] v2, double[] v3) {
            return (v2[0] - v1[0]) * (v3[1] - v1[1]) - (v3[0] - v1[0]) * (v2[1] - v1[1]);
        }

        private List<int> RejectFaces(List<List<int>> faces, Matrix<double> vertices) {
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

                bool sign = GetNormal(v1, v2, v3) < 0;
   
                if (sign) {
                    visible_faces_indexes.Add(index);
                }
                index += 1;
            }

            return visible_faces_indexes;
        }          

        private List<double[]> DDA_Line(double x1, double x2, double y1, double y2, double z1, double z2) {            
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

        private void DrawPoints(PaintEventArgs pea, Brush brush, List<double[]> xy) {
            for (int i = 0; i < xy.Count; i++) {
                int x_indx = Convert.ToInt32(Math.Round(xy[i][0]));
                int y_indx = Convert.ToInt32(Math.Round(xy[i][1]));
                if (xy[i][2] < zBuffer[x_indx, y_indx]) {
                  //  zBuffer[x_indx, y_indx] = xy[i][2];
                 //   pea.Graphics.FillRectangle(brush, Convert.ToSingle(xy[i][0]), Convert.ToSingle(xy[i][1]), 1, 1);
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e) {            
            bool shift = false;
            if (e.Shift) {
                shift = true;
            }

            switch (e.KeyCode) {
                case Constants.LEFT_BUTTON:                    
                    model.MoveFigure(new Point(-Constants.SPEED, 0, 0));
                    break;
                case Constants.RIGHT_BUTTON:                    
                    model.MoveFigure(new Point(Constants.SPEED, 0, 0));
                    break;
                case Constants.UP_BUTTON:
                    model.MoveFigure(new Point(0, Constants.SPEED, 0));
                    break;
                case Constants.DOWN_BUTTON:
                    model.MoveFigure(new Point(0, -Constants.SPEED, 0));
                    break;
                case Constants.X_ROTATE_BUTTON:                    
                    model.RotateFigure(TransformationMatrices.XRotationMatrix, shift);
                    break;
                case Constants.Y_ROTATE_BUTTON:                    
                    model.RotateFigure(TransformationMatrices.YRotationMatrix, shift);
                    break;
                case Constants.Z_ROTATE_BUTTON:
                    model.RotateFigure(TransformationMatrices.ZRotationMatrix, shift);
                    break;
                case Constants.CHANGE_PROJECTION_BUTTON:
                    model.ChangeProjection();
                    break;
                case Constants.OPEN_FILE_BUTTON:
                    
                    var file_path = string.Empty;
                    using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                        openFileDialog.InitialDirectory = "C:\\Users\\hp\\Desktop";
                        openFileDialog.Filter = "obj files (*.obj)|*.obj|All files (*.*)|*.*";                        
                        openFileDialog.RestoreDirectory = true;

                        if (openFileDialog.ShowDialog() == DialogResult.OK) {                            
                            file_path = openFileDialog.FileName;
                            model.SetModel(file_path);                            
                        }
                    }
                    break;
                default:
                    return;                                    
            }
            Refresh();
        }

        private void MainForm_MouseWheel(object sender, MouseEventArgs e) {            
            int angle = e.Delta;
            model.ScaleFigure(angle);
            Refresh();
        }

    }
}
