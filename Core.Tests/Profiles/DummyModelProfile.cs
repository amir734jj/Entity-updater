using System;
using Core.Tests.Models;
using EntityUpdater.Abstracts;

namespace Core.Tests.Profiles
{
    public class DummyModelProfile : EntityProfile<DummyModel>
    {
        public DummyModelProfile()
        {
            MapPrimitive(x => x.Id)
                //.MapReference(x => x.NestedDummyModel)
                //.MapReference(x => x.CircularModel)
                .MapPrimitive(x => x.Age)
                .MapRestrict(x => x.Firstname)
                .MapRestrict(x => x.Lastname)
                .MapPrimitive(x => x.IsPresent)
                .MapPrimitive(x => x.DateOfBirth)
                .Comparison((x, y) => x.Id == y.Id);
        }
    }
}