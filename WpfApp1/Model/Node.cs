using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Model
{
    public class Node 
    {
        public int NodeID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public List<Edge> Edges = new List<Edge>(); 

        public double? TwoDX { get; set; }
        public double? TwoDY { get; set; }

        private Color rgb;

        public Color RGB
        {
            get => rgb;
            set => rgb = value;
        }

        public String RGBString => RGB.R + " " + RGB.G + " " + RGB.B;

        public int i { get; set; }
        public int j { get; set; }

        public double D
        {
            get
            {
                return Math.Sqrt(Math.Pow(Y, 2) + Math.Pow(Z, 2));
            }
        }

        private double[,] Iprivate;

        public double[,] I
        {
            get
            {
                if (Iprivate == null)
                {
                    Iprivate = new double[3, 3];
                    for (var i = 0; i < 3; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            if (i == j)
                                I[i, j] = 1;
                            else
                                I[i, j] = 0;
                        }
                    }
                }
                return Iprivate;
            }
        }

public bool CanRemove
        {
            get { return Valence == 6; }
        }

        public int Valence
        {
            get { return Edges.Count(e => e.Nodes.Any(n => n.NodeID == NodeID)); }
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
            RGB = new Color(0,0,0);
        }

        public double Deethtenth(Node A, Node B, Node C)
        {
            var matrix = new double[3, 4];
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
                for (int j = i - 1; j >= 0; j--)
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
                    //var listIDs = polygons.Select(p => p.PolygonID).ToList();
                    foreach (var polygon in polygons)
                    {
                        foreach (var edge in polygon.Edges)
                        {
                            edge.Polygons.RemoveAll(p => p.PolygonID == polygon.PolygonID);
                        }
                    }
                    var listIDs = polygons.Select(p => p.PolygonID).ToList();
                    foreach (var polygon in listIDs)
                        Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
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
                    BC = BCEdges.SelectMany(e => e.Nodes).Where(n => Anabrth.All(na => na.NodeID != n.NodeID));
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
                        //var listIDs = polygons.Select(p => p.PolygonID).ToList();
                        foreach (var polygon in polygons)
                        {
                            foreach (var edge in polygon.Edges)
                            {
                                edge.Polygons.RemoveAll(p => p.PolygonID == polygon.PolygonID);
                            }
                            
                        }
                        var listIDs = polygons.Select(p => p.PolygonID).ToList();
                        foreach (var polygon in listIDs)
                            Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
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
            var l = s.Split(' ').Where(p => p != "").ToList();
            X = double.Parse(l[1]);
            Y = double.Parse(l[2]);
            Z = double.Parse(l[3]);
        }

        public Node(String[] s)
        {
            var l = s[0].Split(' ');
            NodeID = int.Parse(l[1]);
            
            X = double.Parse(l[3]);
            Y = double.Parse(l[4]);
            Z = double.Parse(l[5]);

            //var rgbS = s[1].Split(')')[0].Split('=')[1];
            //var uvS = s[1].Split(')')[1].Split('=').Last();
             

        }

        public override string ToString()
        {
            return NodeID + ": " + X + " " + Y + " " + Z + " ";
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

        public Vector3 normsl
        {
            get
            {
                var polygons = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == NodeID)).ToList();
                var norms = polygons.Select(p => p.getnorm).ToList();
                
                var no = new Vector3();
                foreach (var norm in norms)
                {
                    no += norm;
                }
                var l = no.Length();
                for (int i = 0; i < 3; i++)
                {
                    no[i] = no[i] / l;
                }
                return no;
            }
        }

        public static double[,] Mult(double[,] A, double[,] B, int m, int l, int n)
        {
            var c = new double[m, n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {

                    c[i, j] = 0;

                }
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < l; k++)
                    {
                        c[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return c;
        }

        public void Shift()
        {
            if (Edges.Count(e => e.Polygons.Count < 2) >= 2) return;

            //var polygons = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == NodeID)).ToList();
            var nodes = Edges.Where(e => e.Nodes.Any(n => n.NodeID == NodeID)).ToList()
                .Select(e => e.Nodes.FirstOrDefault(n => n.NodeID != NodeID)).ToList();
            //var es = Graph.Edges.Where(e => e.Nodes.Any(n => n.NodeID == NodeID)).ToList();

            var g = new double[3, 1];
            g[0, 0] = X;
            g[1, 0] = Y;
            g[2, 0] = Z;

            var gD = new double[3, 1];
            var v = Valence;

            gD[0, 0] = 1.0 / v * (nodes.Sum(n => n.X));
            gD[1, 0] = 1.0 / v * (nodes.Sum(n => n.Y));
            gD[2, 0] = 1.0 / v * (nodes.Sum(n => n.Z));

            var norm = new double[3, 1];
            var normT = new double[1, 3];
            var normlPreCalc = normsl;
            for (int i = 0; i < 3; i++)
            {
                normT[0, i] = norm[i, 0] = normlPreCalc[i];
            }

            var lamda = 0.5;
            var nn = Mult(norm, normT, 3, 1, 3);
            var pp = new double[3, 1];
            for (int i = 0; i < 3; i++)
            {
                pp[i, 0] = gD[i, 0] - g[i, 0];
            }
            var Inn = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Inn[i, j] = I[i, j] - nn[i, j];
                }
            }
            var Innpp = Mult(Inn, pp, 3, 3, 1);
            for (int i = 0; i < 3; i++)
            {
                Innpp[i, 0] = Innpp[i, 0] * lamda;
            }
            X = X + Innpp[0, 0];
            Y = Y + Innpp[1, 0];
            Z = Z + Innpp[2, 0];




            //var nodesByD = nodes.Where(n => n.D - D < 0.5).ToList();
            //var C = new Node { X = nodes.Average(n => n.X), Y = nodesByD.Average(n => n.Y), Z = nodesByD.Average(n => n.Z) };

            //var d = C.D;
            //var sign = Math.Asin(C.Y / d);
            //double r = sign > 0 ? Math.Acos(C.Z / d) : 2 * Math.PI - Math.Acos(C.Z / d);
            //var dn = D;
            //if (Math.Abs(X) < 4.75)
            //    X = C.X;
            //Y = C.Y;
            //Z = C.Z;
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
}
