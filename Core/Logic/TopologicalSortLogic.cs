using System.Collections.Generic;
using System.Linq;
using EntityUpdater.Extensions;
using EntityUpdater.Interfaces;
using EntityUpdater.Models;
using QuickGraph;
using QuickGraph.Algorithms;

namespace EntityUpdater.Logic
{
    public class TopologicalSortLogic
    {
        protected readonly IReadOnlyCollection<IEntityProfile> _profiles;

        protected TopologicalSortLogic(IReadOnlyCollection<IEntityProfile> profiles)
        {
            _profiles = BuildGraph(profiles).TopologicalSort().Select(x => x.Profile).ToList();
        }
        
        /// <summary>
        ///     Build graph from list 
        /// </summary>
        /// <returns></returns>
        private static UndirectedGraph<Vertex, Edge> BuildGraph(IReadOnlyCollection<IEntityProfile> profiles)
        {
            var graph = new UndirectedGraph<Vertex, Edge>();
            
            foreach (var profile in profiles)
            {
                graph.AddVertex(profile.AsVertex());
            }

            foreach (var profile in profiles)
            {
                foreach (var (_, dependentProfile) in profiles
                    .Select(x => (Member: x, Profile: profiles.FirstOrDefault(y => x.Type == y.Type)))
                    .Where(x => x.Profile != null))
                {
                    graph.AddEdge((profile.AsVertex(), dependentProfile.AsVertex()).AsEdge());
                }
            }

            return graph;
        }
    }
}