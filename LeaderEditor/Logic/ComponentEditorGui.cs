﻿using ImGuiNET;
using LeaderEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace LeaderEditor.Logic
{
    public static class ComponentEditorGui
    {
        public static void Transform(Component e)
        {
            Transform transform = (Transform)e;

            Vector3 posSys = transform.position.ToSystemVector3();
            ImGui.DragFloat3("Position", ref posSys, 0.05f);
            transform.position = posSys.ToOTKVector3();

            Vector3 rotSys = transform.rotationEuler.ToSystemVector3();
            ImGui.DragFloat3("Rotation", ref rotSys, 0.05f);
            transform.rotationEuler = rotSys.ToOTKVector3();

            Vector3 scaleSys = transform.scale.ToSystemVector3();
            ImGui.DragFloat3("Scale", ref scaleSys, 0.05f);
            transform.scale = scaleSys.ToOTKVector3();
        }
    }
}
