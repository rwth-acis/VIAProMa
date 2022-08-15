using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Class for the citation network node, used by the citation network.
    /// </summary>
    public class CitationNetworkNode : MonoBehaviour
    {
        /// <summary>
        /// Content of the citation network node.
        /// </summary>
        public Paper Content { get; set; }

        /// <summary>
        /// Children of the citation network node.
        /// </summary>
        public List<CitationNetworkNode> Children { get; set; } = new List<CitationNetworkNode>();

        /// <summary>
        /// Checks whether the node is a leaf node, i.e. has no children.
        /// </summary>
        /// <returns>true if node is leaf.</returns>
        public bool IsLeaf()
        {
            return Children == null || Children.Count == 0;
        }

        /// <summary>
        /// Gets the node corrisponding to a paper in the node and its descendents.
        /// </summary>
        /// <param name="paper">Paper to search for.</param>
        /// <returns>Node with content <paramref name="paper"/> if it exists, otherwise null.</returns>
        public CitationNetworkNode GetNode(Paper paper)
        {
            if (Content.Equals(paper))
            {
                return this;
            }
            if (IsLeaf()) 
            {
                return null; 
            }
            for (int i = 0; i < Children.Count; i++)
            {
                CitationNetworkNode node = Children[i].GetNode(paper);
                if (!(node is null))
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all papers down from a node.
        /// </summary>
        /// <returns></returns>
        public List<Paper> GetAllPapers()
        {
            if (IsLeaf())
            {
                List<Paper> papers = new List<Paper>();
                papers.Add(Content);
                return papers;
            }
            else
            {

                List<Paper> papers = new List<Paper>();
                papers.Add(Content);
                for (int i = 0; i < Children.Count; i++)
                {
                    papers.AddRange(Children[i].GetAllPapers());
                }
                return papers;
            }

        }

        /// <summary>
        /// Returns the amount of layers in the citation network.
        /// </summary>
        /// <returns>Number of layers.</returns>
        public int GetLayers()
        {
            if (IsLeaf())
            {
                return 1;
            }
            int max = 0;
            foreach (CitationNetworkNode node in Children)
            {
                int layers = node.GetLayers();
                if (layers > max)
                {
                    max = layers;
                }
            }
            return max + 1;

        }

        /// <summary>
        /// Turns citation network node to string.
        /// </summary>
        /// <returns>Node in string form.</returns>
        public override string ToString()
        {
            if (IsLeaf())
            {
                return "| " + Content.DOI + " |";
            }

            string output = Content.DOI + " (";
            foreach (CitationNetworkNode node in Children)
            {
                output += node.ToString();
            }
            return output + ")";
        }

        /// <summary>
        /// Checks whether an object is the same citation network node.
        /// </summary>
        /// <param name="other">Other object.</param>
        /// <returns>true if both are the same.</returns>
        public override bool Equals(object other)
        {
            CitationNetworkNode node = other as CitationNetworkNode;
            if (node == null)
            {
                return false;
            }
            return Content.Equals(node.Content);
        }
        /// <summary>
        /// Checks whether a node is the same citation network node.
        /// </summary>
        /// <param name="node">Other node.</param>
        /// <returns>true if both are the same.</returns>
        public bool Equals(CitationNetworkNode node)
        {
            if (node == null)
            {
                return false;
            }
            return Content.Equals(node.Content);
        }
        /// <summary>
        /// Gets the hash code of the citation network node.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets all nodes below current node.
        /// </summary>
        /// <returns>All nodes below.</returns>
        public List<CitationNetworkNode> GetAllNodes()
        {

            List<CitationNetworkNode> nodes = new List<CitationNetworkNode>();
            nodes.Add(this);
            if (IsLeaf())
            {
                return nodes;
            }
            foreach(CitationNetworkNode node in Children)
            {
                nodes.AddRange(node.GetAllNodes());
            }
            return nodes;

        }

        /// <summary>
        /// Gets all the edges downward from the citation node.
        /// </summary>
        /// <returns>List of all edges if edges exist, otherwise empy list.</returns>
        public List<(CitationNetworkNode, CitationNetworkNode)> GetConnections()
        {
            List<(CitationNetworkNode, CitationNetworkNode)> connections = new List<(CitationNetworkNode, CitationNetworkNode)>();
            if (IsLeaf())
            {
                return connections;
            }
            foreach (CitationNetworkNode node in Children)
            {
                connections.Add((this, node));
            }
            foreach (CitationNetworkNode node in Children)
            {
                connections.AddRange(node.GetConnections());
            }
            return connections;
        }
    }

}
