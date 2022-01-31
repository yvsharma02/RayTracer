namespace RayTracing
{
    public struct Transformation
    {
        public readonly Vector3D Position;
        public readonly Quaternion Rotation;
        public readonly Vector3D Scale;

        public Transformation()
        {
            this.Position = Vector3D.Zero;
            this.Rotation = Quaternion.Identity;
            this.Scale = new Vector3D(1, 1, 1);
        }

        public Transformation(Vector3D pos, Quaternion rot, Vector3D scale)
        {
            this.Position = pos;
            this.Rotation = rot;
            this.Scale = scale;
        }

        public Transformation(Vector3D pos, Vector3D rot, Vector3D scale) : this(pos, Quaternion.FromEulerAngles(rot), scale) { }

        public Transformation(Vector3D pos) : this(pos, Quaternion.Identity, new Vector3D(1f, 1f, 1f)) { }

        public Transformation Move(Vector3D step)
        {
            return new Transformation(Position + step, Rotation, Scale);
        }

        public Transformation Rotate(Vector3D rotateBy)
        {
            return new Transformation(Position, Quaternion.ToEulerAngles(Rotation) + rotateBy, Scale);
        }

        public Transformation ModifyScale(Vector3D scaleMultiplier)
        {
            return new Transformation(Position, Rotation, new Vector3D(Scale.x * Scale.x, scaleMultiplier.y * Scale.y, Scale.z * Scale.z));
        }

        public Transformation ModifyScale(float multiplier)
        {
            return new Transformation(Position, Rotation, Scale * multiplier);
        }

        public Vector3D Transform(Vector3D point, bool position = true, bool rotation = true, bool scale = true)
        {
            float[,] transformedCoords = new float[3, 1];

            for (int i = 0; i < 3; i++)
                transformedCoords[i, 0] = point[i] * (scale ? Scale[i] : 1f);

            if (rotation)
            {
                Vector3D rotatedCoords = Rotation.Rotate(new Vector3D(transformedCoords[0, 0], transformedCoords[1, 0], transformedCoords[2, 0]));

                for (int i = 0; i < 3; i++)
                    transformedCoords[i, 0] = rotatedCoords[i];

            }
            return (position ? Position : Vector3D.Zero) + new Vector3D(transformedCoords[0, 0], transformedCoords[1, 0], transformedCoords[2, 0]);
        }

        public Vector3D InverseTransform(Vector3D point, bool position = true, bool rotation = true, bool scale = true)
        {
            if (position)
                point -= Position;

            float[,] invertedCoords = new float[3, 1] { { point[0] }, { point[1] }, { point[2] } };

            if (rotation)
            {
                Quaternion inverseRotation = new Quaternion(-Rotation.real, Rotation.i, Rotation.j, Rotation.k);

                Vector3D rotatedCoords = inverseRotation.Rotate(new Vector3D(invertedCoords[0, 0], invertedCoords[1, 0], invertedCoords[2, 0]));

                for (int i = 0; i < 3; i++)
                    invertedCoords[i, 0] = rotatedCoords[i];

            }

            if (scale)
                for (int i = 0; i < 3; i++)
                    invertedCoords[i, 0] /= Scale[i];

            Vector3D result = new Vector3D(invertedCoords[0, 0], invertedCoords[1, 0], invertedCoords[2, 0]);
            return result;
        }

        public override string ToString()
        {
            return String.Format("Position: {0}, Rotation: {1}, Scale: {2}", Position, Rotation, Scale);
        }

        public static Transformation CalculateRequiredRotationTransform(Vector3D pivot, Vector3D source, Vector3D destination)
        {
            Vector3D srcDirVec = source - pivot;
            Vector3D destDirVec = destination - pivot;

            Vector3D cross = Vector3D.Cross(destDirVec, srcDirVec);
            float angle = Vector3D.Angle(destDirVec, srcDirVec);

            return new Transformation(Vector3D.Zero, Quaternion.CreateRotationQuaternion(cross, angle), Vector3D.One);
        }

        public static Transformation Add(Transformation a, Transformation b)
        {
            Vector3D finalRotation = Quaternion.ToEulerAngles(a.Rotation) + Quaternion.ToEulerAngles(b.Rotation);
            return new Transformation(a.Position + b.Position, finalRotation, new Vector3D(a.Scale.x * b.Scale.x, a.Scale.y * b.Scale.y, a.Scale.z * b.Scale.z));
        }
        public static Transformation operator +(Transformation a, Transformation b)
        {
            return Add(a, b);
        }

        public static Transformation operator -(Transformation a)
        {
            return new Transformation(-a.Position, -Quaternion.ToEulerAngles(a.Rotation), new Vector3D(1f / a.Scale.x, 1f / a.Scale.y, 1f / a.Scale.z));
        }

        public static Transformation operator -(Transformation a, Transformation b)
        {
            return a + (-b);
        }
    }
}