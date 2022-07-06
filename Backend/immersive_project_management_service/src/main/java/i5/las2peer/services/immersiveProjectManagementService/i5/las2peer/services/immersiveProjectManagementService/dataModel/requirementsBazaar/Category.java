package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 09.05.2019.
 */
//@JsonFilter("shortFilter")
@JsonIgnoreProperties(ignoreUnknown = true)
public class Category {
    public int id;
    public String name;
    public String description;
    public int projectId;
    public ReqBazUser leader;
    public String creationDate;
    public String lastUpdatedDate;
    public int numberOfRequirements;
    public int numberOfFollowers;
}
