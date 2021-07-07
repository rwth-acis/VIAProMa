package i5.las2peer.services.immersiveProjectManagementService;

import java.net.HttpURLConnection;
import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;

import com.google.gson.Gson;

import i5.las2peer.api.ManualDeployment;
import i5.las2peer.restMapper.RESTService;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import io.swagger.annotations.ApiResponse;
import io.swagger.annotations.ApiResponses;
import io.swagger.annotations.Contact;
import io.swagger.annotations.Info;
import io.swagger.annotations.License;
import io.swagger.annotations.SwaggerDefinition;
import net.minidev.json.parser.JSONParser;
import net.minidev.json.parser.ParseException;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.ResponseBody;

//import com.google.gson.Gson;
import net.minidev.json.JSONObject;
import net.minidev.json.JSONArray;
/**
 * las2peer-GetWeather-Service
 * 
 * A RESTful service that provides current weather of a city.
 * 
 */

@Api
@SwaggerDefinition(
		info = @Info(
				title = "las2peer Get Weather Service",
				version = "0.1.0",
				description = "A las2peer service for getting current temperature of a city.",
				termsOfService = "",
				contact = @Contact(
						name = "Tran Lan Anh",
						url = "https://las2peer.org",
						email = "tran@dbis.rwth-aachen.de"),
				license = @License(
						name = "",
						url = "")))
@Path("weather")
@ManualDeployment

public class GetWeatherMainClass extends RESTService {
	
	private String appid = "347e72f54a7cde54465418abd431fcf0";
	
	public GetWeatherMainClass() {
		//read and set properties values
		setFieldValues();
	}
	
	// Get weather's info of a city 
	@GET
	@Path("/getTemp/{city}")
	@Produces(MediaType.APPLICATION_JSON)
	@ApiOperation(
			value = "getWeather",
			notes = "Get current temperature of a city")
	@ApiResponses(
			value = { @ApiResponse(
					code = HttpURLConnection.HTTP_OK,
					message = "Get Weather service is ready!"),

					@ApiResponse(
					code = HttpURLConnection.HTTP_INTERNAL_ERROR,
					message = "Internal Server Error") })
	
	public Response getWeather(@PathParam("city") String city) {
				
		String url = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=" + appid;
		Request requestUrl = new Request.Builder().url(url).build();			  			  
		OkHttpClient client = new OkHttpClient();
		Gson gsonObj = new Gson();		
		JSONObject text = new JSONObject();
		WeatherInfo result = null;		
		
		try {	
				okhttp3.Response resp = client.newCall(requestUrl).execute();		    
		        ResponseBody info = resp.body();		        
		        result = gsonObj.fromJson(info.string(), WeatherInfo.class);
		        int temp = (int)(result.getTemperature().getTemp() - 273.15);
		        text.put("text", "The temperature in "+city+" is "+String.valueOf(temp)+"oC");
		        text.put("closeContext", "true");
				return Response.status(Status.OK).entity(text).build();
				
		} catch (Exception e) {
		        e.printStackTrace();
		        text.put("text", appid);
		        text.put("closeContext", "true");
				return Response.status(Status.OK).entity(text).build();
		}	     
	} 
	
	
	// Get weather's info of a city 
	@POST
	@Path("/getTempbot")
	@Consumes(MediaType.TEXT_PLAIN)
	@Produces(MediaType.APPLICATION_JSON)
	@ApiOperation(
			value = "getWeatherbot",
			notes = "Give current temperature of a city")
	@ApiResponses(
			value = { @ApiResponse(
					code = HttpURLConnection.HTTP_OK,
					message = "Weather bot is ready!"),

					@ApiResponse(
					code = HttpURLConnection.HTTP_INTERNAL_ERROR,
					message = "Internal Server Error") })
		
	public Response getWeatherbot(String body) throws ParseException {
					

			//JSONParser p = new JSONParser(JSONParser.MODE_PERMISSIVE);
			//JSONObject triggeredBody = (JSONObject) p.parse(body);
			//String city = triggeredBody.getAsString("city");
			String city = "Hanoi";
			String url = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=" + appid;
			Request requestUrl = new Request.Builder().url(url).build();			  			  
			OkHttpClient client = new OkHttpClient();		
			JSONObject text = new JSONObject();	
			
			try {	
					okhttp3.Response resp = client.newCall(requestUrl).execute();		    
			        ResponseBody info = resp.body();
			        JSONParser jp = new JSONParser(JSONParser.MODE_PERMISSIVE);
					JSONObject obj = (JSONObject) jp.parse(info.string());
					JSONArray arrdesp = (JSONArray) jp.parse(obj.getAsString("weather"));
					JSONObject objdesp = (JSONObject) jp.parse(arrdesp.get(0).toString());
					String desp = objdesp.getAsString("description");
					JSONObject objtemp = (JSONObject) jp.parse(obj.getAsString("main"));
					int temp = (int)(Float.parseFloat(objtemp.getAsString("temp"))-273.15);
			        text.put("text", "There is "+desp+" in "+city+" today. The current temperature is "+temp+"oC.");
			        text.put("closeContext", "true");
			        System.out.println(text);
					return Response.status(Status.OK).entity(text).build();
					
			} catch (Exception e) {
			        e.printStackTrace();
			        text.put("text", "Can't get the city name");
			        text.put("closeContext", "true");
					return Response.status(Status.OK).entity(text).build();
			}	     
		} 

}