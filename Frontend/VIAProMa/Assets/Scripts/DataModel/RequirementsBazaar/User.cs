/// <summary>
/// Represents a user in the Requirements Bazaar
/// </summary>
public class User : IListViewItemData
{
    public int id;
    public string userName;
    public string firstName;
    public string lastName;
    public long las2peerId;
    public string profileImage;
    public bool emailLeadSubscription;
    public bool emailFollowSubscription;
}