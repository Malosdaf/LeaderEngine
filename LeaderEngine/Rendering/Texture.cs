﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LeaderEngine
{
    public sealed class Texture : IDisposable
    {
        public readonly string Name;
        private int handle;
        
        public Vector2i Size;

        private byte[] rawData;
        private int singlePixelSize;

        private PixelInternalFormat pixelInternalFormat;
        private PixelFormat pixelFormat;
        private PixelType pixelType;

        private Texture(string name)
        {
            Name = name;

            DataManager.Textures.Add(this);
        }

        public static Texture FromFile(string name, string path)
        {
            using (var image = Image.Load<Rgba32>(path))
                return FromImage(name, image);
        }

        public unsafe static Texture FromImage(string name, Image<Rgba32> image)
        {
            Span<Rgba32> pixelSpan;
            if (!image.TryGetSinglePixelSpan(out pixelSpan))
            {
                Rgba32[] pixelArray = new Rgba32[image.Width * image.Height];
                for (int i = 0; i < image.Height; i++)
                {
                    var row = image.GetPixelRowSpan(i);
                    for (int j = 0; j < image.Width; j++)
                    {
                        pixelArray[i * image.Height + j] = row[j];
                    }
                }
                pixelSpan = new Span<Rgba32>(pixelArray);
            }

            Texture tex = FromPointer(name, image.Width, image.Height, (IntPtr)Unsafe.AsPointer(ref pixelSpan[0]), 4);

            return tex;
        }

        public static Texture FromPointer(string name, int width, int height, IntPtr data, int singlePixelSize, 
            PixelInternalFormat internalFormat = PixelInternalFormat.SrgbAlpha, 
            PixelFormat format = PixelFormat.Rgba, 
            PixelType pixelType = PixelType.UnsignedByte)
        {
            Texture texture = new Texture(name);

            //copy pixel array
            texture.rawData = new byte[width * height * singlePixelSize];
            Marshal.Copy(data, texture.rawData, 0, width * height * singlePixelSize);

            texture.singlePixelSize = singlePixelSize;
            texture.pixelInternalFormat = internalFormat;
            texture.pixelFormat = format;
            texture.pixelType = pixelType;

            texture.handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture.handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, pixelType, data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.ObjectLabel(ObjectLabelIdentifier.Texture, texture.handle, name.Length, name);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            texture.Size = new Vector2i(width, height);

            return texture;
        }

        public static Texture FromBytes(string name, int width, int height, byte[] data, int singlePixelSize,
            PixelInternalFormat internalFormat = PixelInternalFormat.SrgbAlpha,
            PixelFormat format = PixelFormat.Rgba,
            PixelType pixelType = PixelType.UnsignedByte)
        {
            Texture texture = new Texture(name);

            //copy pixel array
            texture.rawData = data;

            texture.singlePixelSize = singlePixelSize;
            texture.pixelInternalFormat = internalFormat;
            texture.pixelFormat = format;
            texture.pixelType = pixelType;

            texture.handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture.handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format, pixelType, data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.ObjectLabel(ObjectLabelIdentifier.Texture, texture.handle, name.Length, name);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            texture.Size = new Vector2i(width, height);

            return texture;
        }

        public void SetMinFilter(TextureMinFilter textureMinFilter)
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SetMagFilter(TextureMagFilter textureMagFilter)
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMagFilter);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SetWrapS(TextureWrapMode textureWrapMode)
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)textureWrapMode);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SetWrapT(TextureWrapMode textureWrapMode)
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)textureWrapMode);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Serialize(BinaryWriter writer)
        {
            //write name
            writer.Write(Name);

            //write pixel info
            writer.Write(singlePixelSize);

            writer.Write((int)pixelInternalFormat);
            writer.Write((int)pixelFormat);
            writer.Write((int)pixelType);

            //write width and height
            writer.Write(Size.X);
            writer.Write(Size.Y);

            //write pixels
            writer.Write(rawData);
        }

        public static Texture Deserialize(BinaryReader reader)
        {
            //read name
            string name = reader.ReadString();

            //read pixel info
            int singlePixelSize = reader.ReadInt32();

            PixelInternalFormat pixelInternalFormat = (PixelInternalFormat)reader.ReadInt32();
            PixelFormat pixelFormat = (PixelFormat)reader.ReadInt32();
            PixelType pixelType = (PixelType)reader.ReadInt32();

            //read width and height
            Vector2i size = new Vector2i(reader.ReadInt32(), reader.ReadInt32());

            //read pixels
            byte[] data = reader.ReadBytes(size.X * size.Y);

            return FromBytes(name,
                size.X, size.Y,
                data, singlePixelSize,
                pixelInternalFormat, pixelFormat,
                pixelType);
        }

        public void Use(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        public int GetHandle()
        {
            return handle;
        }

        public void Dispose()
        {
            GL.DeleteTexture(handle);

            DataManager.Textures.Remove(this);
        }
    }
}
