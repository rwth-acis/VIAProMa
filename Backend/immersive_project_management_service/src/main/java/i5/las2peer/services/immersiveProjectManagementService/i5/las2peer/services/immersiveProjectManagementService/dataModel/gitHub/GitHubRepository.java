package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 20.07.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class GitHubRepository {

    private int id;
    private String name;

    public int getId() {
        return id;
    }

    public String getName() {
        return name;
    }
}
