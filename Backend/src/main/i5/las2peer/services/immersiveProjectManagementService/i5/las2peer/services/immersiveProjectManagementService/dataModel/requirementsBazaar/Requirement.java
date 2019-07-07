package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

/**
 * Created by bened on 09.05.2019.
 */
public class Requirement {
    public int id;
    public String name;
    public String description;
    public String realized;
    public int projectId;
    public ReqBazUser creator;
    public Category[] categories;
    public String creationDate;
    public String lastUpdatedDate;
    public int numberOfComments;
    public int numberOfAttachments;
    public int numberOfFollowers;
    public int upVotes;
    public int downVotes;
    public String userVoted;
}
