using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Model
{
    public class Polygon
    {
        public int PolygonID { get; set; }
        public Edge f { get { return Edges[0]; } set { Edges[0] = value; } }
        public Edge s { get { return Edges[1]; } set { Edges[1] = value; } }
        public Edge t { get { return Edges[2]; } set { Edges[2] = value; } }
        public List<Edge> Edges { get; set; }

        public List<Node> Nodes
        {
            get
            {
                return Edges.SelectMany(e => e.Nodes).Distinct().ToList();
            }
        }

        public Polygon()
        {

        }

        public Polygon(Edge a, Edge b, Edge c)
        {
            var max = 1;
            if (Graph.Polygons.Any())
            {
                max = Math.Max(Graph.Polygons.Max(p => p.PolygonID) + 1, max);
            }
            PolygonID = max;
            Edges = new List<Edge> { a, b, c };
            foreach (var edge in Edges)
            {
                edge.Polygons.Add(this);
            }
        }

        public Polygon(Node a, Node b, Node c)
        {
            var max = 1;
            if (Graph.Polygons.Any())
            {
                max = Graph.Polygons.Count + 1;
                //max = Math.Max(Graph.Polygons.Max(p => p.PolygonID) + 1, max);
            }
            PolygonID = max;
            Edges = new List<Edge> { Edge.GetEdge(a, b, 1), Edge.GetEdge(b, c, 2), Edge.GetEdge(c, a, 3) };
            foreach (var edge in Edges)
            {
                edge.Polygons.Add(this);
            }
            Edge.CalcID(Edges.ToArray());
        }

        public override string ToString()
        {
            return string.Join(" ", Nodes.Select(n => n.NodeID));
            return base.ToString();
        }

        public Node GetC(Edge edge)
        {
            var node = Nodes.FirstOrDefault(n => edge.Nodes.All(ne => ne != n));
            return node;
        }

        public double GetCross(Edge edge)
        {
            Node node = GetC(edge);
            var result = edge.Cross(node);
            return result;
        }

        public Vector3 getnorm
        {
            get
            {
                Vector3 n = new Vector3();

                double mult = 0;
                //-- a - общая вершина двух векторов --// 
                n[0] = (float) ((Nodes[1].Y - Nodes[0].Y) * (Nodes[2].Z - Nodes[0].Z) - (Nodes[1].Z - Nodes[0].Z) * (Nodes[2].Y - Nodes[0].Y));
                n[1] = (float) ((Nodes[2].X - Nodes[0].X) * (Nodes[1].Z - Nodes[0].Z) - (Nodes[1].X - Nodes[0].X) * (Nodes[2].Z - Nodes[0].Z));
                n[2] = (float) ((Nodes[1].X - Nodes[0].X) * (Nodes[2].Y - Nodes[0].Y) - (Nodes[2].X - Nodes[0].X) * (Nodes[1].Y - Nodes[0].Y));

                mult += Nodes[0].X * n[0];
                mult += Nodes[0].Y * n[1];
                mult += Nodes[0].Z * n[2];

                if (mult < 0)
                    for (int j = 0; j < 3; j++)
                        n[j] = -n[j];
                return n;
            }
        }
    }
}
