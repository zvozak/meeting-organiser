using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MeetingOrganiserDesktopApp.Model;
using System.Linq;

namespace UnitTests.MeetingOrganiserDesktopApp
{
    [TestClass]
    public class GraphTest
    {
        #region Test adding a node
        [TestMethod]
        public void AddNode_NotContainingIt()
        {
            Graph graph = new Graph();
            int nodeId = 5;

            int originalNumberOfNodes = graph.NumberOfNodes;

            graph.AddNode(nodeId);

            Assert.IsNotNull(graph.Nodes);
            Assert.AreEqual(graph.Nodes.Count, originalNumberOfNodes + 1);
            Assert.AreEqual(graph.NumberOfEdges, 0);
            Assert.AreEqual(graph.NumberOfNodes, originalNumberOfNodes + 1);
            Assert.IsTrue(graph.Nodes.Where(n => n.Id == nodeId).Count() == 1);
        }
        [TestMethod]
        public void AddNode_withPositiveId_containing()
        {
            Graph graph = new Graph();
            int nodeId = 8;
            graph.AddNode(nodeId, 10);

            Assert.ThrowsException<Graph.NodeIdAlreadyPresentException>(
               new Action(() =>
               graph.AddNode(nodeId, 20)));
        }
        [TestMethod]
        public void AddNode_withNonPositiveId()
        {
            Graph graph = new Graph();
            int originalNumberOfNodes = graph.NumberOfNodes;
            HashSet<Node> originalNodes = graph.Nodes;

            int nodeId = -8;

            Assert.ThrowsException<Graph.InvalidNodeIdException>(
               new Action(() =>
               graph.AddNode(nodeId)));
        }
        [TestMethod]
        public void AddNode_withNegativeNumberOfPotentialNeighbours()
        {
            Graph graph = new Graph();
            int originalNumberOfNodes = graph.NumberOfNodes;
            HashSet<Node> originalNodes = graph.Nodes;

            int nodeId = 8;
            int potentialNumberOfNeighbours = -7;

            Assert.ThrowsException<ArgumentException>(
                new Action(() =>
                graph.AddNode(nodeId, potentialNumberOfNeighbours)));
        }
        #endregion
    }

}
