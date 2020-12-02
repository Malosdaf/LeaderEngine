﻿using LeaderEditor.Gui;
using OpenTK.Graphics.OpenGL4;
using LeaderEngine;
using ImGuiNET;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text;

using Texture = LeaderEngine.Texture;


namespace LeaderEditor.Logic
{
    public class EditorController : Component
    {
        GameObject ImGuiController;

        public override void Start()
        {
            ImGuiController = new GameObject("Controller");
            ImGuiController.AddComponent<ImGuiController>().OnImGui += OnImGui;

            gameObject.AddComponent<InputManager>();
            gameObject.AddComponent<Viewport>();
            gameObject.AddComponent<SceneHierachy>();
            gameObject.AddComponent<Inspector>();
        }

        private void OnImGui()
        {
            ImGui.DockSpaceOverViewport();
        }
    }
}
