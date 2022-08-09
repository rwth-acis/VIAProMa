using i5.VIAProMa.UI.ListView.Core;
using System;
using UnityEngine;

namespace i5.VIAProMa.DataModel.API
{
    /// <summary>
    /// A generalized user from one of the data sources
    /// </summary>
    [Serializable]
    public class User : IListViewItemData
    {
        [SerializeField] private DataSource source;
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private string firstName;
        [SerializeField] private string lastName;
        [SerializeField] private string profileImageUrl;
        [SerializeField] private string eMail;


        /// <summary>
        /// The data source where this user is registered
        /// </summary>
        public DataSource Source { get => source; }
        /// <summary>
        /// The id of the user on the given source
        /// </summary>
        public int Id { get => id; }
        /// <summary>
        /// The profile's nick name
        /// </summary>
        public string UserName { get => userName; }
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
        public string ProfileImageUrl { get => profileImageUrl; }
        /// <summary>
        /// The url to the profile image
        /// </summary>
        public string EMail { get => eMail; }

        public User()
        {
        }

        public User(DataSource source, int id, string userName, string firstName, string lastName, string profileImageUrl, string eMail)
        {
            this.source = source;
            this.id = id;
            this.userName = userName;
            this.firstName = firstName;
            this.lastName = lastName;
            this.profileImageUrl = profileImageUrl;
            this.eMail = eMail;
        }
    }
}