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
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


namespace acg_dotnet
{
    public partial class MainForm : Form
    {
        Model model;

        public MainForm() {
            InitializeComponent();
            
            Size = new Size(Constants.WIN_WIDTH, Constants.WIN_HEIGHT);
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
            List<List<int>> faces = model.FacesV;

            foreach (List<int> face in faces) {
                double x1, x2, y1, y2;
                for (int i = 1; i < face.Count; i++) {                    
                    x1 = vertices.At(0, face[i - 1] - 1);
                    y1 = vertices.At(1, face[i - 1] - 1);

                    x2 = vertices.At(0, face[i] - 1);
                    y2 = vertices.At(1, face[i] - 1);
                    pea.Graphics.DrawLine(pen, new Point(Convert.ToInt32(x1), Convert.ToInt32(y1)), new Point(Convert.ToInt32(x2), Convert.ToInt32(y2)));
                    //DDA_Line(pea, brush, x1, x2, y1, y2);
                }         
                x1 = vertices.At(0, face[0] - 1);
                y1 = vertices.At(1, face[0] - 1);

                x2 = vertices.At(0, face.Last() - 1);
                y2 = vertices.At(1, face.Last() - 1);

                pea.Graphics.DrawLine(pen, new Point(Convert.ToInt32(x1), Convert.ToInt32(y1)), new Point(Convert.ToInt32(x2), Convert.ToInt32(y2)));
                //DDA_Line(pea, brush, x1, x2, y1, y2);                
            }
        
        }

        private void DDA_Line(PaintEventArgs pea, Brush brush, double x1, double x2, double y1, double y2) {            
            double L = Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
            double dx = (x2 - x1) / L;
            double dy = (y2 - y1) / L;

            List<double> x = new List<double>();
            List<double> y = new List<double>();
            int i = 0;
            x.Add(x1);
            y.Add(y1);
            i += 1;

            while (i < L) {
                x.Add(x[i - 1] + dx);
                y.Add(y[i - 1] + dy);
                i += 1;
            }

            x.Add(x2);
            y.Add(y2);
            for (int j = 0; j < x.Count; j++) {
                Console.WriteLine(x[j]);
            }
            while (i <= L) {
                //Console.WriteLine(x[i].ToString() + " " + y[i].ToString());
                pea.Graphics.FillRectangle(brush, Convert.ToSingle(x[i]), Convert.ToSingle(y[i]), 1, 1);
                i += 1;
            }            
        }

    }
}
