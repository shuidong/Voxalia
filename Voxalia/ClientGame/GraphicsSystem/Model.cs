﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Frenetic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Voxalia.ClientGame.GraphicsSystem
{
    /// <summary>
    /// Represents a 3D model.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// All currently loaded models.
        /// </summary>
        public static List<Model> LoadedModels;

        public static Model Cube;

        /// <summary>
        /// Prepares the model system.
        /// </summary>
        public static void Init()
        {
            LoadedModels = new List<Model>();
            Cube = FromString("cube", CubeData);
            LoadedModels.Add(Cube);
        }

        static string CubeData = "o Cube\nv 0.500000 -0.500000 -0.500000\nv 0.500000 -0.500000 0.500000\nv -0.500000 -0.500000 0.500000\n" +
            "v -0.500000 -0.500000 -0.500000\nv 0.500000 0.500000 -0.500000\nv 0.500000 0.500000 0.500000\nv -0.500000 0.500000 0.500000\n" +
            "v -0.500000 0.500000 -0.500000\nvt 1.000000 0.000000\nvt 1.000000 1.000000\nvt 0.000000 1.000000\nvt 0.000000 0.000000\n" +
            "f 2/1 3/2 4/3\nf 8/1 7/2 6/3\nf 1/4 5/1 6/2\nf 2/4 6/1 7/2\nf 7/1 8/2 4/3\nf 1/1 4/2 8/3\nf 1/4 2/1 4/3\nf 5/4 8/1 6/3\n" +
            "f 2/3 1/4 6/2\nf 3/3 2/4 7/2\nf 3/4 7/1 4/3\nf 5/4 1/1 8/3\n";

        public static Model LoadModel(string filename)
        {
            try
            {
                filename = FileHandler.CleanFileName(filename);
                if (!FileHandler.Exists("models/" + filename + ".obj"))
                {
                    SysConsole.Output(OutputType.WARNING, "Cannot load model, file '" +
                        TextStyle.Color_Standout + "models/" + filename + ".obj" + TextStyle.Color_Warning +
                        "' does not exist.");
                    return null;
                }
                return FromString(filename, FileHandler.ReadText("models/" + filename + ".obj"));
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Failed to load model from filename '" +
                    TextStyle.Color_Standout + "models/" + filename + ".obj" + TextStyle.Color_Error + "'" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets the texture object for a specific texture name.
        /// </summary>
        /// <param name="texturename">The name of the texture</param>
        /// <returns>A valid texture object</returns>
        public static Model GetModel(string modelname)
        {
            modelname = FileHandler.CleanFileName(modelname);
            for (int i = 0; i < LoadedModels.Count; i++)
            {
                if (LoadedModels[i].Name == modelname)
                {
                    return LoadedModels[i];
                }
            }
            Model Loaded = LoadModel(modelname);
            if (Loaded == null)
            {
                Loaded = Model.FromString(modelname, CubeData);
            }
            LoadedModels.Add(Loaded);
            return Loaded;
        }

        /// <summary>
        /// loads a model from a .obj file string
        /// </summary>
        /// <param name="name">The name of the model</param>
        /// <param name="data">The .obj file string</param>
        /// <returns>A valid model</returns>
        public static Model FromString(string name, string data)
        {
            Model result = new Model(name);
            ModelMesh currentMesh = null;
            string[] lines = data.Replace("\r", "").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains('#'))
                {
                    int index = lines[i].IndexOf('#');
                    if (index == 0)
                    {
                        continue;
                    }
                    lines[i] = lines[i].Substring(0, index);
                }
                string[] args = lines[i].Split(' ');
                if (args.Length <= 1)
                {
                    continue;
                }
                switch (args[0])
                {
                    case "mtllib":
                        break; // TODO: Maybe enable materials?
                    case "usemtl":
                        break;
                    case "s": // TODO: use 'smooth shading'?
                        break;
                    case "o":
                        currentMesh = new ModelMesh(args[1]);
                        result.Meshes.Add(currentMesh);
                        break;
                    case "v":
                        result.Vertices.Add(new Location(Utilities.StringToDouble(args[1]),
                            Utilities.StringToDouble(args[2]), Utilities.StringToDouble(args[3])));
                        break;
                    case "vt":
                        result.TextureCoords.Add(new Location(Utilities.StringToDouble(args[1]),
                            -Utilities.StringToDouble(args[2]), 0));
                        break;
                    case "f":
                        string[] a1s = args[1].Split('/');
                        string[] a2s = args[2].Split('/');
                        string[] a3s = args[3].Split('/');
                        int v1 = Utilities.StringToInt(a3s[0]);
                        int v2 = Utilities.StringToInt(a2s[0]);
                        int v3 = Utilities.StringToInt(a1s[0]);
                        Plane plane = new Plane(result.Vertices[v1 - 1], result.Vertices[v2 - 1], result.Vertices[v3 - 1]);
                        currentMesh.Faces.Add(new ModelFace(v1, v2, v3,
                            Utilities.StringToInt(a1s[1]), Utilities.StringToInt(a2s[1]),
                            Utilities.StringToInt(a3s[1]), plane.Normal));
                        break;
                    default:
                        SysConsole.Output(OutputType.WARNING, "Invalid model key '" + args[0] + "'");
                        break;
                }
            }
            result.GenerateVBO();
            return result;
        }

        public Model(string _name)
        {
            Name = _name;
            Meshes = new List<ModelMesh>();
            Vertices = new List<Location>();
            TextureCoords = new List<Location>();
        }

        /// <summary>
        /// The name of  this model.
        /// </summary>
        public string Name;

        /// <summary>
        /// All the meshes this model has.
        /// </summary>
        public List<ModelMesh> Meshes;

        uint VBO;
        uint VBONormals;
        uint VBOTexCoords;
        uint VBOIndices;
        Vector3[] Positions;
        Vector3[] Normals;
        Vector2[] TexCoords;
        ushort[] Indices;

        public void GenerateVBO()
        {
            List<Vector3> Vecs = new List<Vector3>();
            List<Vector3> Norms = new List<Vector3>();
            List<Vector2> Texs = new List<Vector2>();
            List<ushort> Inds = new List<ushort>();
            for (int i = 0; i < Meshes.Count; i++)
            {
                for (int x = 0; x < Meshes[i].Faces.Count; x++)
                {
                    Location normal = Meshes[i].Faces[x].Normal;
                    Norms.Add(new Vector3((float)normal.X, (float)normal.Y, (float)normal.Z));
                    Location vec1 = Vertices[Meshes[i].Faces[x].L1 - 1];
                    Vecs.Add(new Vector3((float)vec1.X, (float)vec1.Y, (float)vec1.Z));
                    Inds.Add((ushort)(Vecs.Count - 1));
                    Location tex1 = TextureCoords[Meshes[i].Faces[x].T1 - 1];
                    Texs.Add(new Vector2((float)tex1.X, (float)tex1.Y));
                    Norms.Add(new Vector3((float)normal.X, (float)normal.Y, (float)normal.Z));
                    Location vec2 = Vertices[Meshes[i].Faces[x].L2 - 1];
                    Vecs.Add(new Vector3((float)vec2.X, (float)vec2.Y, (float)vec2.Z));
                    Inds.Add((ushort)(Vecs.Count - 1));
                    Location tex2 = TextureCoords[Meshes[i].Faces[x].T2 - 1];
                    Texs.Add(new Vector2((float)tex2.X, (float)tex2.Y));
                    Norms.Add(new Vector3((float)normal.X, (float)normal.Y, (float)normal.Z));
                    Location vec3 = Vertices[Meshes[i].Faces[x].L3 - 1];
                    Vecs.Add(new Vector3((float)vec3.X, (float)vec3.Y, (float)vec3.Z));
                    Inds.Add((ushort)(Vecs.Count - 1));
                    Location tex3 = TextureCoords[Meshes[i].Faces[x].T3 - 1];
                    Texs.Add(new Vector2((float)tex3.X, (float)tex3.Y));
                }
            }
            Positions = Vecs.ToArray();
            Normals = Norms.ToArray();
            TexCoords = Texs.ToArray();
            Indices = Inds.ToArray();
            // Normal buffer
            GL.GenBuffers(1, out VBONormals);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBONormals);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Normals.Length * Vector3.SizeInBytes),
                Normals, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // TexCoord buffer
            GL.GenBuffers(1, out VBOTexCoords);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOTexCoords);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TexCoords.Length * Vector2.SizeInBytes),
                TexCoords, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Vertex buffer
            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Positions.Length * Vector3.SizeInBytes),
                Positions, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Index buffer
            GL.GenBuffers(1, out VBOIndices);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(ushort)),
                Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void DeleteVBO()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(VBONormals);
            GL.DeleteBuffer(VBOTexCoords);
        }

        /// <summary>
        /// Draws the model.
        /// </summary>
        public void Draw()
        {
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBONormals);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOTexCoords);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOIndices);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedShort, IntPtr.Zero);
            GL.Finish();
            GL.PopClientAttrib();
        }

        public void DrawImmediate()
        {
            GL.PushMatrix();
            for (int i = 0; i < Meshes.Count; i++)
            {
                GL.Begin(PrimitiveType.Triangles);
                for (int x = 0; x < Meshes[i].Faces.Count; x++)
                {
                    Location normal = Meshes[i].Faces[x].Normal;
                    GL.Normal3(normal.X, normal.Y, normal.Z);
                    Location vec1 = Vertices[Meshes[i].Faces[x].L1 - 1];
                    Location tex1 = TextureCoords[Meshes[i].Faces[x].T1 - 1];
                    GL.TexCoord2(tex1.X, tex1.Y);
                    GL.Vertex3(vec1.X, vec1.Y, vec1.Z);
                    Location vec2 = Vertices[Meshes[i].Faces[x].L2 - 1];
                    Location tex2 = TextureCoords[Meshes[i].Faces[x].T2 - 1];
                    GL.TexCoord2(tex2.X, tex2.Y);
                    GL.Vertex3(vec2.X, vec2.Y, vec2.Z);
                    Location vec3 = Vertices[Meshes[i].Faces[x].L3 - 1];
                    Location tex3 = TextureCoords[Meshes[i].Faces[x].T3 - 1];
                    GL.TexCoord2(tex3.X, tex3.Y);
                    GL.Vertex3(vec3.X, vec3.Y, vec3.Z);
                }
                GL.End();
            }
            GL.PopMatrix();
        }

        /// <summary>
        /// Alll the mesh's vertices.
        /// </summary>
        public List<Location> Vertices;

        /// <summary>
        /// Alll the mesh's texture coords.
        /// </summary>
        public List<Location> TextureCoords;
    }

    public class ModelMesh
    {
        /// <summary>
        /// The name of this mesh.
        /// </summary>
        public string Name;

        public ModelMesh(string _name)
        {
            Name = _name;
            Faces = new List<ModelFace>();
        }

        /// <summary>
        /// All the mesh's faces.
        /// </summary>
        public List<ModelFace> Faces;
    }

    public class ModelFace
    {
        public ModelFace(int _l1, int _l2, int _l3, int _t1, int _t2, int _t3, Location _normal)
        {
            L1 = _l1;
            L2 = _l2;
            L3 = _l3;
            T1 = _t1;
            T2 = _t2;
            T3 = _t3;
            Normal = _normal;
        }

        public Location Normal;

        public int L1;
        public int L2;
        public int L3;

        public int T1;
        public int T2;
        public int T3;
    }
}