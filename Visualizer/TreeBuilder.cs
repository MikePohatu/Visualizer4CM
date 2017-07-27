using System.Collections.Generic;
using System.Linq;
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
                if (col == null)
                {
                    //log and continue
                    continue;
                }
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
                        if (limcol == null)
                        {
                            //log and continue
                            continue;
                        }
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
                if (colrel == null) { continue; }
                if (graph.FindNode(colrel.DependentCollectionID) == null)
                {
                    SccmCollection col = library.GetCollection(colrel.DependentCollectionID);
                    if (col == null) { continue; }
                    graph.AddNode(new CollectionNode(col.ID, col));
                }

                if (graph.FindNode(colrel.SourceCollectionID) == null)
                {
                    SccmCollection col = library.GetCollection(colrel.SourceCollectionID);
                    if (col == null) { continue; }
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

        public static Graph BuildApplicationTree(SccmConnector connector, SccmApplication application)
        {
            Graph graph = new Graph();

            //build the graph
            graph.AddNode(new SccmNode(application.ID, application));
            BuildApplicationMeshLinks(connector, graph, application);
            return graph;
        }

        private static void BuildApplicationMeshLinks(SccmConnector connector, Graph graph, SccmApplication application)
        {
            
            List<SccmApplicationRelationship> relationships = connector.GetApplicationRelationships(application.ID);
            Dictionary<string, string> relatedapplicationids = new Dictionary<string, string>();

            foreach (SccmApplicationRelationship relationship in relationships)
            {
                string outid;
                if (relatedapplicationids.TryGetValue(relationship.FromApplicationCIID,out outid) == false)
                {
                    relatedapplicationids.Add(relationship.FromApplicationCIID,null);
                }

                if (relatedapplicationids.TryGetValue(relationship.ToApplicationCIID, out outid) == false)
                {
                    relatedapplicationids.Add(relationship.ToApplicationCIID, null);
                }
            }
            
            Dictionary<string, SccmApplication> applications = connector.GetApplicationDictionaryFromIDs(relatedapplicationids.Keys.ToList());

            foreach (SccmApplicationRelationship rel in relationships)
            {
                if (graph.FindNode(rel.FromApplicationCIID) == null)
                {
                    SccmApplication fromapp;
                    if (applications.TryGetValue(rel.FromApplicationCIID,out fromapp))
                    {
                        if (fromapp.IsLatest == false) { continue; }
                        graph.AddNode(new SccmNode(fromapp.ID, fromapp));
                    }
                    else { continue; }
                }

                if (graph.FindNode(rel.ToApplicationCIID) == null)
                {
                    SccmApplication toapp;
                    if (applications.TryGetValue(rel.ToApplicationCIID, out toapp))
                    {
                        if (toapp.IsLatest == false) { continue; }
                        graph.AddNode(new SccmNode(toapp.ID, toapp));
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

        public static Graph BuildCollectionDeploymentsTree(SccmConnector connector, SccmCollection collection, List<IDeployment> deployments)
        {
            Graph graph = new Graph();

            //build the graph
            graph.AddNode(new SccmNode(collection.ID, collection));
            BuildCollectionDeploymentLinks(connector, graph, collection.ID, deployments);
            collection.IsHighlighted = true;
            return graph;
        }

        private static void BuildCollectionDeploymentLinks(SccmConnector connector, Graph graph, string rootcollectionid, List<IDeployment> deployments)
        {
            foreach (SMS_DeploymentSummary deployment in deployments)
            {
                if (graph.FindNode(deployment.SoftwareName) == null)
                {
                    SccmDeployableItem item = Factory.GetSccmDeployableItemFromDeploymentSummary(deployment);
                    if (item != null)
                    {
                        graph.AddNode(new SccmNode(deployment.SoftwareName, item));
                        Edge newedge = graph.AddEdge(deployment.SoftwareName, rootcollectionid);
                    }
                }
            }
        }

        public static Graph BuildCIDeploymentsTree(SccmConnector connector, ISccmObject ci, List<IDeployment> deployments)
        {
            Graph graph = new Graph();

            //build the graph
            graph.AddNode(new SccmNode(ci.ID, ci));
            foreach (IDeployment deployment in deployments)
            {
                if (graph.FindNode(deployment.CollectionID) == null)
                {
                    graph.AddNode(new CollectionNode(deployment.CollectionID, new SccmCollection(deployment.CollectionID, deployment.CollectionName, string.Empty)));
                    Edge newedge = graph.AddEdge(ci.ID, deployment.CollectionID);
                }
            }
            ci.IsHighlighted = true;
            return graph;
        }

        public static Graph BuildPackageDeploymentsTree(SccmConnector connector, SccmPackage package, List<IDeployment> deployments)
        {
            Graph graph = new Graph();

            //build the graph
            graph.AddNode(new SccmNode(package.ID, package));

            foreach (SccmPackageProgram program in package.Programs)
            {
                string targetid = program.ProgramName;
                if (graph.FindNode(targetid) == null)
                {
                    SccmNode prognode = new SccmNode(targetid, program);
                    graph.AddNode(prognode);
                    Edge newedge = graph.AddEdge(package.ID, targetid);
                }
            }


            foreach (IDeployment deployment in deployments)
            {
                SMS_DeploymentInfo depinfo = (SMS_DeploymentInfo)deployment;
                if (graph.FindNode(deployment.CollectionID) == null)
                {
                    graph.AddNode(new CollectionNode(deployment.CollectionID, new SccmCollection(deployment.CollectionID, deployment.CollectionName, string.Empty)));
                }
                string targetid = depinfo.TargetSubName;
                Edge newedge = graph.AddEdge(targetid, deployment.CollectionID);
            }
            package.IsHighlighted = true;
            return graph;
        }


        public static Graph BuildPackagesTree(PackageLibrary library, SccmPackage rootpackage)
        {
            Graph graph = new Graph();

            //build the graph
            graph.AddNode(new SccmNode(rootpackage.ID, rootpackage));

            foreach (SccmPackageProgram program in rootpackage.Programs)
            {
                if (graph.FindNode(program.ID) == null)
                {
                    SccmNode newnode = new SccmNode(program.ID,program);
                    graph.AddNode(newnode);  
                }

                Edge newedge = graph.AddEdge(rootpackage.ID, program.ID);
                if (program.DependentSccmPackageProgram != null) { BuildPackageProgramTree(graph,library, program, program.DependentSccmPackageProgram); }
            }
            rootpackage.IsHighlighted = true;
            return graph;
        }

        private static void BuildPackageProgramTree(Graph graph, PackageLibrary library, SccmPackageProgram program, SccmPackageProgram dependentprogram)
        {
            //Add node for the program
            if (graph.FindNode(dependentprogram.ID) == null)
            {
                graph.AddNode(new SccmNode(dependentprogram.ID, dependentprogram)); 
            }
            graph.AddEdge(program.ID, dependentprogram.ID);

            //Add node for the parent package
            if (graph.FindNode(dependentprogram.ParentPackage.ID) == null)
            {
                graph.AddNode(new SccmNode(dependentprogram.ParentPackage.ID, dependentprogram.ParentPackage));
                graph.AddEdge(dependentprogram.ParentPackage.ID, dependentprogram.ID);
            }

            if (dependentprogram.DependentSccmPackageProgram != null) { BuildPackageProgramTree(graph, library, dependentprogram, dependentprogram.DependentSccmPackageProgram); }
        }
    }
}
