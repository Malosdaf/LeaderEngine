﻿using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using System.Text;
using OpenTK.Graphics.OpenGL4;

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

    public class Material
    {
        #region DefaultMaterials
        public static Material NoRender;
        #endregion

        private Shader shader;

        private Dictionary<string, MaterialProp> materialProps = new Dictionary<string, MaterialProp>();
        private Dictionary<TextureUnit, Texture> materialTextures = new Dictionary<TextureUnit, Texture>();

        public Material(Shader shader)
        {
            this.shader = shader;
        }

        public static void InitDefaults()
        {
            NoRender = new Material(Shader.NoRender);
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

        public void Use()
        {
            foreach (var prop in materialProps)
                switch (prop.Value.PropType)
                {
                    case MaterialPropType.Int:
                        shader.SetInt(prop.Key, (int)prop.Value.Data);
                        break;
                    case MaterialPropType.Float:
                        shader.SetFloat(prop.Key, (float)prop.Value.Data);
                        break;
                    case MaterialPropType.Vector3:
                        shader.SetVector3(prop.Key, (Vector3)prop.Value.Data);
                        break;
                    case MaterialPropType.Vector4:
                        shader.SetVector4(prop.Key, (Vector4)prop.Value.Data);
                        break;
                    case MaterialPropType.Matrix4:
                        shader.SetMatrix4(prop.Key, (Matrix4)prop.Value.Data);
                        break;
                }

            foreach (var tex in materialTextures)
                tex.Value.Use(tex.Key);

            shader.Use();
        }
    }
}
