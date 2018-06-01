using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public static class Graph
    {
        public static List<Polygon> Polygons { get; set; } = new List<Polygon>();

        public static List<Edge> Edges
        {
            get
            {
                return Polygons.Select(x => x.Edges).SelectMany(x => x).Distinct().ToList();
            }
        }
        public static List<Node> Nodes
        {
            get
            {
                return Polygons.Select(x => x.Edges.Select(e => e.Nodes).SelectMany(e => e)).SelectMany(x => x).Distinct().ToList();
            }
        }

        public static List<Edge> EdgesOrderBy
        {
            get
            {
                return Polygons.Select(x => x.Edges).SelectMany(x => x).Distinct().OrderBy(e => e.EdgeID).ToList();
            }
        }
        public static List<Node> NodesOrderBy
        {
            get
            {
                return Polygons.Select(x => x.Edges.Select(e => e.Nodes).SelectMany(e => e)).SelectMany(x => x).Distinct().OrderBy(e => e.NodeID).ToList();
            }
        }

    }
}
