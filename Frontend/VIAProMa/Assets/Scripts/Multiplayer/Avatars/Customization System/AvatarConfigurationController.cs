using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurationController : MonoBehaviour
{
    [SerializeField] private AvatarPartControllerCategory[] avatarPartControllers;

    public AvatarPartControllerCategory[] AvatarPartControllers { get => avatarPartControllers; }
}