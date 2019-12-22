using Core.Tests.Models;
using EntityUpdater.Abstracts;

namespace Core.Tests.Profiles
{
    public class NestedDummyModelProfile : EntityProfile<NestedDummyModel>
    {
        public NestedDummyModelProfile()
        {
            MapPrimitive(x => x.Id)
                .MapPrimitive(x => x.Age)
                .MapPrimitive(x => x.IsPresent)
                .MapPrimitive(x => x.DateOfBirth);
        }
    }
}