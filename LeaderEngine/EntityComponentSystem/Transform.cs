﻿using OpenTK.Mathematics;

namespace LeaderEngine
{
    public class Transform
    {
        public Vector3 OriginOffset = Vector3.Zero;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public Quaternion Rotation 
        {
            get => internalRotation;
            set
            {
                internalRotation = value;
                Quaternion.ToEulerAngles(value, out Vector3 euler);

                //convert to degrees
                internalEulerAngles = new Vector3(
                    MathHelper.RadiansToDegrees(euler.X),
                    MathHelper.RadiansToDegrees(euler.Y),
                    MathHelper.RadiansToDegrees(euler.Z));
            }
        }
        public Vector3 EulerAngles
        {
            get => internalEulerAngles;
            set
            {
                internalEulerAngles = value;

                //convert to radians
                Vector3 radEuler = new Vector3(
                    MathHelper.DegreesToRadians(value.X),
                    MathHelper.DegreesToRadians(value.Y),
                    MathHelper.DegreesToRadians(value.Z));

                Quaternion.FromEulerAngles(radEuler, out internalRotation);
            }
        }

        private Quaternion internalRotation = Quaternion.Identity;
        private Vector3 internalEulerAngles = Vector3.Zero;

        //direction vectors
        public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Quaternion.Conjugate(internalRotation));
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Quaternion.Conjugate(internalRotation));
        public Vector3 Up => Vector3.Transform(Vector3.UnitY, Quaternion.Conjugate(internalRotation));

        internal Matrix4 ModelMatrix = Matrix4.Identity;

        private Entity baseEntity;

        internal Transform(Entity baseEntity)
        {
            this.baseEntity = baseEntity;
        }

        internal void CalculateModelMatrixRecursively()
        {
            //calculate the model matrix;
            Matrix4 res = Matrix4.Identity;
            if (baseEntity.Parent != null)
                res = baseEntity.Parent.Transform.ModelMatrix;

            res *= Matrix4.CreateTranslation(OriginOffset)
                * Matrix4.CreateScale(Scale)
                * Matrix4.CreateFromQuaternion(internalRotation)
                * Matrix4.CreateTranslation(Position);

            ModelMatrix = res;

            //calculate on children
            baseEntity.Children.ForEach(x => x.Transform.CalculateModelMatrixRecursively());
        }
    }
}
