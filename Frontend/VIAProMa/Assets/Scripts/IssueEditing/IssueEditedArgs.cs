using i5.VIAProMa.DataModel.API;
using System;

public class IssueEditedArgs : EventArgs
{
    /// <summary>
    /// Creates the issue edited arguments
    /// </summary>
    /// <param name="issueID">The ID of the issue that was edited</param>
    /// <param name="projectID">ID if the project the issue belongs to</param>
    /// <param name="newName">The edited name</param>
    /// <param name="newDescription">The edited description</param>
    public IssueEditedArgs(int issueID, String newName, String newDescription)
    {
        IssueID = issueID;
        NewName = newName;
        NewDescription = newDescription;
    }

    /// <summary>
    /// The ID of the issue that has been edited
    /// </summary>
    public int IssueID { get; private set; }

    /// <summary>
    /// Give the new name of the issue
    /// </summary>
    public String NewName { get; private set; }

    /// <summary>
    /// Give the new description of the issue
    /// </summary>
    public String NewDescription { get; private set; }
}
