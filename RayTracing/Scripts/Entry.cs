namespace RayTracing
{
    public class Entry
    {
        private const string LOG_FILE_PATH = @"D:\Projects\VisualStudio\RayTracing\Generated\Log.txt";

        public static void Main(String[] args)
        {
            Console.WriteLine(Quaternion.FromEulerAngles(new Vector3D(0f, 90f, 0f) * RTMath.DEG_TO_RAD));

            /*
            Random r = new Random(); 
            while (true)
            {
                Quaternion q = new Quaternion((float) r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());

                Vector3D e = Quaternion.ToEulerAngles(q);

                Quaternion q2 = Quaternion.FromEulerAngles(e);

                Console.WriteLine(q);
                Console.WriteLine(e * RTMath.RAD_TO_DEG);
                Console.WriteLine(q2);

                Console.ReadKey();
            }
            */
            //            Console.WriteLine(Quaternion.ToEulerAngles(Quaternion.FromEulerAngles(new Vector3D(1f, 0.0001f, 0.0001f))) * RTMath.RAD_TO_DEG);

//            Quaternion q = Quaternion.FromEulerAngles(new Vector3D(MathF.PI / 2, 0, 0));

//            Console.WriteLine(q);
//            Console.WriteLine(q.Rotate(new Vector3D(1f, 0f, 0f)));



//            Console.WriteLine(Quaternion.CreateRotationQuaternion(new Vector3D(1f, 0f, 0f), MathF.PI / 2f).Rotate(new Vector3D(1f, 0f, 0f)));

            /*
            Random r = new Random();

            while (true)
            {
                Vector3D src = new Vector3D(r.Next(), r.Next(), r.Next()).Normalize();
                Vector3D dest = new Vector3D(r.Next(), r.Next(), r.Next()).Normalize();

                Console.WriteLine("Source: {0}, Destination: {1}", src, dest);

                Transformation reqTransform = Transformation.CalculateRequiredRotationTransform(Vector3D.Zero, src, dest);
                Console.WriteLine("Required Transformed: {0}", reqTransform);
                Console.WriteLine("Transformed Source: {0}", reqTransform.Transform(src));

                System.Console.ReadLine();
            }
            */
//            Log.Initialise(LOG_FILE_PATH, true, true, true, true);

//            TestScene scene = new TestScene();
        }
    }
}