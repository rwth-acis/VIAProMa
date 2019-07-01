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

    private static Dictionary<string, Texture2D> profileImages;

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
            Sprite sprite = Sprite.Create(profileImage, new Rect(0, 0, profileImage.width, profileImage.height), new Vector2(0.5f, 0.5f), 2000f);
            profileImageSprite.sprite = sprite;
        }

        userNameLabel.text = content.firstName + " " + content.lastName;
    }

    private static async Task<Texture2D> GetProfileImage(User user)
    {
        // first check if we downloaded the profile picture before
        if (profileImages.ContainsKey(user.profileImage))
        {
            return profileImages[user.profileImage];
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
                return null;
            }
        }
    }

    private static async Task<ApiResult<Texture2D>> FetchProfileImage(User user)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(user.profileImage);
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
