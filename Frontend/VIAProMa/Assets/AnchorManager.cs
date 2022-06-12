using HoloToolkit.Unity;
using UnityEngine;

namespace i5.VIAProMa.Anchoring
{
    public class AnchorManager : Singleton<AnchorManager>
    {
        private bool useAnchor = true;
        [SerializeField] private GameObject anchor;

        public void AttachToAnchor(GameObject objectToAttach)
        {
            if (useAnchor)
            {
                //attach to anchor
                objectToAttach.transform.SetParent(anchor.transform);
            }
        }
    }
}