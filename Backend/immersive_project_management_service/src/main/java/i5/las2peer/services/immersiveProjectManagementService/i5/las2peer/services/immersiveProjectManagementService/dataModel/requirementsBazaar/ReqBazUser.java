package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 09.05.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class ReqBazUser {
    public int id;
    public String userName;
    public String firstName;
    public String lastName;
    public boolean admin;
    public long las2peerId;
    public String profileImage;
    public boolean emailLeadSubscription;
    public boolean emailFollowSubscription;

    @Override
    public boolean equals(Object obj) {
        if (!(obj instanceof ReqBazUser))
        {
            return false;
        }
        return this.id == ((ReqBazUser) obj).id;
    }
}
