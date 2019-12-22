using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityUpdater.Interfaces
{
    public interface IEntityProfile
    {
        bool TypeCheck(object instance);

        Type Type { get; }

        MethodInfo ComparerMethodInfo { get; }

        IList<PropertyInfo> Members { get; }

        Func<object, object, bool> Compare { get; }
    }

    public interface IMap<T>
    {
        IMapUCompare<T> MapReference<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : new();

        IMapUCompare<T> MapPrimitive<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : struct;

        IMapUCompare<T> MapRestrict<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : IComparable, IConvertible, IEquatable<TProperty>;
    }

    public interface IMapAndMap<T> : IMap<T>
    {
    }

    public interface IMapUCompare<T> : ICompare<T>, IMapAndMap<T>
    {
    }

    public interface ICompare<out T>
    {
        void Comparison(Func<T, T, bool> comparer);
    }
}