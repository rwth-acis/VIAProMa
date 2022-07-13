package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 18.07.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class GitHubUser {
    private String login;
    private int id;
    private String node_id;
    private String avatar_url;

    public String getLogin() {
        return login;
    }

    public int getId() {
        return id;
    }

    public String getNode_id() {
        return node_id;
    }

    public String getAvatar_url() {
        return avatar_url;
    }
}
