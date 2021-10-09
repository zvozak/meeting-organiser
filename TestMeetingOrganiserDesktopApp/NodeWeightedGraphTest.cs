using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingOrganiserDesktopApp.Model;

namespace UnitTests.MeetingOrganiserDesktopApp
{
    [TestClass]
    public class NodeWeightedGraphTest
    {
        #region Test adding a node
        [TestMethod]
        public void AddNode_NotContainingIt()
        {
            NodeWeightedGraph graph = new NodeWeightedGraph();
            int nodeId = 5;
            int weight = 65;

            int originalNumberOfNodes = graph.NumberOfNodes;

            graph.AddNode(nodeId, weight: weight);

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.Nodes.Count, originalNumberOfNodes + 1);
            Assert.AreEqual(graph.NumberOfEdges, 0);
            Assert.AreEqual(graph.NumberOfNodes, originalNumberOfNodes + 1);
            Assert.IsTrue(graph.Nodes.Where( n=> n.Id == nodeId).Count() == 1);
            Assert.AreEqual(graph.Nodes.Single(n => n.Id == nodeId).Weight, weight);
        }
        [TestMethod]
        public void AddNode_withPositiveId_containing()
        {
            NodeWeightedGraph graph = new NodeWeightedGraph();
            int nodeId = 8;
            graph.AddNode(nodeId, 10);

            Assert.ThrowsException<NodeWeightedGraph.NodeIdAlreadyPresentException>(
               new Action(() =>
               graph.AddNode(nodeId, 20)));
        }
        [TestMethod]
        public void AddNode_withNegativeWeight()
        {
            NodeWeightedGraph graph = new NodeWeightedGraph();
            int nodeId = 8;
            int weight = -65;

            Assert.ThrowsException<NodeWeightedGraph.InvalidWeightException>(
               new Action(() =>
               graph.AddNode(nodeId, weight: weight)));
        }
        [TestMethod]
        public void AddNode_withNonPositiveId()
        {
            NodeWeightedGraph graph = new NodeWeightedGraph();
            int originalNumberOfNodes = graph.NumberOfNodes;
            HashSet<WeightedNode> originalNodes = graph.Nodes;

            int nodeId = -8;

            Assert.ThrowsException<NodeWeightedGraph.InvalidNodeIdException>(
               new Action(() =>
               graph.AddNode(nodeId)));
        }
        [TestMethod]
        public void AddNode_withNegativeNumberOfPotentialNeighbours()
        {
            NodeWeightedGraph graph = new NodeWeightedGraph();
            int originalNumberOfNodes = graph.NumberOfNodes;
            HashSet<WeightedNode> originalNodes = graph.Nodes;

            int nodeId = 8;
            int potentialNumberOfNeighbours = -7;

            Assert.ThrowsException<ArgumentException>(
                new Action(() =>
                graph.AddNode(nodeId, potentialNumberOfNeighbours: potentialNumberOfNeighbours)));
        }
        #endregion

        #region Test Constructint a Connected Dominating Set With Algorithm TCDS

        [TestMethod]
        public void ConstructConnectedDominatingSet_WithTCDS_InChainGraph_WithHomogenousWeights()
        {
            NodeWeightedGraph graph = InitTestGraph_ChainGraph_WithHomogenousWeights();
            HashSet<int> cds = graph.ConstructConnectedDominatingSet_WithTCDS();
            HashSet<int> expectedCds = graph.GetIdsOfNodes();
            expectedCds.Remove(1);
            expectedCds.Remove(graph.NumberOfNodes);

            Assert.IsTrue(expectedCds.SetEquals(cds));
        }
        [TestMethod]
        public void ConstructConnectedDominatingSet_WithTCDS_InChainGraph_WithVaryingWeights()
        {
            NodeWeightedGraph graph = InitTestGraph_ChainGraph_WithVaryingWeights();
            HashSet<int> cds = graph.ConstructConnectedDominatingSet_WithTCDS();
            HashSet<int> expectedCds = graph.GetIdsOfNodes();
            expectedCds.Remove(1);
            expectedCds.Remove(graph.NumberOfNodes);

            Assert.IsTrue(expectedCds.SetEquals(cds));
        }
        [TestMethod]
        public void ConstructConnectedDominatingSet_WithTCDS_InGeneralConnectedGraph_WithHomogenousWeights()
        {
            NodeWeightedGraph graph = InitTestGraph_GeneralConnectedGraph_WithHomogenousWeights();
            HashSet<int> cds = graph.ConstructConnectedDominatingSet_WithTCDS();
            HashSet<int> expectedCds = new HashSet<int>(4);
            expectedCds.Add(6);
            expectedCds.Add(9);
            expectedCds.Add(7);
            expectedCds.Add(8);

            Assert.IsTrue(expectedCds.SetEquals(cds));
        }
        [TestMethod]
        public void ConstructConnectedDominatingSet_WithTCDS_InGeneralConnectedGraph_WithVaryingWeights()
        {
            NodeWeightedGraph graph = InitTestGraph_GeneralConnectedGraph_WithVaryingWeights();
            HashSet<int> cds = graph.ConstructConnectedDominatingSet_WithTCDS();
            HashSet<int> expectedCds = new HashSet<int>(4);
            expectedCds.Add(6);
            expectedCds.Add(9);
            expectedCds.Add(7);
            expectedCds.Add(8);

            Assert.IsTrue(expectedCds.SetEquals(cds));
        }
        [TestMethod]
        public void ConstructConnectedDominatingSet_WithTCDS_InNotConnectedGraph()
        {
            NodeWeightedGraph graph = InitTestGraph_GraphWithoutEdges();

            Assert.ThrowsException<NodeWeightedGraph.GraphIsNotConnectedException>(new Action(
                () => graph.ConstructConnectedDominatingSet_WithTCDS()));
        }
        #endregion


        #region Initialise graph to test methods on

        private static NodeWeightedGraph InitTestGraph_ChainGraph_WithHomogenousWeights()
        {
            int numberOfNodes = 10;
            NodeWeightedGraph graph = new NodeWeightedGraph();
            graph.AddNode( new WeightedNode(1));
            for (int i = 2; i < numberOfNodes; i++)
            {
                graph.AddNode(new WeightedNode(i));
                graph.AddEdge(i, i - 1);
            }

            return graph;
        }
        private static NodeWeightedGraph InitTestGraph_ChainGraph_WithVaryingWeights()
        {
            int numberOfNodes = 10;
            NodeWeightedGraph graph = new NodeWeightedGraph();
            graph.AddNode(new WeightedNode(1, weight: 30));
            for (int i = 2; i<numberOfNodes; i++)
            {
                graph.AddNode(new WeightedNode(i, weight: i*2));
                graph.AddEdge(i, i - 1);
            }

            return graph;
        }
private static NodeWeightedGraph InitTestGraph_GeneralConnectedGraph_WithHomogenousWeights()
        {
            NodeWeightedGraph graph = new NodeWeightedGraph();
            for (int i = 1; i <= 10; i++)
            {
                graph.AddNode(new WeightedNode(i));
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
        private static NodeWeightedGraph InitTestGraph_GeneralConnectedGraph_WithVaryingWeights()
        {
            NodeWeightedGraph graph = new NodeWeightedGraph();
            for (int i = 1; i <= 10; i++)
            {
                graph.AddNode( new WeightedNode(i, weight: i));
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

        private static NodeWeightedGraph InitTestGraph_GraphWithoutEdges()
        {
            int numberOfNodes = 10;
            NodeWeightedGraph graph = new NodeWeightedGraph();
            for (int i = 1; i < numberOfNodes; i++)
            {
                graph.AddNode(new WeightedNode(i));
            }
            return graph;
        }
        #endregion
    }
}
