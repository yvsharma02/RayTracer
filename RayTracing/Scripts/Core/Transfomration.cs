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

        public Vector3D Transform(Vector3D point)
        {
            float[,] transformedCoords = new float[3, 1];

            for (int i = 0; i < 3; i++)
                transformedCoords[i, 0] = point[i] * Scale[i];

            transformedCoords = RTMath.MultiplyMatrix(GetRotationMatrix(), transformedCoords);

            return Position + new Vector3D(transformedCoords[0, 0], transformedCoords[1, 0], transformedCoords[2, 0]);
        }

        public Vector3D InverseTransform(Vector3D point)
        {
            point -= Position;

            float[,] invertedCoords = new float[3, 1] { { point[0] }, { point[1] }, { point[2] } };

            invertedCoords = RTMath.MultiplyMatrix(GetInverseRotationMatrix(), invertedCoords);

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

            return RTMath.MultiplyMatrix(RTMath.MultiplyMatrix(x, y), z);
        }

        public float[,] GetInverseRotationMatrix()
        {
            return RTMath.InvertMatrix(GetRotationMatrix());
        }
    }
}