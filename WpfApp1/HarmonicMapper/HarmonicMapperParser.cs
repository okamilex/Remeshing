using System;
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

        void SaveAndShow(int i = -1)
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
                    swi = swi + "Vertex " + node.NodeID + " " + node.X + " " + node.Y + " " + node.Z + " {rgb=(" + node.RGB + ")" + twoD + "}" + "\n";
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

        void LoadObj()
        {
            List<String> poligonSrtings = new List<string>();
            StreamReader sr = new StreamReader(MFilePath);
            //StreamReader sr = new StreamReader(@"C:\Users\alex_\source\repos\WpfApp1\WpfApp1\MaleLow.obj");
            while (!sr.EndOfStream)
            {
                var s = sr.ReadLine();
                if (!string.IsNullOrEmpty(s))
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
            foreach (var str in poligonSrtings)
            {
                var li = str.Split(' ');
                var a = nodes[int.Parse(li[1]) - 1];
                var b = nodes[int.Parse(li[2]) - 1];
                var c = nodes[int.Parse(li[3]) - 1];
                Graph.Polygons.Add(new Model.Polygon(a, b, c));
            }
        }
    }
}
