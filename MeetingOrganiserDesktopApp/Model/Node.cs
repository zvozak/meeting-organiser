using System;
using System.Collections.Generic;

namespace MeetingOrganiserDesktopApp.Model
{
    public class Node
    {
        #region Custom exceptions for class Node

        public class NodeIdNotFoundException : ArgumentException
        {
            public NodeIdNotFoundException(int nodeId)
                : base(string.Format(
                    "Node with ID {0} is not among the neighbours."
                    , nodeId))
            { }
        }

        public class NodeIdAlreadyPresent : ArgumentException
        {
            public NodeIdAlreadyPresent(int nodeId)
                : base(string.Format(
                    "Node with ID {0} is already among the neighbours."
                    , nodeId))
            { }
        }
        public class NodeIdNonPositiveException : ArgumentException
        {
            public NodeIdNonPositiveException()
                : base(string.Format(
                    "ID must be positive."))
            { }
        }

        #endregion

        protected const int DEFAULT_NUMBER_OF_NEIGHBOURS = 10;
        public int Id { get; private set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Node))
                return false;
            else
                return Id == ((Node)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
        public HashSet<int> NeighbourIds { get; protected set; }

        public Node(int id, int potentialNumberOfNeighbours = DEFAULT_NUMBER_OF_NEIGHBOURS)
        {
            Id = id;
            if (potentialNumberOfNeighbours < 0)
            {
                throw new ArgumentException(
                    "Number of potential neighbours cannot be negative.");
            }
            this.NeighbourIds = new HashSet<int>(potentialNumberOfNeighbours);
        }

        public Node(int id, HashSet<int> neighbourIds)
        {
            CheckNodeIdValidity(id);

            Id = id;

            if (neighbourIds == null)
            {
                throw new ArgumentNullException();
            }
            this.NeighbourIds = neighbourIds;
        }

        public int GetNumberOfNeighbours()
        {
            return NeighbourIds.Count;
        }
        public bool IsNeighbourOf(int nodeId)
        {
            return NeighbourIds.Contains(nodeId);
        }

        public void AddNeighbour(int neighbourId)
        {
            CheckNodeIdValidity(neighbourId);

            NeighbourIds.Add(neighbourId);
        }
        public bool DeleteNeighbour(int neighbourId)
        {
            CheckNodeIdValidity(neighbourId);

            return NeighbourIds.Remove(neighbourId);
        }

        protected void CheckNodeIdValidity(int nodeId)
        {
            if (nodeId <= 0)
            {
                throw new NodeIdNonPositiveException();
            }
        }
    }

    public class WeightedNode : Node
    {
        private const int DEFAULTWEIGHT = 0;
        public int Weight { get; private set; }     
        

        public WeightedNode(int id, int weight = DEFAULTWEIGHT
            , int potentialNumberOfNeighbours = DEFAULT_NUMBER_OF_NEIGHBOURS)
            : base(id, potentialNumberOfNeighbours)
        {
            if (potentialNumberOfNeighbours < 0)
            {
                throw new ArgumentException("Potential number of nodes cannot be negative.");
            }
            Weight = weight;
        }


        public WeightedNode(int id, HashSet<int> neighbourIds
            , int weight = DEFAULTWEIGHT)
            : base(id, neighbourIds)
        {
            Weight = weight;
        }


        public void SetWeight(int weight)
        {
            Weight = weight;
        }
    }
}