using UnityEngine;

namespace i5.VIAProMa.Utilities
{
    /// <summary>
    /// General purpose script which aligns the child transforms of this transform with the given offset
    /// This way, horizontal and vertical arrays can be realized but also arbitrary lines of objects
    /// The component does not regard the bounding boxes of the child objects; alignment is done with regard to the offset
    /// </summary>
    public class ObjectArray : MonoBehaviour
    {
        public Vector3 offset;

        /// <summary>
        /// Aligns the child objects of this transform
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localPosition = offset * i;
            }
        }

        /// <summary>
        /// Updates the alignment in edit mode if the offset is changed
        /// </summary>
        private void OnValidate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localPosition = offset * i;
            }
        }
    }
}