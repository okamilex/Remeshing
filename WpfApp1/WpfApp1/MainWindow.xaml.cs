using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.IO;
using HelixToolkit.Wpf;
using System.Windows.Media.Converters;
using System.Xml.Linq;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random Rand = new Random();
        List<Node> nodes = new List<Node>();
        public Polygon polygon;

        public double rand
        {
            get
            {
                //return 0;
                return Convert.ToDouble(Rand.Next(99)) / 100;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Graph.Polygons = new List<Polygon>();
            nodes = new List<Node>();

            LoadObj();

            //genObj();

            //Remesh1();
           // Remesh2();
           // Remesh2B();
            Remesh2C();

            SaveAndShow();


        }

        public void Remesh1()
        {
            for (int i = 0; i < 5; i++)
            {
                Split();
                Colaps();
                Flip();
                Shift();
            }
        }

        public void Remesh2()
        {
            for (int i = 0; i < 5; i++)
            {
                Adapte();
            }
        }

        public void Remesh2B()
        {
            for (int i = 0; i < 2; i++)
            {
                var polygons = Graph.Polygons.Where(p => p.Nodes.All(n => Math.Abs(n.D-p.Nodes[0].D)<0.5));
                var listIDs = polygons.Select(p => p.PolygonID).ToList();
                foreach (var polygon in listIDs)
                {
                    Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
                }
                AdapteB();
            }
        }

        public void Remesh2C()
        {
            for (int i = 0; i < 10; i++)
            {
                Flip();
                AdapteC();
                //Shift2();
                //SaveAndShow(i);
            }
        }

        public void AdapteC()
        {
            var i = 0;
            var m = Graph.Nodes.Max(n => n.NodeID);
            while (true)
            {
                var edge = Graph.Nodes.OrderBy(n => n.NodeID).FirstOrDefault(e => e.NodeID > i);
                if (edge == null || edge.NodeID > m)
                    break;
                i = edge.NodeID;
                edge.AdaptNodeC();
             }
         }


        public void AdapteB()
        {
            var i = 0;
            var m = Edge.GetID((0));
            while (true)
            {
                var edge = Graph.Edges.Where(e => e.D).FirstOrDefault(e => e.EdgeID > i);
                if (edge == null || edge.EdgeID > m)
                    break;
                i = edge.EdgeID;
                edge.AdaptEdgeB();

            }

        }

        public void Adapte()
        {
            var list = Graph.Edges.OrderBy(e => e.Nodes.Max(n => n.R)).ToList().Select((Value, Index) => new { Value, Index });
            foreach (var pare in list)
            {
                pare.Value.EdgeID = pare.Index + 1;
            }
            var i = 0;
            var j = 0;
            var k = 0;
            var m = Edge.GetID((0));
            while (true)
            {
                var min = Graph.Nodes.Min(n => n.X);
                var max = Graph.Nodes.Max(n => n.X);
                var edge = Graph.Edges.Where(e => e.Nodes.All(n => Math.Abs(n.X -min) < 0.5 || Math.Abs(n.X - max) < 0.5)).FirstOrDefault(e => e.EdgeID > i);
                if (edge == null || edge.EdgeID > m)
                    break;



                i = edge.EdgeID;
                edge.AdaptEdge();
                 
             }
     
        }

        public void Split()
        {
            var i = 0;
            var j = 0;
            var k = 0;
            var m = Edge.GetID((0));
            while (true)
            {
                var edge = Graph.Edges.FirstOrDefault(e => e.EdgeID > i);
                if (edge == null || edge.EdgeID > m)
                    break;
                i = edge.EdgeID;
                
                //if (i == 42 || test.Any(p => p < 2))
                //if (i == 972 || Graph.Polygons.Any(p => p.Edges == null) ||
                //    Graph.Polygons.Any(p => p.Edges.Any(e => e == null)) || Graph.Polygons.Any(p => p.Nodes.Count > 3))
                //{
                //    var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                //    var t = 0;
                //    t = t + 5;
                   
                //}
                //var test = Graph.Edges.Select(p =>
                //    Graph.Polygons.Count(edgeP => edgeP.Nodes.Contains(p.A) && edgeP.Nodes.Contains(p.B)));
                //if (test.Any(p => p < 2) || test.Any(p => p > 2))
                //{
                //    var tu =Graph.Edges.Where(p =>
                //        Graph.Polygons.Count(edgeP => edgeP.Nodes.Contains(p.A) && edgeP.Nodes.Contains(p.B)) > 2).ToList();
                //    var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                //    var t = 0;
                //    t = t + 5;
                //}
                if (edge.Length > (4.0 / 3.0) * 1)
                {
                    j =  edge.EdgeID;
                    edge.Split();
                    //var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                }
            }
        }

        public void Colaps()
        {
            var i = 0;
            var j = 0;
            var k = 0;
            var m = Edge.GetID((0));
            while (true)
            {
                var edge = Graph.Edges.FirstOrDefault(e => e.EdgeID > i);
                if (edge == null || edge.EdgeID > m)
                    break;
                i = edge.EdgeID;

                //if (i == 42 || test.Any(p => p < 2))
                //if (i == 972 || Graph.Polygons.Any(p => p.Edges == null) ||
                //    Graph.Polygons.Any(p => p.Edges.Any(e => e == null)) || Graph.Polygons.Any(p => p.Nodes.Count > 3))
                //{
                //    var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                //    var t = 0;
                //    t = t + 5;

                //}
                //var test = Graph.Edges.Select(p =>
                //    Graph.Polygons.Count(edgeP => edgeP.Nodes.Contains(p.A) && edgeP.Nodes.Contains(p.B)));
                //if (test.Any(p => p < 2) || test.Any(p => p > 2))
                //{
                //    var tu =Graph.Edges.Where(p =>
                //        Graph.Polygons.Count(edgeP => edgeP.Nodes.Contains(p.A) && edgeP.Nodes.Contains(p.B)) > 2).ToList();
                //    var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                //    var t = 0;
                //    t = t + 5;
                //}
                if (edge.Length < (4.0 / 5.0) / 1)
                {
                    k = edge.EdgeID;
                    //var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                    edge.Colaps();
                }
            }
        }


        public void Flip()
        {
            var i = 0;
            var j = 0;
            var k = 0;
            var m = Edge.GetID((0));
            while (true)
            {
                var edge = Graph.Edges.FirstOrDefault(e => e.EdgeID > i);
                if (edge == null || edge.EdgeID > m)
                    break;
                i = edge.EdgeID;

                //if (i == 42 || test.Any(p => p < 2))
                //if (i == 972 || Graph.Polygons.Any(p => p.Edges == null) ||
                //    Graph.Polygons.Any(p => p.Edges.Any(e => e == null)) || Graph.Polygons.Any(p => p.Nodes.Count > 3))
                //{
                //    var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                //    var t = 0;
                //    t = t + 5;

                //}
                //var test = Graph.Edges.Select(p =>
                //    Graph.Polygons.Count(edgeP => edgeP.Nodes.Contains(p.A) && edgeP.Nodes.Contains(p.B)));
                //if (test.Any(p => p < 2) || test.Any(p => p > 2))
                //{
                //    var tu =Graph.Edges.Where(p =>
                //        Graph.Polygons.Count(edgeP => edgeP.Nodes.Contains(p.A) && edgeP.Nodes.Contains(p.B)) > 2).ToList();
                //    var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                //    var t = 0;
                //    t = t + 5;
                //}
                if (edge.Length < (4.0 / 5.0) / 1)
                {
                    k = edge.EdgeID;
                    //var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                    edge.Flip();
                }
            }
        }
        public void Shift()
        {
            foreach (var node in Graph.Nodes)
            {
             node.Shift();   
            }
        }

        public void Shift2()
        {
            foreach (var node in Graph.Nodes)
            {
                node.Shift2();
            }
        }


        void LoadObj()
        {
            List<String> poligonSrtings = new List<string>();
            StreamReader sr = new StreamReader(@"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\test1.obj");
            //StreamReader sr = new StreamReader(@"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\MaleLow.obj");
            while (!sr.EndOfStream)
            {
                var s = sr.ReadLine();
                if (!string.IsNullOrEmpty(s))
                {
                    var lit = s.Split(' ');
                    if (lit[0].Equals("v"))
                    {
                        nodes.Add(new Node(s, nodes.Count + 1));
                    }
                    else if (lit[0].Equals("f"))
                    {
                        poligonSrtings.Add(s);
                    }
                }
            }
            foreach (var str in poligonSrtings)
            {
                var li = str.Split(' ');
                var a = nodes[int.Parse(li[1]) - 1];
                var b = nodes[int.Parse(li[2]) - 1];
                var c = nodes[int.Parse(li[3]) - 1];
                Graph.Polygons.Add(new Polygon(a, b, c));
            }
        }

        void SaveAndShow(int i = -1)
        {
            var max = Graph.Nodes.Max(n => Math.Abs(n.X));

            var s = @"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\test2.obj";
            if (i > -1)
            {
                s = @"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\test2" + i + ".obj";
            }
            using (StreamWriter sw = new StreamWriter(s))
            {
                var swi = string.Empty;
                var list = Graph.Nodes.OrderBy(n => n.NodeID).ToList().Select((Value, Index) => new { Value, Index });
                foreach (var pare in list)
                {
                    pare.Value.NodeID = pare.Index + 1;
                }

                foreach (var node in Graph.Nodes.OrderBy(n => n.NodeID))
                {
                    swi = swi + "v " + (node.X - max  / 2) + " " + node.Y + " " + node.Z + "\n";
                }
                foreach (var polygonForWrit in Graph.Polygons)
                {
                    var nodesOfP = polygonForWrit.Nodes;

                    if (nodesOfP.Count > 2)
                    {
                        swi = swi + "f " + nodesOfP[0].NodeID + " " + nodesOfP[1].NodeID + " " + nodesOfP[2].NodeID +
                              "\n";
                    }
                }
                sw.Write(swi);
            }




            ObjReader CurrentHelixObjReader = new ObjReader();
            // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"D:\3DModel\dinosaur_FBX\dinosaur.fbx");
            Model3DGroup MyModel = CurrentHelixObjReader.Read(@"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\test2.obj");

            model.Content = MyModel;
            //MyModel.Children.Add(MyModel);
        }

        void genObj()
        {
            var p = 20;
            var l = 8;
            var x = 0;
            var step = 1.0;
            var rStep = 0.0;
            var d = new List<int>();
            d.Add(2);
            d.Add(1);

            var di = 2;
            for (var j = 0; j < p; j++)
            {
                double r;
                if (j == 0)
                {
                    r = (j + rStep) * 2 * Math.PI / p;
                    nodes.Add(new Node { NodeID = nodes.Count + 1, X = x + 0, Y = d[0] * Math.Sin(r), Z = d[0] * Math.Cos(r), i = 0, j = j });
                    nodes.Add(new Node { NodeID = nodes.Count + 1, X = x + 0, Y = d[1] * Math.Sin(r), Z = d[1] * Math.Cos(r), i = 0, j = j });
                }
                var count = nodes.Count;
                var A = nodes[count - 2];
                var B = nodes[count - 1];
                var C = new Node();
                var D = new Node();
                if (j < p - 1)
                {
                    r = ((j + 1 + rStep) + rand) * 2 * Math.PI / p;
                    nodes.Add(new Node { NodeID = nodes.Count + 1, X = x + 0, Y = d[0] * Math.Sin(r), Z = d[0] * Math.Cos(r), i = 0, j = j });
                    r = ((j + 1 + rStep) + rand) * 2 * Math.PI / p;
                    nodes.Add(new Node { NodeID = nodes.Count + 1, X = x + 0, Y = d[1] * Math.Sin(r), Z = d[1] * Math.Cos(r), i = 0, j = j });
                    count = nodes.Count;
                    C = nodes[count - 2];
                    D = nodes[count - 1];
                }
                else
                {
                    C = nodes[0];
                    D = nodes[1];
                }
                polygon = new Polygon(A, B, C);
                Graph.Polygons.Add(polygon);
                polygon = new Polygon(B, C, D);
                Graph.Polygons.Add(polygon);
            }
            ////////////////////////////////////////////////////////////////////////////
            rStep = 0.5;

            for (var j = 0; j < p; j++)
            {
                double r;
                var count = nodes.Count;
                var A = new Node();
                var B = new Node();
                var C = new Node();
                var D = new Node();
                if (j < p - 1)
                {
                    A = nodes.Find(n => n.NodeID == count - 2 * p + 1);
                    B = nodes.Find(n => n.NodeID == count - 2 * p + 2);
                    C = nodes.Find(n => n.NodeID == count - 2 * p + 3);
                    D = nodes.Find(n => n.NodeID == count - 2 * p + 4);
                }
                else
                {
                    A = nodes.Find(n => n.NodeID == count - 2 * p + 1);
                    B = nodes.Find(n => n.NodeID == count - 2 * p + 2);
                    C = nodes.Find(n => n.NodeID == count - 4 * p + 3);
                    D = nodes.Find(n => n.NodeID == count - 4 * p + 4);
                }
                r = (j + rStep + rand) * 2 * Math.PI / p;
                nodes.Add(new Node { NodeID = nodes.Count + 1, X = x + 0.5 + rand, Y = d[0] * Math.Sin(r), Z = d[0] * Math.Cos(r), i = 1, j = j });
                r = (j + rStep + rand) * 2 * Math.PI / p;
                nodes.Add(new Node { NodeID = nodes.Count + 1, X = x + 0.5 + rand, Y = d[1] * Math.Sin(r), Z = d[1] * Math.Cos(r), i = 1, j = j });
                count = nodes.Count;
                var E = nodes[count - 2];
                var F = nodes[count - 1];
                polygon = new Polygon(E, A, C);
                Graph.Polygons.Add(polygon);
                polygon = new Polygon(F, B, D);
                Graph.Polygons.Add(polygon);
            }

            for (int i = 2; i <= l + 1; i++)
            {
                rStep = i % 2 * 0.5;
                for (int j = 0; j < p; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        double r;
                        r = (j + rStep + rand) * 2 * Math.PI / p;
                        nodes.Add(new Node
                        {
                            NodeID = nodes.Count + 1,
                            X = x + i - 0.5 + rand,
                            Y = d[k] * Math.Sin(r),
                            Z = d[k] * Math.Cos(r),
                            i = i,
                            j = j
                        });
                        var count = nodes.Count();
                        var N = nodes[count - 1];
                        var A = nodes.Find(n => n.NodeID == count - 4 * p);
                        var edges = Graph.Edges.Where(edge => edge.Nodes.Contains(A) && edge.Nodes.Any(n => n.i > A.i));
                        var B = edges.FirstOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID);
                        var C = edges.LastOrDefault().Nodes.FirstOrDefault(n => n.NodeID != A.NodeID);
                        polygon = new Polygon(N, A, B);
                        Graph.Polygons.Add(polygon);
                        polygon = new Polygon(N, A, C);
                        Graph.Polygons.Add(polygon);
                    }
                }
            }

            var temp = new List<Node>();
            var tempN = new List<Node>();
            var tempK = new List<Node>();
            rStep = l % 2 * 0.5;
            for (int j = 0; j < p; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    var N = new Node();
                    double r;
                    r = (j + rStep + rand) * 2 * Math.PI / p;
                    nodes.Add(new Node
                    {
                        NodeID = nodes.Count + 1,
                        X = x + l + 2 - 0.5,
                        Y = d[k] * Math.Sin(r),
                        Z = d[k] * Math.Cos(r),
                        i = l + 2
                    });
                    var count = nodes.Count();
                    N = nodes[count - 1];
                    var A = nodes.Find(n => n.NodeID == count - 4 * p);
                    var edges = Graph.Edges.Where(edge => edge.Nodes.Contains(A) && edge.Nodes.Any(n => n.i > A.i));
                    var tempNodes = edges.Select(edge => edge.Nodes.FirstOrDefault(n => n.NodeID != A.NodeID)).ToList().OrderBy(n => n.j);
                    var B = tempNodes.FirstOrDefault();//.Nodes.FirstOrDefault(n => n.NodeID != A.NodeID);
                    var C = tempNodes.LastOrDefault();//.Nodes.FirstOrDefault(n => n.NodeID != A.NodeID);
                    polygon = new Polygon(N, A, B);
                    Graph.Polygons.Add(polygon);
                    polygon = new Polygon(N, A, C);
                    Graph.Polygons.Add(polygon);
                    if (temp.Count >= 2)
                    {
                        polygon = new Polygon(N, tempK[k], temp[k]);
                        Graph.Polygons.Add(polygon);
                        polygon = new Polygon(N, temp[0], temp[1]);
                        Graph.Polygons.Add(polygon);
                        temp[k] = N;
                        tempK[k] = C;
                    }
                    else
                    {
                        temp.Add(N);
                        tempN.Add(N);
                        tempK.Add(B);
                    }
                }
            }
            polygon = new Polygon(tempN[1], tempK[1], temp[1]);
            Graph.Polygons.Add(polygon);
            polygon = new Polygon(tempN[0], tempK[0], temp[0]);
            Graph.Polygons.Add(polygon);
            polygon = new Polygon(temp[0], temp[1], tempN[0]);
            Graph.Polygons.Add(polygon);
            polygon = new Polygon(temp[1], tempN[1], tempN[0]);
            Graph.Polygons.Add(polygon);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
