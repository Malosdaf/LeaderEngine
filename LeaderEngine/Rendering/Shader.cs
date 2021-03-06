using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace LeaderEngine
{
    public sealed class Shader : IDisposable
    {
        private int handle;

        private readonly Dictionary<string, int> uniformLocations;

        public readonly string Name;

        public Shader(string name, string vertSource, string fragSource)
        {
            bool success;

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertSource);
            CompileShader(vertexShader, out success);

            if (!success)
                return;

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragSource);
            CompileShader(fragmentShader, out success);

            if (!success)
                return;

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            LinkProgram(handle, out success);

            if (!success)
                return;

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(handle, i, out _, out _);
                var location = GL.GetUniformLocation(handle, key);

                uniformLocations.Add(key, location);
            }

            Name = name;

            GL.ObjectLabel(ObjectLabelIdentifier.Program, handle, name.Length, name);
        }

        public static Shader FromSourceFile(string name, string vertPath, string fragPath)
        {
            return new Shader(name, File.ReadAllText(vertPath), File.ReadAllText(fragPath));
        }

        private static void CompileShader(int shader, out bool success)
        {
            success = true;

            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                Logger.LogError($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
                success = false;
            }
        }

        private static void LinkProgram(int program, out bool success)
        {
            success = true;

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                Logger.LogError($"Error occurred whilst linking Program({program})");
                success = false;
            }
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            if (uniformLocations.TryGetValue(name, out int loc))
                GL.Uniform1(loc, data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            if (uniformLocations.TryGetValue(name, out int loc))
                GL.Uniform1(loc, data);
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            if (uniformLocations.TryGetValue(name, out int loc))
                GL.UniformMatrix4(loc, true, ref data);
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            if (uniformLocations.TryGetValue(name, out int loc))
                GL.Uniform3(loc, data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            if (uniformLocations.TryGetValue(name, out int loc))
                GL.Uniform4(loc, data);
        }

        public int GetHandle()
        {
            return handle;
        }

        public void Dispose()
        {
            GL.DeleteShader(handle);
        }
    }
}
