using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using acg_dotnet.Tools.Transformations;
using acg_dotnet.Tools;


namespace acg_dotnet
{
    class Model {

        ObjLoader objLoader;
        Matrix<double> vertices;
        Matrix<double> vertices_normals;
        Matrix<double> moving_matrix;
        Matrix<double> scale_matrix;
        Matrix<double> rotate_matrix;
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
    }
}