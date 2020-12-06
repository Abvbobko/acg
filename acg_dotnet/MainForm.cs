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

            //Size = new Size(Constants.WIN_WIDTH, Constants.WIN_HEIGHT);
            model = new Model();            
        }

        protected override void OnPaint(PaintEventArgs pea) {
            DrawModel(pea);
        }

        private void DrawModel(PaintEventArgs pea) {

            // Defines pen 
            Pen pen = new Pen(ForeColor);
            Brush brush = Brushes.Black;

            Matrix<double> vertices = model.TransformCoordinates();
            List<List<int>> faces = RejectFaces(model.FacesV, vertices);

            zBuffer = new ZBuffer(Size.Width, Size.Height);

            foreach (List<int> face in faces) {
                List<double[]> polygonPoints = new List<double[]>();
                double x1, x2, y1, y2, z1, z2;
                ///////////////// по идее можно удалить
                /* for (int i = 1; i < face.Count; i++) {
                    x1 = vertices.At(0, face[i - 1] - 1);
                    y1 = vertices.At(1, face[i - 1] - 1);
                    z1 = vertices.At(2, face[i - 1] - 1);

                    x2 = vertices.At(0, face[i] - 1);
                    y2 = vertices.At(1, face[i] - 1);
                    z2 = vertices.At(2, face[i] - 1);
                    //pea.Graphics.DrawLine(pen, new Point(Convert.ToInt32(x1), Convert.ToInt32(y1)), new Point(Convert.ToInt32(x2), Convert.ToInt32(y2)));
                    polygonPoints.AddRange(DDA_Line(x1, x2, y1, y2, z1, z2));
                }*/
                //////////////////////////////////////////
                x1 = vertices.At(0, face[0] - 1);
                y1 = vertices.At(1, face[0] - 1);
                z1 = vertices.At(2, face[0] - 1);

                x2 = vertices.At(0, face.Last() - 1);
                y2 = vertices.At(1, face.Last() - 1);
                z2 = vertices.At(2, face.Last() - 1);
                Brush brush1 = Brushes.Green;

                double x1_ = vertices.At(0, face[0] - 1), y1_ = vertices.At(1, face[0] - 1), z1_ = vertices.At(2, face[0] - 1);
                double x2_ = vertices.At(0, face[1] - 1), y2_ = vertices.At(1, face[1] - 1), z2_ = vertices.At(2, face[1] - 1);
                double x3_ = vertices.At(0, face[2] - 1), y3_ = vertices.At(1, face[2] - 1), z3_ = vertices.At(2, face[2] - 1);
                FillPolygon(
                    pea, 
                    brush1,
                    Convert.ToInt32(Math.Round(x1_)), Convert.ToInt32(Math.Round(y1_)), Convert.ToInt32(Math.Round(z1_)),
                    Convert.ToInt32(Math.Round(x2_)), Convert.ToInt32(Math.Round(y2_)), Convert.ToInt32(Math.Round(z2_)),
                    Convert.ToInt32(Math.Round(x3_)), Convert.ToInt32(Math.Round(y3_)), Convert.ToInt32(Math.Round(z3_))
                );

                //pea.Graphics.DrawLine(pen, new Point(Convert.ToInt32(x1), Convert.ToInt32(y1)), new Point(Convert.ToInt32(x2), Convert.ToInt32(y2)));
                //polygonPoints.AddRange(DDA_Line(x1, x2, y1, y2, z1, z2));

                //FillPolygon(pea, polygonPoints);
                
                //DrawPoints(pea, brush, polygonPoints);
            }            
            // !!!!!!!!!!!!!! DrawPoints

        }

        private int[] Swap(int a, int b) {
            return new int[] { b, a };
        }

        private void FillPolygon(PaintEventArgs pea, Brush brush, 
            int x0, int y0, int z0, int x1, int y1, int z1, int x2, int y2, int z2) {

            if (y0 > y1) {
                int[] tmp = Swap(y0, y1);
                y0 = tmp[0];
                y1 = tmp[1];

                tmp = Swap(x0, x1);
                x0 = tmp[0];
                x1 = tmp[1];

                tmp = Swap(z0, z1);
                z0 = tmp[0];
                z1 = tmp[1];

            }

            if (y0 > y2) {
                int[] tmp = Swap(y0, y2);
                y0 = tmp[0];
                y2 = tmp[1];

                tmp = Swap(x0, x2);
                x0 = tmp[0];
                x2 = tmp[1];

                tmp = Swap(z0, z2);
                z0 = tmp[0];
                z2 = tmp[1];
            }

            if (y1 > y2) {
                int[] tmp = Swap(y1, y2);
                y1 = tmp[0];
                y2 = tmp[1];

                tmp = Swap(x1, x2);
                x1 = tmp[0];
                x2 = tmp[1];

                tmp = Swap(z1, z2);
                z1 = tmp[0];
                z2 = tmp[1];

            }
            
            int total_height = y2 - y0;
            for (int i = 0; i < total_height; i++) {
                bool second_half = i > y1 - y0 || y1 == y0;
                int segment_height = second_half ? y2 - y1 : y1 - y0;
                float alpha = (float)i / total_height;
                float beta = (float)(i - (second_half ? y1 - y0 : 0)) / segment_height;
                float Ax = x0 + (x2 - x0) * alpha;
                float Ay = y0 + (y2 - y0) * alpha;
                float Az = z0 + (z2 - z0) * alpha;

                float Bx = second_half ? x1 + (x2 - x1) * beta : x0 + (x1 - x0) * beta;
                float By = second_half ? y1 + (y2 - y1) * beta : y0 + (y1 - y0) * beta;
                float Bz = second_half ? z1 + (z2 - z1) * beta : z0 + (z1 - z0) * beta;

                if (Ax > Bx) {
                    //std::swap(A, B);
                    float tmp = Ax;
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
                    //float phi = B.x == A.x ? 1. : (float)(j - A.x) / (float)(B.x - A.x);
                    //Vec3i P = Vec3f(A) + Vec3f(B - A) * phi;
                    float phi = Bx == Ax ? 1 : (float)(j - Ax) / (float)(Bx - Ax);
                    double Pz = Az + (Bz - Az) * phi;
                    if (Pz < zBuffer[j, y0 + i]) {
                        zBuffer[j, y0 + i] = Pz;
                        pea.Graphics.FillRectangle(brush, j, y0 + i, 1, 1);
                    }
                    // double T = (x1 - x0) * i;
                    // double K = (j - x0) * (y1 - y0);
                    // double B = (T-K)/(y2*(x1-x0) + y1*(x2-x1) + y1*(x0-x2));
                    // double A = (j - x0 - B * (x2 - x0)) / (x1 - x0);
                    // double z = z0 + A * (z1 - z0) + B * (z2 - z0);
                    // z = Az + (Bz - Az) * (j - Ax)/(Bx - Ax);
                    // if (z < zBuffer[j, y0 + i]) {
                    //    zBuffer[j, y0 + i] = z;
                    //pea.Graphics.FillRectangle(brush, j, y0 + i, 1, 1);
                   // }
                    
                    //image.set(j, y0 + i); // attention, due to int casts t0.y+i != A.y
                }
            }            

        }

        private List<List<int>> RejectFaces(List<List<int>> faces, Matrix<double> vertices) {
            List<List<int>> visible_faces = new List<List<int>>();
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

                bool sign = (v2[0] - v1[0]) * (v3[1] - v1[1]) - (v3[0] - v1[0]) * (v2[1] - v1[1]) < 0;
   
                if (sign) {
                    visible_faces.Add(face);
                }
            }

            return visible_faces;
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
