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
    class Model
    {

        ObjLoader objLoader;
        Matrix<double> vertices;
        Matrix<double> moving_matrix;
        Matrix<double> scale_matrix;
        Matrix<double> rotate_matrix;


        Model() {
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

        public void RotateFigure(Matrix<double> rotate_matrix, bool direction = true) {
            // true - clockwise
            // false - counterclockwise
        }

    }
}
