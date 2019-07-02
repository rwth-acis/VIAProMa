using System;
/// <summary>
/// Represents a user in the Requirements Bazaar
/// </summary>
[Serializable]
public class User : IListViewItemData
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
}