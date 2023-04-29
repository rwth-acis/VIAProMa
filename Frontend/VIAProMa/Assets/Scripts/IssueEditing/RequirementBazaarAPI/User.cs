using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Org.Requirements_Bazaar.DataModel
{

    [Serializable]
    public class User
    {

        /// <summary>
        /// The values of the user
        /// </summary>
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private string firstName;
        [SerializeField] private string lastName;
        [SerializeField] private bool admin;
        [SerializeField] private long las2peerId;
        [SerializeField] private string profileImage;
        [SerializeField] private bool emailLeadSubscription;
        [SerializeField] private bool emailFollowSubscription;
        [SerializeField] private string creationDate;
        [SerializeField] private string lastUpdatedDate;
        [SerializeField] private string lastLoginDate;

        #region Properties

        public int Id
        {
            get
            {
                return id;
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }

        public bool Admin
        {
            get
            {
                return admin;
            }

            set
            {
                admin = value;
            }
        }

        public long Las2peerId
        {
            get
            {
                return las2peerId;
            }
        }

        public string ProfileImage
        {
            get
            {
                return profileImage;
            }

            set
            {
                profileImage = value;
            }
        }

        public bool EmailLeadSubscription
        {
            get
            {
                return emailLeadSubscription;
            }

            set
            {
                emailLeadSubscription = value;
            }
        }

        public bool EmailFollowSubscription
        {
            get
            {
                return emailFollowSubscription;
            }

            set
            {
                emailFollowSubscription = value;
            }
        }

        public string CreationDate
        {
            get
            {
                return creationDate;
            }
        }

        public string LastUpdatedDate
        {
            get
            {
                return lastUpdatedDate;
            }
        }

        public string LastLoginDate
        {
            get
            {
                return lastLoginDate;
            }
        }

        #endregion
    }

}