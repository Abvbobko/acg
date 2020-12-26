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
            Matrix<double> vertices_t = model.TransformVerticesTextures();

            // возвращать только индексы нужных faces
            List<int> indexes = CoordinateOperations.RejectFaces(model.FacesV, vertices);
            List<List<int>> faces = model.FacesV;
            List<List<int>> faces_vn = model.FacesVn;
            List<List<int>> faces_vt = model.FacesVt;

            zBuffer = new ZBuffer(Size.Width, Size.Height);            
            
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
                
                // ----- normals ------
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


                // ------ textures ------
                double[] vt1 = new double[] {
                    vertices_t.At(0, faces_vt[index][0] - 1),
                    vertices_t.At(1, faces_vt[index][0] - 1),
                    vertices_t.At(2, faces_vt[index][0] - 1)
                };

                double[] vt2 = new double[] {
                    vertices_t.At(0, faces_vt[index][1] - 1),
                    vertices_t.At(1, faces_vt[index][1] - 1),
                    vertices_t.At(2, faces_vt[index][1] - 1)
                };

                double[] vt3 = new double[] {
                    vertices_t.At(0, faces_vt[index][2] - 1),
                    vertices_t.At(1, faces_vt[index][2] - 1),
                    vertices_t.At(2, faces_vt[index][2] - 1)
                };
                /*Brush polygon_brush = GetBrush(
                    new double[] { x1_, y1_, z1_ },
                    new double[] { x2_, y2_, z2_ },
                    new double[] { x3_, y3_, z3_ },
                    vn1, vn2, vn3                    
                );  */

                FillPolygon(
                    pea,
                    //polygon_brush,
                    Convert.ToInt32(Math.Round(x1_)), Convert.ToInt32(Math.Round(y1_)), z1_,
                    Convert.ToInt32(Math.Round(x2_)), Convert.ToInt32(Math.Round(y2_)), z2_,
                    Convert.ToInt32(Math.Round(x3_)), Convert.ToInt32(Math.Round(y3_)), z3_,
                    vn1, vn2, vn3,
                    vt1, vt2, vt3
                );                
            }            
            
        }
                    
        private double[] Swap(double a, double b) {
            return new double[] { b, a };
        }

        private int[] Swap(int a, int b) {
            return new int[] { b, a };
        }        

        private void FillPolygon(PaintEventArgs pea,
            int x0, int y0, double z0, int x1, int y1, double z1, int x2, int y2, double z2,
            double[] vn1, double[] vn2, double[] vn3, double[] vt1, double[] vt2, double[] vt3) {

            double[] a = new double[] { x0, y0, z0 };
            double[] b = new double[] { x1, y1, z1 };
            double[] c = new double[] { x2, y2, z2 };

            if (!CoordinateOperations.notOnOneLine(a, b, c)) {
                return ;
            }
            
            
            if (y0 > y1) {
                int[] tmp = Swap(y0, y1);
                y0 = tmp[0];
                y1 = tmp[1];

                tmp = Swap(x0, x1);
                x0 = tmp[0];
                x1 = tmp[1];

                double[] tmp_d = Swap(z0, z1);
                z0 = tmp_d[0];
                z1 = tmp_d[1];

            }

            if (y0 > y2) {
                int[] tmp = Swap(y0, y2);
                y0 = tmp[0];
                y2 = tmp[1];

                tmp = Swap(x0, x2);
                x0 = tmp[0];
                x2 = tmp[1];

                double[] tmp_d = Swap(z0, z2);
                z0 = tmp_d[0];
                z2 = tmp_d[1];
            }

            if (y1 > y2) {
                int[] tmp = Swap(y1, y2);
                y1 = tmp[0];
                y2 = tmp[1];

                tmp = Swap(x1, x2);
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
                        Brush brush = Lighting.PhongLighting(
                            model, a, b, c, new double[] { j, y0 + i, Pz }, vn1, vn2, vn3, vt1, vt2, vt3
                        );
                        pea.Graphics.FillRectangle(brush, j, y0 + i, 1, 1);
                    }                    
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
