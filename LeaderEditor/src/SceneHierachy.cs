﻿using ImGuiNET;
using LeaderEditor.Data;
using LeaderEditor.Gui;
using LeaderEngine;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;
using System.Linq;

namespace LeaderEditor
{
    public class SceneHierachy : WindowComponent
    {
        public static List<GameObject> SceneObjects { 
            get 
            {
                List<GameObject> gameObjects = new List<GameObject>();

                gameObjects.AddRange(Application.Main.WorldGameObjects.Where(x => x.Tag != "Editor"));
                gameObjects.AddRange(Application.Main.WorldGameObjects_Transparent);
                gameObjects.AddRange(Application.Main.GuiGameObjects);

                return gameObjects;
            } 
        }

        public static GameObject SelectedObject = null;

        private static readonly Dictionary<RenderHint, string> renderHintText = new Dictionary<RenderHint, string>()
        {
            { RenderHint.World, "[World]" },
            { RenderHint.Transparent, "[World, Transparent]" },
            { RenderHint.Gui, "[Gui]" }
        };

        private string[] objectTypes = { "World", "Transparent", "Gui" };
        private string currentType = "World";

        public override void EditorStart()
        {
            ImGuiController.AddImGuiFunc(OnImGui);

            MainMenuBar.RegisterWindow("Scene Hierachy", this);
        }

        public override void EditorUpdate()
        {
            //delete object
            if (Input.GetKeyDown(Keys.Delete) && SelectedObject != null)
            {
                SelectedObject.Destroy();
                SceneObjects.Remove(SelectedObject);

                SelectedObject = null;
            }
        }

        private void OnImGui()
        {
            //render scene hierachy gui
            if (IsOpen)
                if (ImGui.Begin("Scene Hierachy", ref IsOpen))
                {
                    //select
                    ImGui.SetNextItemWidth(120.0f);

                    if (ImGui.BeginCombo("##combo", currentType))
                    {
                        foreach (string typeStr in objectTypes)
                        {
                            if (ImGui.Selectable(typeStr, currentType == typeStr))
                                currentType = typeStr;
                        }
                        ImGui.EndCombo();
                    }

                    if (SelectedObject != null)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button($"Go to {SelectedObject.Name}"))
                        {
                            EditorCamera.Main.LookAt(SelectedObject.transform.Position);
                        }
                    }

                    //draw all objects
                    RenderObjectTree();
                    ImGui.End();
                }
        }

        //new object function
        private void CreateNewObject(GameObject parent)
        {
            if (string.IsNullOrEmpty(AssetLoader.LoadedProjectDir))
                return;

            RenderHint renderHint = RenderHint.World;

            switch (currentType)
            {
                case "World":
                    renderHint = RenderHint.World;
                    break;
                case "Transparent":
                    renderHint = RenderHint.Transparent;
                    break;
                case "Gui":
                    renderHint = RenderHint.Gui;
                    break;
            }

            GameObject go = new GameObject("New GameObject", renderHint);

            go.Parent = parent;
        }

        private int index = 0;

        private void RenderObjectTree()
        {
            List<GameObject> _sceneObjects = SceneObjects;

            index = 0;
            if (ImGui.BeginChild("Scene"))
            {
                for (int i = 0; i < SceneObjects.Count; i++)
                {
                    var go = _sceneObjects[i];

                    if (go.Parent == null)
                        RecursivelyRender(go);
                }

                if (!ImGui.IsAnyItemHovered() && ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows))
                {
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                        ImGui.OpenPopup("GameObject Menu");

                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                        SelectedObject = null;
                }

                if (ImGui.BeginPopup("GameObject Menu")) 
                { 
                    if (ImGui.MenuItem("New GameObject"))
                        Application.Main.ExecuteNextUpdate(() => CreateNewObject(null));

                    ImGui.EndPopup();
                }

                ImGui.EndChild();
            }
        }

        private void RecursivelyRender(GameObject go)
        {
            ImGui.PushID(go.Name + index);

            index++;

            ImGuiTreeNodeFlags nodeFlags = ImGuiTreeNodeFlags.OpenOnArrow;

            if (SelectedObject == go)
                nodeFlags |= ImGuiTreeNodeFlags.Selected;

            bool nodeOpen = ImGui.TreeNodeEx(go.Name, nodeFlags);

            if (ImGui.IsItemClicked())
                SelectedObject = go;

            if (ImGui.BeginPopupContextItem("GameObject Popup"))
            {
                if (ImGui.MenuItem("New GameObject"))
                    Application.Main.ExecuteNextUpdate(() => CreateNewObject(go));

                if (ImGui.MenuItem("Delete"))
                    Application.Main.ExecuteNextUpdate(() => go.Destroy());

                ImGui.EndPopup();
            }

            ImGui.SameLine();

            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.4f, 0.4f, 0.4f, 1.0f));
            ImGui.Text(renderHintText[go.RenderHint]);
            ImGui.PopStyleColor();

            if (nodeOpen)
            {
                for (int i = 0; i < go.Children.Count; i++)
                    RecursivelyRender(go.Children[i]);

                ImGui.TreePop();
            }

            ImGui.PopID();
        }
    }
}
