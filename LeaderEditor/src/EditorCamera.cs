﻿using LeaderEngine;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LeaderEditor
{
    public class EditorCamera : EditorComponent
    {
        public static EditorCamera Main;

        public float FOV = 1.04719755f; //60 degrees

        public float Speed = 4.0f;
        public float Sensitivity = 0.4f;

        private float speedMultiplier = 1.0f;

        private Matrix4 ViewMatrix;
        private Matrix4 ProjectionMatrix;

        public override void EditorStart()
        {
            if (Main == null)
                Main = this;

            Application.Main.SceneRender += SceneRender;
            Application.Main.GuiRender += GuiRender;
        }

        public void LookAt(Vector3 position)
        {
            Vector3 newPos = position + new Vector3(2.0f, 2.0f, 2.0f);
            transform.LocalPosition = newPos;
            transform.RotationEuler = new Vector3(30.0f, -45.0f, 0.0f);
        }

        public void UpdateCamMove()
        {
            if (Input.GetKey(Keys.LeftShift))
                speedMultiplier = 2.5f;
            else speedMultiplier = 1.0f;

            float moveX = Input.GetAxis(Axis.Horizontal);
            float moveZ = Input.GetAxis(Axis.Vertical);

            Vector3 move = transform.Forward * moveZ + transform.Right * moveX;
            transform.LocalPosition += move * Time.deltaTime * Speed * speedMultiplier;

            if (Input.GetMouse(MouseButton.Right))
            {
                Vector2 delta = Input.GetMouseDelta() * Sensitivity;
                transform.RotationEuler.X += delta.Y;
                transform.RotationEuler.Y += delta.X;
            }

            LightingController.CameraPos = transform.Position;
        }

        private void SceneRender()
        {
            if (!Enabled)
                return;

            Vector3 pos = transform.Position;

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FOV, Application.Main.ViewportSize.X / (float)Application.Main.ViewportSize.Y, 0.02f, 512.0f);

            ViewMatrix = Matrix4.LookAt(
                    pos,
                    pos + transform.Forward,
                    transform.Up
                );

            RenderingGlobals.Projection = ProjectionMatrix;
            RenderingGlobals.View = ViewMatrix;
        }

        private void GuiRender()
        {
            RenderingGlobals.Projection = Matrix4.CreateOrthographic(Application.Main.ViewportSize.X, Application.Main.ViewportSize.Y, 0.0f, 100.0f);
            RenderingGlobals.View = Matrix4.Identity;
        }
    }
}
