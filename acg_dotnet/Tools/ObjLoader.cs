using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using acg_dotnet.Tools.Transformations;
using System.Globalization;

namespace acg_dotnet.Tools
{
    class ObjLoader {
        private List<List<double>> vertices = new List<List<double>>();
        private List<List<double>> vertex_textures = new List<List<double>>();
        private List<List<double>> vertex_normals = new List<List<double>>();

        private List<List<int>> faces_v = new List<List<int>>();
        private List<List<int>> faces_vn = new List<List<int>>();
        private List<List<int>> faces_vt = new List<List<int>>();

        private const String F_SEPARATOR_WITH_MISSING = "//";
        private const char F_SEPARATOR = '/';

        public void Reset() {
            vertices.Clear();
            vertex_textures.Clear();
            vertex_normals.Clear();

            faces_v.Clear();
            faces_vn.Clear();
            faces_vt.Clear();
        }
        
        public void Load(String path) {
            Reset();
            //StreamReader file = new StreamReader(path);
            using (StreamReader file = new StreamReader(path)) {
                string line;
                while ((line = file.ReadLine()) != null) {
                    string[] values = line.Split(new char[] { ' ' });
                    if (values.Length == 0) {
                        continue;
                    }
                    switch (values[0]) {
                        case "v": 
                            List<double> vertex = new List<double>();
                            for (int i = 1; i < values.Length; i++) {                                                                                                                             
                                vertex.Add(double.Parse(values[i], CultureInfo.InvariantCulture));
                            }
                            if (vertex.Count < 4) {
                                vertex.Add(1);
                            }                            
                            vertices.Add(vertex);

                            break;
                        case "vt":                            
                            vertex = new List<double>();
                            for (int i = 2; i < values.Length; i++) {
                                vertex.Add(double.Parse(values[i], CultureInfo.InvariantCulture));
                            }
                            vertex_textures.Add(vertex);
                            break;
                        case "vn":                            
                            vertex = new List<double>();
                            for (int i = 2; i < values.Length; i++) {
                                vertex.Add(double.Parse(values[i], CultureInfo.InvariantCulture));
                            }
                            if (vertex.Count < 4) {
                                vertex.Add(1);
                            }
                            vertex_normals.Add(vertex);
                            break;
                        case "f":
                            if (values[1].Contains(F_SEPARATOR_WITH_MISSING)) {
                                // f v1//vn1 v2//vn2 v3//vn3 ...
                                List<List<int>> f = new List<List<int>>();
                                for (int i = 1; i < values.Length; i++) {
                                    string[] v = values[i].Split(new char[] { F_SEPARATOR });
                                    f.Add(new List<int> { int.Parse(v[0]), -1, int.Parse(v[2]) });
                                }

                                List<int> result_fv = new List<int>();
                                List<int> result_fvn = new List<int>();

                                for (int i = 0; i < f.Count; i++) {
                                    result_fv.Add(f[i][0]);
                                    result_fvn.Add(f[i][2]);
                                }
                                faces_v.Add(result_fv);
                                faces_vn.Add(result_fvn);
                            }
                            else {
                                int num_of_vertices = values[1].Split(new char[] { F_SEPARATOR }).Length;
                                if (num_of_vertices == 2) {                                    
                                    List<List<int>> f = new List<List<int>>();
                                    for (int i = 1; i < values.Length; i++) {
                                        string[] v = values[i].Split(new char[] { F_SEPARATOR });
                                        f.Add(new List<int> { int.Parse(v[0]), int.Parse(v[1]), -1 });
                                    }

                                    List<int> result_fv = new List<int>();
                                    List<int> result_fvt = new List<int>();

                                    for (int i = 0; i < f.Count; i++) {
                                        result_fv.Add(f[i][0]);
                                        result_fvt.Add(f[i][2]);
                                    }
                                    faces_v.Add(result_fv);
                                    faces_vt.Add(result_fvt);

                                }
                                else if (num_of_vertices == 3) {
                                    // f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3 ...
                                    List<List<int>> f = new List<List<int>>();
                                    for (int i = 1; i < values.Length; i++) {
                                        string[] v = values[i].Split(new char[] { F_SEPARATOR });
                                        f.Add(new List<int> { int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2]) });
                                    }

                                    List<int> result_fv = new List<int>();
                                    List<int> result_fvn = new List<int>();
                                    List<int> result_fvt = new List<int>();

                                    for (int i = 0; i < f.Count; i++) {
                                        result_fv.Add(f[i][0]);
                                        result_fvt.Add(f[i][1]);
                                        result_fvn.Add(f[i][2]);
                                    }
                                    faces_v.Add(result_fv);
                                    faces_vt.Add(result_fvt);
                                    faces_vn.Add(result_fvn);

                                }
                                else {
                                    throw new Exception("Incorrect file structure");
                                }
                            }                            
                            break;
                    }
                }
                
            }
                   
        }

        public Matrix<double> Vertices {
            get {                
                return DenseMatrix.OfArray(TransformationMatrices.ListToArray<double>(vertices));
            }
        }

        public Matrix<double> VerticesTextures {
            get {
                return DenseMatrix.OfArray(TransformationMatrices.ListToArray<double>(vertex_textures));
            }
        }

        public Matrix<double> VerticesNormals {
            get {
                return DenseMatrix.OfArray(TransformationMatrices.ListToArray<double>(vertex_normals));
            }
        }

        public List<List<int>> FacesV {
            get {
                return faces_v;//DenseMatrix.OfArray(TransformationMatrices.ListToArray<int>(faces_v));
            }
        }

        public List<List<int>> FacesVn {
            get {
                return faces_vn;//DenseMatrix.OfArray(TransformationMatrices.ListToArray<int>(faces_vn));
            }
        }

        public List<List<int>> FacesVt {
            get {
                return faces_vt;//DenseMatrix.OfArray(TransformationMatrices.ListToArray<int>(faces_vt));
            }
        }

    }
}
