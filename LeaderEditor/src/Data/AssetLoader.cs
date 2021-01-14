﻿using LeaderEditor.Compilation;
using LeaderEngine;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;

namespace LeaderEditor.Data
{
    public static class AssetLoader
    {
        private static List<Type> loadedTypes = new List<Type>();
        public static string LoadedProjectDir;

        public static void LoadProject(string prjPath)
        {
            LoadProjectInternal(prjPath);
            Logger.Log("Loaded project " + Path.GetFileName(prjPath));
        }

        private static void LoadProjectInternal(string prjPath)
        {
            SceneHierachy.SceneEntities.ForEach(x => x.Destroy());
            SceneHierachy.SceneEntities.Clear();

            loadedTypes.ForEach(x => Inspector.SerializeableComponents.Remove(x));
            loadedTypes.Clear();

            LoadedProjectDir = Path.GetDirectoryName(prjPath);

            string scriptsDir = Path.Combine(LoadedProjectDir, "Scripts");
            Directory.CreateDirectory(scriptsDir);

            string[] sourcePaths = Directory.GetFiles(scriptsDir, "*.cs", SearchOption.AllDirectories);

            if (sourcePaths.Length == 0)
                return;

            string[] sources = new string[sourcePaths.Length];

            for (int i = 0; i < sourcePaths.Length; i++)
                sources[i] = File.ReadAllText(sourcePaths[i]);

            EmitResult compilationResult;

            Compiler compiler = new Compiler();
            Type[] types = compiler.Compile(sources, out compilationResult);

            if (compilationResult.Success)
            {
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(Component)))
                    {
                        loadedTypes.Add(type);
                        Inspector.SerializeableComponents.Add(type, null);
                    }
                }
            }
        }

        public static string LoadAsset(string path)
        {
            if (!string.IsNullOrEmpty(LoadedProjectDir))
            {
                string fileName = Path.GetFileName(path);
                Directory.CreateDirectory(Path.Combine(LoadedProjectDir, "Assets"));
                string newPath = Path.Combine(LoadedProjectDir, "Assets", fileName);

                if (!File.Exists(newPath))
                    File.Copy(path, newPath);

                Logger.Log(Path.GetFileName(path) + " copied to " + newPath);

                return newPath;
            }
            return null;
        }
    }
}
