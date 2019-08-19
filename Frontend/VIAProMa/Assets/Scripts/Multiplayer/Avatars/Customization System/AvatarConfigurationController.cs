using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurationController : MonoBehaviour
{
    [SerializeField] private AvatarPartConfigurationController hairController;
    [SerializeField] private AvatarPartConfigurationController glassesController;
    [SerializeField] private AvatarPartConfigurationController clothesController;

    public AvatarPartConfigurationController HairController { get => hairController; }
    public AvatarPartConfigurationController GlassesController { get => glassesController; }
    public AvatarPartConfigurationController ClothesController { get => clothesController; }
}