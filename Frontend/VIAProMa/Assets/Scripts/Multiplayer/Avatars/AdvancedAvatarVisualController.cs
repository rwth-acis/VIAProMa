using Photon.Pun;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars
{
    /// <summary>
    /// Controls the visiblilty of the avatar
    /// </summary>
    public class AdvancedAvatarVisualController : MonoBehaviourPun
    {
        /// <summary>
        /// Initializes the component
        /// Deactivates the avatar's visual representation if it is the own avatar
        /// If the avatar represents a remote player, it displays the player's name
        /// </summary>
        private void Start()
        {
            if (photonView.IsMine)
            {
                SetVisibility(false);
            }
            else
            {
                SetVisibility(true);
            }
        }

        /// <summary>
        /// Makes sure that all child objects of the avatar are turned on/off
        /// It is not possible to use gameobject.SetActive because the logic on the gameobject should still be executed
        /// Therefore, only the children are deactivated if visibility is set to false
        /// </summary>
        /// <param name="visibility">States if the avatar should be visible</param>
        private void SetVisibility(bool visibility)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(visibility);
            }
        }
    }
}