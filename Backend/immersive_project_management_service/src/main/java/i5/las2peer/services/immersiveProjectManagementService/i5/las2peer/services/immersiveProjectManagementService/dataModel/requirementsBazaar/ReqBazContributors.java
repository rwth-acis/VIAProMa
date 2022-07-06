package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

/**
 * Created by bened on 02.07.2019.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class ReqBazContributors {
    public ReqBazUser creator;
    public ReqBazUser leadDeveloper;
    public ReqBazUser[] developers;
    public ReqBazUser[] commentCreator;
    public ReqBazUser[] attachmentCreator;
}
