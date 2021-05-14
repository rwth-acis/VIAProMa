using i5.VIAProMa.DataModel.API;
using System;

public class IssueEditedArgs : EventArgs
{
    /// <summary>
    /// Creates the issue edited arguments
    /// </summary>
    /// <param name="issueName">The previous name of the issue that was edited</param>
    /// <param name="projectID">Id if the project the issue belongs to</param>
    /// <param name="newName">The edited name</param>
    /// <param name="newDescription">The edited description</param>
    public IssueEditedArgs(String issueName, String newName, String newDescription)
    {
        IssueName = issueName;
        NewName = newName;
        NewDescription = newDescription;
    }

    /// <summary>
    /// The name of the issue that has been edited
    /// </summary>
    public String IssueName { get; private set; }

    /// <summary>
    /// Give the new name of the issue
    /// </summary>
    public String NewName { get; private set; }

    /// <summary>
    /// Give the new description of the issue
    /// </summary>
    public String NewDescription { get; private set; }
}
