using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public static class TreeBuilder
    {
        public static Graph BuildCollectionLimitingPath(List<SccmCollection> collections, CollectionLibrary library)
        {
            Graph graph = new Graph();
            foreach (SccmCollection col in collections)
            {
                Node colnode = graph.FindNode(col.ID);
                if (colnode == null)
                {
                    colnode = new CollectionNode(col.ID, col);
                    graph.AddNode(colnode);
                }

                if (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false)
                {
                    Node limitingnode = graph.FindNode(col.LimitingCollectionID);
                    if (limitingnode == null)
                    {
                        SccmCollection limcol = library.GetCollection(col.LimitingCollectionID);
                        limitingnode = new CollectionNode(limcol.ID, limcol);
                        graph.AddNode(limitingnode);
                    }

                    graph.AddEdge(col.ID, col.LimitingCollectionID);
                }
            }
            return graph;
        }

        public static List<SccmCollection> HighlightCollectionMembers(Graph graph, List<string> collectionids)
        {
            if (graph == null) { return null; }
            List<SccmCollection> highlightedcollections = new List<SccmCollection>();

            foreach (string colid in collectionids)
            {
                CollectionNode node = graph?.FindNode(colid) as CollectionNode;
                if (node != null)
                {
                    highlightedcollections.Add(node.Collection);
                    node.Collection.IsMemberPresent = true;
                }
            }

            return highlightedcollections;
        }

        /// <summary>
        /// Build a mesh mode tree on the graph specified, starting from the specified collection ID
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="graph"></param>
        /// <param name="library"></param>
        /// <param name="collectionid"></param>
        /// <returns></returns>
        public static Graph BuildCollectionTreeMeshMode(SccmConnector connector, CollectionLibrary library, string collectionid)
        {
            SccmCollection searchcol;
            Graph graph = new Graph();

            //build the graph
            searchcol = library.GetCollection(collectionid);
            if (searchcol != null)
            {
                graph.AddNode(new CollectionNode(searchcol.ID, searchcol));
                BuildCollectionMeshLinks(connector,graph, library, searchcol);
            }
            return graph;
        }

        private static void BuildCollectionMeshLinks(SccmConnector connector, Graph graph, CollectionLibrary library, SccmCollection collection)
        {
            List<SccmCollectionRelationship> relationships = connector.GetCollectionDependencies(collection.ID);

            foreach (SccmCollectionRelationship colrel in relationships)
            {
                if (graph.FindNode(colrel.DependentCollectionID) == null)
                {
                    SccmCollection col = library.GetCollection(colrel.DependentCollectionID);
                    graph.AddNode(new CollectionNode(col.ID, col));
                }

                if (graph.FindNode(colrel.SourceCollectionID) == null)
                {
                    SccmCollection col = library.GetCollection(colrel.SourceCollectionID);
                    graph.AddNode(new CollectionNode(col.ID, col));
                    BuildCollectionMeshLinks(connector,graph, library, col); //recursive build
                }

                Edge newedge = graph.AddEdge(colrel.DependentCollectionID, colrel.Type.ToString(), colrel.SourceCollectionID);

                if (colrel.Type == SccmCollectionRelationship.RelationShipType.Exclude)
                { newedge.Attr.Color = Color.Red; }
                else if (colrel.Type == SccmCollectionRelationship.RelationShipType.Include)
                { newedge.Attr.Color = Color.Blue; }
                else if (colrel.Type == SccmCollectionRelationship.RelationShipType.Limiting)
                { newedge.Attr.Color = Color.Black; }
            }
        }

        public static Graph BuildCollectionTreeLimitingMode(CollectionLibrary library, string collectionid)
        {
            Graph graph = null;

            //build the graph
            SccmCollection searchcol = library.GetCollection(collectionid);
            if (searchcol != null)
            {
                graph = BuildCollectionLimitingPath(searchcol.GetCollectionPathList(), library);
                searchcol.IsHighlighted = true;
            }

            return graph;
        }

        public static Graph BuildCollectionTreeAllCollections(CollectionLibrary library)
        {
            Graph graph = null;

            //build the graph
            graph = BuildCollectionLimitingPath(library.GetAllCollections(), library);

            return graph;
        }

        public static Graph BuildApplicationTree(SccmConnector connector, Dictionary<string,SccmApplication> applications, SccmApplication application)
        {
            Graph graph = new Graph();

            //build the graph
            graph.AddNode(new ApplicationNode(application.CIID, application));
            BuildApplicationMeshLinks(connector, graph, applications, application);
            return graph;
        }

        private static void BuildApplicationMeshLinks(SccmConnector connector, Graph graph, Dictionary<string,SccmApplication> applications, SccmApplication application)
        {
            List<SccmApplicationRelationship> relationships = connector.GetApplicationRelationships(application.CIID);

            foreach (SccmApplicationRelationship rel in relationships)
            {
                if (graph.FindNode(rel.FromApplicationCIID) == null)
                {
                    SccmApplication fromapp;
                    if (applications.TryGetValue(rel.FromApplicationCIID,out fromapp))
                    {
                        if (fromapp.IsLatest == false) { continue; }
                        graph.AddNode(new ApplicationNode(fromapp.CIID, fromapp));
                    }
                    else { continue; }
                }

                if (graph.FindNode(rel.ToApplicationCIID) == null)
                {
                    SccmApplication toapp;
                    if (applications.TryGetValue(rel.ToApplicationCIID, out toapp))
                    {
                        if (toapp.IsLatest == false) { continue; }
                        graph.AddNode(new ApplicationNode(toapp.CIID, toapp));
                    }
                    else { continue; }
                }

                Edge newedge = graph.AddEdge(rel.FromApplicationCIID, rel.Type.ToString(), rel.ToApplicationCIID);

                if (rel.Type == SccmApplicationRelationship.RelationShipType.Install)
                { newedge.Attr.Color = Color.Blue; }
                else
                { newedge.Attr.Color = Color.Black; }

            }
        }
    }
}
