using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetingOrganiserDesktopApp.Model
{
    public class NodeWeightedGraph : GenericGraph<WeightedNode>
    {
        public class InvalidWeightException : ArgumentException
        {
            public InvalidWeightException()
               : base("Weight cannot be negative.") { }
        }

        public NodeWeightedGraph(int potentialNumberOfVertices = DEFAULT_NUMBER_OF_NODES) : base(potentialNumberOfVertices) { }

        public NodeWeightedGraph(GenericGraph<WeightedNode> genericGraph)
        {
            Nodes = genericGraph.Nodes;
            NumberOfEdges = genericGraph.NumberOfEdges;
            NumberOfNodes = genericGraph.NumberOfNodes;
        }

        public int GetWeightOf(int nodeId)
        {
            CheckNodeIdValidity(nodeId);

            try
            {
                return Nodes.Single(node => node.Id == nodeId).Weight;
            }
            catch
            {
                throw new NodeIdNotFoundException(nodeId);
            }
        }

        public void AddNode(int nodeId, int weight = 0, int potentialNumberOfNeighbours = DEFAULT_NUMBER_OF_NODES)
        {
            if (potentialNumberOfNeighbours < 0)
            {
                throw new ArgumentException("Potential number of neighbours cannot be negative.");
            }

            CheckWeightValidity(weight);
            CheckNodeIdValidity(nodeId);

            if (!Nodes.Add( new WeightedNode(
                    id: nodeId,
                    weight: weight,
                    potentialNumberOfNeighbours: potentialNumberOfNeighbours)))
                throw new NodeIdAlreadyPresentException(nodeId);
            NumberOfNodes++;
        }

        public void SetWeightOfNode(int nodeId, int weight)
        {
            CheckWeightValidity(weight);
            CheckNodeIdValidity(nodeId);

            Nodes.Single(n => n.Id == nodeId).SetWeight(weight);
        }

        public void AddNode(int nodeId, HashSet<int> neighbourIds, int weight = 0)
        {
            CheckWeightValidity(weight);

            Nodes.Add(new WeightedNode(
                id: nodeId,
                neighbourIds: neighbourIds, 
                weight: weight));
        }

        protected void CheckWeightValidity(int weight)
        {
            if (weight < 0)
            {
                throw new InvalidWeightException();
            }
        }
        public HashSet<int> GetIdsOfGivenWeight(int weight)
        {
            HashSet<int> ids = new HashSet<int>(NumberOfNodes);
            foreach (int node in GetIdsOfNodes())
            {
                if (GetWeightOf(node) == weight)
                {
                    ids.Add(node);
                }
            }

            return ids;
        }

        public HashSet<int> ConstructDominatingSetWithWeights()
        {
            HashSet<int> ds = new HashSet<int>(NumberOfNodes);

            ds = UnionOf(ds, GetNeighboursOfLeaves());

            HashSet<int> nodesNotCoveredYet = SetDifferenceOf(
                GetIdsOfNodes(),
                UnionOf(ds, GetNeighboursOf(ds)));
            ds = UnionOf(ds, ConstructDominatingSetFrom(ref nodesNotCoveredYet));

            return ds;
        }
        public HashSet<int> ConstructConnectedDominatingSet_WithTCDS()
        {
            CheckIfGraphIsConnected();

            HashSet<int> cds = ConstructDominatingSetWithWeights();

            cds = UnionOf(cds, GetNeighboursOfLeaves());

            HashSet<int> nodesNotCoveredYet = SetDifferenceOf(
                GetIdsOfNodes(),
                UnionOf(cds, GetNeighboursOf(cds)));
            cds = UnionOf(cds, ConstructDominatingSetFrom(ref nodesNotCoveredYet));

            HashSet<int> connectors;
            AddConnectorsTo_WithTCDS(cds, out connectors);

            Prune_WithTCDS(ref cds, connectors);

            return cds;
        }
        protected void AddConnectorsTo_WithTCDS(HashSet<int> dominatingSet, out HashSet<int> connectors)
        {
            connectors = new HashSet<int>(NumberOfNodes);
            HashSet<int> potentialConnectors = SetDifferenceOf(GetIdsOfNodes(), dominatingSet);

            while (!IsGraphConnectedSpanBy(dominatingSet))
            {
                int bestConnector = potentialConnectors.ElementAt(0);
                int minNumberOfComponents = NumberOfNodes;

                foreach (int potentialConnector in potentialConnectors)
                {
                    dominatingSet.Add(potentialConnector);

                    int numberOfComponents = NumberOfComponentsInGraphSpanBy(dominatingSet);

                    if (IsBetterConnector_WithTCDS(
                        potentialConnector, bestConnector,
                        numberOfComponents, minNumberOfComponents))
                    {
                        bestConnector = potentialConnector;
                    }

                    dominatingSet.Remove(potentialConnector);
                }

                dominatingSet.Add(bestConnector);
                connectors.Add(bestConnector);
                potentialConnectors.Remove(bestConnector);
            }
        }
        protected bool IsBetterConnector_WithTCDS(int supposedlyBetter, int other, int numberOfComponentsWithBetter = 0, int numberOfComponentsWithOther = 0)
        {
            return numberOfComponentsWithBetter < numberOfComponentsWithOther
                        ||
                        (numberOfComponentsWithBetter == numberOfComponentsWithOther &&
                        GetNumberOfNeighbours(supposedlyBetter) > GetNumberOfNeighbours(other))
                        ||
                        (numberOfComponentsWithBetter == numberOfComponentsWithOther &&
                        GetNumberOfNeighbours(supposedlyBetter) == GetNumberOfNeighbours(other) &&
                        GetWeightOf(supposedlyBetter) > GetWeightOf(other));
        }

        protected void Prune_WithTCDS(ref HashSet<int> cds, in HashSet<int> connectors)
        {
            while (connectors.Count > 0)
            {
                int worstConnector = connectors.ElementAt(0);
                foreach (int connector in connectors)
                {
                    if (IsBetterConnector_WithTCDS(worstConnector, connector))
                    {
                        worstConnector = connector;
                    }
                }
                cds.Remove(worstConnector);
                if (cds.Count == 0 || !IsGraphConnectedSpanBy(cds))
                {
                    cds.Add(worstConnector);
                }
                connectors.Remove(worstConnector);
            }
        }
    }
}