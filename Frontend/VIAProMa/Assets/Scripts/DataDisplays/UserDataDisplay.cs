using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

namespace i5.VIAProMa.DataDisplays
{

    /// <summary>
    /// Displays user data on a GameObject
    /// </summary>
    public class UserDataDisplay : DataDisplay<User>
    {
        [SerializeField] private Renderer profileImageRenderer;
        [SerializeField] TextMeshPro userNameLabel;

        private const float pixelDensity = 2000f / 512f;

        private static Dictionary<string, Texture2D> profileImages = new Dictionary<string, Texture2D>();

        private static Dictionary<int, string> nonfunctionalProfileURLs = new Dictionary<int, string>();

        /// <summary>
        /// Checks the setup of the component
        /// </summary>
        private void Awake()
        {
            if (profileImageRenderer == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(profileImageRenderer));
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

                // prefer clear names but if they are not given, take the user name instead
                if (!string.IsNullOrEmpty(content.FirstName) || !string.IsNullOrEmpty(content.LastName))
                {
                    userNameLabel.text = $"{content.FirstName} {content.LastName}";
                }
                else
                {
                    userNameLabel.text = content.UserName;
                }
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
            if (profileImageRenderer != null)
            {
                profileImageRenderer.material.mainTexture = profileImage;
                if (profileImage.width == 0 || profileImage.height == 0)
                {
                    return;
                }
                // adjust the scaling to avoid stretching
                float minSize = Mathf.Min(profileImage.width, profileImage.height);
                Vector2 scale = new Vector2(minSize / profileImage.width, minSize / profileImage.height);
                profileImageRenderer.material.mainTextureScale = scale;
                profileImageRenderer.material.mainTextureOffset = 0.5f * (Vector2.one - scale);
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
            // check whether the url can be used to fetch a profile image
            if (string.IsNullOrEmpty(user.ProfileImageUrl) || user.ProfileImageUrl == "https://api.learning-layers.eu/profile.png" || nonfunctionalProfileURLs.ContainsKey(user.Id))
            {
                // try and fetch the profile image from Gravatar 
                if (user.EMail != null)
                {
                    ApiResult<Texture2D> res = await FetchProfileImageFromGravatar(user);
                    if (res.Successful)
                    {
                        return res.Value;
                    }
                    else
                    {
                        return ResourceManager.Instance.DefaultProfileImage;
                    }
                }
                // otherwise the default profile image
                else
                {
                return ResourceManager.Instance.DefaultProfileImage;
                }
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
                    // Check and add the url to the dictionary, as it is not functional
                    if (!nonfunctionalProfileURLs.ContainsKey(user.Id))
                    {
                        nonfunctionalProfileURLs.Add(user.Id, user.ProfileImageUrl);
                        Debug.LogError(res.ResponseCode + ": " + res.ErrorMessage);
                        Debug.Log("The profile image of the user " + user.UserName + " could not be fetched.");
                    }

                    // try and fetch the profile image from Gravatar instead
                    if (user.EMail != null)
                    {
                        res = await FetchProfileImageFromGravatar(user);
                        if (res.Successful)
                        {
                            return res.Value;
                        }
                        else
                        {
                            return ResourceManager.Instance.DefaultProfileImage;
                        }
                    }
                    // otherwise the default profile image
                    else
                    {
                        return ResourceManager.Instance.DefaultProfileImage;
                    }
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

        /// <summary>
        /// Fetches the profile image of the user from Gravatar, if no other profile is provided
        /// </summary>
        /// <param name="user">The user whose profile image should be fetched</param>
        /// <returns>The profile image of the given user</returns>
        private static async Task<ApiResult<Texture2D>> FetchProfileImageFromGravatar(User user)
        {
            // create hash from email
            string hashedEmail = CreateMD5(user.EMail.Trim().ToLower());

            // request gravatar image
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://www.gravatar.com/avatar/" + hashedEmail + "?r=g&d=404");
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

        /// <summary>
        /// Creates a MG5 hash of a string
        /// </summary>
        /// <param name="email">The email address that is to be hashed</param>
        /// <returns>The result hash</returns>
        public static string CreateMD5(string email)
        {
            using (MD5 md5 = MD5.Create())
            {
                // applies the MD5 hash
                byte[] inputBytes = Encoding.ASCII.GetBytes(email);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // convert hash to string
                StringBuilder finalHash = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    finalHash.Append(hashBytes[i].ToString("X2"));
                }
                return finalHash.ToString().ToLower();
            }
        }

    }

}