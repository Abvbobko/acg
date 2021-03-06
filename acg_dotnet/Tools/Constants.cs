﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using acg_dotnet.Tools.Transformations;
using acg_dotnet.Tools;
using System.Windows.Forms;


namespace acg_dotnet.Tools
{
    static class Constants
    {
        // m_to_w - model to world
        // w_to_o - world to observer
        // o_to_p - observer to projection
        // p_to_v - projection to viewport


        public const string DEFAULT_PATH = "C:\\Users\\hp\\Desktop\\acg_dotnet\\acg_dotnet\\data\\head\\Model.obj";
        public const int WIN_WIDTH = 800;
        public const int WIN_HEIGHT = 600;

        public static readonly double[] EYE = { 0, 0, 300 };
        public static readonly double[] TARGET = { 0, 0, 0 };
        public static readonly double[] UP = { 0, 1, 0 };      

        public const int O_TO_P_WIDTH = WIN_WIDTH;
        public const int O_TO_P_HEIGHT = WIN_HEIGHT;
        public const int Z_NEAR = 0;
        public const int Z_FAR = 700;

        public const int P_TO_V_WIDTH = WIN_WIDTH;
        public const int P_TO_V_HEIGHT = WIN_HEIGHT;
        public const int X_MIN = 100;
        public const int Y_MIN = -100;

        public const int SPEED = 5;

        public const int RIGHT_SPEED = SPEED;
        public const int LEFT_SPEED = SPEED;
        public const int UP_SPEED = SPEED;
        public const int DOWN_SPEED = SPEED;

        public const int INCREASE_SPEED = SPEED;
        public const int DECREASE_SPEED = SPEED;

        public const double ROTATE_SPEED = 0.1;

        public static Matrix<double> INIT_MODEL_TO_WORLD_MATRIX = TransformationMatrices.ScaleMatrix(new Point(250, 250, 250));

        public const double SCALE_UP_SPEED = 1.2;
        public const double SCALE_DOWN_SPEED = 1.0 / SCALE_UP_SPEED;

        public static Matrix<double> W_TO_O = TransformationMatrices.WorldToObserver(EYE, TARGET, UP);
        public static Matrix<double> O_TO_P = TransformationMatrices.orthographicMatrix(
            O_TO_P_HEIGHT, O_TO_P_WIDTH, Z_NEAR, Z_FAR
        );

        public static Matrix<double> O_TO_P_PERSPECTIVE = TransformationMatrices.perspectiveMatrix(
            O_TO_P_HEIGHT, O_TO_P_WIDTH, Z_NEAR, Z_FAR
        );

        public static Matrix<double> P_TO_V = TransformationMatrices.viewportMatrix(
            P_TO_V_HEIGHT, P_TO_V_WIDTH, X_MIN, Y_MIN
        );

        public static Matrix<double> W_TO_V = P_TO_V.Multiply(O_TO_P).Multiply(W_TO_O);
        public static Matrix<double> W_TO_V_perspective = P_TO_V.Multiply(O_TO_P_PERSPECTIVE).Multiply(W_TO_O);


        // keys
        public const Keys LEFT_BUTTON = Keys.A;
        public const Keys RIGHT_BUTTON = Keys.D;
        public const Keys UP_BUTTON = Keys.W;
        public const Keys DOWN_BUTTON = Keys.S;

        public const Keys X_ROTATE_BUTTON = Keys.X;
        public const Keys Y_ROTATE_BUTTON = Keys.Y;
        public const Keys Z_ROTATE_BUTTON = Keys.Z;

        public const Keys CHANGE_PROJECTION_BUTTON = Keys.P;
        public const Keys OPEN_FILE_BUTTON = Keys.O;
    }
}
