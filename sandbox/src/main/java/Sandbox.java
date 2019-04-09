import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectWriter;

import javax.json.Json;
import javax.json.JsonArray;
import javax.json.JsonObject;
import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.Invocation;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

/**
 * Created by bened on 09.04.2019.
 */
public class Sandbox {

    public static void main(String[] args) throws Exception {
        System.out.println("Hello World");
        Client client = ClientBuilder.newClient();
        WebTarget webTarget = client.target("https://api.github.com/repos/rwth-acis/RequirementsBazaar/stats/punch_card");
        Invocation.Builder invocationBuilder = webTarget.request(MediaType.APPLICATION_JSON);
        Response response = invocationBuilder.get();
        String json = response.readEntity(String.class);

        System.out.println(json);

        ObjectMapper mapper = new ObjectMapper();
        int[][] array = mapper.readValue(json, int[][].class);

         ObjectWriter writer = mapper.writer().withRootName("array");
         String result = writer.writeValueAsString(array);
         System.out.println(result);

    }
}
