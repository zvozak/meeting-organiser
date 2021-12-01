using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingOrganiserDesktopApp.Model
{
    public class GenericGraph<TNode>
        where TNode : Node
    {
        #region Custom exceptions for class

        public class InvalidSetException : Exception
        {
            public InvalidSetException()
                : base("Given set is not a subset of all nodes in the graph.") { }
        }

        public class GraphIsNotConnectedException : Exception
        {
            public GraphIsNotConnectedException(int sourceId, int numberOfReachableNodes)
                : base(string.Format("Graph must be connected. {0} nodes can be reached from source node with ID {1}.", numberOfReachableNodes, sourceId)) { }
            public GraphIsNotConnectedException()
                : base("Graph must be connected.") { }
        }
        public class NodeIdNotFoundException : ArgumentException
        {
            public NodeIdNotFoundException(int nodeId)
                : base(string.Format("Graph does not contain Node with ID {0}.", nodeId)) { }
        }
        public class NodeIdAlreadyPresentException : ArgumentException
        {
            public NodeIdAlreadyPresentException(int nodeId)
                : base(string.Format("Graph already contains node with ID {0}.", nodeId)) { }
        }

        public class NoLinkFoundException : KeyNotFoundException
        {
            public NoLinkFoundException(int firstNodeId, int secondNodeId)
                : base(string.Format("There is no link from Node {0} to Node {1}.", firstNodeId, secondNodeId)) { }
        }
        public class InvalidNodeIdException : ArgumentException
        {
            public InvalidNodeIdException()
                : base("Node id must be positive.") { }
        }

        #endregion


        #region Members
        protected const int DEFAULT_NUMBER_OF_NODES = 10;
        public int NumberOfNodes { get; protected set; }
        public int NumberOfEdges { get; protected set; }
        public HashSet<TNode> Nodes { get; protected set; }
        #endregion


        #region Public methods

        public GenericGraph(int potentialNumberOfVertices = DEFAULT_NUMBER_OF_NODES)
        {
            if (potentialNumberOfVertices < 0)
            {
                throw new ArgumentException(
                    "Number of vertices cannot be negative.");
            }

            Nodes = new HashSet<TNode>(potentialNumberOfVertices);
        }

        public static explicit operator Graph(GenericGraph<TNode> generic)
        {
            HashSet<Node> nodes = new HashSet<Node>(generic.NumberOfNodes);
            foreach(var node in generic.Nodes)
            {
                nodes.Add((Node)node);
            }
            return new Graph
            {
                Nodes = nodes,
                NumberOfNodes = generic.NumberOfNodes,
                NumberOfEdges = generic.NumberOfEdges
            };
        }

        public HashSet<int> GetIdsOfNodes()
        {
            return Nodes.Select(node => node.Id).ToHashSet();
        }

        public int GetNumberOfNeighbours(int nodeId)
        {
            return Nodes.Single(n => n.Id == nodeId).GetNumberOfNeighbours();
        }

        public void AddEdge(int firstNodeId, int secondNodeId)
        {
            try
            {
                Nodes.Single(node => node.Id == firstNodeId).AddNeighbour(secondNodeId);
            }
            catch (ArgumentNullException)
            {
                throw new NodeIdNotFoundException(firstNodeId);
            }

            try
            {
                Nodes.Single(node => node.Id == secondNodeId).AddNeighbour(firstNodeId);
            }
            catch (KeyNotFoundException)
            {
                Nodes.Single(node => node.Id == firstNodeId).DeleteNeighbour(secondNodeId);
                throw new NodeIdNotFoundException(secondNodeId);
            }

            NumberOfEdges++;
        }

        public void DeleteEdge(int firstNodeId, int secondNodeId)
        {
            bool areNeighbours = true;
            try
            {
                areNeighbours &= Nodes.Single(node => node.Id == firstNodeId).DeleteNeighbour(secondNodeId);
            }
            catch (InvalidOperationException)
            {
                throw new NodeIdNotFoundException(firstNodeId);
            }

            try
            {
                areNeighbours &= Nodes.Single(node => node.Id == secondNodeId).DeleteNeighbour(firstNodeId);
            }
            catch (InvalidOperationException)
            {
                throw new NodeIdNotFoundException(secondNodeId);
            }

            if (areNeighbours)
            {
                NumberOfEdges--;
            }
        }

        public void AddNode (int nodeId, Func<int, TNode> constructor)
        {
            CheckNodeIdValidity(nodeId);
            Nodes.Add(constructor(nodeId));
        }
        public void AddNode(TNode newNode)
        {
            CheckNodeIdValidity(newNode.Id);

            if (!Nodes.Add(newNode))
                throw new NodeIdAlreadyPresentException(newNode.Id);

            NumberOfNodes++;
        }

        public void DeleteNode(int delNodeId)
        {
            CheckNodeIdValidity(delNodeId);

            if (Nodes.RemoveWhere(node => node.Id == delNodeId) == 0)
            {
                throw new NodeIdNotFoundException(delNodeId);
            }

            NumberOfNodes--;

            foreach (int nodeId in Nodes.Select (node => node.Id))
            {
                DeleteRemainingHalfEdge(nodeId, delNodeId);
            }
        }

        public bool IsGraphConnectedSpanBy(HashSet<int> nodeIds)
        {
            HashSet<int> nodeIdsCopy = nodeIds;
            if (nodeIdsCopy.Count == 0)
            {
                throw new ArgumentException("Given set of ids is empty.");
            }
            CheckIfSubsetOfNodes(nodeIdsCopy);

            int sourceId = nodeIdsCopy.ElementAt(0);

            HashSet<int> visitedIds = new HashSet<int>(nodeIdsCopy.Count);
            visitedIds.Add(sourceId);

            Queue<int> idsToBeProcessed = new Queue<int>(nodeIdsCopy.Count);
            idsToBeProcessed.Enqueue(sourceId);

            while (idsToBeProcessed.Count > 0)
            {
                int currentNodeId = idsToBeProcessed.Dequeue();

                foreach (int neighbour in SetIntersectionOf(Nodes.Single(node => node.Id == currentNodeId).NeighbourIds, nodeIdsCopy))
                {
                    if (!visitedIds.Contains(neighbour))
                    {
                        visitedIds.Add(neighbour);
                        idsToBeProcessed.Enqueue(neighbour);
                    }
                }
            }
            return visitedIds.Count == nodeIdsCopy.Count;
        }

        public bool IsGraphConnected()
        {
            int sourceId = Nodes.Select(n => n.Id).ElementAt(0);

            HashSet<int> visitedIds = new HashSet<int>(NumberOfNodes);
            visitedIds.Add(sourceId);

            Queue<int> idsToBeProcessed = new Queue<int>(NumberOfNodes);
            idsToBeProcessed.Enqueue(sourceId);

            while (idsToBeProcessed.Count > 0)
            {
                int currentNodeId = idsToBeProcessed.Dequeue();
                foreach (int neighbour in Nodes.Single(node => node.Id == currentNodeId).NeighbourIds)
                {
                    if (!visitedIds.Contains(neighbour))
                    {
                        visitedIds.Add(neighbour);
                        idsToBeProcessed.Enqueue(neighbour);
                    }
                }
            }

            return visitedIds.Count == NumberOfNodes;
        }

        public HashSet<int> ConstructCDS_WithCDOM()
        {
            int sourceId = Nodes.Select(n => n.Id).ElementAt(0);
            return ConstructCDS_WithCDOM(sourceId);
        }

        public HashSet<int> ConstructCDSInForest(int sourceId)
        {
            HashSet<TNode> cds = new HashSet<TNode>(Nodes);
            foreach (var node in Nodes)
            {
                if (node.GetNumberOfNeighbours() == 1)
                {
                    var singleNeighbour = Nodes.First(n => n.Id == node.NeighbourIds.First());
                    if (singleNeighbour.GetNumberOfNeighbours() == 1)
                    {
                        cds.Remove(singleNeighbour);
                    }
                    else
                    {
                        cds.Remove(node);
                    }
                }
            }

            return cds.Select(n => n.Id).ToHashSet<int>();
        }

        public HashSet<int> ConstructCDS_WithCDOM(int sourceId)
        {
            CheckNodeIdValidity(sourceId);
            CheckExistenceOfNode(sourceId);

            bool isConnectedGraph;
            NodeWeightedGraph spanningTree = ConstructSpanningTree_WithBFS(sourceId, out isConnectedGraph);

            if (!isConnectedGraph)
            {
                throw new GraphIsNotConnectedException(sourceId, spanningTree.NumberOfNodes);
            }

            HashSet<int> cds = new HashSet<int>(NumberOfNodes);
            cds.Add(sourceId);

            HashSet<int> dominatedIds = new HashSet<int>(NumberOfNodes);

            HashSet<int> independentSetOfCurrentLevel = new HashSet<int>(NumberOfNodes);
            independentSetOfCurrentLevel.Add(sourceId);

            int NUMBER_OF_LEVELS = spanningTree.Nodes
                .Max(n => spanningTree.GetWeightOf(n.Id));

            for (int i = 1; i <= NUMBER_OF_LEVELS; i++)
            {
                RefreshDominatedIds(independentSetOfCurrentLevel, ref dominatedIds);
                independentSetOfCurrentLevel.Clear();

                HashSet<int> notDominatedIdsOfLevel = SetDifferenceOf(
                    spanningTree.GetIdsOfGivenWeight(i),
                    dominatedIds);

                independentSetOfCurrentLevel = ConstructDominatingSetFrom(ref notDominatedIdsOfLevel);

                HashSet<int> connectors = GetConnectors_CDOM(independentSetOfCurrentLevel, spanningTree);

                cds.UnionWith(independentSetOfCurrentLevel);
                cds.UnionWith(connectors);
            }

            return cds;
        }

        public HashSet<int> ConstructDominatingSetFrom(ref HashSet<int> nodeIdsToBeDominated)
        {
            CheckIfSubsetOfNodes(nodeIdsToBeDominated);

            HashSet<int> dominatingSet = new HashSet<int>(nodeIdsToBeDominated.Count);

            while (nodeIdsToBeDominated.Count > 0)
            {
                int bestCandidate = nodeIdsToBeDominated.ElementAt(0);
                int maxNumberOfNewlyDominated = 0;
                foreach (int id in nodeIdsToBeDominated)
                {
                    HashSet<int> neighbours = Nodes.Single(node => node.Id == id).NeighbourIds;
                    int numberOfNewlyDominated = SizeOfIntersection(nodeIdsToBeDominated, neighbours);
                    if (maxNumberOfNewlyDominated < numberOfNewlyDominated
                        || (maxNumberOfNewlyDominated == numberOfNewlyDominated
                        && GetNumberOfNeighbours(id) > GetNumberOfNeighbours(bestCandidate)))
                    {
                        bestCandidate = id;
                        maxNumberOfNewlyDominated = numberOfNewlyDominated;
                    }
                }
                dominatingSet.Add(bestCandidate);
                HashSet<int> newlyDominatedNodes = Nodes.Single(node => node.Id == bestCandidate).NeighbourIds;
                nodeIdsToBeDominated = SetDifferenceOf(nodeIdsToBeDominated, newlyDominatedNodes);
                nodeIdsToBeDominated.Remove(bestCandidate);
            }

            return dominatingSet;
        }

        public HashSet<int> ConstructDominatingSet()
        {
            HashSet<int> allNodes = GetIdsOfNodes();
            return ConstructDominatingSetFrom(ref allNodes);
        }

        #endregion


        #region Protected methods

        protected void AddHalfEdge(int sourceId, int destinationId)
        {
            CheckExistenceOfNode(sourceId);
            try
            {
                Nodes.Single(node => node.Id == sourceId).AddNeighbour(destinationId);
            }
            catch (KeyNotFoundException)
            {
                throw new NodeIdNotFoundException(sourceId);
            }
            NumberOfEdges++;
        }

        protected void DeleteRemainingHalfEdge(int fromId, int toId)
        {
            if (Nodes.Single(node => node.Id == fromId).DeleteNeighbour(toId))
            {
                NumberOfEdges--;
            }
        }


        protected void CheckNodeIdValidity(int nodeId)
        {
            if (nodeId <= 0)
            {
                throw new InvalidNodeIdException();
            }
        }

        protected void CheckExistenceOfNode(int nodeId)
        {
            if (!Nodes.Any(node => node.Id == nodeId))
            {
                throw new NodeIdNotFoundException(nodeId);
            }
        }

        protected void CheckIfGraphIsConnected()
        {
            if (!IsGraphConnected())
            {
                throw new GraphIsNotConnectedException();
            }
        }

        protected NodeWeightedGraph ConstructSpanningTree_WithBFS(int sourceId, out bool isConnectedGraph)
        {
            CheckNodeIdValidity(sourceId);
            CheckExistenceOfNode(sourceId);

            NodeWeightedGraph spanningTree = new NodeWeightedGraph();
            spanningTree.AddNode(sourceId, weight: 0);

            HashSet<int> visitedIds = new HashSet<int>(NumberOfNodes);
            visitedIds.Add(sourceId);

            Queue<int> idsToBeProcessed = new Queue<int>(NumberOfNodes);
            idsToBeProcessed.Enqueue(sourceId);

            while (idsToBeProcessed.Count > 0)
            {
                int parent = idsToBeProcessed.Dequeue();
                foreach (int child in Nodes.Single(node => node.Id == parent).NeighbourIds)
                {
                    if (!visitedIds.Contains(child))
                    {
                        spanningTree.AddNode(child, weight: spanningTree.Nodes.Single(node => node.Id == parent).Weight + 1);
                        spanningTree.AddHalfEdge(child, parent);
                        visitedIds.Add(child);
                        idsToBeProcessed.Enqueue(child);
                    }
                }
            }

            isConnectedGraph = visitedIds.Count == NumberOfNodes;

            return spanningTree;
        }

        protected HashSet<int> SetDifferenceOf(HashSet<int> originalSet, HashSet<int> setToBeRemoved)
        {
            foreach (int element in setToBeRemoved)
            {
                originalSet.Remove(element);
            }
            return originalSet;
        }

        protected void CheckIfSubsetOfNodes(HashSet<int> set)
        {
            if (!set.All(element => Nodes.Any( node => node.Id == element)))
            {
                throw new InvalidSetException();
            }
        }

        protected HashSet<int> GetConnectors_CDOM(HashSet<int> idsToConnect, NodeWeightedGraph spanningTree)
        {
            HashSet<int> connector = new HashSet<int>(idsToConnect.Count);
            foreach (int node in idsToConnect)
            {
                connector.Add(GetParentOf(node, spanningTree));
            }
            return connector;
        }
        protected int GetParentOf(int nodeId, NodeWeightedGraph spanningTree)
        {
            if (spanningTree.Nodes.Single(node => node.Id == nodeId).NeighbourIds.Count > 1)
            {
                throw new Exception("Error in spanning tree - node can have at most 1 parent.");
            }
            return spanningTree.Nodes.Single(node => node.Id == nodeId).NeighbourIds.ElementAt(0);
        }


        protected void RefreshDominatedIds(HashSet<int> dominatorIds, ref HashSet<int> dominatedIds)
        {
            foreach (int nodeId in dominatorIds)
            {
                foreach (int neighbour in Nodes.Single(node => node.Id == nodeId).NeighbourIds)
                {
                    dominatedIds.Add(neighbour);
                }
                dominatedIds.Add(nodeId);
            }
        }

        protected int SizeOfIntersection(HashSet<int> setA, HashSet<int> setB)
        {
            int size = 0;
            if (setA.Count > setB.Count)
            {
                SizeOfIntersection(setB, setA);
            }
            foreach (int element in setA)
            {
                if (setB.Contains(element))
                {
                    size++;
                }
            }
            return size;
        }

        protected HashSet<int> SetIntersectionOf(HashSet<int> setA, HashSet<int> setB)
        {
            HashSet<int> intersection = new HashSet<int>(setA.Count + setB.Count);
            if (setA.Count > setB.Count)
            {
                SetIntersectionOf(setB, setA);
            }
            foreach (int element in setA)
            {
                if (setB.Contains(element))
                {
                    intersection.Add(element);
                }
            }

            return intersection;
        }

        protected HashSet<int> GetNeighboursOfLeaves()
        {
            HashSet<int> neighbours = new HashSet<int>(NumberOfNodes);

            foreach (var node in Nodes)
            {
                if (GetNumberOfNeighbours(node.Id) == 1)
                {
                    neighbours.Add(Nodes.Single(n => n.Id == node.Id).NeighbourIds.ElementAt(0));
                }
            }

            return neighbours;
        }

        protected int NumberOfComponentsInGraphSpanBy(HashSet<int> nodeIds)
        {
            int components = 0;
            HashSet<int> nodeIdsCopy = new HashSet<int>(nodeIds);
            HashSet<int> currentComponent = new HashSet<int>(nodeIdsCopy.Count);
            HashSet<int> newNeighboursOfComponent;
            HashSet<int> union;

            while (nodeIdsCopy.Count > 0)
            {
                currentComponent.Add(nodeIdsCopy.ElementAt(0));
                newNeighboursOfComponent = Nodes.Single(node => node.Id == nodeIdsCopy.ElementAt(0)).NeighbourIds;
                union = UnionOf(currentComponent, newNeighboursOfComponent);

                while (newNeighboursOfComponent.Count > 0)
                {
                    newNeighboursOfComponent = SetDifferenceOf(
                        GetNeighboursOf(newNeighboursOfComponent),
                        currentComponent);
                    currentComponent = union;
                    union = UnionOf(currentComponent, newNeighboursOfComponent);
                }
                nodeIdsCopy = SetDifferenceOf(nodeIdsCopy, currentComponent);
                components++;
            }

            return components;
        }

        protected HashSet<int> GetNeighboursOf(HashSet<int> nodes)
        {
            HashSet<int> neighbours = new HashSet<int>(NumberOfNodes);
            foreach (int element in nodes)
            {
                neighbours = UnionOf(neighbours, Nodes.Single(node => node.Id == element).NeighbourIds);
            }

            return neighbours;
        }
        protected HashSet<int> UnionOf(HashSet<int> setA, HashSet<int> setB) // reason: to prevent overhead of casting btw IEnumerable and HashSet
        {
            if (setA.Count < setB.Count)
            {
                return UnionOf(setB, setA);
            }

            HashSet<int> union = new HashSet<int>(setA.Count + setB.Count);

            foreach (int element in setA)
            {
                union.Add(element);
            }
            foreach (int element in setB)
            {
                union.Add(element); // it is satisfactory, since hashset insures that elements remain unique
            }

            return union;
        }

        #endregion
    }
}