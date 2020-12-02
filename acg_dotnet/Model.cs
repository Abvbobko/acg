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
        Matrix<double> moving_matrix;
        Matrix<double> scale_matrix;
        Matrix<double> rotate_matrix;
        bool projection_type = true; // true - ortographic / false - perspective


        public Model() {
            objLoader = new ObjLoader();
            SetModel(Constants.DEFAULT_PATH);
        }

        public void SetModel(string path) {
            vertices = LoadModel(path).Transpose();
            Reset();
        }

        public void Reset() {
            moving_matrix = TransformationMatrices.GetEye4();
            scale_matrix = TransformationMatrices.GetEye4().Multiply(Constants.INIT_MODEL_TO_WORLD_MATRIX);
            rotate_matrix = TransformationMatrices.GetEye4();
        }

        public Matrix<double> LoadModel(string path) {
            objLoader.Load(path);
            return objLoader.Vertices;
        }

        public void MoveFigure(Point matrix_args) {
            moving_matrix = TransformationMatrices.MovingMatrix(matrix_args).Multiply(
                moving_matrix
            );
        }

        public delegate Matrix<double> RotateMatrix(double speed);

        public void RotateFigure(RotateMatrix rotation_matrix, bool direction = true) {
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
                speed = -Constants.SCALE_DOWN_SPEED;
            }
            if (speed != 0) {
                scale_matrix = TransformationMatrices.ScaleMatrix(new Point(speed, speed, speed)).Multiply(scale_matrix);
            }
        }

        public void ChangeProjection() {
            projection_type = !projection_type;
        }

        public Matrix<double> TransformCoordinates() {
            return Constants.W_TO_V.Multiply(moving_matrix
                ).Multiply(rotate_matrix
                ).Multiply(scale_matrix
                ).Multiply(vertices);
        }

        public List<List<int>> FacesV {
            get {
                return objLoader.FacesV;
            }
        }
    }
}
