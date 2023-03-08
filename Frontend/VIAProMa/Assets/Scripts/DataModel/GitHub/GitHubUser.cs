using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.UI.ListView.Core;
using System;
using UnityEngine;

namespace i5.VIAProMa.DataModel.GitHub
{
    /// <summary>
    /// A generalized user from one of the data sources
    /// </summary>
    [Serializable]
    public class GitHubUser : IListViewItemData
    {
        [SerializeField] private int id;
        [SerializeField] private string login;
        [SerializeField] private string firstName;
        [SerializeField] private string lastName;
        [SerializeField] private string avatar_url;
        [SerializeField] private string eMail;

        /// <summary>
        /// The id of the user on the given source
        /// </summary>
        public int Id { get => id; }
        /// <summary>
        /// The profile's nick name
        /// </summary>
        public string UserName { get => login; }
        /// <summary>
        /// The first name of the user
        /// </summary>
        public string FirstName { get => firstName; }
        /// <summary>
        /// The last name of the user
        /// </summary>
        public string LastName { get => lastName; }
        /// <summary>
        /// The url to the profile image
        /// </summary>
        public string ProfileImageUrl { get => avatar_url; }
        /// <summary>
        /// The users email
        /// </summary>
        public string EMail { get => eMail; }

        public GitHubUser()
        {
        }

        public GitHubUser(int id, string login, string firstName, string lastName, string avatar_url, string eMail)
        {
            this.id = id;
            this.login = login;
            this.firstName = firstName;
            this.lastName = lastName;
            this.avatar_url = avatar_url;
            this.eMail = eMail;
        }
    }
}