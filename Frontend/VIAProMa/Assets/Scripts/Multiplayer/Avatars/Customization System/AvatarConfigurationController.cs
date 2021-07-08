using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurationController : MonoBehaviour
{
    [SerializeField] private AvatarPartControllerCategory[] avatarPartControllers;

    public int SelectedFace { get => FaceCategory.ConfigurationController.ModelIndex; }

    public AvatarPartControllerCategory[] AvatarPartControllers { get => avatarPartControllers; }

    public AvatarPartControllerCategory FaceCategory { get => avatarPartControllers[0]; }

    public AvatarPartControllerCategory HairCategory { get => avatarPartControllers[1]; }

    public AvatarPartControllerCategory EyesCategory { get => avatarPartControllers[2]; }

    public AvatarPartControllerCategory GlassesCategory { get => avatarPartControllers[3]; }

    public AvatarPartControllerCategory ClothesCategory { get => avatarPartControllers[4]; }

    //public AvatarPartControllerCategory InformationCategory { get => avatarPartControllers[5]}

    /// <summary>
    /// Initializes the avatar by apply the initial configuration
    /// </summary>
    private void Start()
    {
        foreach(AvatarPartControllerCategory category in avatarPartControllers)
        {
            category.ConfigurationController.ApplyConfiguration();
        }
        // the face model is important: if it changes, all variants must be changed on all parts
        FaceCategory.ConfigurationController.ModelChanged += FaceModelChanged;
    }

    /// <summary>
    /// Called if the face part controller changes the model
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">The event arguments</param>
    private void FaceModelChanged(object sender, EventArgs e)
    {
        // go over all part controllers and update their avatar index and apply the new configuration
        foreach(AvatarPartControllerCategory category in avatarPartControllers)
        {
            category.ConfigurationController.AvatarIndex = SelectedFace;
            category.ConfigurationController.ApplyConfiguration();
        }
    }
}