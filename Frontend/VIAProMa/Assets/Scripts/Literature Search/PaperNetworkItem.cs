using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class PaperNetworkItem : MonoBehaviour
    {
        public CitationNetworkNode Node { get; set; }
      
        /// <summary>
        /// Highlights the node.
        /// </summary>
        public void OnHighlightNode()
        {
            PaperController.Instance.HighlightNode(Node);
        }   
    
    }

}
