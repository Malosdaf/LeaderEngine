﻿using ImGuiNET;
using LeaderEngine;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LeaderEditor
{
    public class SerializeFunc
    {
        //serialize transform component
        public static void Transform(Transform transform)
        {
            ImGui.PushID("PositionV3");
            ImGuiExtension.DragVector3("Position", ref transform.Position, Vector3.Zero, 0.05f);
            ImGui.PopID();

            ImGui.PushID("RotationV3");
            Vector3 euler = transform.EulerAngles;
            ImGuiExtension.DragVector3("Rotation", ref euler, Vector3.Zero, 0.5f);
            transform.EulerAngles = euler;
            ImGui.PopID();

            ImGui.PushID("ScaleV3");
            ImGuiExtension.DragVector3("Scale", ref transform.Scale, Vector3.One, 0.05f);
            ImGui.PopID();
        }

        public static void DefaultSerializeFunc(Component obj)
        {
            bool guiDrawn = false;

            //serialize fields
            var fields = obj.GetType().GetFields();
            foreach (var field in fields)
            {
                if (fieldDrawFuncs.TryGetValue(field.FieldType, out var drawFunc) && field.IsPublic && !field.IsStatic)
                {
                    ImGui.PushID(field.Name);
                    field.SetValue(obj, drawFunc.Invoke(field.Name, field.GetValue(obj)));
                    ImGui.PopID();
                    guiDrawn = true;
                }
            }

            //serialize properties
            var props = obj.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (fieldDrawFuncs.TryGetValue(prop.PropertyType, out var drawFunc))
                {
                    ImGui.PushID(prop.Name);
                    prop.SetValue(obj, drawFunc.Invoke(prop.Name, prop.GetValue(obj)));
                    ImGui.PopID();
                    guiDrawn = true;
                }
            }

            if (!guiDrawn)
                ImGui.Text("No property");
        }

        //default serializers
        private static Dictionary<Type, Func<string, object, object>> fieldDrawFuncs = new Dictionary<Type, Func<string, object, object>>()
        {
            { typeof(int), DefaultInt },
            { typeof(float), DefaultFloat },
            { typeof(string), DefaultString },
            { typeof(Vector4), DefaultV4 },
            { typeof(Vector3), DefaultV3 },
            { typeof(Vector2), DefaultV2 },
            { typeof(AudioClip), DefaultAC }
        };

        private static object DefaultAC(string name, object obj)
        {
            AudioClip value = (AudioClip)obj;

            if (ImGui.BeginCombo(name, value != null ? value.Name : "[None]"))
            {
                if (ImGui.Selectable("[None]", value == null))
                    value = null;

                foreach (var clip in DataManager.AudioClips)
                    if (ImGui.Selectable(clip.Name, value == clip))
                        value = clip;

                ImGui.EndCombo();
            }

            return value;
        }

        private static object DefaultV2(string name, object obj)
        {
            Vector2 value = (Vector2)obj;
            ImGuiExtension.DragVector2(name, ref value, Vector2.Zero);
            return value;
        }

        private static object DefaultV3(string name, object obj)
        {
            Vector3 value = (Vector3)obj;
            ImGuiExtension.DragVector3(name, ref value, Vector3.Zero);
            return value;
        }

        private static object DefaultV4(string name, object obj)
        {
            Vector4 value = (Vector4)obj;
            ImGuiExtension.DragVector4(name, ref value, Vector4.Zero);
            return value;
        }

        private static object DefaultInt(string name, object obj)
        {
            int value = (int)obj;
            ImGui.DragInt(name, ref value);
            return value;
        }

        private static object DefaultFloat(string name, object obj)
        {
            float value = (float)obj;
            ImGui.DragFloat(name, ref value, 0.05f);
            return value;
        }

        private static object DefaultString(string name, object obj)
        {
            string value = (string)obj ?? string.Empty;
            ImGui.InputText(name, ref value, 65535, ImGuiInputTextFlags.Multiline);
            return value;
        }
    }
}