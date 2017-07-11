using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace CollectionViewer
{
    public static class TreeBuilder
    {
        public static Graph BuildLimitingPath(List<SccmCollection> collections, CollectionLibrary library)
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
                        limitingnode = new CollectionNode(col.ID, col);
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
        public static Graph BuildTreeMeshMode(SccmConnector connector, CollectionLibrary library, string collectionid)
        {
            SccmCollection searchcol;
            Graph graph = new Graph();

            //build the graph
            searchcol = library.GetCollection(collectionid);
            if (searchcol != null)
            {
                graph.AddNode(new CollectionNode(searchcol.ID, searchcol));
                BuildMeshLinks(connector,graph, library, searchcol);
            }
            return graph;
        }

        private static void BuildMeshLinks(SccmConnector connector, Graph graph, CollectionLibrary library, SccmCollection collection)
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
                    BuildMeshLinks(connector,graph, library, col); //recursive build
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

        public static Graph BuildTreeLimitingMode(CollectionLibrary library, string collectionid)
        {
            Graph graph = null;

            //build the graph
            SccmCollection searchcol = library.GetCollection(collectionid);
            if (searchcol != null)
            {
                graph = BuildLimitingPath(searchcol.GetCollectionPathList(), library);
                searchcol.IsHighlighted = true;
            }

            return graph;
        }

        public static Graph BuildTreeAllCollections(CollectionLibrary library)
        {
            Graph graph = null;

            //build the graph
            graph = BuildLimitingPath(library.GetAllCollections(), library);

            return graph;
        }

        public static void ClearHighlightedCollections(List<SccmCollection> highlightedcollections)
        {
            foreach (SccmCollection col in highlightedcollections)
            {
                col.IsHighlighted = false;
            }
            highlightedcollections.Clear();
        }
    }
}
