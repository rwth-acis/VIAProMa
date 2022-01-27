using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    public class HighlighterAnimation : MonoBehaviour
    {

        private GameObject parent;
        private int activeDirection;
        private bool moveVertically;
        private Vector3 customOffset;
        private bool moveHorizontally;
        private Vector3 startingPosition;

        [SerializeField] private float speed = 0.2f;
        [SerializeField] private float bouncingDistance = 0.1f;

        [SerializeField] private Vector3 offsetTop;
        [SerializeField] private Vector3 offsetLeft;
        [SerializeField] private Vector3 offsetRight;
        [SerializeField] private Vector3 offsetBottom;

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
