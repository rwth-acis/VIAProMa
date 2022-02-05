using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.BuildingProgressBar
{
    [Serializable]
    public class SizeAndOffset
    {
        public float height;
        public Vector2 heightSize;
        public Vector2 offset;
    }

    /// <summary>
    /// Stores information about the building's size on different levels
    /// </summary>
    public class BuildingSizeData : MonoBehaviour
    {
        [Tooltip("The overall height of the building")]
        [SerializeField] private float buildingHeight = 1f;

        [Tooltip("The array of height sizes and offsets")]
        [SerializeField] private List<SizeAndOffset> heightSizesAndOffsets;

        /// <summary>
        /// The overall height of the building
        /// </summary>
        public float BuildingHeight { get => buildingHeight; }

        /// <summary>
        /// Called in the editor; This method visualizes the sizes which have been entered as red, semi transparent cuboids
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            foreach (SizeAndOffset heightSizeAndOffset in heightSizesAndOffsets)
            {
                // Draw a semitransparent blue cube at the transforms position
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(transform.position + new Vector3(0, transform.lossyScale.y * heightSizeAndOffset.height, 0) //Places the cube at the center of the building and on the right height
                    + new Vector3(transform.lossyScale.x*heightSizeAndOffset.offset.x, 0, transform.lossyScale.z * heightSizeAndOffset.offset.y) //Add the offset
                    , Vector3.Scale(transform.lossyScale, new Vector3(heightSizeAndOffset.heightSize.x, 0.001f, heightSizeAndOffset.heightSize.y)));
            }
        }

        //Calculates the smallest index which height is bigger than the given height
        private int getSizeAndOffsetIndex(float height)
        {
            heightSizesAndOffsets = heightSizesAndOffsets.OrderBy(o => o.height).ToList();
            for (int i = 0; i < heightSizesAndOffsets.Count; i++)
            {
                if (height <= heightSizesAndOffsets[i].height)
                {
                    return i;
                }
            }
            return heightSizesAndOffsets.Count - 1;
        }

        //Interpolates linear between previousVector and nextVector depending on where wantedHeight lies in the interval from previouseHeight to nextHeight
        private Vector2 interpolateBetweenHeightSizes(Vector2 previousVector, float previouseHeight, Vector2 nextVector, float nextHeight, float wantedHeight)
        {
            float intervalRatio = (wantedHeight - previouseHeight) / (nextHeight - previouseHeight);
            return Vector2.Lerp(previousVector, nextVector, intervalRatio);
        }

        /// <summary>
        /// Gets the size of a building on a particular height
        /// Interpolates between the input building size planes between the given height
        /// </summary>
        /// <param name="height">The height where the size of the building should be sampled</param>
        /// <returns>The 2D size of the building on this level</returns>
        public Vector2 GetBuildingSize(float height)
        {
            int i = getSizeAndOffsetIndex(height);
            Vector2 heightSize = heightSizesAndOffsets[i].heightSize;
            

            if (i == 0)
            {
                return heightSize;
            }
            else
            {
                Vector2 previousHeightSize = heightSizesAndOffsets[i - 1].heightSize;
                float previousHeight = heightSizesAndOffsets[i - 1].height;
                float nextHeight = heightSizesAndOffsets[i].height;
                return interpolateBetweenHeightSizes(previousHeightSize, previousHeight,
                                                     heightSize, nextHeight, height);
            }
        }

        /// <summary>
        /// Gets the offset of a building on a particular height
        /// Interpolates between the input building offset values between the given height
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public Vector2 GetOffset(float height)
        {
            int i = getSizeAndOffsetIndex(height);
            Vector2 offset = heightSizesAndOffsets[i].offset;
           

            if (i == 0)
            {
                return offset;
            }
            else
            {
                Vector3 previousOffset = heightSizesAndOffsets[i-1].offset;
                float previousHeight = heightSizesAndOffsets[i - 1].height;
                float nextHeight = heightSizesAndOffsets[i].height;
                return interpolateBetweenHeightSizes(previousOffset, previousHeight,
                                                     offset, nextHeight, height);
            }
        }

        /// <summary>
        /// Gets the bounds of the set size planes
        /// </summary>
        /// <returns>The bounds of the size planes</returns>
        public Bounds GetBounds()
        {
            if (heightSizesAndOffsets.Count == 0)
            {
                return new Bounds();
            }
            Bounds bounds = new Bounds(new Vector3(heightSizesAndOffsets[0].offset.x, heightSizesAndOffsets[0].height, heightSizesAndOffsets[0].offset.y), new Vector3(heightSizesAndOffsets[0].heightSize.x, 0.001f, heightSizesAndOffsets[0].heightSize.y));
            for (int i = 1; i < heightSizesAndOffsets.Count; i++)
            {
                Bounds cubeBounds = new Bounds(new Vector3(heightSizesAndOffsets[0].offset.x, heightSizesAndOffsets[i].height, heightSizesAndOffsets[0].offset.y), new Vector3(heightSizesAndOffsets[i].heightSize.x, 0.001f, heightSizesAndOffsets[i].heightSize.y));
                bounds.Encapsulate(cubeBounds);
            }
            return bounds;
        }
    }
}