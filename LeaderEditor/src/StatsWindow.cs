﻿using ImGuiNET;
using LeaderEditor.Gui;
using LeaderEngine;
using System;

namespace LeaderEditor
{
    public class StatsWindow : WindowComponent
    {
        public override void EditorStart()
        {
            ImGuiController.RegisterImGui(OnImGui);

            MainMenuBar.RegisterWindow("Stats", this);
        }

        private void OnImGui()
        {
            if (IsOpen)
                if (ImGui.Begin("Stats"))
                {
                    ImGui.Text("FPS: " + MathF.Round(100.0f / Time.deltaTimeUnscaled) / 100.0f);
                    ImGui.Text("Frametime: " + MathF.Round(Time.deltaTimeUnscaled * 100000.0f) / 100.0f + "ms");
                    ImGui.SliderFloat("Time scale", ref Time.timeScale, 0.0f, 10.0f);
                    ImGui.End();
                }
        }
    }
}
