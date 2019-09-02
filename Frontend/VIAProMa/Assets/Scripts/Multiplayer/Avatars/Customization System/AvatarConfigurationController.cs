using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurationController : MonoBehaviour
{
    [SerializeField] private AvatarPartControllerCategory[] avatarPartControllers;

    public AvatarPartControllerCategory[] AvatarPartControllers { get => avatarPartControllers; }

    public AvatarPartControllerCategory FaceCategory { get => avatarPartControllers[0]; }

    public AvatarPartControllerCategory HairCategory { get => avatarPartControllers[1]; }

    public AvatarPartControllerCategory GlassesCategory { get => avatarPartControllers[2]; }

    public AvatarPartControllerCategory ClothesCategory { get => avatarPartControllers[3]; }

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