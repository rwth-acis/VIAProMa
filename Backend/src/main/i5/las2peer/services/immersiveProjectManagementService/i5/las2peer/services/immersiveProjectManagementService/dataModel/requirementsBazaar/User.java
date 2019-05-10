package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 09.05.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class User {
    public int id;
    public String userName;
    public String firstName;
    public String lastName;
    public boolean admin;
    public long las2peerId;
    public String profileImiage;
    public boolean emailLeadSubscription;
    public boolean emailFollowSubscription;
}
