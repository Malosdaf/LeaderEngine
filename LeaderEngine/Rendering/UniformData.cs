﻿using OpenTK.Mathematics;
using System.Collections.Generic;

namespace LeaderEngine
{
    public enum UniformType
    {
        Int,
        Float,
        Vector3,
        Vector4,
        Matrix4
    }

    public struct Uniform
    {
        public UniformType UniformType;
        public object Data;

        public Uniform(UniformType type, object data)
        {
            UniformType = type;
            Data = data;
        }
    }

    public class UniformData
    {
        private Dictionary<string, Uniform> uniforms = new Dictionary<string, Uniform>();

        public void SetUniform(string name, Uniform uniform)
        {
            uniforms.SetOrAdd(name, uniform);
        }

        public void Use(Shader shader)
        {
            foreach (var kvp in uniforms)
            {
                switch (kvp.Value.UniformType)
                {
                    case UniformType.Int:
                        shader.SetInt(kvp.Key, (int)kvp.Value.Data);
                        break;
                    case UniformType.Float:
                        shader.SetFloat(kvp.Key, (float)kvp.Value.Data);
                        break;
                    case UniformType.Vector3:
                        shader.SetVector3(kvp.Key, (Vector3)kvp.Value.Data);
                        break;
                    case UniformType.Vector4:
                        shader.SetVector4(kvp.Key, (Vector4)kvp.Value.Data);
                        break;
                    case UniformType.Matrix4:
                        shader.SetMatrix4(kvp.Key, (Matrix4)kvp.Value.Data);
                        break;
                }
            }
        }
    }
}
