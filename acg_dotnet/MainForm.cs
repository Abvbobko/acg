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


namespace acg_dotnet
{
    public partial class MainForm : Form
    {
        public MainForm() {
            InitializeComponent();

            //Model model = new Model();
            
        }




        protected override void OnPaint(PaintEventArgs pea) {
            // Defines pen 
            Pen pen = new Pen(ForeColor);

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
