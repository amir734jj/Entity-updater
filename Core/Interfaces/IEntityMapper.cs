namespace EntityUpdater.Interfaces
{
    public interface IEntityMapper
    {
        /// <summary>
        ///     Updates entity given dto
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dto"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void Update<T>(T entity, T dto);
    }
}