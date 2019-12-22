using Core.Tests.Models;
using EntityUpdater.Abstracts;

namespace Core.Tests.Profiles
{
    public class CircularModelProfile : EntityProfile<CircularModel>
    {
        public CircularModelProfile()
        {
            MapPrimitive(x => x.Id)
                .MapReference(x => x.CircularModelRef);
        }
    }
}