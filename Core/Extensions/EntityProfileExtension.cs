using EntityUpdater.Interfaces;
using EntityUpdater.Models;

namespace EntityUpdater.Extensions
{
    public static class EntityProfileExtension
    {
        /// <summary>
        ///     Convert the profile to vertex
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static Vertex AsVertex(this IEntityProfile profile)
        {
            return new Vertex(profile);
        }

        /// <summary>
        ///     Convert tuple of vertices to an edge
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static Edge AsEdge(this (Vertex source, Vertex target) argument)
        {
            var (source, target) = argument;
            return new Edge(source, target);
        }
    }
}