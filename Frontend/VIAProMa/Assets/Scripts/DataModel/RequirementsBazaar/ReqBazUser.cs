using System;
/// <summary>
/// Represents a user in the Requirements Bazaar
/// </summary>
[Serializable]
public class ReqBazUser : IListViewItemData, IUninitializable
{
    public int id;
    public string userName;
    public string firstName;
    public string lastName;
    public bool admin;
    public long las2peerId;
    public string profileImage;
    public bool emailLeadSubscription;
    public bool emailFollowSubscription;

    public bool IsUninitialized
    {
        get
        {
            if (id == 0 && las2peerId == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}