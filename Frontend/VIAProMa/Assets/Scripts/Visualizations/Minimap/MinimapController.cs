using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.Minimap;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace i5.VIAProMa.Visualizations.Minimap
{
    public class MinimapController : MonoBehaviour, IVisualizationVisualController
    {
        [Tooltip("All of the minimap items sit on this surface")] [SerializeField]
        private Transform minimapSurface;

        private Vector3 lastPointerPosPos;
        private Vector3 lastPointerPosNeg;

        [Header("UI Elements")] [Tooltip("Reference to the title of the minimap")] [SerializeField]
        private Transform header; // Contains both the title and the background

        [SerializeField] private TextMeshPro headerTitle;
        [SerializeField] private Transform headerBackground;

        [SerializeField] private Transform handleLeft;
        [SerializeField] private Transform handleRight;
        [SerializeField] private Transform handleTop;
        [SerializeField] private Transform handleBottom;
        [SerializeField] private ObjectGrid grid;

        [Tooltip("Reference to the bounding box of the minimap")] [SerializeField]
        private BoundingBox boundingBox;

        [Header("Values")] private List<GameObject> items;
        // Used to resize the minimap from the handles
        private Vector2 size;
        private Renderer backgroundRenderer;
        private Renderer headerBackgroundRenderer;

        private BoxCollider boundingBoxCollider;
        public Visualization[] HighlightedItems { get; set; }

        public string Title
        {
            get => headerTitle.text;
            set
            {
                headerTitle.text = value;
                headerTitle.gameObject.SetActive(!string.IsNullOrEmpty(value));
            }
        }

        public float Width
        {
            get => minimapSurface.localScale.x;
            set
            {
                size.x = value;
                UpdateSize();
            }
        }

        public float Height
        {
            get => minimapSurface.localScale.y;
            set
            {
                size.y = value;
                UpdateSize();
            }
        }

        public Color Color
        {
            get => backgroundRenderer.material.color;
            set => backgroundRenderer.material.color = value;
        }


        private void Awake()
        {
            // TODO
            items = new List<GameObject>();
            backgroundRenderer = minimapSurface.gameObject.GetComponent<Renderer>();
            headerBackgroundRenderer = headerBackground.gameObject.GetComponent<Renderer>();
        }

        public void StartResizing(Vector3 pointerPosition, bool handleOnPositiveCap)
        {
            if (handleOnPositiveCap)
            {
                lastPointerPosPos = pointerPosition;
            }
            else
            {
                lastPointerPosNeg = pointerPosition;
            }
        }

        public void SetHandles(Vector3 PointerPosition, bool handleOnPositiveCap)
        {
            //Vector3 newHandlePosition;
            //if (handleOnPositiveCap)
            //{
            //    newHandlePosition =
            //        new Vector3(capPos.localPosition.x - ProjectOnRight(lastPointerPosPos, PointerPosition), 0, 0);
            //    lastPointerPosPos = PointerPosition;
            //}
            //else
            //{
            //    newHandlePosition =
            //        new Vector3(capNeg.localPosition.x - ProjectOnRight(lastPointerPosNeg, PointerPosition), 0, 0);
            //    lastPointerPosNeg = PointerPosition;
            //}

            //AdjustLengthToHandles(newHandlePosition, handleOnPositiveCap);
        }

        private void AdjustLengthToHandles(Vector3 handlePosition, bool handleOnPositiveCap)
        {
            //Vector3 newHandlePositionPositive;
            //Vector3 newHandlePositionNegative;
            //if (handleOnPositiveCap)
            //{
            //    newHandlePositionPositive = handlePosition;
            //    newHandlePositionNegative = capNeg.localPosition;
            //}
            //else
            //{
            //    newHandlePositionPositive = capPos.localPosition;
            //    newHandlePositionNegative = handlePosition;
            //}

            //float newLength = Vector3.Distance(newHandlePositionPositive, newHandlePositionNegative);
            //if (newLength >= minLength && newLength <= maxLength)
            //{
            //    //Update the tubes
            //    tubes.localScale = new Vector3(newLength, 1f, 1f);

            //    //Update the parent
            //    Vector3 newHandlePositionPositiveWorld =
            //        transform.localToWorldMatrix * new Vector4(newHandlePositionPositive.x, 0, 0, 1);
            //    Vector3 newHandlePositionNegativWorld =
            //        transform.localToWorldMatrix * new Vector4(newHandlePositionNegative.x, 0, 0, 1);
            //    transform.position = newHandlePositionNegativWorld +
            //                         0.5f * (newHandlePositionPositiveWorld - newHandlePositionNegativWorld);

            //    //Update positions of the caps
            //    capPos.position = newHandlePositionPositiveWorld;
            //    capNeg.position = newHandlePositionNegativWorld;

            //    //Update box colliders and bounding box
            //    tubeCollider.height =
            //        newLength +
            //        0.1f; // add 0.1 so that the cylindrical part covers the full length (otherwise it is too short because of the rounded caps)
            //    boundingBoxCollider.size = new Vector3(
            //        newLength + 0.05f, // add 0.05f to encapsulate the end caps
            //        boundingBoxCollider.size.y,
            //        boundingBoxCollider.size.z);

            //    UpdateTextLabelPositioning(newLength);
            //}
        }


        private float ProjectOnRight(Vector3 vector, Vector3 position)
        {
            Vector3 delta = vector - position;
            return Vector3.Dot(transform.right, delta);
        }

        private void UpdateVisuals()
        {
        }

        private void UpdateSize()
        {
            minimapSurface.localScale = new Vector3(
                size.x,
                size.y,
                header.localScale.z);

            headerBackground.localScale = new Vector3(
                size.x,
                headerBackground.localScale.y,
                headerBackground.localScale.z);

            handleLeft.localPosition = new Vector3(
                -size.x / 2f,
                0f,
                0.01f
            );
            handleRight.localPosition = new Vector3(
                size.x / 2f,
                0f,
                0.01f
            );
            handleTop.localPosition = new Vector3(
                0f,
                size.y / 2f,
                0.01f
            );

            handleBottom.localPosition = new Vector3(
                0f,
                -size.y / 2f,
                0.01f
            );

            boundingBoxCollider.size = 1.01f * minimapSurface.localScale;

            UpdateVisuals();
        }
    }
}