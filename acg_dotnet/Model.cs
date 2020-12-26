using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using acg_dotnet.Tools.Transformations;
using acg_dotnet.Tools;
using System.Drawing;


namespace acg_dotnet
{
    class Model {

        ObjLoader objLoader;
        Matrix<double> vertices;
        Matrix<double> vertices_normals;
        Matrix<double> vertices_textures;

        Matrix<double> moving_matrix;
        Matrix<double> scale_matrix;
        Matrix<double> rotate_matrix;

        private Bitmap diffuse_map;
        private Bitmap normal_map;
        private Bitmap specular_map;

        bool projection_type = true; // true - ortographic / false - perspective


        public Model() {
            objLoader = new ObjLoader();
            SetModel(Constants.DEFAULT_PATH);
        }

        public void SetModel(string path) {
            LoadModel(path);
            Reset();
        }

        public void Reset() {
            moving_matrix = TransformationMatrices.GetEye4();
            scale_matrix = TransformationMatrices.GetEye4().Multiply(Constants.INIT_MODEL_TO_WORLD_MATRIX);
            rotate_matrix = TransformationMatrices.GetEye4();
        }

        public void LoadModel(string path) {
            objLoader.Load(path);
            vertices = objLoader.Vertices.Transpose();
            vertices_normals = objLoader.VerticesNormals.Transpose();
            vertices_textures = objLoader.VerticesTextures.Transpose();

            diffuse_map = objLoader.DiffuseMap;
            normal_map = objLoader.NormalMap;
            specular_map = objLoader.SpecularMap;
        }

        public void MoveFigure(Point matrix_args) {
            moving_matrix = TransformationMatrices.MovingMatrix(matrix_args).Multiply(
                moving_matrix
            );
        }

        public delegate Matrix<double> RotateMatrix(double theta);

        public void RotateFigure(RotateMatrix rotation_matrix, bool direction = false) {
            // true - clockwise
            // false - counterclockwise

            double rotate_speed = Constants.ROTATE_SPEED;
            if (!direction) {
                rotate_speed = -rotate_speed;
            }
            rotate_matrix = rotate_matrix.Multiply(rotation_matrix(rotate_speed));
        }

        public void ScaleFigure(int angle) {
            // wheelEvent in the python version
            double speed = 0;
            if (angle > 0) {
                speed = Constants.SCALE_UP_SPEED;
            }
            else if (angle < 0) {
                speed = Constants.SCALE_DOWN_SPEED;
            }
            if (speed != 0) {
                scale_matrix = TransformationMatrices.ScaleMatrix(new Point(speed, speed, speed)).Multiply(scale_matrix);
            }
        }

        public void ChangeProjection() {
            projection_type = !projection_type;
        }

        public Matrix<double> TransformVertices() {
            return TransformCoordinates(vertices);
        }
    
        public Matrix<double> TransformVerticesNormals() {
            return TransformCoordinates(vertices_normals, true);
        }

        public Matrix<double> TransformNormal(Vector<double> normal) {
            return TransformCoordinates(DenseMatrix.OfColumnVectors(normal), true);
        }

        public Matrix<double> TransformVerticesTextures() {
            return vertices_textures;//TransformCoordinates(vertices_normals, true);
        }        

        public Matrix<double> TransformCoordinates(Matrix<double> v, bool to_view=false) {
            if (to_view) {

                // can delete moving and scale matrix
                return Constants.W_TO_O.Multiply(moving_matrix
                    ).Multiply(rotate_matrix
                    ).Multiply(scale_matrix
                    ).Multiply(v);
            }

            if (projection_type) {
                return Constants.W_TO_V.Multiply(moving_matrix
                    ).Multiply(rotate_matrix
                    ).Multiply(scale_matrix
                    ).Multiply(v);
            }

            Matrix<double> perspective = Constants.W_TO_P_perspective.Multiply(moving_matrix
              ).Multiply(rotate_matrix
              ).Multiply(scale_matrix
              ).Multiply(v);            

            for (int i = 0; i < perspective.RowCount; i++) {
                for (int j = 0; j < perspective.ColumnCount; j++) {
                    perspective[i, j] /= 1.0* perspective[perspective.RowCount - 1, j];
                }
            }            

            return Constants.P_TO_V.Multiply(perspective);
        }

        public List<List<int>> FacesV {
            get {
                return objLoader.FacesV;
            }
        }

        public List<List<int>> FacesVn {
            get {
                return objLoader.FacesVn;
            }
        }

        public List<List<int>> FacesVt {
            get {
                return objLoader.FacesVt;
            }
        }

        public int[] GetColor(Bitmap map, double x, double y) {
            y = 1 - y;
            int new_x = Convert.ToInt32(map.Width * x);
            int new_y = Convert.ToInt32(map.Height * y);
            
            if (new_x < 0) {
                new_x = 0;
            }
            else if (new_x >= map.Width) {
                new_x = map.Width - 1;
            }

            if (new_y < 0) {
                new_y = 0;
            }
            else if (new_y >= map.Height) {
                new_y = map.Height - 1;
            }

            Color color = map.GetPixel(
                new_x,
                new_y
            );
            return new int[] { color.R, color.G, color.B };
        }

        public int[] GetDiffuseColor(double x, double y) {
            return GetColor(diffuse_map, x, y);
        }

        public double[] GetNormal(double x, double y) {
            int[] rgb = GetColor(normal_map, x, y);
            double[] normal = new double[rgb.Length+1];
            for (int i = 0; i < rgb.Length; i++) {
                normal[i] = (rgb[i] / 255.0) * 2 - 1;
            }
            normal[normal.Length - 1] = 1;
            //normal = VectorOperations.NormalizeArray(normal);    
            //int tmp = TransformNormal(DenseMatrix.OfColumnVectors(DenseVector.OfArray(normal))).AsArray().Length;
            //Console.WriteLine(tmp);            
            Matrix<double> tmp = TransformNormal(DenseVector.OfArray(normal));
            //Console.WriteLine(tmp.RowCount);
            //Console.WriteLine(tmp.ColumnCount);

            normal = new double[tmp.RowCount - 1];
            for (int i = 0; i < tmp.RowCount-1; i++) {
                normal[i] = tmp[i, 0];
            }            
            return normal;
            //return GetColor(normal_map, x, y);
        }
    }
}