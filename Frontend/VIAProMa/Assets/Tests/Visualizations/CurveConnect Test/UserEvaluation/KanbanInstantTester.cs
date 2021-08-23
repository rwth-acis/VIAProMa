using Photon.Pun;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.Visualizations.KanbanBoard;
using i5.VIAProMa.DataModel.API;

public class KanbanInstantTester : MonoBehaviourPunCallbacks
{
    public KanbanBoardColumn kanbanBoardColumn;
    public string titel;
    //public int numberOfIssues = 10;
    string[] issuDesriptions = 
        {
            "Creating the Final Virtual Environment Prefabs",
            "SaveLoadManager never instantiates loaded objects as scene object",
            "LoginMenu Error after opening the AvatarMenu",
            "Incorrect project savefile problem",
            "Disable target selection mode if connection line menu is closed",
            "Saved project cannot be loaded without restart",
            "User keeps scrolling in the issus, instead of doing the evaluation"
        };

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SingleIssuesProvider provider = new SingleIssuesProvider();
            for (int i = 0; i < issuDesriptions.Length; i++)
            {
                provider.Issues.Add(new Issue(DataSource.REQUIREMENTS_BAZAAR, 1, "Issue " + i, issuDesriptions[i], 1, new User(), IssueStatus.OPEN, "", "", new User[0], new User[0]));
            }
            kanbanBoardColumn.ContentProvider = provider;
            if (titel != "")
            {
                kanbanBoardColumn.Title = titel;
            }
        }
    }
}
