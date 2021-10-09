using System;
using System.Collections.Generic;

namespace MeetingOrganiserDesktopApp.Model
{
    public class Graph : GenericGraph<Node>
    {
        public Graph(int potentialNumberOfVertices = DEFAULT_NUMBER_OF_NODES) : base(potentialNumberOfVertices) { }
        public void AddNode(int nodeId, int potentialNumberOfNeighbours = DEFAULT_NUMBER_OF_NODES)
        {
            AddNode( new Node(nodeId, potentialNumberOfNeighbours));
        }
        public void AddNode(int nodeId, HashSet<int> neighbourIds)
        {
            AddNode(new Node(nodeId, neighbourIds));
        }
    }
}
