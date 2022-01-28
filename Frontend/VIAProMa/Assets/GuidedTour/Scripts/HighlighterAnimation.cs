using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{

    /**
     * <summary>
     * This creates a bouncing animation for highlighter-arrows, that sit next to, on top or under Buttons and text-fields 
     * and point directly at them. 
     * </summary>
     */
    public class HighlighterAnimation : MonoBehaviour
    {

        private GameObject parent;
        private int activeDirection;
        private bool moveVertically;
        private Vector3 customOffset;
        private bool moveHorizontally;
        private Vector3 startingPosition;

        // The speed of the bouncing animation
        [SerializeField] private float speed = 0.2f;

        // How far the arrow will bounce
        [SerializeField] private float bouncingDistance = 0.1f;

        // These are standardized values that can be changed in the arrow-prefab.
        // From our testing they should work fine with most objects. 
        // If adjusting is required, use the indicatorOffset in the Interactable- and KeyboardTourTask in the Inspector
        [SerializeField] private Vector3 offsetTop;
        [SerializeField] private Vector3 offsetLeft;
        [SerializeField] private Vector3 offsetRight;
        [SerializeField] private Vector3 offsetBottom;

        /**
         * <summary>
         * Sets up the positions and rotations of the highlighter arrows, depending on the settings in the
         * Interactable- or KeyboardTourTasks. 
         * </summary>
         */
        internal void HighlighterSetUP(int direction, GameObject highlightedObject, Vector3 userOffset) {
            parent = highlightedObject;
            customOffset = userOffset;
            switch (direction) {
                // point right
                case 0:
                    activeDirection = 0;
                    SetStartingPosition(0);
                    transform.position = (transform.position + offsetLeft + customOffset);
                    transform.Rotate(0, 90, 0);
                    moveVertically = false;
                    moveHorizontally = true;
                    break;
                // point down
                case 1:
                    activeDirection = 1;
                    SetStartingPosition(1);
                    transform.position = (transform.position + offsetTop + customOffset);
                    transform.Rotate(0, 180, 0);
                    moveVertically = true;
                    break;
                // point left
                case 2:
                    activeDirection = 2;
                    SetStartingPosition(2);
                    transform.position = (transform.position + offsetRight + customOffset);
                    transform.Rotate(0, -90, 0);
                    moveVertically = false;
                    moveHorizontally = true;
                    break;
                // point up
                case 3:
                    activeDirection = 3;
                    SetStartingPosition(3);
                    transform.position = (transform.position + offsetBottom + customOffset);
                    transform.Rotate(180, -180, 0);
                    moveVertically = true;
                    break;
            }

        }

        /**
         * <summary>
         * This sets the position, from which the arrow will start it's bouncing.
         * It must be updated in case the object, that the arrow points towards, is moved in the scene.
         * </summary>
         */
        private void SetStartingPosition(int pos) {
            switch (pos) {
                case 0:
                    startingPosition = (parent.transform.position + offsetLeft + customOffset);
                    break;
                case 1:
                    startingPosition = (parent.transform.position + offsetTop + customOffset);
                    break;
                case 2:
                    startingPosition = (parent.transform.position + offsetRight + customOffset);
                    break;
                case 3:
                    startingPosition = (parent.transform.position + offsetBottom + customOffset);
                    break;
            }

        }

        /**
         * <summary>
         * This ensures, that the arrow will always point at the desired object, even if that one is moved in the scene.
         * </summary>
         */
        private void Update() {
            if (gameObject.activeInHierarchy) {
                if (moveVertically) {
                    transform.position = new Vector3(parent.transform.position.x, Mathf.PingPong(Time.time * speed, startingPosition.y + bouncingDistance - startingPosition.y) + startingPosition.y, parent.transform.position.z);
                }
                else if (moveHorizontally) {
                    transform.position = new Vector3(Mathf.PingPong(Time.time * speed, startingPosition.x + bouncingDistance - startingPosition.x) + startingPosition.x, parent.transform.position.y, parent.transform.position.z);
                }

                SetStartingPosition(activeDirection);
            }
        }

    }
}
