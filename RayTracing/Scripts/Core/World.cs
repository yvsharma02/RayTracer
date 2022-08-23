using System.Collections.Generic;

namespace RayTracing
{
    public class World
    {
        private int OCTANT_COUNT = 8;

        private int subdivisionLevel;
        private Bounds bounds;

        private World[] subwords;

        private List<Shape> shapes;
        private List<LightSource> lightSources;

        private Camera mainCamera;

        private bool IsLeaf => subdivisionLevel == 0;
        private bool isRoot;

        public Camera MainCamera
        {
            get => mainCamera;
            set => mainCamera = value;
        }

        /* Since we can have duplicates now.
        public int ShapesCount
        {
            get
            {
                return shapes.Count;
            }
        }
        */

        public int LightSourcesCount
        {
            get
            {
                return lightSources.Count;
            }
        }
        private World(Camera mainCamera, int subdivisionLevel, Bounds defaultBounds, bool isRoot)
        {
            this.bounds = defaultBounds;
            this.mainCamera = mainCamera;
            this.subdivisionLevel = subdivisionLevel;
            this.isRoot = isRoot;

            if (subdivisionLevel <= 0)
            {
                this.MainCamera = mainCamera;
            }
            else
            {
                Bounds[] subDividedBounds = defaultBounds.SubdivideIntoOctants();
                subwords = new World[OCTANT_COUNT];

                for (int i = 0; i < subwords.Length; i++)
                    subwords[i] = new World(null, subdivisionLevel - 1, subDividedBounds[i], false);
            }

            if (this.isRoot || this.IsLeaf)
                this.shapes = new List<Shape>();
            if (this.isRoot)
                this.lightSources = new List<LightSource>();
        }

        public World(Camera mainCamera, int subDivisionLevel, Bounds worldSize) : this(mainCamera, subDivisionLevel, worldSize, true) { }

        public Shape ClosestShapeHit(Ray ray, out Vector3D pointOfContact)
        {
            if (IsLeaf)
            {
                float minDistSq = float.MaxValue;
                Shape closestHitShape = null;
                pointOfContact = new Vector3D();
                Vector3D poc = new Vector3D();

                for (int i = 0; i < shapes.Count; i++)
                {
                    Shape currentHitSubshape;

                    if (shapes[i].HitsRay(ray, out poc, out currentHitSubshape))
                    {
                        float distSq = ray.Origin.DistanceFromSq(poc);
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                            closestHitShape = (currentHitSubshape == null) ? shapes[i] : currentHitSubshape;
                            pointOfContact = poc;
                        }
                    }
                }

                return closestHitShape;
            }
            else
            {
                for (int i = 0; i < subwords.Length; i++)
                {
                    if (RTMath.RayBoundsContact(ray, subwords[i].bounds.LowerBounds, subwords[i].bounds.UpperBounds) != null)
                    {
                        Vector3D poc;
                        Shape hit = subwords[i].ClosestShapeHit(ray, out poc);

                        if (hit != null)
                        {
                            pointOfContact = poc;
                            return hit;
                        }
                    }
                }
                pointOfContact = default;
                return null;
            }
        }

        private void AddShapesInternal(Shape shape)
        {
            if (IsLeaf || isRoot)
                this.shapes.Add(shape);

            if (!IsLeaf)
                for (int i = 0; i < subwords.Length; i++)
                    if (subwords[i].bounds.ContainsBoundPartially(shape.BoundaryBox))
                        subwords[i].AddShapesInternal(shape);
        }

        // Call should be initiated from the root itself.
        private void RegenerateSubwordsAndBounds(Bounds? newBounds, World root = null)
        {
            if (root == null)
                root = this;

            if (newBounds == null)
            {
                newBounds = new Bounds();
                if (isRoot)
                    for (int i = 0; i < shapes.Count; i++)
                        newBounds = newBounds.Value.RegenrateContaingBounds(shapes[i].BoundaryBox);
                else
                    newBounds = bounds;

//                Console.WriteLine("\nNewly Generated Bounds: " + newBounds.ToString());
            }
//            else
//                Console.WriteLine("\nOld Bounds were used: " + newBounds.ToString());

            bounds = newBounds.Value;

            if (!IsLeaf)
            {
                Bounds[] subdividedBounds = newBounds.Value.SubdivideIntoOctants();
                for (int i = 0; i < subwords.Length; i++)
                {
                    subwords[i] = new World(null, subdivisionLevel - 1, subdividedBounds[i], false);
                    subwords[i].RegenerateSubwordsAndBounds(null, root);
//                    Console.WriteLine("\nCreated a subworld with bounds: " + subdividedBounds[i].ToString());
                }
            }
            else if (root != this)
            {
                for (int i = 0; i < root.shapes.Count; i++)
                    if (bounds.ContainsBoundPartially(root.shapes[i].BoundaryBox))
                        this.shapes.Add(root.shapes[i]);
            }
        }

        // Since bounds need to recaluclated at each stage, make sure that the call is initiated at the root level.
        public void AddShapes(params Shape[] shapesToAdd)
        {
            if (!isRoot)
                throw new InvalidOperationException("Must be called from the root.");

            Bounds requiredBounds = Bounds.Zero;

            for (int i = 0; i < shapesToAdd.Length; i++)
                requiredBounds = requiredBounds.RegenrateContaingBounds(shapesToAdd[i].BoundaryBox);
            
            if (this.bounds.ContainsBoundCompletely(requiredBounds))
            {
                for (int i = 0; i < shapesToAdd.Length; i++)
                {
                    AddShapesInternal(shapesToAdd[i]);
                }
            }
            else
            {
                for (int i = 0; i < shapesToAdd.Length; i++)
                    this.shapes.Add(shapesToAdd[i]);

                RegenerateSubwordsAndBounds(requiredBounds);
            }
        }

        public void RemoveShape(Shape s)
        {
            shapes.Remove(s);
        }

        public void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }
        public void AddLightSource(LightSource source)
        {
            lightSources.Add(source);
        }

        public void RemoveLightSource(LightSource source)
        {
            lightSources.Remove(source);
        }

        public LightSource GetLightSource(int index)
        {
            return lightSources[index];
        }

        public Shape GetShape(int index)
        {
            return shapes[index];
        }
  }
}