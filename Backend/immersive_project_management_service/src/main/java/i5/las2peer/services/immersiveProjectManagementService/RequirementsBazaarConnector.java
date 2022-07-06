package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.ObjectMapper;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.APIResult;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Category;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazProject;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.ReqBazContributors;
import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.requirementsBazaar.Requirement;

import javax.ws.rs.core.Response;
import javax.ws.rs.core.UriBuilder;
import java.io.IOException;
import java.net.URI;

/**
 * Created by bened on 09.05.2019.
 */
public class RequirementsBazaarConnector {

    private  static String baseUrl = "https://requirements-bazaar.org/bazaar/";

    public static Response requestRequirementsInCategory(int categoryId, int page, int itemsPerPage, String searchFilter)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("categories/{categoryId}/requirements")
                .queryParam("page", page)
                .queryParam("per_page", itemsPerPage);

        if (searchFilter != null)
        {
            searchFilter = Utilities.RemoveSpecialSymbols(searchFilter);
            uriBuilder.queryParam("search", searchFilter);
        }

        URI uri = uriBuilder.build(categoryId);
        return  Utilities.getResponse(uri);
    }

    public static  APIResult<Requirement[]> getRequirementsInCategory(int categoryId, int page, int itemsPerPage, String searchFilter)
    {
        try {

            Response response = requestRequirementsInCategory(categoryId, page, itemsPerPage, searchFilter);

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

    public static Response requestProjects(int page, int itemsPerPage)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                        .path("projects")
                        .queryParam("page", page)
                        .queryParam("per_page", itemsPerPage);

        URI uri = uriBuilder.build();
        return  Utilities.getResponse(uri);
    }

    public static APIResult<ReqBazProject[]> getProjects(int page, int itemsPerPage)
    {
        try {
            Response response = requestProjects(page, itemsPerPage);

            if (response.getStatus() != 200 && response.getStatus() != 201)
            {
                return  new APIResult<ReqBazProject[]>(response.readEntity(String.class), response.getStatus());
            }

            String origJson = response.readEntity(String.class);

            // parse JSON data
            ObjectMapper mapper = new ObjectMapper();
            ReqBazProject[] projects = mapper.readValue(origJson, ReqBazProject[].class);
            return  new APIResult<ReqBazProject[]>(response.getStatus(), projects);
        }
        catch (IOException e) {
            return  new APIResult<>(e.getMessage(), 500);
        }
    }

    public static Response requestCategoriesInProject(int projectId)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("projects/{projectId}/categories");

        URI uri = uriBuilder.build(projectId);
        return Utilities.getResponse(uri);
    }

    public  static  APIResult<Category[]> getCategoriesInProject(int projectId)
    {
        try
        {
            Response response = requestCategoriesInProject(projectId);

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

    public static Response requestRequirementsInProject(int projectId, int page, int itemsPerPage, String searchFilter)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("projects/{projectId}/requirements")
                .queryParam("page", page)
                .queryParam("per_page", itemsPerPage);
        if (searchFilter != null) {
            searchFilter = Utilities.RemoveSpecialSymbols(searchFilter);
            uriBuilder.queryParam("search", searchFilter);
        }

        URI uri = uriBuilder.build(projectId);
        return Utilities.getResponse(uri);
    }

    public static APIResult<Requirement[]> getRequirementsInProject(int projectId, int page, int itemsPerPage, String searchFilter)
    {
        try
        {
            Response response = requestRequirementsInProject(projectId, page, itemsPerPage, searchFilter);
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

    public static Response requestRequirement(int requirementId)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("requirements/{requirementId}");
        URI uri = uriBuilder.build(requirementId);
        return Utilities.getResponse(uri);
    }

    public static APIResult<Requirement> getRequirement(int requirementId) {
        try {
            Response response = requestRequirement(requirementId);
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

    public static Response requestRequirementContributors(int requirementId)
    {
        UriBuilder uriBuilder =
                UriBuilder.fromPath(baseUrl)
                .path("requirements/{requirementId}/contributors");
        URI uri = uriBuilder.build(requirementId);
        return Utilities.getResponse(uri);
    }

    public static APIResult<ReqBazContributors> getRequirementContributors(int requirementId)
    {
        try {
            Response response = requestRequirementContributors(requirementId);
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
