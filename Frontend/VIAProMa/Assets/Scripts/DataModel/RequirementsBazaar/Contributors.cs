using System;

[Serializable]
public class Contributors : IListViewItemData, IUninitializable
{
    public User creator;
    public User leadDeveloper;
    public User[] developers;
    public User[] commentCreator;
    public User[] attachmentCreator;

    public bool IsUninitialized
    {
        get
        {
            if (creator.IsUninitialized && leadDeveloper.IsUninitialized && developers.Length == 0 && commentCreator.Length == 0 && attachmentCreator.Length == 0)
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
