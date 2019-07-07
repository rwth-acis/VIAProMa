using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UserDataDisplay : DataDisplay<User>
{
    [SerializeField] private SpriteRenderer profileImageSprite;
    [SerializeField] TextMeshPro userNameLabel;

    private const float pixelDensity = 2000f / 512f;

    private static Dictionary<string, Texture2D> profileImages = new Dictionary<string, Texture2D>();

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

    public override async void UpdateView()
    {
        base.UpdateView();
        // update profile image
        Texture2D profileImage = await GetProfileImage(content);
        if (profileImage == null)
        {
            // use the standard profile image instead
        }
        else
        {
            float pixelsPerUnit = Mathf.Min(profileImage.width, profileImage.height) * pixelDensity;
            Sprite sprite = Sprite.Create(profileImage, new Rect(0, 0, profileImage.width, profileImage.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            profileImageSprite.sprite = sprite;
        }

        userNameLabel.text = content.FirstName + " " + content.LastName;
    }

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
