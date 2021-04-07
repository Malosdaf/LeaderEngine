﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace LeaderEngine
{
    public enum MaterialPropType
    {
        Int,
        Float,
        Vector3,
        Vector4,
        Matrix4,
        Texture2D
    }

    public struct MaterialProp
    {
        public MaterialPropType PropType;
        public object Data;
    }

    public sealed class Material : IDisposable
    {
        public Shader Shader { set; get; }

        private Dictionary<string, MaterialProp> materialProps = new Dictionary<string, MaterialProp>();
        private Dictionary<TextureUnit, Texture> materialTextures = new Dictionary<TextureUnit, Texture>();

        public Material(Shader shader)
        {
            Shader = shader;

            DataManager.CurrentScene.SceneMaterials.Add(this);
        }

        #region SetMethods
        public void SetInt(string name, int value)
        {
            materialProps.SetOrAdd(name, new MaterialProp
            {
                PropType = MaterialPropType.Int,
                Data = value
            });
        }

        public void SetFloat(string name, float value)
        {
            materialProps.SetOrAdd(name, new MaterialProp
            {
                PropType = MaterialPropType.Float,
                Data = value
            });
        }

        public void SetVector3(string name, Vector3 value)
        {
            materialProps.SetOrAdd(name, new MaterialProp
            {
                PropType = MaterialPropType.Vector3,
                Data = value
            });
        }

        public void SetVector4(string name, Vector4 value)
        {
            materialProps.SetOrAdd(name, new MaterialProp
            {
                PropType = MaterialPropType.Vector4,
                Data = value
            });
        }

        public void SetMatrix4(string name, Matrix4 value)
        {
            materialProps.SetOrAdd(name, new MaterialProp
            {
                PropType = MaterialPropType.Matrix4,
                Data = value
            });
        }

        public void SetTexture2D(TextureUnit unit, Texture texture)
        {
            materialTextures.SetOrAdd(unit, texture);
        }
        #endregion

        public Material Clone()
        {
            return new Material(Shader);
        }

        public static Material Clone(Material material)
        {
            return new Material(material.Shader);
        }

        public static void Clone(Material material, out Material newMaterial)
        {
            newMaterial = new Material(material.Shader);
        }

        public void Use()
        {
            Shader usingShader = Shader;

            usingShader.Use();

            foreach (var prop in materialProps)
                switch (prop.Value.PropType)
                {
                    case MaterialPropType.Int:
                        usingShader.SetInt(prop.Key, (int)prop.Value.Data);
                        break;
                    case MaterialPropType.Float:
                        usingShader.SetFloat(prop.Key, (float)prop.Value.Data);
                        break;
                    case MaterialPropType.Vector3:
                        usingShader.SetVector3(prop.Key, (Vector3)prop.Value.Data);
                        break;
                    case MaterialPropType.Vector4:
                        usingShader.SetVector4(prop.Key, (Vector4)prop.Value.Data);
                        break;
                    case MaterialPropType.Matrix4:
                        usingShader.SetMatrix4(prop.Key, (Matrix4)prop.Value.Data);
                        break;
                }

            foreach (var tex in materialTextures)
                tex.Value.Use(tex.Key);
        }

        public void Dispose()
        {
            DataManager.CurrentScene.SceneMaterials.Remove(this);
        }
    }
}
