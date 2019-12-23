using System;
using System.Collections.Generic;
using System.Linq;
using EntityUpdater.Interfaces;

namespace EntityUpdater.Logic
{
    public static class TopologicalSortLogic
    {
        /// <summary>
        ///     Build graph from list 
        /// </summary>
        /// <returns></returns>
        public static Func<IEnumerable<IEntityProfile>> BuildGraph(IReadOnlyCollection<IEntityProfile> profiles)
        {
            var vertices = profiles;
            var dependencies = profiles.ToDictionary(x => x, x => new List<IEntityProfile>());
            
            foreach (var profile in profiles)
            {
                foreach (var dependentProfile in profile.Members
                    .Select(member =>
                        profiles.FirstOrDefault(dependentProfile => dependentProfile.Type == member.PropertyType))
                    .Where(x => x != null))
                {
                    dependencies[profile].Add(dependentProfile);
                }
            }

            return () => TopologicalSort(vertices, dependencies);
        }

        /// <summary>
        /// Topological Sorting (Kahn's algorithm)
        /// In dictionary, key is child and value is parent or list of parents
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Topological_sorting</remarks>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TEnumerable"></typeparam>
        /// <param name="nodes">All nodes of directed acyclic graph.</param>
        /// <param name="dependencies">All dependencies of a node</param>
        /// <returns>Sorted node in topological order.</returns>
        private static IEnumerable<T> TopologicalSort<T, TEnumerable>(IEnumerable<T> nodes,
            IDictionary<T, TEnumerable> dependencies)
            where TEnumerable : IEnumerable<T>
        {
            // Convert to array
            var nodesList = nodes.ToList();

            // List of items yielded
            var visited = new List<T>();

            var comparer = (Func<T, T, bool>) ((x, y) => x.Equals(y));

            // All edges of directed acyclic graph
            var edges = dependencies
                .Select(x => x.Value.Select(y => (Source: x.Key, Target: y)))
                .SelectMany(x => x)
                .ToList();

            // Set of all nodes with no incoming edges
            var s = new HashSet<T>(nodesList.Where(n => edges.All(e => comparer(e.Target, n) == false)));

            // While S is non-empty do
            while (s.Any())
            {
                // Remove a node n from S
                var n = s.First();
                s.Remove(n);

                // Add to visited nodes
                visited.Add(n);

                // Add n to tail of L
                yield return n;

                // For each node m with an edge e from n to m do
                foreach (var edge in edges.Where(e => comparer(e.Source, n)).ToList())
                {
                    var m = edge.Target;

                    // Remove edge e from the graph
                    edges.Remove(edge);

                    // If m has no other incoming edges then
                    if (edges.All(me => comparer(me.Target, m) == false))
                    {
                        // Insert m into S
                        s.Add(m);
                    }
                }
            }

            // Yeild orphan nodes for completeness
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var node in nodesList.Except(visited))
            {
                yield return node;
            }
        }
    }
}