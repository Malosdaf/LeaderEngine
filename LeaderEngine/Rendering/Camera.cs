﻿using OpenTK.Mathematics;

namespace LeaderEngine
{
    public class Camera : Component
    {
        public static Camera Main;

        public float FOV = 60.0f;

        private void Start()
        {
            if (Main == null)
                Main = this;
        }

        public void CalculateViewProjection(out Matrix4 view, out Matrix4 projection)
        {
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), Engine.MainWindow.ClientSize.X / (float)Engine.MainWindow.ClientSize.Y, 0.02f, 200.0f);

            view = Matrix4.LookAt(
                    BaseTransform.Position,
                    BaseTransform.Position + BaseTransform.Forward,
                    BaseTransform.Up
                );
        }

        private void OnRemove()
        {
            if (Main == this)
                Main = null;
        }
    }
}
