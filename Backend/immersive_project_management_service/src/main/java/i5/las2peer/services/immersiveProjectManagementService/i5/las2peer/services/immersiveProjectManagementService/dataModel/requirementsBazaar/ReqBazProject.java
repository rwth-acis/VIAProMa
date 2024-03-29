package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 10.05.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class ReqBazProject {
    public int id;
    public String name;
    public String description;
    public boolean visibility;
    public int defaultCategoryId;
    public ReqBazUser leader;
    public String creationDate;
    public String lastUpdatedDate;
    public int numberOfCategories;
    public int numberOfRequirements;
    public int numberOfFollowers;
    public boolean isFollower;
}
