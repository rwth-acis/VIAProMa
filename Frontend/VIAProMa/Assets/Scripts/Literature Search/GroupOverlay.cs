using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Class fot the overlay of the paper group.
    /// </summary>
    public class GroupOverlay : MonoBehaviour
    {
        /// <summary>
        /// Function is called when the overlay is destroyed and it takes the whole paper group with it.
        /// </summary>
        private void OnDestroy()
        {
            PaperGroup parent = this.GetComponentInParent<PaperGroup>();
            if(parent != null)
            {
                Destroy(parent.gameObject);
            }
        }
    }

}
