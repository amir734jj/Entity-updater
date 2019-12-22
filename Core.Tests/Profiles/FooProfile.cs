using Core.Tests.Models;
using EntityUpdater.Abstracts;

namespace Core.Tests.Profiles
{
    public class FooProfile : EntityProfile<Foo>
    {
        public FooProfile()
        {
            MapRestrict(x => x.F1);
        }
    }
}