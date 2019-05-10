package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

import com.fasterxml.jackson.annotation.JsonFilter;

/**
 * Created by bened on 10.05.2019.
 */
@JsonFilter("projectFilter")
public class Project {
    public int id;
    public String name;
    public String description;
    public boolean visibility;
    public int defaultCategoryId;
    public User leader;
    public String creationDate;
    public String lastUpdatedDate;
    public int numberOfCategories;
    public int numberOfRequirements;
    public int numberOfFollowers;
    public boolean isFollower;
}
