package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 18.07.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class GitHubLabel {
    private int id;
    private String nodeId;
    private String name;
    private String description;
    private String color;

    public int getId() {
        return id;
    }

    public String getNodeId() {
        return nodeId;
    }

    public String getName() {
        return name;
    }

    public String getDescription() {
        return description;
    }

    public String getColor() {
        return color;
    }
}
