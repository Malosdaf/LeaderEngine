﻿using LeaderEngine;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System;
using System.IO;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Init(new GameWindowSettings(), new NativeWindowSettings()
            {
                APIVersion = new System.Version(4, 3)
            }, Init);
        }

        static void Init()
        {
            Shader shader = Shader.FromSourceFile(
                Path.Combine(AppContext.BaseDirectory, "shader.vert"),
                Path.Combine(AppContext.BaseDirectory, "shader.frag"));

            Material material = new Material(shader);

            Mesh mesh = new Mesh();
            mesh.LoadMesh(new Vertex[]
            {
                new Vertex { Position = new Vector3(1.0f, 1.0f, 0.0f), Color = new Vector3(1.0f, 0.0f, 0.0f) },
                new Vertex { Position = new Vector3(1.0f, -1.0f, 0.0f), Color = new Vector3(0.0f, 1.0f, 0.0f) },
                new Vertex { Position = new Vector3(-1.0f, -1.0f, 0.0f), Color = new Vector3(0.0f, 0.0f, 1.0f) },
                new Vertex { Position = new Vector3(-1.0f, 1.0f, 0.0f), Color = new Vector3(1.0f, 1.0f, 0.0f) },
            }, 
            new uint[] 
            {
                0, 1, 3,
                1, 2, 3
            });

            Entity entity = new Entity("bruh");
            entity.AddComponent<Move>();
            var mr = entity.AddComponent<MeshRenderer>();

            mr.Material = material;
            mr.Mesh = mesh;
        }
    }

    public class Move : Component
    {
        void Update()
        {
            BaseTransform.Position += new Vector3(Input.GetAxis(Axis.Horizontal), Input.GetAxis(Axis.Vertical), 0.0f) * Time.DeltaTime * 8.0f;
        }
    }
}
