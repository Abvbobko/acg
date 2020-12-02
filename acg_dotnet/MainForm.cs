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

            Matrix<double> vertices = model.TransformCoordinates();
            List<List<int>> faces = model.FacesV;

            foreach (List<int> face in faces) {
                double x1, x2, y1, y2;
                for (int i = 1; i < face.Count; i++) {
                    x1 = vertices.At(0, face[i - 1] - 1);
                    y1 = vertices.At(1, face[i - 1] - 1);

                    x2 = vertices.At(0, face[i] - 1);
                    y2 = vertices.At(1, face[i] - 1);

                    // dda.dda_line(self.qp, x1, x2, y1, y2)
                }         
                x1 = vertices.At(0, face[0] - 1);
                y1 = vertices.At(1, face[0] - 1);

                x2 = vertices.At(0, face.Last() - 1);
                y2 = vertices.At(1, face.Last() - 1);
                //dda.dda_line(self.qp, x1, x2, y1, y2)
            }



           

            // Defines the both points to connect 
            // pt1 is (30.0, 30.0) which represents (x1, y1) 
            PointF pt1 = new PointF(30.0F, 30.0F);

            // pt2 is (200.0, 300.0) which represents (x2, y2) 
            PointF pt2 = new PointF(200.0F, 300.0F);

            // Draws the line 
            pea.Graphics.DrawLine(pen, pt1, pt2);
        }

    }
}
