using EntityUpdater.Abstracts;
using EntityUpdater.Interfaces;

namespace EntityUpdater.Models
{
    public class Vertex
    {
        public IEntityProfile Profile { get; private set; }

        public Vertex(IEntityProfile profile)
        {
            Profile = profile;
        }
        

    }
}