using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Displays user data on a GameObject
/// </summary>
public class UserDataDisplay : DataDisplay<User>
{
    [SerializeField] private SpriteRenderer profileImageSprite;
    [SerializeField] TextMeshPro userNameLabel;

    private const float pixelDensity = 2000f / 512f;

    private static Dictionary<string, Texture2D> profileImages = new Dictionary<string, Texture2D>();

    /// <summary>
    /// Checks the setup of the component
    /// </summary>
    private void Awake()
    {
        if (profileImageSprite == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(profileImageSprite));
        }
        if (userNameLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(userNameLabel));
        }
    }

    /// <summary>
    /// Updates the visual display of the content
    /// </summary>
    public override async void UpdateView()
    {
        base.UpdateView();
        if (content != null)
        {
            // update profile image
            Texture2D profileImage = await GetProfileImage(content);
            SetProfileImage(profileImage);

            userNameLabel.text = content.FirstName + " " + content.LastName;
        }
        else
        {
            SetProfileImage(ResourceManager.Instance.DefaultProfileImage);
            userNameLabel.text = "";
        }
    }

    /// <summary>
    /// Sets the profile image to the given texture
    /// </summary>
    /// <param name="profileImage">The texture which should be applied to the profile image view</param>
    private void SetProfileImage(Texture2D profileImage)
    {
        if (profileImageSprite != null)
        {
            float pixelsPerUnit = Mathf.Min(profileImage.width, profileImage.height) * pixelDensity;
            Sprite sprite = Sprite.Create(profileImage, new Rect(0, 0, profileImage.width, profileImage.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            profileImageSprite.sprite = sprite;
        }
    }

    /// <summary>
    /// Gets the profile image of the specified user
    /// The method either fetches the image from cache or from the web address which is specified in the user object
    /// </summary>
    /// <param name="user">The user whose profile image should be fetched</param>
    /// <returns>The profile image of the given user</returns>
    private static async Task<Texture2D> GetProfileImage(User user)
    {
        if (string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            return ResourceManager.Instance.DefaultProfileImage;
        }

        // first check if we downloaded the profile picture before
        if (profileImages.ContainsKey(user.ProfileImageUrl))
        {
            return profileImages[user.ProfileImageUrl];
        }
        else // fetch it from the web
        {
            ApiResult<Texture2D> res = await FetchProfileImage(user);
            if (res.Successful)
            {
                return res.Value;
            }
            else
            {
                Debug.LogError(res.ResponseCode + ": " + res.ErrorMessage);
                return ResourceManager.Instance.DefaultProfileImage;
            }
        }
    }

    /// <summary>
    /// Fetches the profile image of the user from the web
    /// </summary>
    /// <param name="user">The user whose profile image should be fetched</param>
    /// <returns>The profile image of the given user</returns>
    private static async Task<ApiResult<Texture2D>> FetchProfileImage(User user)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(user.ProfileImageUrl);
        await www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (www.GetResponseHeaders() == null)
            {
                return new ApiResult<Texture2D>(0, "Device unavailable");
            }
            return new ApiResult<Texture2D>(www.responseCode, www.downloadHandler.text);
        }
        return new ApiResult<Texture2D>(DownloadHandlerTexture.GetContent(www));

    }
}
