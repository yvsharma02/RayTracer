namespace RayTracing
{
    public struct Transfomration
    {
        public readonly Vector3D Position;
        public readonly Vector3D Rotation;
        public readonly Vector3D Scale;

        public readonly Vector3D RotationRadians => Rotation * (2 * MathF.PI / 360f);

        public Transfomration()
        {
            this.Position = Vector3D.Zero;
            this.Rotation = Vector3D.Zero;
            this.Scale = new Vector3D(1, 1, 1);
        }

        public Transfomration(Vector3D pos, Vector3D rot, Vector3D scale)
        {
            this.Position = pos;
            this.Rotation = rot;
            this.Scale = scale;
        }

        public Transfomration(Vector3D pos) : this(pos, Vector3D.Zero, new Vector3D(1f, 1f, 1f)) { }

        public Transfomration Move(Vector3D step)
        {
            return new Transfomration(Position + step, Rotation, Scale);
        }

        public Transfomration Rotate(Vector3D rotateBy)
        {
            return new Transfomration(Position, Rotation + rotateBy, Scale);
        }

        public Transfomration ModifyScale(Vector3D scaleMultiplier)
        {
            return new Transfomration(Position, Rotation, new Vector3D(Scale.x * Scale.x, scaleMultiplier.y * Scale.y, Scale.z * Scale.z));
        }

        public Transfomration ModifyScale(float multiplier)
        {
            return new Transfomration(Position, Rotation, Scale * multiplier);
        }

        public Vector3D Transform(Vector3D point, bool position = true, bool rotation = true, bool scale = true)
        {
            float[,] transformedCoords = new float[3, 1];

            for (int i = 0; i < 3; i++)
                transformedCoords[i, 0] = point[i] * (scale ? Scale[i] : 1f);

            if (rotation)
                transformedCoords = RTMath.MultiplyMatrix(GetRotationMatrix(), transformedCoords);

            return (position ? Position : Vector3D.Zero) + new Vector3D(transformedCoords[0, 0], transformedCoords[1, 0], transformedCoords[2, 0]);
        }

        public Vector3D InverseTransform(Vector3D point, bool position = true, bool rotation = true, bool scale = true)
        {
            if (position)
                point -= Position;

            float[,] invertedCoords = new float[3, 1] { { point[0] }, { point[1] }, { point[2] } };

            if (rotation)
                invertedCoords = RTMath.MultiplyMatrix(GetInverseRotationMatrix(), invertedCoords);

            if (scale)
                for (int i = 0; i < 3; i++)
                    invertedCoords[i, 0] /= Scale[i];

            Vector3D result = new Vector3D(invertedCoords[0, 0], invertedCoords[1, 0], invertedCoords[2, 0]);
            return result;
        }

        public float[,] GetRotationMatrix()
        {
            float cosx = MathF.Cos(RotationRadians.x);
            float sinx = MathF.Sin(RotationRadians.x);
            float[,] x = new float[,]
            {                           { 1, 0, 0 },
                                        { 0, cosx, -sinx },
                                        { 0, sinx, cosx }
            };

            float cosy = MathF.Cos(RotationRadians.y);
            float siny = MathF.Sin(RotationRadians.y);
            float[,] y = new float[,]
            {                           { cosy, 0, -siny },
                                        { 0, 1, 0 },
                                        { siny, 0, cosy }
            };

            float cosz = MathF.Cos(RotationRadians.z);
            float sinz = MathF.Sin(RotationRadians.z);
            float[,] z = new float[,]
            {
                                        { cosz, -sinz, 0 },
                                        { sinz, cosz, 0 },
                                        { 0, 0, 1 }
            };

            return RTMath.MultiplyMatrix(x, RTMath.MultiplyMatrix(y, z));
        }

        public float[,] GetInverseRotationMatrix()
        {
            return RTMath.InvertMatrix(GetRotationMatrix());
        }

        public override string ToString()
        {
            return String.Format("Position: {0}, Rotation: {1}, Scale: {2}", Position, Rotation, Scale);
        }

        public static Transfomration CalculateRequiredRotationTransform(Vector3D pivot, Vector3D source, Vector3D destination)
        {
            Vector3D srcAngles = FindAngles(source, pivot);
            Vector3D destinationAngles = FindAngles(destination, pivot);

            return new Transfomration(Vector3D.Zero, destinationAngles - srcAngles, Vector3D.One);
        }

        public static Vector3D FindAngles(Vector3D point, Vector3D pivot)
        {
            Vector3D v = point - pivot;

            float zCosine = v.x / MathF.Sqrt(v.x * v.x + v.y * v.y);
            float yCosine = v.z / MathF.Sqrt(v.x * v.x + v.z * v.z);
            float xCosine = v.y / MathF.Sqrt(v.y * v.y + v.z * v.z);

            float zAngle = (float.IsNaN(zCosine) || float.IsInfinity(zCosine) ? 0 : MathF.Acos(zCosine)) * (180 / MathF.PI);
            float yAngle = (float.IsNaN(yCosine) || float.IsInfinity(yCosine) ? 0 : MathF.Acos(yCosine)) * (180 / MathF.PI);
            float xAngle = (float.IsNaN(xCosine) || float.IsInfinity(xCosine) ? 0 : MathF.Acos(xCosine)) * (180 / MathF.PI);

            return new Vector3D(xAngle, yAngle, zAngle);
        }

        public static Transfomration Add(Transfomration a, Transfomration b)
        {
            return new Transfomration(a.Position + b.Position, a.Rotation + b.Rotation, new Vector3D(a.Scale.x * b.Scale.x, a.Scale.y * b.Scale.y, a.Scale.z * b.Scale.z));
        }
        public static Transfomration operator +(Transfomration a, Transfomration b)
        {
            return Add(a, b);
        }

        public static Transfomration operator -(Transfomration a)
        {
            return new Transfomration(-a.Position, -a.Rotation, new Vector3D(1f / a.Scale.x, 1f / a.Scale.y, 1f / a.Scale.z));
        }

        public static Transfomration operator -(Transfomration a, Transfomration b)
        {
            return a + (-b);
        }
    }
}