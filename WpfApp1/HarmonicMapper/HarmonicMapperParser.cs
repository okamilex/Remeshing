using System;
using System.Windows;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace HarmonicMapper
{
    public class HarmonicMapperParser
    {
        public string FilePath = @"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\obj.m";
        public string MFilePath;

        List<Node> nodes = new List<Node>();

        public void SaveAndShow(int i = -1)
        {
            var max = Graph.Nodes.Max(n => Math.Abs(n.X));

            var s = FilePath;
            if (i > -1)
            {
                var s_ext = System.IO.Path.GetFileNameWithoutExtension(s);
                s = s_ext + i + ".m";
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
                    var twoD = "";
                    if (node.TwoDX != null && node.TwoDY != null)
                        twoD = " uv=(" + node.TwoDX + " " + node.TwoDY + ")";
                    swi = swi + "Vertex " + node.NodeID + " " + node.X + " " + node.Y + " " + node.Z + " {rgb=(" + node.RGBString + ")" + twoD + "}" + "\n";
                }
                foreach (var polygonForWrit in Graph.Polygons)
                {
                    var nodesOfP = polygonForWrit.Nodes;

                    if (nodesOfP.Count > 2)
                    {
                        swi = swi + "Face " + polygonForWrit.PolygonID + " " + nodesOfP[0].NodeID + " " + nodesOfP[1].NodeID + " " + nodesOfP[2].NodeID +
                              "\n";
                    }
                }
                sw.Write(swi);
            }
        }

        public void LoadObj()
        {
            var swi = string.Empty;
            List<String> poligonSrtings = new List<string>();
            MFilePath = "C:\\Users\\alex_\\Desktop\\ДИПЛОМ\\HarmonicMapper\\demo\\Alex\\Alex.m";
            StreamReader sr = new StreamReader(MFilePath);
            //StreamReader sr = new StreamReader(@"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\MaleLow.obj");
            while (!sr.EndOfStream)
            {
                var s = sr.ReadLine();
                if (!string.IsNullOrEmpty(s) && s[0] != '#')
                {
                    var lit = s.Split(' ');
                    if (lit[0].Equals("Vertex"))
                    {
                        var l = s.Split('{');
                        nodes.Add(new Node(l));
                    }
                    else if (lit[0].Equals("Face"))
                    {
                        poligonSrtings.Add(s);
                    }
                }
            }
            nodes = nodes.OrderBy(n => n.NodeID).ToList();
            var min = nodes.Min(n => n.NodeID);
            var max = nodes.Max(n => n.NodeID);
            var t = new List<int>();
            for (int i = 0; i <= max; i++)
            {
                t.Add(0);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                t[node.NodeID] = i + 1;
                swi = swi + "v " + node.X + " " + node.Y + " " + node.Z + "\n";
            }
            foreach (var str in poligonSrtings)
            {
                var li = str.Split(' ');
                //var a = nodes.FirstOrDefault(n => n.NodeID == int.Parse(li[3]));
                //var b = nodes.FirstOrDefault(n => n.NodeID == int.Parse(li[4]));
                //var c = nodes.FirstOrDefault(n => n.NodeID == int.Parse(li[5]));

                //var a = nodes[t[int.Parse(li[3])]];
                //var b = nodes[t[int.Parse(li[4])]];
                //var c = nodes[t[int.Parse(li[5])]];

                //Graph.Polygons.Add(new Model.Polygon(a, b, c));
                swi = swi + "f " + t[int.Parse(li[3])] + " " + t[int.Parse(li[4])] + " " + t[int.Parse(li[5])] +
                      "\n";
            }

            using (StreamWriter sw = new StreamWriter(@"D:\alex2.obj"))
            {
                sw.Write(swi);
            }
        }
    }
}
