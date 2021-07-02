using i5.VIAProMa.DataModel.API;
using System;

public class IssueDeletedArgs : EventArgs
{
    /// <summary>
    /// Creates the issue deleted arguments
    /// </summary>
    /// <param name="issueID">The id of the issue that was deleted</param>
    public IssueDeletedArgs(int issueID)
    {
        IssueID = issueID;
    }

    /// <summary>
    /// The id of the issue that has been deleted
    /// </summary>
    public int IssueID { get; private set; }

}
