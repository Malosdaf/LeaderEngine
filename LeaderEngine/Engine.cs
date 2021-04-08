﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ErrorCode = OpenTK.Windowing.GraphicsLibraryFramework.ErrorCode;

namespace LeaderEngine
{
    public static class Time
    {
        public static float Elapsed { get; internal set; }
        public static float DeltaTime { get; internal set; }
        public static float UnscaledDeltaTime { get; internal set; }
        public static float TimeScale = 1.0f;
    }

    public static class Engine
    {
        public static GameWindow MainWindow { get; private set; }

        public static GLRenderer Renderer = new ForwardRenderer();

        private static DebugProc debugProcCallback = DebugCallback;
        private static GCHandle debugProcCallbackHandle;

        public static void Init(GameWindowSettings gws, NativeWindowSettings nws, Action initCallback = null)
        {
            MainWindow = new GameWindow(gws, nws);

            //log basic info
            Logger.Log("Base Directory: " + AppContext.BaseDirectory, true);
            Logger.Log("Renderer: " + GL.GetString(StringName.Renderer), true);
            Logger.Log("Vendor: " + GL.GetString(StringName.Vendor), true);
            Logger.Log("Version: " + GL.GetString(StringName.Version), true);
            Logger.Log("Shading Language version: " + GL.GetString(StringName.ShadingLanguageVersion), true);

            //subscribe to window events
            MainWindow.UpdateFrame += UpdateFrame;
            MainWindow.RenderFrame += RenderFrame;

            //intialize engine
            Logger.Log("Initializing LeaderEngine...", true);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            DefaultShaders.Init();

            initCallback?.Invoke();

            Renderer.Init();
            Input.Init(MainWindow.KeyboardState, MainWindow.MouseState);

            //init debug callbacks
            GLFW.SetErrorCallback(LogGLFWError);

            debugProcCallbackHandle = GCHandle.Alloc(debugProcCallback);

            GL.DebugMessageCallback(debugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            stopwatch.Stop();
            //print init complete msg
            Logger.Log($"LeaderEngine initialized. ({stopwatch.ElapsedMilliseconds}ms)", true);

            //open window
            MainWindow.Run();
        }

        private static void LogGLFWError(ErrorCode errorCode, string description)
        {
            Logger.LogError("GLFW: " + errorCode.ToString() + ": " + description);
        }

        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            switch (severity)
            {
                case DebugSeverity.DebugSeverityMedium:
                    Logger.LogWarning($"OpenGL: {messageString}");
                    break;
                case DebugSeverity.DebugSeverityHigh:
                    Logger.LogError($"OpenGL: {messageString}");
                    break;
                default:
                    Logger.Log($"OpenGL: {messageString}");
                    break;
            }
        }

        private static void UpdateFrame(FrameEventArgs obj)
        {
            //updates time
            float t = (float)obj.Time;

            Time.Elapsed += t;
            Time.DeltaTime = t * Time.TimeScale;
            Time.UnscaledDeltaTime = t;

            //update scene
            DataManager.CurrentScene.UpdateSceneHierachy();

            //update renderer
            Renderer.Update();
        }

        private static void RenderFrame(FrameEventArgs obj)
        {
            GL.Viewport(0, 0, MainWindow.ClientSize.X, MainWindow.ClientSize.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            Renderer.Render();

            MainWindow.SwapBuffers();
        }
    }
}
