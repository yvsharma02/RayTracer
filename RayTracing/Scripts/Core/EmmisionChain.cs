namespace RayTracing
{
    public class EmmisionChain
    {
        private EmmisionChain[] parentSources;

        public readonly WorldObject Emmiter;
        public readonly ColoredRay EmmitedRay;

        public int ParentSourcesCount
        {
            get
            {
                return parentSources == null ? 0 : parentSources.Length;
            }
        }

        public EmmisionChain(WorldObject Emmiter, ColoredRay emmitedRay, params EmmisionChain[] parents)
        {
            this.Emmiter = Emmiter;
            this.EmmitedRay = emmitedRay;

            this.parentSources = parents;
        }

        public EmmisionChain this[int index]
        {
            get
            {
                return parentSources[index];
            }
        }
    }
}