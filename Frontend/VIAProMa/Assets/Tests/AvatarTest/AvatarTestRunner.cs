using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarTestRunner : MonoBehaviour
{
    public AvatarConfigurationController avatarConfigController;

    public int modelIndex;
    public int materialIndex;
    public int colorIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            avatarConfigController.ModelIndex = modelIndex;
            avatarConfigController.MaterialIndex = materialIndex;
            avatarConfigController.ColorIndex = colorIndex;

            avatarConfigController.ApplyConfiguration();
        }
    }
}
