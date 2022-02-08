package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 09.05.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class Requirement {
    private int id;
    private String name;
    private String description;
    private String realized;
    private int projectId;
    private ReqBazUser creator;
    private ReqBazUser leadDeveloper;
    private Category[] categories;
    private String creationDate;
    private String lastUpdatedDate;
    private int numberOfComments;
    private int numberOfAttachments;
    private int numberOfFollowers;
    private int upVotes;
    private int downVotes;
    private String userVoted;
    private boolean isFollower;
    private boolean isDeveloper;
    private boolean isContributor;

    public int getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public String getDescription() {
        return description;
    }

    public String getRealized() {
        return realized;
    }

    public int getProjectId() {
        return projectId;
    }

    public ReqBazUser getCreator() {
        return creator;
    }

    public ReqBazUser getLeadDeveloper() {
        return leadDeveloper;
    }

    public Category[] getCategories() {
        return categories;
    }

    public String getCreationDate() {
        return creationDate;
    }

    public String getLastUpdatedDate() {
        return lastUpdatedDate;
    }

    public int getNumberOfComments() {
        return numberOfComments;
    }

    public int getNumberOfAttachments() {
        return numberOfAttachments;
    }

    public int getNumberOfFollowers() {
        return numberOfFollowers;
    }

    public int getUpVotes() {
        return upVotes;
    }

    public int getDownVotes() {
        return downVotes;
    }

    public String getUserVoted() {
        return userVoted;
    }

    public boolean isFollower() {
        return isFollower;
    }

    public boolean isDeveloper() {
        return isDeveloper;
    }

    public boolean isContributor() {
        return isContributor;
    }
}
