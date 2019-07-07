package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Category;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazContributors;
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

    private  static String baseUrl = "https://requirements-bazaar.org/bazaar/";

    public static Response RequestRequirementsInCategory(int categoryId, int page, int itemsPerPage)
    {
        Client client = ClientBuilder.newClient();
        WebTarget webTarget = client.target(baseUrl + "categories/" + categoryId + "/requirements?page=" + page + "&per_page=" + itemsPerPage);
        Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
        Response response = invocationBuilder.get();
        return response;
    }

    public static  APIResult<Requirement[]> GetRequirementsInCategory(int categoryId, int page, int itemsPerPage)
    {
        try {
            Response response = RequestRequirementsInCategory(categoryId, page, itemsPerPage);

            if (response.getStatus() != 200 && response.getStatus() != 201) {
                return new APIResult<Requirement[]>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            Requirement[] requirements = mapper.readValue(origJson, Requirement[].class);
            return new APIResult<Requirement[]>(response.getStatus(), requirements);
        }
        catch (IOException e)
        {
            return new APIResult<Requirement[]>(e.getMessage(), 500);
        }
    }

    public static APIResult<Project[]> GetProjects(int page, int itemsPerPage)
    {
        try {
            Client client = ClientBuilder.newClient();
            WebTarget webTarget = client.target(baseUrl + "projects?page=" + page + "&per_page=" + itemsPerPage);
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
        return Utilities.getResponse(baseUrl + "projects/" + projectId + "/categories");
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

    public static Response RequestRequirementsInProject(int projectId, int page, int itemsPerPage)
    {
        return Utilities.getResponse(baseUrl + "projects/" + projectId + "/requirements?page=" + page + "&itmes_per_page=" + itemsPerPage);
    }

    public static APIResult<Requirement[]> GetRequirementsInProject(int projectId, int page, int itemsPerPage)
    {
        try
        {
            Response response = RequestRequirementsInProject(projectId, page, itemsPerPage);

            if (response.getStatus() != 200 && response.getStatus() != 201)
            {
                return new APIResult<Requirement[]>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            Requirement[] categories = mapper.readValue(origJson, Requirement[].class);
            return  new APIResult<Requirement[]>(response.getStatus(), categories);
        }
        catch (IOException e) {
            return  new APIResult<>(e.getMessage(), 500);
        }
    }

    public static APIResult<Requirement> GetRequirement(int requirementId) {
        try {
            Response response = Utilities.getResponse(baseUrl + "requirements/" + requirementId);
            if (response.getStatus() != 200 && response.getStatus() != 201) {
                return new APIResult<Requirement>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            Requirement requirement = mapper.readValue(origJson, Requirement.class);
            return new APIResult<Requirement>(response.getStatus(), requirement);
        } catch (IOException e) {
            return new APIResult<>(e.getMessage(), 500);
        }
    }

    public static APIResult<ReqBazContributors> GetRequirementContributors(int requirementId)
    {
        try {
            Response response = Utilities.getResponse(baseUrl + "requirements/" + requirementId + "/contributors");
            if (response.getStatus() != 200 && response.getStatus() != 201) {
                return new APIResult<ReqBazContributors>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            ReqBazContributors contributors = mapper.readValue(origJson, ReqBazContributors.class);
            return new APIResult<ReqBazContributors>(response.getStatus(), contributors);
        } catch (IOException e) {
            return new APIResult<>(e.getMessage(), 500);
        }
    }

}
