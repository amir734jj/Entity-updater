using QuickGraph;

namespace EntityUpdater.Models
{
    public class Edge : IEdge<Vertex>
    {
        public Vertex Source { get; }
        public Vertex Target { get; }

        public Edge(Vertex source, Vertex target)
        {
            Source = source;
            Target = target;
        }
    }
}