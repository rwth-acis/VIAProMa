using UnityEngine;

namespace i5.VIAProMa.Visualizations.Diagrams.Common.Axes
{
    public class TextFlipper : MonoBehaviour
    {
        private Transform cameraTransform;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Vector3 transformToCam = cameraTransform.position - transform.position;
            if (Vector3.Dot(transformToCam, transform.forward) < 0)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}