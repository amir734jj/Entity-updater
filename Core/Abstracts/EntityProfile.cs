using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using EntityUpdater.Extensions;
using EntityUpdater.Interfaces;

namespace EntityUpdater.Abstracts
{
    public abstract class EntityProfile<T> : IMapUCompare<T>, IEntityProfile
    {
        /// <summary>
        ///     Returns all mapped property infos
        /// </summary>
        public IList<PropertyInfo> Members { get; set; } = new List<PropertyInfo>();

        /// <summary>
        ///     Comparer function
        /// </summary>
        public Func<object, object, bool> Compare { get; private set; } = (o1, o2) => o1 switch
        {
            T t1 when o2 is T t2 => ReferenceEquals(t1, t2),
            _ => false
        };

        /// <summary>
        ///     Type check if object is instance of type T
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool TypeCheck(object instance) => instance switch
        {
            T _ => true,
            _ => false
        };

        public Type Type { get; } = typeof(T);

        /// <summary>
        ///     Returns MethodInfo of a comparer
        /// </summary>
        public MethodInfo ComparerMethodInfo => Compare.Method;

        /// <summary>
        ///     Override the comparer
        /// </summary>
        /// <param name="comparer"></param>
        public void Comparison(Func<T, T, bool> comparer) => Compare = (o1, o2) => o1 switch
        {
            T t1 when o2 is T t2 => comparer(t1, t2),
            _ => false
        };

        /// <summary>
        ///     Validate PropertyInfo has getter and setter defined
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static PropertyInfo ValidateProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetMethod == null)
            {
                throw new Exception($"Missing get method for property: {propertyInfo.Name}");
            }
            
            if (propertyInfo.SetMethod == null)
            {
                throw new Exception($"Missing set method for property: {propertyInfo.Name}");
            }

            return propertyInfo;
        }

        public IMapUCompare<T> MapReference<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : new()
        {
            return Map(expression);
        }

        public IMapUCompare<T> MapPrimitive<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : struct
        {
            return Map(expression);
        }

        public IMapUCompare<T> MapRestrict<TProperty>(Expression<Func<T, TProperty>> expression) where TProperty : IComparable, IConvertible, IEquatable<TProperty>
        {
            return Map(expression);
        }


        /// <summary>
        ///     Map utility helper
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        private IMapUCompare<T> Map<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            Members.Add(ValidateProperty(expression.ResolvePropertyInfo()));

            return this;
        }
    }
}