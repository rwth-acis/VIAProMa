package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Category;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Project;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Requirement;

import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.Invocation;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.IOException;

/**
 * Created by bened on 09.05.2019.
 */
public class RequirementsBazaarAdapter {

    public static Response RequestRequirementsInCategory(int categoryId)
    {
        Client client = ClientBuilder.newClient();
        WebTarget webTarget = client.target("https://requirements-bazaar.org/bazaar/categories/" + categoryId + "/requirements");
        Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
        Response response = invocationBuilder.get();
        return response;
    }

    public static APIResult<Project[]> GetProjects(int page, int itemsPerPage)
    {
        try {
            Client client = ClientBuilder.newClient();
            WebTarget webTarget = client.target("https://requirements-bazaar.org/bazaar/projects?page=" + page + "&per_page=" + itemsPerPage);
            Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
            Response response = invocationBuilder.get();

            if (response.getStatus() != 200 && response.getStatus() != 201)
            {
                return  new APIResult<Project[]>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            Project[] projects = mapper.readValue(origJson, Project[].class);
            return  new APIResult<Project[]>(response.getStatus(), projects);
        }
        catch (IOException e) {
            return  new APIResult<>(e.getMessage(), 500);
        }
    }

    public static Response RequestCategoriesInProject(int projectId)
    {
        Client client = ClientBuilder.newClient();
        WebTarget webTarget = client.target("https://requirements-bazaar.org/bazaar/projects/" + projectId + "/categories");
        Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
        Response response = invocationBuilder.get();
        return response;
    }

    public  static  APIResult<Category[]> GetCategoriesInProject(int projectId)
    {
        try
        {
            Response response = RequestCategoriesInProject(projectId);

            if (response.getStatus() != 200 && response.getStatus() != 201)
            {
                return new APIResult<Category[]>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            Category[] categories = mapper.readValue(origJson, Category[].class);
            return  new APIResult<Category[]>(response.getStatus(), categories);
        }
        catch (IOException e) {
            return  new APIResult<>(e.getMessage(), 500);
        }
    }

}
