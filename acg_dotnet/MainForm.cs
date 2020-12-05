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

            foreach (List<int> face in faces) {
                List<double[]> polygonPoints = new List<double[]>();
                double x1, x2, y1, y2;
                for (int i = 1; i < face.Count; i++) {
                    x1 = vertices.At(0, face[i - 1] - 1);
                    y1 = vertices.At(1, face[i - 1] - 1);

                    x2 = vertices.At(0, face[i] - 1);
                    y2 = vertices.At(1, face[i] - 1);
                    //pea.Graphics.DrawLine(pen, new Point(Convert.ToInt32(x1), Convert.ToInt32(y1)), new Point(Convert.ToInt32(x2), Convert.ToInt32(y2)));
                    polygonPoints.AddRange(DDA_Line(x1, x2, y1, y2));
                }
                x1 = vertices.At(0, face[0] - 1);
                y1 = vertices.At(1, face[0] - 1);

                x2 = vertices.At(0, face.Last() - 1);
                y2 = vertices.At(1, face.Last() - 1);

                //pea.Graphics.DrawLine(pen, new Point(Convert.ToInt32(x1), Convert.ToInt32(y1)), new Point(Convert.ToInt32(x2), Convert.ToInt32(y2)));
                polygonPoints.AddRange(DDA_Line(x1, x2, y1, y2));

                //FillPolygon(pea, polygonPoints);
                DrawPoints(pea, brush, polygonPoints);
            }            
            // !!!!!!!!!!!!!! DrawPoints

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

        private void FillPolygon(PaintEventArgs pea, List<double[]> polygonPoints) {           
            polygonPoints = polygonPoints.OrderBy(xy => xy[1]).ToList(); // sort by y
                                                                         //polygonPoints = polygonPoints.OrderBy(xy => xy[0]).ToList(); // sort by x

            Console.WriteLine("-----------------------------------");

            for (int j = 0; j < polygonPoints.Count; j++) {
                Console.WriteLine(polygonPoints[j][1].ToString() + " " + polygonPoints[j][0].ToString());
            }

            Brush brush = Brushes.Green;
            int i = 0;
            int first_y = 0;
            while (first_y < polygonPoints.Count) {                
                // add all x to x list
                List<double> x = new List<double>();

                int y_tmp = Convert.ToInt32(polygonPoints[first_y][1]);
                //x.Add(y_tmp);
                first_y += 1;
                for (int j = first_y; j < polygonPoints.Count; j++) {
                    if (Convert.ToInt32(polygonPoints[j][1]) != y_tmp) {
                        first_y = j;
                        break;
                    }
                    x.Add(polygonPoints[j][0]);
                }
                x.Sort();                       

                if (x.Count > 1) {
                    for (int k = 0; k < x.Count-1; k += 2) {                        
                        double x1 = x[k];
                        double x2 = x[k+1];
                        //Console.Write(x1.ToString() + " " + x2.ToString() + " | ");                        

                        DrawPoints(pea, brush, DDA_Line(x1, x2, y_tmp, y_tmp));
                    }
                }
                //Console.Write("\n");
            }
        } 

        private List<double[]> DDA_Line(double x1, double x2, double y1, double y2) {            
            double L = Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
            double dx = (x2 - x1) / L;
            double dy = (y2 - y1) / L;

            List<double[]> xy = new List<double[]>();
            //List<double> y = new List<double>();
            int i = 0;
            xy.Add(new double[] { x1, y1 });
            //x.Add(x1);
            //y.Add(y1);
            i += 1;

            while (i < L) {
                xy.Add(new double[] { xy[i - 1][0] + dx, xy[i - 1][1] + dy });
                //x.Add(x[i - 1] + dx);
                //y.Add(y[i - 1] + dy);
                i += 1;
            }

            xy.Add(new double[] { x2, y2 });
            //x.Add(x2);
            //y.Add(y2);
            /* i = 0;
            while (i <= L) {                                
                //Console.WriteLine(x[i].ToString() + " " + y[i].ToString());
                pea.Graphics.FillRectangle(brush, Convert.ToSingle(xy[i][0]), Convert.ToSingle(xy[i][1]), 1, 1);
                i += 1;
            } */
            return xy;//new Tuple<List<double>, List<double>>(x, y);
        }

        private void DrawPoints(PaintEventArgs pea, Brush brush, List<double[]> xy) {
            for (int i = 0; i < xy.Count; i++) {
                pea.Graphics.FillRectangle(brush, Convert.ToSingle(xy[i][0]), Convert.ToSingle(xy[i][1]), 1, 1);
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
