﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace acg_dotnet.Tools
{
    class ZBuffer
    {
        private double[,] buffer;
        private int width;
        private int height;

        public ZBuffer(int width, int height) {
            this.width = width;
            this.height = height;

            buffer = new double[width, height];
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    buffer[i, j] = double.PositiveInfinity;
                }
            }
        }

        public double this[int x, int y] {
            get {
                if ((x < width) && (y < height) && (x >= 0) && (y >= 0)) {
                    //Console.WriteLine(x + " " + y + " " + width + " " + height);
                    return buffer[x, y];
                }                
                return double.NegativeInfinity;
            }

            set {
                if ((x < width) && (y < height) && (x >= 0) && (y >= 0)) {
                    buffer[x, y] = value;
                }                
            }
        }


    }
}
