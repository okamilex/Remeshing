using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace WpfApp1
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

        
    }

    public class Node
    {
        public int NodeID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int i { get; set; }
        public int j { get; set; }

        public double D
        {
            get
            {
                return Math.Sqrt(Math.Pow(Y, 2) + Math.Pow(Z, 2));
            }
        }

        public bool CanRemove
        {
            get { return Valence == 6; }
        }

        public int Valence
        {
            get { return Graph.Edges.Count(e => e.Nodes.Any(n => n.NodeID == NodeID)); }
        }

        public int ValenceD
        {
            get
            {
                return Graph.Edges.Where(e => e.Nodes.Any(n => n.NodeID == NodeID)).Count(e => e.Nodes.All(n => Math.Abs(n.D - D) < 0.5));
            }
        }

        public double R
        {
            get
            {
                var sign = Math.Asin(Y / D);
                return sign > 0 ? Math.Acos(Z / D) : 2 * Math.PI - Math.Acos(Z / D);

            }
        }

        public Node()
        {
        }

        public double Deethtenth(Node A, Node B, Node C)
        {
            var matrix = new double[3,4];
            matrix[0, 0] = A.X; matrix[0, 1] = A.Y; matrix[0, 2] = A.Z; matrix[0, 3] = -1;
            matrix[1, 0] = B.X; matrix[1, 1] = B.Y; matrix[1, 2] = B.Z; matrix[1, 3] = -1;
            matrix[2, 0] = C.X; matrix[2, 1] = C.Y; matrix[2, 2] = C.Z; matrix[2, 3] = -1;
            for (int i = 0; i < 3; i++)
            {
                for (int k = 3; k >= i; k--)
                {
                    matrix[i, k] /= matrix[i, i];
                }
                for (int j = i + 1; j < 3; j++)
                {
                    for (int k = 3; k >= i; k--)
                    {
                        matrix[j, k] -= matrix[i, k] * matrix[j, i];
                    }
                }
            }
            for (int i = 2; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0 ; j--)
                {
                        matrix[j, 3] -= matrix[i, 3] * matrix[j, i];
                   
                }
            }
            var a = matrix[0, 3];
            var b = matrix[1, 3];
            var c = matrix[2, 3];
            var d = 1;
            var znam = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2) + Math.Pow(c, 2));
            var res = (a * X + b * Y + c * Z + d) / znam;
            return res;
        }

        public void AdaptNodeC()
        {
            if (CanRemove)
            {
                var polygons = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == NodeID));
                var A = polygons.SelectMany(p => p.Nodes.Where(n => n.NodeID != NodeID)).FirstOrDefault();
                var Anabrth = polygons.SelectMany(p => p.Edges).Where(e => e.Nodes.Any(n => n.NodeID == A.NodeID))
                    .SelectMany(e => e.Nodes).Where(n => n.NodeID != A.NodeID && n.NodeID != NodeID);
                var BCPolygonsEdges = polygons.Where(p =>
                    p.Nodes.Any(n => Anabrth.Any(na => na.NodeID == n.NodeID)) && p.Nodes.All(n => n.NodeID != A.NodeID)).SelectMany(p => p.Edges);
                var BCEdges = BCPolygonsEdges.Where(e => e.Nodes.All(n => n.NodeID != NodeID));
                var BC = BCEdges.SelectMany(e => e.Nodes).Where(n => Anabrth.All(na => na.NodeID != n.NodeID));
                var B = BC.FirstOrDefault();
                var C = BC.LastOrDefault();
                if (Math.Abs(Deethtenth(A, B, C)) < 0.1)
                {
                    var E = Anabrth.FirstOrDefault();
                    var F = Anabrth.LastOrDefault();
                    var D = polygons.SelectMany(p => p.Nodes).Distinct().FirstOrDefault(n =>
                        n.NodeID != NodeID && n.NodeID != A.NodeID && n.NodeID != B.NodeID && n.NodeID != B.NodeID &&
                        n.NodeID != E.NodeID && n.NodeID != F.NodeID);
                    var na = Edge.GetEdge(this, D, 0);
                    var nb = Edge.GetEdge(this, E, 0);
                    var nc = Edge.GetEdge(this, F, 0);
                    na.FlipAnsave(); nb.FlipAnsave(); nc.FlipAnsave();
                    polygons = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == NodeID));
                    var polygonNodes = polygons.SelectMany(p => p.Nodes).Where(n => n.NodeID != NodeID).Distinct().ToList();
                    if (polygonNodes.Count != 3 || !polygonNodes.Any(n => n.NodeID == A.NodeID) || !polygonNodes.Any(n => n.NodeID == B.NodeID) || !polygonNodes.Any(n => n.NodeID == B.NodeID))
                    {
                        return;
                    }
                    var listIDs = polygons.Select(p => p.PolygonID).ToList();
                    foreach (var polygon in listIDs)
                    {
                        Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
                    }
                    Graph.Polygons.Add(new Polygon(polygonNodes[0], polygonNodes[1], polygonNodes[2]));
                }
                else
                {
                    A = Anabrth.FirstOrDefault();
                    Anabrth = polygons.SelectMany(p => p.Edges).Where(e => e.Nodes.Any(n => n.NodeID == A.NodeID))
                        .SelectMany(e => e.Nodes).Where(n => n.NodeID != A.NodeID && n.NodeID != NodeID);
                    BCPolygonsEdges = polygons.Where(p =>
                        p.Nodes.Any(n => Anabrth.Any(na => na.NodeID == n.NodeID)) && p.Nodes.All(n => n.NodeID != A.NodeID)).SelectMany(p => p.Edges);
                    BCEdges = BCPolygonsEdges.Where(e => e.Nodes.All(n => n.NodeID != NodeID));
                    BC = BCEdges.SelectMany(e => e.Nodes).Where( n => Anabrth.All(na => na.NodeID != n.NodeID));
                    B = BC.FirstOrDefault();
                    C = BC.LastOrDefault();
                    if (Math.Abs(Deethtenth(A, B, C)) < 0.1)
                    {
                        var E = Anabrth.FirstOrDefault();
                        var F = Anabrth.LastOrDefault();
                        var D = polygons.SelectMany(p => p.Nodes).Distinct().FirstOrDefault(n =>
                            n.NodeID != NodeID && n.NodeID != A.NodeID && n.NodeID != B.NodeID && n.NodeID != B.NodeID &&
                            n.NodeID != E.NodeID && n.NodeID != F.NodeID);
                        var na = Edge.GetEdge(this, D, 0);
                        var nb = Edge.GetEdge(this, E, 0);
                        var nc = Edge.GetEdge(this, F, 0);
                        na.FlipAnsave(); nb.FlipAnsave(); nc.FlipAnsave();
                        polygons = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == NodeID));
                        var polygonNodes = polygons.SelectMany(p => p.Nodes).Where(n => n.NodeID != NodeID).Distinct().ToList();
                        if (polygonNodes.Count != 3 || !polygonNodes.Any(n => n.NodeID == A.NodeID) || !polygonNodes.Any(n => n.NodeID == B.NodeID) || !polygonNodes.Any(n => n.NodeID == B.NodeID))
                        {
                            return;
                        }
                        var listIDs = polygons.Select(p => p.PolygonID).ToList();
                        foreach (var polygon in listIDs)
                        {
                            Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
                        }
                        Graph.Polygons.Add(new Polygon(polygonNodes[0], polygonNodes[1], polygonNodes[2]));
                    }
                }
            }
        }

        public Node(Node a, Node b)
        {
            var max = 1;
            if (Graph.Polygons.Any())
            {
                max = Math.Max(Graph.Nodes.Max(e => e.NodeID) + 1, max);
            }
            NodeID = max;
            X = (a.X + b.X) / 2;
            Y = (a.Y + b.Y) / 2;
            Z = (a.Z + b.Z) / 2;
        }

        public Node(String s)
        {
            var l = s.Split(' ');
            X = double.Parse(l[1]);
            Y = double.Parse(l[2]);
            Z = double.Parse(l[3]);
        }

        public Node(String s, int id)
        {
            NodeID = id;
            var l = s.Split(' ');
            X = double.Parse(l[1]);
            Y = double.Parse(l[2]);
            Z = double.Parse(l[3]);
        }

        public override string ToString()
        {
            return NodeID + ": "+ X + " " + Y + " " + Z + " " ;
            return base.ToString();
        }

        public void Shift2()
        {
            var nodes = Graph.Edges.Where(e => e.Nodes.Any(n => n.NodeID == NodeID)).ToList()
                .Select(e => e.Nodes.FirstOrDefault(n => n.NodeID != NodeID)).ToList();
            var nodesByD = nodes.Where(n => n.D - D < 0.5).ToList();
            var C = new Node { X = nodes.Average(n => n.X), Y = nodesByD.Average(n => n.Y), Z = nodesByD.Average(n => n.Z) };

            var d = C.D;
            var sign = Math.Asin(C.Y / d);
            double r = sign > 0 ? Math.Acos(C.Z / d) : 2 * Math.PI - Math.Acos(C.Z / d);
            var dn = D;
           // X = C.X;
            Y = C.Y;
            Z = C.Z;
            Y = dn * Math.Sin(r);
            Z = dn * Math.Cos(r);
            var t = Edge.GetEdge(this, C, 0).Length;
            if (t > 0.1)
            {
                var y = 0;
                y = y + 5;
            }
            //Y = dn*Math.Cos(r);
            //Z = dn*Math.Sin(r);
        }

        public void Shift()
        {
            var nodes = Graph.Edges.Where(e => e.Nodes.Any(n => n.NodeID == NodeID)).ToList()
                .Select(e => e.Nodes.FirstOrDefault(n => n.NodeID != NodeID)).ToList();
            var nodesByD = nodes.Where(n => n.D - D < 0.5).ToList();
            var C = new Node{X = nodes.Average(n => n.X), Y = nodesByD.Average(n => n.Y), Z = nodesByD.Average(n => n.Z)};
        
            var d = C.D;
            var sign = Math.Asin(C.Y / d);
            double r = sign > 0 ? Math.Acos(C.Z / d) : 2 * Math.PI - Math.Acos(C.Z / d);
            var dn = D;
            if (Math.Abs(X) < 4.75)
            X = C.X;
            Y = C.Y;
            Z = C.Z;
            //Y = dn*Math.Sin(r);
            //Z = dn*Math.Cos(r);
            //var t = Edge.GetEdge(this, C, 0).Length;
            //if (t > 0.1)
            //{
            //    var y = 0;
            //    y = y + 5;
            //}
            //Y = dn*Math.Cos(r);
            //Z = dn*Math.Sin(r);
        }
    }

    public class Edge
    {
        public int EdgeID { get; set; }
        public Node A { get { return Nodes[0]; } set { Nodes[0] = value; } }
        public Node B { get { return Nodes[1]; } set { Nodes[1] = value; } }
        public static double eps = 0.5;

        public bool D => (Math.Abs(A.D - B.D) < 0.5);

        public bool Eps(double a, double b)
        {
            var f = a - b;
            if (f < 0) f = f + 2 * Math.PI;
            var s = b - a;
            if (s < 0) s = s + 2 * Math.PI;
            return Math.Min(f,s) < eps;
        }

        public bool XR(Node N,  Node C)
        {
            var a = A.R;
            var b = B.R;
            var c = C.R;
            var n = N.R;
            var f = a - c;
            if (f < 0) f = f + 2 * Math.PI;
            var s = c - a;
            if (s < 0) s = s + 2 * Math.PI;
            var d1 = Math.Min(f, s);
            var min = Math.Min(a, c);
            if (min < d1)
            {
                a += Math.PI;
                if (a > 2 * Math.PI)
                {
                    a -= 2 * Math.PI;
                }
                b += Math.PI;
                if (b > 2 * Math.PI)
                {
                    b -= 2 * Math.PI;
                }
                c += Math.PI;
                if (c > 2 * Math.PI)
                {
                    c -= 2 * Math.PI;
                }
                n += Math.PI;
                if (n > 2 * Math.PI)
                {
                    n -= 2 * Math.PI;
                }

            }

            f = b - c;
            if (f < 0) f = f + 2 * Math.PI;
            s = c - b;
            if (s < 0) s = s + 2 * Math.PI;
            var d2 = Math.Min(f, s);
            min = Math.Min(b, c);
            if (min < d2)
            {
                a += Math.PI;
                if (a > 2 * Math.PI)
                {
                    a -= 2 * Math.PI;
                }
                b += Math.PI;
                if (b > 2 * Math.PI)
                {
                    b -= 2 * Math.PI;
                }
                c += Math.PI;
                if (c > 2 * Math.PI)
                {
                    c -= 2 * Math.PI;
                }
                n += Math.PI;
                if (n > 2 * Math.PI)
                {
                    n -= 2 * Math.PI;
                }
            }

            if (n < Math.Min(b, c))
            {
                return false;
            }
            if (n > Math.Max(a, c))
            {
                return false;
            }
            if ((a + d1 * N.X) / Math.Abs(A.X - C.X) < n)
            {
                return false;
            }
            if ((b + d2 * N.X) / Math.Abs(B.X - C.X) > n)
            {
                return false;
            }
            
            return true;
        }

        public int Count
        {
            get { return Graph.Edges.Count(e => e.Nodes.Contains(A) && e.Nodes.Contains(B)); }
        }

        public static Edge GetEdge(Node a, Node b, int i)
        {
            var edge = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(a) && e.Nodes.Contains(b));
            if (edge != null)
                return edge;
             return new Edge(a,b, i);
        }
        public static int GetID(int i)
        {
            var max = 0;
            if (Graph.Polygons.Any())
            {
                max = Math.Max(Graph.Edges.Max(e => e.EdgeID), max);
            }
            return max + i;
        
        }

        public double Cross(Node node)
        {
            Vector3 left = new Vector3(((float) node.X - (float) A.X), ((float) node.Y - (float) A.Y),
                ((float) node.Z - (float) A.Z));
            Vector3 right = new Vector3(((float) node.X - (float) B.X), ((float) node.Y - (float) B.Y),
                ((float) node.Z - (float) B.Z));
            Vector3 returnValue;
            returnValue.X = left.Y * right.Z - left.Z * right.Y;
            returnValue.Y = left.Z * right.X - left.X * right.Z;
            returnValue.Z = left.X * right.Y - left.Y * right.X;
            return returnValue.Length();
        }

        public Node GetNodeForAdaptiveInD()
        {
            var nodes = Graph.Nodes.Where(n => Nodes.Any(ne => ne.NodeID != n.NodeID)).ToList();
            var nodesByD = nodes.Where(n => Nodes.All(ne => Math.Abs(n.D - ne.D) < 0.2)).ToList();
            var nodesByR = nodesByD.Where(n => Nodes.All(ne => Eps(ne.R , n.R)));
            var nodeMinA = nodesByR.OrderByDescending(Cross).FirstOrDefault();
            if (nodeMinA == null)
            {
                return null;
            }
            var nodea = Cross(nodeMinA);
            

            var nodesAnsAreasMax = nodesByR.Where(node => Math.Abs(Cross(node) - nodea) < 0.5);

            //var nodea = nodesAnsAreas.OrderByDescending(naa => naa.area).ToList().FirstOrDefault();
            //var nodesAnsAreasMAx = nodesAnsAreas.Where(naa => naa.area - nodea.area > -0.1).Select(naa => naa.node).ToList();
            var nodesAnsAreasMaxByVNode = nodesAnsAreasMax.OrderBy(n => n.ValenceD).FirstOrDefault();
            var nodesAnsAreasMaxByV = nodesAnsAreasMax.Where(n => n.ValenceD == nodesAnsAreasMaxByVNode.D);

            return nodesAnsAreasMaxByV.OrderBy(n => Nodes.Max(ne => Edge.GetEdge(n, ne, 0).Count)).FirstOrDefault();
        }

        public void DeletePolygons(Node C)
        {
            var nodesByAC = Graph.Nodes.Where(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID && n.NodeID != C.NodeID && XR(n, C)).ToList();
            var polygons = nodesByAC.SelectMany(n => Graph.Polygons.Where(p => p.Nodes.Any(np => np.NodeID == n.NodeID)));
            var listIDs = polygons.Select(p => p.PolygonID).ToList();
            foreach (var polygon in listIDs)
            {
                Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
            }
        }

        public void AdaptEdge()
        {
            if (A.D - B.D < 0.5)
            {
                var C = GetNodeForAdaptiveInD();
                if (C != null)
                {
                    DeletePolygons(C);
                    Graph.Polygons.Add(new Polygon(A, B, C));
                }
            }
        }

        public void AdaptEdgeB()
        {
                var C = GetNodeForAdaptiveInD();
                if (C != null)
                {
                    Graph.Polygons.Add(new Polygon(A, B, C));
                }
            
        }

        public Edge(Node a, Node b, int i)
        {
            EdgeID = GetID(i);
            if (EdgeID == 162)
            {
                var t = 0;
                t = t + 0;
            }
            Nodes = new List<Node>();
            Nodes.Add(a);
            Nodes.Add(b);
        }

        public List<Node> Nodes { get; set; }

        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2) + Math.Pow(A.Z - B.Z, 2));
            }
        }

        public void Split()
        {
            var C = A;
            var D = B;
            var polygons = Graph.Polygons.Where(p => p.Edges.Any(e => e.Nodes.Contains(A) && e.Nodes.Contains(B)));
            var E = polygons.FirstOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);
            var F = polygons.LastOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);

            var listIDs = polygons.Select(p => p.PolygonID).ToList();
            foreach (var polygon in listIDs)
            {
                Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
            }


            Create(C, D, E, F);
        }

        public void Colaps()
        {
            var polygons = Graph.Polygons.Where(p => p.Edges.Any(e => e.Nodes.Contains(A) && e.Nodes.Contains(B)));
            //var C = polygons.SelectMany(e => e.Nodes).Distinct().FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);
            //var D = polygons.SelectMany(e => e.Nodes).Distinct().FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID && n.NodeID != C.NodeID);
            var secondLayer = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == A.NodeID) ^ p.Nodes.Any(n => n.NodeID == B.NodeID)).ToList();
            var listIDs = polygons.Select(p => p.PolygonID).ToList();
            foreach (var polygon in listIDs)
            {
                Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
            }

            var N = new Node(A, B);
            var allEdges = Graph.Edges.Where(e =>
                e.Nodes.Any(n => n.NodeID == A.NodeID || n.NodeID == B.NodeID)).ToList();
            foreach (var edge in allEdges)
            {
                edge.Nodes.Remove(A);
                edge.Nodes.Remove(B);
                edge.Nodes.Add(N);
                edge.EdgeID = GetID(1);
            }
            //foreach (var polygon in secondLayer)
            //{
            //    if (polygon.PolygonID == 21)
            //    {
            //        var t = 0;
            //        t = t + 0;
            //    }
            //    var notDone = true;
            //    Edge edge;
            //    //edge = polygon.Edges.FirstOrDefault(e => (e.Nodes.Contains(C) && e.Nodes.Contains(A)) || (e.Nodes.Contains(C) && e.Nodes.Contains(B)));
            //    //if (edge != null)
            //    //{
            //    //    polygon.Edges.Remove(edge);
            //    //    polygon.Edges.Add(edgeC);
                   
            //    //}
            //    //edge = polygon.Edges.FirstOrDefault(e => (e.Nodes.Contains(D) && e.Nodes.Contains(A)) || (e.Nodes.Contains(D) && e.Nodes.Contains(B)));
            //    //if (edge != null)
            //    //{
            //    //    polygon.Edges.Remove(edge);
            //    //    polygon.Edges.Add(edgeD);
                  
            //    //}
            //    if (notDone)
            //    {
            //        var edges = polygon.Edges.Where(e =>
            //            e.Nodes.Any(n => n.NodeID == A.NodeID || n.NodeID == B.NodeID)).ToList();
            //        if (edges.Any())
            //        {
            //            edge = edges[0];
            //            edge.Nodes.Remove(A);
            //            edge.Nodes.Remove(B);
            //            edge.Nodes.Add(N);
            //            edge.EdgeID = GetID(1);
            //        }
            //        if (edges.Count > 1)
            //        {
            //            edge = edges[1];
            //            edge.Nodes.Remove(A);
            //            edge.Nodes.Remove(B);
            //            edge.Nodes.Add(N);
            //            edge.EdgeID = GetID(1);
            //        }
            //    }
            //}
        }

        public void Create(Node C, Node D, Node E, Node F)
        {
            var N = new Node(A, B);
            var e1 = GetEdge(C, N, 1);
            var e2 = GetEdge(E, N, 2);
            var e3 = GetEdge(D, N, 3);
            var e4 = GetEdge(F, N, 4);

            var eo1 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(C) && e.Nodes.Contains(E));
            var eo2 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(D) && e.Nodes.Contains(E));
            var eo3 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(D) && e.Nodes.Contains(F));
            var eo4 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(C) && e.Nodes.Contains(F));

            if (eo1 != null)
            {
                Graph.Polygons.Add(new Polygon(eo1, e1, e2));
            }
            if (eo2 != null)
            {
                Graph.Polygons.Add(new Polygon(eo2, e2, e3));
            }
            if (eo3 != null)
            {
                Graph.Polygons.Add(new Polygon(eo3, e3, e4));
            }
            if (eo4 != null)
            {
                Graph.Polygons.Add(new Polygon(eo4, e4, e1));
            }
        }

        public override string ToString()
        {
            return string.Join(" ", Nodes.Select(n => n.NodeID));
            return base.ToString();
        }

        public bool NeedFlip
        {
            get
            {
                //Random Rand = new Random();
                //if (Rand.Next() % 10 == 0)
                //{
                //    return true;
                //}
                //return false;
                var C = A;
                var D = B;
                var polygons = Graph.Polygons.Where(p => p.Edges.Any(e => e.Nodes.Contains(A) && e.Nodes.Contains(B)));
                var E = polygons.FirstOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);
                var F = polygons.LastOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);

                var v = (C.Valence + B.Valence + E.Valence + F.Valence) / 4;
                var a = (Math.Abs(C.Valence - v) + Math.Abs(D.Valence - v) + Math.Abs(E.Valence - v) +
                         Math.Abs(F.Valence - v));
                var b = (Math.Abs(C.Valence - 1 - v) + Math.Abs(D.Valence - 1 - v) + Math.Abs(E.Valence + 1- v) +
                         Math.Abs(F.Valence + 1 - v));

                return b < a;
            }
        }

        public void FlipAnsave()
        {
            if (true)
            {
                var C = A;
                var D = B;
                var polygons = Graph.Polygons.Where(p => p.Edges.Any(e => e.Nodes.Contains(A) && e.Nodes.Contains(B)));
              
                var E = polygons.FirstOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);
                var F = polygons.LastOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);

                var listIDs = polygons.Select(p => p.PolygonID).ToList();
                foreach (var polygon in listIDs)
                {
                    Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
                }

                var edge = GetEdge(F, E, 1);

                var eo1 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(C) && e.Nodes.Contains(E));
                var eo2 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(C) && e.Nodes.Contains(F));
                var eo3 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(D) && e.Nodes.Contains(F));
                var eo4 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(D) && e.Nodes.Contains(E));

                if ((eo1 != null) && (eo2 != null))
                {
                    Graph.Polygons.Add(new Polygon(edge, eo1, eo2));
                    eo1.EdgeID = GetID(1);
                    eo2.EdgeID = GetID(1);

                }
                if ((eo3 != null) && (eo4 != null))
                {
                    Graph.Polygons.Add(new Polygon(edge, eo3, eo4));
                    eo3.EdgeID = GetID(1);
                    eo4.EdgeID = GetID(1);
                }

            }
        }

        public void Flip()
        {
            if (NeedFlip)
            {
                var C = A;
                var D = B;
                var polygons = Graph.Polygons.Where(p => p.Edges.Any(e => e.Nodes.Contains(A) && e.Nodes.Contains(B)));
                var E = polygons.FirstOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);
                var F = polygons.LastOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID && n.NodeID != B.NodeID);

                var listIDs = polygons.Select(p => p.PolygonID).ToList();
                foreach (var polygon in listIDs)
                {
                    Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
                }

                var edge = GetEdge(F, E, 1);

                var eo1 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(C) && e.Nodes.Contains(E));
                var eo2 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(C) && e.Nodes.Contains(F));
                var eo3 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(D) && e.Nodes.Contains(F));
                var eo4 = Graph.Edges.FirstOrDefault(e => e.Nodes.Contains(D) && e.Nodes.Contains(E));

                if ((eo1 != null) && (eo2 != null))
                {
                    Graph.Polygons.Add(new Polygon(edge, eo1, eo2));
                    eo1.EdgeID = GetID(1);
                    eo2.EdgeID = GetID(1);

                }
                if ((eo3 != null) && (eo4 != null))
                {
                    Graph.Polygons.Add(new Polygon(edge, eo3, eo4));
                    eo3.EdgeID = GetID(1);
                    eo4.EdgeID = GetID(1);
                }
          
            }
        }
    }

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
            Edges = new List<Edge> {a, b, c};
        }

        public Polygon(Node a, Node b, Node c)
        {
            var max = 1;
            if (Graph.Polygons.Any())
            {
                max = Math.Max(Graph.Polygons.Max(p => p.PolygonID) + 1, max);
            }
            PolygonID = max;
            Edges = new List<Edge> {Edge.GetEdge(a, b, 1), Edge.GetEdge(b, c, 2), Edge.GetEdge(c, a, 3)};
        }

        public override string ToString()
        {
            return string.Join(" ", Nodes.Select(n => n.NodeID));
            return base.ToString();
        }
    }
}
