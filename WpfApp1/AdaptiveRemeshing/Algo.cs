﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace AdaptiveRemeshing
{
    public static class Algo
    {
        public static void Remesh1()
        {
            for (int i = 0; i < 5; i++)
            {
                Split();
                Colaps();
                Flip();
                Shift();
            }
        }

        public static void Remesh2()
        {
            for (int i = 0; i < 5; i++)
            {
                Adapte();
            }
        }

        public static void Remesh2B()
        {
            for (int i = 0; i < 2; i++)
            {
                var polygons = Graph.Polygons.Where(p => p.Nodes.All(n => Math.Abs(n.D - p.Nodes[0].D) < 0.5));
                var listIDs = polygons.Select(p => p.PolygonID).ToList();
                foreach (var polygon in listIDs)
                {
                    Graph.Polygons.RemoveAll(p => p.PolygonID == polygon);
                }
                AdapteB();
            }
        }

        public static void Remesh2C()
        {
            for (int i = 0; i < 10; i++)
            {
                Flip();
                AdapteC();
                //Shift2();
                //SaveAndShow(i);
            }
        }

        public static void AdapteC()
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


        public static void AdapteB()
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

        public static void Adapte()
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
                var edge = Graph.Edges.Where(e => e.Nodes.All(n => Math.Abs(n.X - min) < 0.5 || Math.Abs(n.X - max) < 0.5)).FirstOrDefault(e => e.EdgeID > i);
                if (edge == null || edge.EdgeID > m)
                    break;



                i = edge.EdgeID;
                edge.AdaptEdge();

            }

        }

        public static void Split()
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
                    j = edge.EdgeID;
                    edge.Split();
                    //var ty = Graph.Polygons.Where(p => p.Nodes.Any(n => n.NodeID == 3));
                }
            }
        }

        public static void Colaps()
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


        public static void Flip()
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
        public static void Shift()
        {
            foreach (var node in Graph.Nodes)
            {
                node.Shift();
            }
        }

        public static void Shift2()
        {
            foreach (var node in Graph.Nodes)
            {
                node.Shift2();
            }
        }
    }
}