using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using MeetingOrganiserDesktopApp.Model;
using System.Linq;

namespace UnitTests.MeetingOrganiserDesktopApp
{
    [TestClass]
    public class GenericGraphTest
    {
        #region Test constructor
        [TestMethod]
        public void Initialise_WithNoArgument()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.Nodes.Count, 0);
            Assert.AreEqual(graph.NumberOfEdges, 0);
            Assert.AreEqual(graph.NumberOfNodes, 0);
        }

        [TestMethod]
        public void Initialise_WithPositiveCapacity()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>(20);

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.Nodes.Count, 0);
            Assert.AreEqual(graph.NumberOfEdges, 0);
            Assert.AreEqual(graph.NumberOfNodes, 0);
        }

        [TestMethod]
        public void Initialise_WithNegativeCapacity()
        {
            try
            {
                GenericGraph<Node> graph = new GenericGraph<Node>(-10);
            }
            catch(ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("negative"));
            }
        }
        #endregion

        #region Test adding a node
        public void AddNode_withPositiveId_notContaining()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int originalNumberOfNodes = graph.NumberOfNodes;

            int nodeId = 1;
            graph.AddNode(new Node(nodeId));

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.Nodes.Count, originalNumberOfNodes+1);
            Assert.AreEqual(graph.NumberOfEdges, 0);
            Assert.AreEqual(graph.NumberOfNodes, originalNumberOfNodes + 1);
            Assert.IsTrue(graph.Nodes.Where(n => n.Id == nodeId).Count() == 1);
        }
        public void AddNode_withPositiveId_containing()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int nodeId = 8;
            graph.AddNode(new Node(nodeId));

            Assert.ThrowsException<Graph.NodeIdAlreadyPresentException>(
               new Action(() =>
               graph.AddNode( new Node(nodeId))));
             
        }
        public void AddNode_withNonPositiveId()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int nodeId = -8;

            Assert.ThrowsException<Graph.InvalidNodeIdException>(
               new Action(() =>
               graph.AddNode( new Node(nodeId))));
        }
        #endregion

        #region Test deleting a node
        [TestMethod]
        public void DeleteNode_WithExistingId()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int delNodeId = 1;
            int neighbourId = delNodeId + delNodeId;
            graph.AddNode(new Node(delNodeId));
            graph.AddNode( new Node(neighbourId));
            graph.AddEdge(delNodeId, neighbourId);
            
            int originalNumberOfEdges = graph.NumberOfEdges;
            int originalNumberOfNodes = graph.NumberOfNodes;
            HashSet<Node> originalNodes = graph.Nodes;

            graph.DeleteNode(delNodeId);

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.Nodes, originalNodes);
            Assert.AreEqual(graph.NumberOfEdges, originalNumberOfEdges - 1);
            Assert.AreEqual(graph.NumberOfNodes, originalNumberOfNodes - 1);
            Assert.IsTrue(graph.Nodes.Count(n => n.Id == delNodeId) == 0);
            Assert.IsFalse(graph.Nodes.Single( n=> n.Id == neighbourId).IsNeighbourOf(delNodeId));
        }
        [TestMethod]
        public void DeleteNode_WithNonExistentId()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int nodeId = 8;

            Assert.ThrowsException<Graph.NodeIdNotFoundException>(
               new Action(() =>
               graph.DeleteNode(nodeId)));
        }
        [TestMethod]
        public void DeleteNode_WithNonpositiveId()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int nodeId = -8;

            Assert.ThrowsException<Graph.InvalidNodeIdException>(
               new Action(() =>
               graph.DeleteNode(nodeId)));
        }
        #endregion

        #region Test adding an edge
        [TestMethod]
        public void AddEdge_WithExistingIds()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();

            int nodeId = 1;
            int neighbourId = nodeId + nodeId;
            graph.AddNode( new Node(nodeId));
            graph.AddNode( new Node(neighbourId));

            int originalNumberOfEdges = graph.NumberOfEdges;
            int originalNumberOfNodes = graph.NumberOfNodes;
            
            graph.AddEdge(nodeId, neighbourId);

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.NumberOfEdges, originalNumberOfEdges + 1);
            Assert.AreEqual(graph.NumberOfNodes, originalNumberOfNodes);
            Assert.IsTrue(graph.Nodes.Count(n => n.Id == nodeId) == 1);
            Assert.IsTrue(graph.Nodes.Count(n => n.Id == neighbourId) == 1);
            Assert.IsTrue(graph.Nodes.Single(n => n.Id == neighbourId).IsNeighbourOf(nodeId));
            Assert.IsTrue(graph.Nodes.Single(n => n.Id == nodeId).IsNeighbourOf(neighbourId));
        }
        public void AddEdge_WithNonExistentIds()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int nodeId = 1;
            int neighbourId = 5;

            Assert.ThrowsException<Graph.NodeIdNotFoundException>(
               new Action(() =>
               graph.AddEdge(nodeId, neighbourId)));
        }

        [TestMethod]
        public void AddEdge_ExistingEdge()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();

            int nodeId = 1;
            int neighbourId = nodeId + nodeId;
            graph.AddNode( new Node(nodeId));
            graph.AddNode( new Node(neighbourId));
            graph.AddEdge(nodeId, neighbourId);

            int originalNumberOfNodes = graph.Nodes.Count();

            graph.AddEdge(nodeId, neighbourId);

            Assert.AreEqual(originalNumberOfNodes, graph.Nodes.Count());
            Assert.AreEqual(originalNumberOfNodes, graph.NumberOfNodes);
        }
        #endregion

        #region Test deleting an edge
        [TestMethod]
        public void DeleteEdge_ExistingEdge()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();

            int nodeId = 1;
            int neighbourId = nodeId + nodeId;
            graph.AddNode( new Node(nodeId));
            graph.AddNode( new Node(neighbourId));
            graph.AddEdge(nodeId, neighbourId);

            int originalNumberOfEdges = graph.NumberOfEdges;
            int originalNumberOfNodes = graph.NumberOfNodes;

            graph.DeleteEdge(nodeId, neighbourId);

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.Nodes.Count, originalNumberOfNodes);
            Assert.AreEqual(graph.NumberOfEdges, originalNumberOfEdges-1);
            Assert.AreEqual(graph.NumberOfNodes, originalNumberOfNodes);
            Assert.IsFalse(graph.Nodes.Single(n => n.Id == neighbourId).IsNeighbourOf(nodeId));
            Assert.IsFalse(graph.Nodes.Single(n => n.Id == nodeId).IsNeighbourOf(neighbourId));
        }
        [TestMethod]
        public void DeleteEdge_WithExistingIds()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();

            int nodeId = 1;
            int neighbourId = nodeId + nodeId;
            graph.AddNode( new Node(nodeId));
            graph.AddNode( new Node(neighbourId));

            int originalNumberOfEdges = graph.NumberOfEdges;
            int originalNumberOfNodes = graph.NumberOfNodes;

            graph.DeleteEdge(nodeId, neighbourId);

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.NumberOfEdges, originalNumberOfEdges);
            Assert.AreEqual(graph.NumberOfNodes, originalNumberOfNodes);
            Assert.IsTrue(graph.Nodes.Count(n => n.Id == nodeId) == 1);
            Assert.IsTrue(graph.Nodes.Count(n => n.Id == neighbourId) == 1);
            Assert.IsFalse(graph.Nodes.Single( n => n.Id == neighbourId).IsNeighbourOf(nodeId));
            Assert.IsFalse(graph.Nodes.Single(n => n.Id == nodeId).IsNeighbourOf(neighbourId));
        }
        [TestMethod]
        public void DeleteEdge_WithNonExistentIds()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            int nodeId = 1;
            int neighbourId = 5;

            Assert.ThrowsException<Graph.NodeIdNotFoundException>(
               new Action(() =>
               graph.DeleteEdge(nodeId, neighbourId)));
        }
        #endregion

        #region Test classifying nodes to distance level from given source with BFS
        /*
        [TestMethod]
        public void GetIdsAndLevelsWithBreadthFirstSearch_ValidSource_ChainGraph()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            graph.AddNode(1, new Node());
            for(int i=2; i<=10; i++)
            {
                graph.AddNode(i, new Node());
                graph.AddEdge(i, i - 1);
            }

            void CheckBFSfromSource(int sourceId)
            {
                Dictionary<int, int> levelsFrom1 = graph.GetIdsAndLevelsWithBreadthFirstSearch(sourceId);
                for (int i = 1; i < sourceId; i++)
                {
                    Assert.AreEqual(sourceId - i, levelsFrom1[i]);
                }
                for (int i = sourceId; i <= 10; i++)
                {
                    Assert.AreEqual(i - sourceId, levelsFrom1[i]);
                }
            }
            
            for(int i=1; i<=10; i++)
            {
                CheckBFSfromSource(i);
            }
        }
        [TestMethod]
        public void GetIdsAndLevelsWithBreadthFirstSearch_ValidSource_ConnectedGraph()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            for (int i = 1; i <= 10; i++)
            {
                graph.AddNode(i, new Node());
            }
            for (int i = 1; i < 6; i++)
            {
                graph.AddEdge(6, i);
            }
            for (int i = 6; i < 10; i++)
            {
                graph.AddEdge(i, i + 1);
            }
            graph.AddEdge(1, 10);

            Dictionary<int, int> levelsFrom1 = graph.GetIdsAndLevelsWithBreadthFirstSearch(1);

            Assert.AreEqual(0, levelsFrom1[1]);
            for (int i=2; i <= 5; i++)
            {
                Assert.AreEqual(2,levelsFrom1[i]);
            }
            Assert.AreEqual(2, levelsFrom1[7]);
            Assert.AreEqual(3, levelsFrom1[8]);
            Assert.AreEqual(2, levelsFrom1[9]);
            Assert.AreEqual(1, levelsFrom1[10]);

            Dictionary<int, int> levelsFrom8 = graph.GetIdsAndLevelsWithBreadthFirstSearch(8);

            Assert.AreEqual(0, levelsFrom8[8]);
            for (int i = 1; i <= 5; i++)
            {
                Assert.AreEqual(3, levelsFrom8[i]);
            }
            Assert.AreEqual(2, levelsFrom8[6]);
            Assert.AreEqual(1, levelsFrom8[7]);
            Assert.AreEqual(1, levelsFrom8[9]);
            Assert.AreEqual(2, levelsFrom8[10]);
        }

        [TestMethod]
        public void GetIdsAndLevelsWithBreadthFirstSearch_inValidSource()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            graph.AddNode(1, new Node());
            for (int i = 2; i <= 10; i++)
            {
                graph.AddNode(i, new Node());
                graph.AddEdge(i, i - 1);
            }

            Assert.ThrowsException<Graph.InvalidNodeIdException>(
               new Action(() =>
               graph.GetIdsAndLevelsWithBreadthFirstSearch(-8)));

            Assert.ThrowsException<Graph.NodeIdNotFoundException>(
               new Action(() =>
               graph.GetIdsAndLevelsWithBreadthFirstSearch(99)));
        }
        [TestMethod]
        public void GetIdsAndLevelsWithBreadthFirstSearch_NotConnectedGraph()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            graph.AddNode(1, new Node());
            for (int i = 2; i <= 10; i++)
            {
                graph.AddNode(i, new Node());
                graph.AddEdge(i, i - 1);
            }
            graph.DeleteEdge(5, 6);

            Assert.ThrowsException<Graph.GraphIsNotConnectedException>(
               new Action(() =>
               graph.GetIdsAndLevelsWithBreadthFirstSearch(1)));
        }
        */
        #endregion

        #region Test constructing a dominating set in the subgraph spanned by a given subset of vertices of the whole graph
        [TestMethod]
        public void ConstructDominatingSetFrom_EmptySet()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            HashSet<int> emptySet = new HashSet<int>();
            HashSet<int> emptyDominatingSet = graph.ConstructDominatingSetFrom(ref emptySet);

            Assert.IsNotNull(emptyDominatingSet);
            Assert.AreEqual(0, emptyDominatingSet.Count);
        }

        [TestMethod]
        public void ConstructDominatingSetFrom_InvalidSet()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            for(int i=1; i<10; i++)
            {
                graph.AddNode(new Node(i));
            }

            HashSet<int> invalidSet = new HashSet<int>(3);
            for(int i=11; i<14; i++)
            {
                invalidSet.Add(i);
            }

            Assert.ThrowsException<Graph.InvalidSetException>(new Action( () => graph.ConstructDominatingSetFrom(ref invalidSet)));
        }

        [TestMethod]
        public void ConstructDominatingSetFrom_Chain()
        {
            GenericGraph<Node> graph = InitTestGraph_ChainGraph();
            
            HashSet<int> chain = graph.GetIdsOfNodes();
            HashSet<int> dominatingSet = graph.ConstructDominatingSetFrom(ref chain);

            int expectedSizeOfDominatingSet = graph.NumberOfNodes / 3 + graph.NumberOfNodes % 3;
            HashSet<int> expectedDominatingSet = new HashSet<int>(expectedSizeOfDominatingSet);
            for (int i = 1; i <= graph.NumberOfNodes / 3; i++)
            {
                expectedDominatingSet.Add(i * 3-1);
            }

            Assert.IsTrue(expectedDominatingSet.SetEquals(dominatingSet));
        }

        [TestMethod]
        public void ConstructDominatingSetFrom_GeneralConnectedSet()
        {
            GenericGraph<Node> graph = InitTestGraph_ConnectedGraph();

            int expectedSizeOfDominatingSet = 2;
            HashSet<int> expectedDominatingSet = new HashSet<int>(expectedSizeOfDominatingSet);
            expectedDominatingSet.Add(6);
            expectedDominatingSet.Add(9);

            HashSet<int> nodes = graph.GetIdsOfNodes();
            Assert.IsTrue(expectedDominatingSet.SetEquals(graph.ConstructDominatingSetFrom(ref nodes)));
        }

        [TestMethod]
        public void ConstructDominatingSetFrom_NotConnectedSet()
        {
            GenericGraph<Node> graph = InitTestGraph_GraphWithoutEdges();
            HashSet<int> subset = graph.GetIdsOfNodes();

            HashSet<int> dominatingSet = graph.GetIdsOfNodes();
            subset = graph.GetIdsOfNodes();

            Assert.IsTrue(subset.SetEquals(dominatingSet));
            
            graph.AddEdge(1, 2);
            dominatingSet = graph.ConstructDominatingSetFrom(ref subset);
            subset = graph.GetIdsOfNodes();
            subset.Remove(2);

            Assert.IsTrue(subset.SetEquals(dominatingSet));

            subset.Remove(1);
            subset.Add(2);
            dominatingSet = graph.ConstructDominatingSetFrom(ref subset);
            subset = graph.GetIdsOfNodes();
            subset.Remove(1);
            subset.Add(2);
            Assert.IsTrue(subset.SetEquals( dominatingSet));
        }


        #endregion

        #region Test constructing a connected dominating set of the graph using CDOM (beginning from a given ID)
        [TestMethod]
        public void ConstructCDS_WithCDOM_InChainGraph()
        {
            GenericGraph<Node> graph = InitTestGraph_ChainGraph();
            HashSet<int> innerNodes = graph.GetIdsOfNodes();

            Assert.IsTrue(innerNodes.SetEquals(
                graph.ConstructCDS_WithCDOM(1)));
            Assert.IsTrue(innerNodes.SetEquals(
                graph.ConstructCDS_WithCDOM(graph.NumberOfNodes)));

            for (int id=1; id<graph.NumberOfNodes/2; id++)
            {
                Assert.IsTrue(innerNodes.SetEquals(
                    graph.ConstructCDS_WithCDOM(id*2-1)));
            }

            innerNodes.Remove(1);
            innerNodes.Remove(graph.NumberOfNodes);

            for (int id = 1; id < graph.NumberOfNodes / 2; id++)
            {
                Assert.IsTrue(innerNodes.SetEquals(
                    graph.ConstructCDS_WithCDOM(id * 2)));
            }
            
        }

        [TestMethod]
        public void ConstructCDS_WithCDOM_InConnectedGraph()
        {
            GenericGraph<Node> graph = InitTestGraph_ConnectedGraph();

            HashSet<int> cds_from1 = graph.ConstructCDS_WithCDOM(1);
            HashSet<int> expectedCDS_from1 = graph.GetIdsOfNodes();
            expectedCDS_from1.Remove(8);

            HashSet<int> cds_from2 = graph.ConstructCDS_WithCDOM(2);
            HashSet<int> expectedCDS_from2_option1 = graph.GetIdsOfNodes();
            expectedCDS_from2_option1.Remove(10);
            HashSet<int> expectedCDS_from2_option2 = graph.GetIdsOfNodes();
            expectedCDS_from2_option2.Remove(8);

            HashSet<int> cds_from6 = graph.ConstructCDS_WithCDOM(6);
            HashSet<int> expectedCDS_from6 = new HashSet<int>(5);
            expectedCDS_from6.Add(6);
            expectedCDS_from6.Add(1);
            expectedCDS_from6.Add(7);
            expectedCDS_from6.Add(10);
            expectedCDS_from6.Add(8);

            HashSet<int> cds_from7 = graph.ConstructCDS_WithCDOM(7);
            HashSet<int> expectedCDS_from7 = graph.GetIdsOfNodes();
            expectedCDS_from7.Remove(10);

            HashSet<int> cds_from8 = graph.ConstructCDS_WithCDOM(8);
            HashSet<int> expectedCDS_from8 = new HashSet<int>(5);
            expectedCDS_from8.Add(6);
            expectedCDS_from8.Add(9);
            expectedCDS_from8.Add(7);
            expectedCDS_from8.Add(8);
            expectedCDS_from8.Add(10);

            HashSet<int> cds_from9 = graph.ConstructCDS_WithCDOM(9);
            HashSet<int> expectedCDS_from9 = graph.GetIdsOfNodes();
            
            Assert.IsTrue(cds_from1.SetEquals(expectedCDS_from1));
            Assert.IsTrue(
                cds_from2.SetEquals(expectedCDS_from2_option1) || 
                cds_from2.SetEquals(expectedCDS_from2_option2));
            Assert.IsTrue(cds_from6.SetEquals(expectedCDS_from6));
            Assert.IsTrue(cds_from7.SetEquals(expectedCDS_from7));
            Assert.IsTrue(cds_from8.SetEquals(expectedCDS_from8));
            Assert.IsTrue(cds_from9.SetEquals(expectedCDS_from9));
        }
        [TestMethod]
        public void ConstructCDS_WithCDOM_InNotConnectedGraph()
        {
            GenericGraph<Node> graph = InitTestGraph_GraphWithoutEdges();
            
            Assert.ThrowsException<Graph.GraphIsNotConnectedException>(new Action(
                () => graph.ConstructCDS_WithCDOM(1)));
        }
        [TestMethod]
        public void ConstructCDS_WithCDOM_FromInvalidId()
        {
            GenericGraph<Node> graph = InitTestGraph_ChainGraph();

            Assert.ThrowsException<Graph.InvalidNodeIdException>(new Action(
                () => graph.ConstructCDS_WithCDOM(0)));
            Assert.ThrowsException<Graph.NodeIdNotFoundException>(new Action(
                () => graph.ConstructCDS_WithCDOM(100)));
        }
        #endregion

        #region Initialise graph to test methods on
        private static GenericGraph<Node> InitTestGraph_ChainGraph()
        {
            int numberOfNodes = 10;
            GenericGraph<Node> graph = new GenericGraph<Node>();
            graph.AddNode( new Node(1));
            for (int i = 2; i < numberOfNodes; i++)
            {
                graph.AddNode( new Node(i));
                graph.AddEdge(i, i - 1);
            }

            return graph;
        }
        private static GenericGraph<Node> InitTestGraph_ConnectedGraph()
        {
            GenericGraph<Node> graph = new GenericGraph<Node>();
            for (int i = 1; i <= 10; i++)
            {
                graph.AddNode( new Node(i));
            }
            for (int i = 1; i < 6; i++)
            {
                graph.AddEdge(6, i);
            }
            for (int i = 6; i < 10; i++)
            {
                graph.AddEdge(i, i + 1);
            }
            graph.AddEdge(1, 10);

            return graph;
        }
        private static GenericGraph<Node> InitTestGraph_GraphWithoutEdges()
        {
            int numberOfNodes = 10;
            GenericGraph<Node> graph = new GenericGraph<Node>();
            for (int i = 1; i < numberOfNodes; i++)
            {
                graph.AddNode(new Node(i));
            }
            return graph;
        }
        #endregion
    }
}
