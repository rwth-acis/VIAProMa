package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.apiModel;

import com.fasterxml.jackson.annotation.JsonIgnore;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.gitHub.GitHubUser;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazUser;

/**
 * Created by bened on 07.07.2019.
 */
public class CrossUser {
    private DataSource source;
    private int id;
    private String userName;
    private String firstName;
    private String lastName;
    private String profileImageUrl;

    public CrossUser()
    {
    }

    public CrossUser(DataSource source, int id, String userName, String firstName, String lastName, String profileImageUrl) {
        this.source = source;
        this.id = id;
        this.userName = userName;
        this.firstName = firstName;
        this.lastName = lastName;
        this.profileImageUrl = profileImageUrl;
    }

    @JsonIgnore
    public DataSource getDataSource() {
        return source;
    }

    public int getSource() {return source.ordinal();}

    public int getId() {
        return id;
    }

    public String getUserName() {
        return userName;
    }

    public String getFirstName() {
        return firstName;
    }

    public String getLastName() {
        return lastName;
    }

    public String getProfileImageUrl() {
        return profileImageUrl;
    }

    public static CrossUser fromReqBazUser(ReqBazUser rbUser)
    {
        CrossUser user = new CrossUser(
                DataSource.REQUIREMENTS_BAZAAR,
                rbUser.id,
                rbUser.userName,
                rbUser.firstName,
                rbUser.lastName,
                rbUser.profileImage
        );
        return user;
    }

    public static CrossUser[] fromReqBazUsers(ReqBazUser[] rbUsers)
    {
        CrossUser[] users = new CrossUser[rbUsers.length];
        for(int i=0;i<rbUsers.length;i++)
        {
            users[i] = fromReqBazUser(rbUsers[i]);
        }
        return  users;
    }

    public static CrossUser fromGitHubUser(GitHubUser ghUser)
    {
        CrossUser user = new CrossUser(
                DataSource.GITHUB,
                ghUser.getId(),
                ghUser.getLogin(),
                "",
                ghUser.getLogin(),
                ghUser.getAvatar_url()
        );
        return user;
    }

    public static CrossUser[] fromGitHubUsers(GitHubUser[] ghUsers)
    {
        CrossUser[] users = new CrossUser[ghUsers.length];
        for (int i=0;i<ghUsers.length;i++)
        {
            users[i] = fromGitHubUser(ghUsers[i]);
        }
        return users;
    }

    @Override
    public boolean equals(Object obj) {
        if (!(obj instanceof CrossUser))
        {
            return false;
        }
        CrossUser other = (CrossUser)obj;
        return this.source == other.source && this.id == other.id;
    }
}
