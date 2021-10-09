using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MeetingOrganiserDesktopApp.Model;

namespace UnitTests.MeetingOrganiserDesktopApp
{
    [TestClass]
    public class NodeTest
    {
        #region Test constructor
        [TestMethod]
        public void Initialise_WithNoArgument()
        {
            Node node = new Node(1);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 0);
        }

        [TestMethod]
        public void Initialise_WithPotentialNumberOfNeighbours()
        {
            Node node = new Node(1, 10);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 0);
        }

        [TestMethod]
        public void Initialise_WithSetOfIds()
        {
            int size = 10;
            HashSet<int> setOfIds = new HashSet<int>(size);
            for (int i = 1; i <= size; i++)
            {
                setOfIds.Add(i + i);
            }
            Node node = new Node(1, setOfIds);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsTrue(neighbours == setOfIds);
        }
        #endregion

        #region Test adding neighbour
        [TestMethod]
        public void AddNeighbour_ToEmptySet()
        {
            Node node = new Node(1);

            int newNeighbourId = 2;
            node.AddNeighbour(newNeighbourId);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsTrue(neighbours.Count == 1);
            Assert.IsTrue(neighbours.Contains(newNeighbourId));
        }
        [TestMethod]
        public void AddNeighbour_ToSetNotContainingAddedNeighbour()
        {
            int size = 4;
            HashSet<int> setOfIds = new HashSet<int>(size);

            setOfIds.Add(1);
            setOfIds.Add(4);
            setOfIds.Add(100);

            Node node = new Node(1,setOfIds);

            int newNeighbourId = 2;
            node.AddNeighbour(newNeighbourId);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsTrue(neighbours.Count == size);
            Assert.IsTrue(neighbours.Contains(newNeighbourId));
        }

        [TestMethod]
        public void AddNeighbour_ToSetContainingAddedNeighbour()
        {
            int size = 3;
            HashSet<int> setOfIds = new HashSet<int>(size);

            setOfIds.Add(2);
            setOfIds.Add(4);
            setOfIds.Add(100);

            Node node = new Node(1, setOfIds);

            int newNeighbourId = 2;
            try
            {
                node.AddNeighbour(newNeighbourId);
            }
            catch (Node.NodeIdAlreadyPresent)
            {
                HashSet<int> neighbours = node.NeighbourIds;

                Assert.IsTrue(neighbours.Count == size);
                Assert.IsTrue(neighbours.Contains(newNeighbourId));
            }
        }

        [TestMethod]
        public void AddNeighbour_WithNegativeId()
        {
            Node node = new Node(1);
            HashSet<int> neighbours = node.NeighbourIds;

            int newNeighbourId = -2;
            try
            {
                node.AddNeighbour(newNeighbourId);
            }
            catch(Node.NodeIdNonPositiveException)
            {
                Assert.IsTrue(neighbours == node.NeighbourIds);
            }
        }
        #endregion

        #region Test deleting neighbour
        [TestMethod]
        public void DeleteNeighbour_FromContainingIt()
        {
            Node node = new Node(1);
            int neighbourA = 1;
            int neighbourB = 23;
            int delNeighbourId = 89;

            node.AddNeighbour(neighbourA);
            node.AddNeighbour(delNeighbourId);
            node.AddNeighbour(neighbourB);

            Assert.IsTrue(node.DeleteNeighbour(delNeighbourId));
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.AreEqual(neighbours.Count, 2);
            Assert.IsFalse(neighbours.Contains(delNeighbourId));
            Assert.IsTrue(neighbours.Contains(neighbourA));
            Assert.IsTrue(neighbours.Contains(neighbourB));
        }
        [TestMethod]
        public void DeleteNeighbour_FromEmptySet()
        {
            Node node = new Node(1);
            int newNeighbourId = 89;

            Assert.IsFalse( node.DeleteNeighbour(newNeighbourId));
                
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 0);
        }
        [TestMethod]
        public void DeleteNeighbour_FromSetNotContainingIt()
        {
            int size = 3;
            HashSet<int> setOfIds = new HashSet<int>(size);

            setOfIds.Add(2);
            setOfIds.Add(4);
            setOfIds.Add(100);

            Node node = new Node(1,setOfIds);

            int delNeighbourId = 70;

            Assert.IsFalse(node.DeleteNeighbour(delNeighbourId));
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsFalse(neighbours.Contains(delNeighbourId));
            Assert.AreEqual(setOfIds, neighbours);
        }
        [TestMethod]
        public void DeleteNeighbour_WithNegativeId()
        {
            Node node = new Node(1);
            HashSet<int> neighbours = node.NeighbourIds;

            int newNeighbourId = -2;
            try
            {
                node.DeleteNeighbour(newNeighbourId);
            }
            catch (Node.NodeIdNonPositiveException)
            {
                Assert.AreEqual(neighbours, node.NeighbourIds);
            }
        }
        #endregion

    }

    [TestClass]
    public class WeightedNodeTest
    {
        #region Test Constructor
        [TestMethod]
        public void Initialise_WithNoArgument()
        {
            WeightedNode node = new WeightedNode(1);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 0);
            Assert.IsTrue(node.Weight == 0);
        }

        [TestMethod]
        public void Initialise_WithPotentialNumberOfNeighbours()
        {
            WeightedNode node = new WeightedNode(1, potentialNumberOfNeighbours: 10);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 0);
            Assert.IsTrue(node.Weight == 0);
        }

        [TestMethod]
        public void Initialise_WithSetOfIds()
        {
            int size = 10;
            HashSet<int> setOfIds = new HashSet<int>(size);
            for (int i = 1; i <= size; i++)
            {
                setOfIds.Add(i + i);
            }
            WeightedNode node = new WeightedNode(1, setOfIds);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsTrue(neighbours == setOfIds);
            Assert.IsTrue(node.Weight == 0);
        }

        [TestMethod]
        public void Initialise_WithWeight()
        {
            int w = 10;
            WeightedNode node = new WeightedNode(1, weight: w);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 0);
            Assert.IsTrue(node.Weight == w);
        }

        [TestMethod]
        public void Initialise_WithPotentialNumberOfNeighbours_AndWeight()
        {
            int w = 2000;
            WeightedNode node = new WeightedNode(
                id: 1,
                potentialNumberOfNeighbours: 10,
                weight: w);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsNotNull(neighbours);
            Assert.IsTrue(neighbours.Count == 0);
            Assert.IsTrue(node.Weight == w);
        }

        [TestMethod]
        public void Initialise_WithSetOfIds_AndWeight()
        {
            int size = 10;
            HashSet<int> setOfIds = new HashSet<int>(size);
            for (int i = 1; i <= size; i++)
            {
                setOfIds.Add(i + i);
            }
            int w = 2000;
            WeightedNode node = new WeightedNode(
                id: 1,
                neighbourIds: setOfIds,
                weight: w);
            HashSet<int> neighbours = node.NeighbourIds;

            Assert.IsTrue(neighbours == setOfIds);
            Assert.IsTrue(node.Weight == w);
        }

        #endregion
    }
}
