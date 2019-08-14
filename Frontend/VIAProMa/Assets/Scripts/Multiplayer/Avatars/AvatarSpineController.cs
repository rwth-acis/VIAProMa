using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSpineController : MonoBehaviour
{
    [SerializeField] private Transform chestBone;
    [SerializeField] private Transform neckBone;
    [SerializeField] private Transform headBone;

    private Vector3 position;
    private Vector3 eulerAngles;

    private Vector3 initialChestRotation;
    private Vector3 initialNeckRotation;
    private Vector3 initialHeadRotation;
}
