﻿using LeaderEngine;
using OpenTK.Graphics.OpenGL4;

namespace LeaderEditor
{
    public class EditorRenderer : ForwardRenderer
    {
        private ImGuiController ImGuiController { get; } = new ImGuiController();

        public Framebuffer Framebuffer;

        public override void Init()
        {
            Framebuffer = new Framebuffer("viewport-fbo", 1, 1, new Attachment[]
            {
                new Attachment
                {
                    Draw = false,
                    PixelInternalFormat = PixelInternalFormat.Rgba,
                    PixelFormat = PixelFormat.Rgba,
                    PixelType = PixelType.Float,
                    FramebufferAttachment = FramebufferAttachment.ColorAttachment0,
                    TextureParamsInt = new TextureParamInt[]
                    {
                        new TextureParamInt { ParamName = TextureParameterName.TextureMinFilter, Param = (int)TextureMinFilter.Linear },
                        new TextureParamInt { ParamName = TextureParameterName.TextureMagFilter, Param = (int)TextureMagFilter.Linear }
                    }
                },
                new Attachment
                {
                    Draw = false,
                    PixelInternalFormat = PixelInternalFormat.DepthComponent,
                    PixelFormat = PixelFormat.DepthComponent,
                    PixelType = PixelType.Float,
                    FramebufferAttachment = FramebufferAttachment.DepthAttachment,
                    TextureParamsInt = new TextureParamInt[]
                    {
                        new TextureParamInt { ParamName = TextureParameterName.TextureMinFilter, Param = (int)TextureMinFilter.Nearest },
                        new TextureParamInt { ParamName = TextureParameterName.TextureMagFilter, Param = (int)TextureMagFilter.Nearest }
                    }
                }
            });

            ImGuiController.Init();

            base.Init();
        }

        public override void Update()
        {
            base.Update();

            if (ViewportSize.X * ViewportSize.Y > 0)
                Framebuffer.Resize(ViewportSize.X, ViewportSize.Y);

            ImGuiController.Update(Time.DeltaTime);
        }

        public override void Render()
        {
            //begin fbo
            Framebuffer.Begin();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            base.Render();

            Framebuffer.End();

            //restore viewport
            GL.Viewport(0, 0, Engine.MainWindow.ClientSize.X, Engine.MainWindow.ClientSize.Y);

            ImGuiController.RenderImGui();
        }
    }
}
