using UnityEngine;

/// <summary>
/// Test runner for the avatar system
/// </summary>
public class AvatarTestRunner : MonoBehaviour
{
    [Tooltip("Configuration Controller for which to which the settings should be applied")]
    public AvatarPartConfigurationController avatarConfigController;

    [Tooltip("The index of the model variant")]
    public int modelIndex;
    [Tooltip("The index of the used material")]
    public int materialIndex;
    [Tooltip("The index of the color variant for the chosen material")]
    public int colorIndex;

    /// <summary>
    /// If F5 is pressed, the chosen configuration is applied to the configuration controller
    /// </summary>
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
