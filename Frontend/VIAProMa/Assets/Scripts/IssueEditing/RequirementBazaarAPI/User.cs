using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Org.Requirements_Bazaar.DataModel
{

    [Serializable]
    public class User
    {
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private string firstName;
        [SerializeField] private string lastName;
        [SerializeField] private string eMail;
        [SerializeField] private long las2peerId;
        [SerializeField] private string profileImage;
        [SerializeField] private bool emailLeadSubscription;
        [SerializeField] private bool emailFollowSubscription;
        [SerializeField] private bool personalizationEnabled;
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

        public string EMail
        {
            get
            {
                return eMail;
            }

            set
            {
                eMail = value;
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

        public bool PersonalizationEnabled
        {
            get
            {
                return personalizationEnabled;
            }

            set
            {
                personalizationEnabled = value;
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