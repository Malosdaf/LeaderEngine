using OpenTK.Mathematics;
using System;

namespace LeaderEngine
{
    public enum DrawType
    {
        ShadowMap,
        Opaque,
        Transparent,
        GUI
    }

    public struct GLDrawData
    {
        public Mesh Mesh;
        public Shader Shader;
        public UniformData Uniforms;
        public Material Material;
    }

    public abstract class GLRenderer
    {
        private Vector2i _viewPortSize = new Vector2i(1600, 900);
        public Vector2i ViewportSize
        {
            get => _viewPortSize;
            set
            {
                _viewPortSize.X = Math.Max(1, value.X);
                _viewPortSize.Y = Math.Max(1, value.Y);
            }
        }

        public abstract void Init();
        public abstract void PushDrawData(DrawType drawType, GLDrawData drawData);
        public abstract void Update();
        public abstract void Render();
    }
}
