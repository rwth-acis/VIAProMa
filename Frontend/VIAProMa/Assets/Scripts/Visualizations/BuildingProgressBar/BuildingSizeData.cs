using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.BuildingProgressBar
{
    /// <summary>
    /// Stores information about the building's size on different levels
    /// </summary>
    public class BuildingSizeData : MonoBehaviour
    {
        [Tooltip("The overall height of the building")]
        [SerializeField] private float buildingHeight = 1f;

        [Tooltip("The array of height sizes")]
        [SerializeField] private List<Vector3> heightSizes;

        /// <summary>
        /// The overall height of the building
        /// </summary>
        public float BuildingHeight { get => buildingHeight; }

        /// <summary>
        /// Called in the editor; This method visualizes the sizes which have been entered as red, semi transparent cuboids
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            foreach (Vector3 heightSize in heightSizes)
            {
                // Draw a semitransparent blue cube at the transforms position
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(transform.position + new Vector3(0, transform.lossyScale.y * heightSize.y, 0), Vector3.Scale(transform.lossyScale, new Vector3(heightSize.x, 0.001f, heightSize.z)));
            }
        }

        /// <summary>
        /// Gets the size of a building on a particular height
        /// Interpolates between the input building size planes between the given height
        /// </summary>
        /// <param name="height">The height where the size of the building should be sampled</param>
        /// <returns>The 2D size of the building on this level</returns>
        public Vector2 GetBuildingSize(float height)
        {
            heightSizes = heightSizes.OrderBy(o => o.y).ToList();
            for (int i = 0; i < heightSizes.Count; i++)
            {
                if (height <= heightSizes[i].y)
                {
                    if (i == 0)
                    {
                        return new Vector2(heightSizes[i].x, heightSizes[i].z);
                    }
                    else
                    {
                        float intervalRatio = (height - heightSizes[i - 1].y) / (heightSizes[i].y - heightSizes[i - 1].y);
                        Vector2 previousSize = new Vector2(heightSizes[i - 1].x, heightSizes[i - 1].z);
                        Vector2 nextSize = new Vector2(heightSizes[i].x, heightSizes[i].z);
                        return Vector2.Lerp(previousSize, nextSize, intervalRatio);
                    }
                }
            }
            return new Vector2(heightSizes[heightSizes.Count - 1].x, heightSizes[heightSizes.Count - 1].z);
        }

        /// <summary>
        /// Gets the bounds of the set size planes
        /// </summary>
        /// <returns>The bounds of the size planes</returns>
        public Bounds GetBounds()
        {
            if (heightSizes.Count == 0)
            {
                return new Bounds();
            }
            Bounds bounds = new Bounds(new Vector3(0, heightSizes[0].y, 0), new Vector3(heightSizes[0].x, 0.001f, heightSizes[0].z));
            for (int i = 1; i < heightSizes.Count; i++)
            {
                Bounds cubeBounds = new Bounds(new Vector3(0, heightSizes[i].y, 0), new Vector3(heightSizes[i].x, 0.001f, heightSizes[i].z));
                bounds.Encapsulate(cubeBounds);
            }
            return bounds;
        }
    }
}