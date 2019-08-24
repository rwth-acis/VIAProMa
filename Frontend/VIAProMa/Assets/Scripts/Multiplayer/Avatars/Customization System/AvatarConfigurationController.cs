using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurationController : MonoBehaviour
{
    [SerializeField] private AvatarPartControllerCategory[] avatarPartControllers;

    public AvatarPartControllerCategory[] AvatarPartControllers { get => avatarPartControllers; }

    /// <summary>
    /// Initializes the avatar by apply the initial configuration
    /// </summary>
    private void Start()
    {
        foreach(AvatarPartControllerCategory category in avatarPartControllers)
        {
            category.ConfigurationController.ApplyConfiguration();
        }
    }
}