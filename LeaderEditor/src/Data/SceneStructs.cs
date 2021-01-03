﻿using LeaderEngine;
using OpenTK.Mathematics;
using System;

namespace LeaderEditor.Data
{
    public struct ComponentFieldInfo
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string AssemblyName { get; set; }
        public object Data { get; set; }
    }

    public struct ComponentInfo
    {
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public ComponentFieldInfo[] Fields { get; set; }
    }

    public struct GameObjectInfo
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public RenderHint RenderHint { get; set; }
        public ComponentInfo[] Components { get; set; }
    }

    public struct SceneInfo
    {
        public string[] Models { get; set; }
        public Vector3 EditorCamPosition { get; set; }
        public Vector3 EditorCamRotation { get; set; }
        public GameObjectInfo[] GameObjects { get; set; }
    }
}