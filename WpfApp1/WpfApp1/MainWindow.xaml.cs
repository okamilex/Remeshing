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
using Model;
using AdaptiveRemeshing;
using HarmonicMapper;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random Rand = new Random();
        List<Node> nodes = new List<Node>();
        public Model.Polygon polygon;

        public string LoadPath = @"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\test1.obj";
        public string SavePath;
        public string ShowPath;

        public double rand
        {
            get
            {
                return Convert.ToDouble(Rand.Next(99)) / 100;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var s = System.IO.Path.GetFileNameWithoutExtension(ShowPath);
            Algo.Name = s;
            Algo.Remesh1();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            //Remesh2();
            //Remesh2B();
            //Algo.Remesh2C();

            var parser = new HarmonicMapperParser();
            parser.LoadObj();
            parser.SaveAndShow();
        }

        private void Gen_Click(object sender, RoutedEventArgs e)
        {
            Graph.Polygons = new List<Model.Polygon>();
            nodes = new List<Node>();

            genObj();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog() { Filter = "3D objects |*.obj" };
            var result = sfd.ShowDialog();
            if (result == false) return;
            SavePath = sfd.FileName;
            SaveAndShow();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "3D objects |*.obj" };
            var result = ofd.ShowDialog();
            if (result == false) return;
            LoadPath = ofd.FileName;

            Graph.Polygons = new List<Model.Polygon>();
            nodes = new List<Node>();

            LoadObj();
        }

        void LoadObj()
        {
            List<String> poligonSrtings = new List<string>();
            StreamReader sr = new StreamReader(LoadPath);
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
                Graph.Polygons.Add(new Model.Polygon(a, b, c));
            }
            ShowPath = LoadPath;
            Show();
        }

        void SaveAndShow(int i = -1, string a = "")
        {
            var maxX = Graph.Nodes.Max(n => n.X);
            var minX = Graph.Nodes.Min(n => n.X);

            var maxY = Graph.Nodes.Max(n => n.Y);
            var minY = Graph.Nodes.Min(n => n.Y);

            var maxZ = Graph.Nodes.Max(n => n.Z);
            var minZ = Graph.Nodes.Min(n => n.Z);

            var s = SavePath;
            if (i > -1)
            {
                var s_ext = System.IO.Path.GetFileNameWithoutExtension(s);
                s = s_ext + i + a + ".obj";
            }
            using (StreamWriter sw = new StreamWriter(s))
            {
                var swi = string.Empty;
                var list = Graph.Nodes.OrderBy(n => n.NodeID).ToList().Select((Value, Index) => new { Value, Index });
                foreach (var pare in list)
                {
                    pare.Value.NodeID = pare.Index + 1;
                }

                var xc = minX + (maxX - minX) / 2;
                var yc = minY + (maxY - minY) / 2;
                var zc = minZ + (maxZ - minZ) / 2;
                foreach (var node in Graph.Nodes.OrderBy(n => n.NodeID))
                {
                    swi = swi + "v " + (node.X - xc) + " " + (node.Y - yc) + " " + (node.Z - zc) + "\n";
                }
                foreach (var polygonForWrit in Graph.Polygons)
                {
                    var nodesOfP = polygonForWrit.Nodes.OrderBy(n => n.NodeID).ToList();

                    if (nodesOfP.Count > 2)
                    {
                        swi = swi + "f " + nodesOfP[0].NodeID + " " + nodesOfP[1].NodeID + " " + nodesOfP[2].NodeID +
                              "\n";
                    }
                }
                sw.Write(swi);
            }


            ShowPath = s;
            Show();
        }

        void Show()
        {
            var s = ShowPath;
            ObjReader CurrentHelixObjReader = new ObjReader();
            // Model3DGroup MyModel = CurrentHelixObjReader.Read(@"D:\3DModel\dinosaur_FBX\dinosaur.fbx");
            Model3DGroup MyModel = CurrentHelixObjReader.Read(s);

            model.Content = MyModel;
            //MyModel.Children.Add(MyModel);

            List<double> crossings = new List<double>();
            foreach (var polygon in Graph.Polygons)
            {
                crossings.Add(polygon.GetCross(polygon.Edges[0]));
                crossings.Add(polygon.GetCross(polygon.Edges[1]));
                crossings.Add(polygon.GetCross(polygon.Edges[2]));
            }
            var avr = crossings.Average();
            Avr.Content = avr;
            var dis = crossings.Sum(x => (x - avr) * (x - avr)) / crossings.Count;
            Dis.Content = dis;
        
            var l = Graph.Edges.Average(e => e.Length);
            var ld = Graph.Edges.Average(e =>
            {
                var p = e.Length * 100 / 0.2;
                return Math.Abs(p - 100);
            });
            Avr_Copy.Content = l;
            Dis_Copy.Content = ld;
            var v = Graph.Nodes.Count(n => n.Valence != 6) / (Graph.Nodes.Count * 1.0);
            Avr_Cop2y.Content = v;
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
                polygon = new Model.Polygon(A, B, C);
                Graph.Polygons.Add(polygon);
                polygon = new Model.Polygon(B, C, D);
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
                polygon = new Model.Polygon(E, A, C);
                Graph.Polygons.Add(polygon);
                polygon = new Model.Polygon(F, B, D);
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
                        polygon = new Model.Polygon(N, A, B);
                        Graph.Polygons.Add(polygon);
                        polygon = new Model.Polygon(N, A, C);
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
                    polygon = new Model.Polygon(N, A, B);
                    Graph.Polygons.Add(polygon);
                    polygon = new Model.Polygon(N, A, C);
                    Graph.Polygons.Add(polygon);
                    if (temp.Count >= 2)
                    {
                        polygon = new Model.Polygon(N, tempK[k], temp[k]);
                        Graph.Polygons.Add(polygon);
                        polygon = new Model.Polygon(N, temp[0], temp[1]);
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
            polygon = new Model.Polygon(tempN[1], tempK[1], temp[1]);
            Graph.Polygons.Add(polygon);
            polygon = new Model.Polygon(tempN[0], tempK[0], temp[0]);
            Graph.Polygons.Add(polygon);
            polygon = new Model.Polygon(temp[0], temp[1], tempN[0]);
            Graph.Polygons.Add(polygon);
            polygon = new Model.Polygon(temp[1], tempN[1], tempN[0]);
            Graph.Polygons.Add(polygon);
        }
    }
}
