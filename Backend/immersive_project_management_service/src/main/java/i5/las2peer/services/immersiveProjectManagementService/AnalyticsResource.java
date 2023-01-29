package i5.las2peer.services.immersiveProjectManagementService;

import com.fasterxml.jackson.databind.*;

import i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.LogPoint.LogPointLRS;

import io.swagger.annotations.*;

import net.minidev.json.JSONArray;
import net.minidev.json.JSONObject;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;
import java.io.*;
import java.net.HttpURLConnection;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.sql.*;
import java.util.HashMap;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

/**
 * AnalyticsResource
 * <p>
 * This is the backend service which is used to collect and export analytics data
 */
// TODO Adjust the following configuration
@Api(value = "Analytics", description = "")
@SwaggerDefinition(
        info = @Info(
                title = "Immersive ReqBazProject Management Service",
                version = "1.0.0",
                description = "Backend service of the immersive analytics project management framework",
                termsOfService = "",
                contact = @Contact(
                        name = "Benedikt Hensen",
                        url = "",
                        email = ""),
                license = @License(
                        name = "MIT",
                        url = "")))
@Path("/analytics")
public class AnalyticsResource {
    HashMap<String, Connection> connections = new HashMap<>();

    @POST
    @Path("/dummy/{project}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Logs analytics data with no specific endpoint",
            notes = "Data will not be in the SQL Database (or exported)")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_CREATED,
                    message = "Created log entry")})
    public Response getIssuesInRepository(@PathParam("project") String project, String text) {
        File parentDir = new File(getProjectDirectory(project), "analyticsJSON");
        File file;
        if (!parentDir.exists())
            parentDir.mkdir();
        try {
            file = new File(parentDir, System.currentTimeMillis() + ".json");
            PrintWriter out = new PrintWriter(file);
            out.println(text);
            out.close();
        } catch (FileNotFoundException e) {
            return Response.serverError().entity(e.getMessage()).build();
        }
        String result = text;
        return Response.ok().header("Location", file.getPath()).entity(result).build();
    }

    @POST
    @Path("/lrs/{project}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Logs analytics data in the LRS Format",
            notes = "")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_CREATED,
                    message = "Created LRS-log entry")})
    public Response logLRS(@PathParam("project") String project, String text) {
        Connection con = getConnectionForProject(project);
        ObjectMapper mapper = new ObjectMapper();
        mapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);
        mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
        ObjectReader reader = mapper.readerFor(LogPointLRS.class);

        try {
            LogPointLRS logpPoint = reader.readValue(text);
            PreparedStatement statement = con.prepareStatement("INSERT INTO LRS VALUES (?,?,?,?,?)");
            statement.setString(1, logpPoint.getActor());
            statement.setString(2, logpPoint.getVerb());
            statement.setString(3, logpPoint.getObjectType());
            statement.setString(4, logpPoint.getObjectId());
            statement.setTimestamp(5, logpPoint.getTimestamp());
            statement.execute();
        } catch (Exception e) {
            e.printStackTrace();
            return Response.serverError().entity(e.getMessage()).build();
        }
        return Response.ok().entity(text).build();
    }

    @GET
    @Path("/json-export/{project}")
    @Produces(MediaType.APPLICATION_JSON)
    @ApiOperation(
            value = "Returns the analytics data as JSON",
            notes = "")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "OK")})
    public Response getDataAsJSON(@PathParam("project") String project, String text) {
        Connection con = getConnectionForProject(project);
        StringBuilder str = new StringBuilder("{");
        try {
            ResultSet tables = con.createStatement().executeQuery("SELECT name FROM main.sqlite_master WHERE type='table'");
            while (tables.next()) {
                String tableName = tables.getString(1);
                PreparedStatement statement = con.prepareStatement("SELECT * FROM " + tableName);
                ResultSet table = statement.executeQuery();

                str.append('\"');
                str.append(tableName);
                str.append("\":");
                tableToJSON(str, table);
                str.append(',');
            }
            str.setLength(str.length() - 1);
            str.append('}');

        } catch (SQLException e) {
            e.printStackTrace();
            return Response.serverError().entity(e.getMessage()).build();
        }
        return Response.ok().entity(str.toString()).build();
    }

    @GET
    @Path("/csv-export/{project}")
    @Produces(MediaType.APPLICATION_OCTET_STREAM)
    @ApiOperation(
            value = "Returns the analytics data as CSV",
            notes = "")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "OK")})
    public Response getDataAsCSV(@PathParam("project") String project, String text) {
        Connection con = getConnectionForProject(project);
        ByteArrayOutputStream buffer = new ByteArrayOutputStream();
        ZipOutputStream zipOutputStream = new ZipOutputStream(buffer);
        try {
            ResultSet tables = con.createStatement().executeQuery("SELECT name FROM main.sqlite_master WHERE type='table'");
            while (tables.next()) {
                String tableName = tables.getString(1);
                ZipEntry zipEntry = new ZipEntry(tableName + ".csv");
                zipOutputStream.putNextEntry(zipEntry);
                PreparedStatement statement = con.prepareStatement("SELECT * FROM " + tableName);
                ResultSet table = statement.executeQuery();
                zipOutputStream.write(tableToCSV(table).getBytes(StandardCharsets.UTF_8));
                zipOutputStream.closeEntry();
            }
            zipOutputStream.close();

        } catch (SQLException e) {
            e.printStackTrace();
            return Response.serverError().entity(e.getMessage()).build();
        } catch (IOException e) {
            return Response.serverError().entity(e.getMessage()).build();
        }
        return Response.ok().entity(buffer.toByteArray()).build();
    }

    @GET
    @Path("/sqlite-export/{project}")
    @Produces(MediaType.APPLICATION_OCTET_STREAM)
    @ApiOperation(
            value = "Returns the analytics data as a SQLite database file",
            notes = "")
    @ApiResponses(
            value = {@ApiResponse(
                    code = HttpURLConnection.HTTP_OK,
                    message = "OK")})
    public Response getDataAsSQLite(@PathParam("project") String project, String text) {
        getConnectionForProject(project);
        byte[] result = new byte[0];
        try {
            result = Files.readAllBytes(Paths.get(getProjectDirectory(project).getPath() + "/analytics.sqlite"));
        } catch (Exception e) {
            Response.serverError().entity(e.getMessage()).build();
        }
        return Response.ok().entity(result).build();
    }

    Connection getConnectionForProject(String project) {

        Connection connection = connections.get(project);
        if (connection == null) {
            try {
                // create a database connection
                connection = DriverManager.getConnection("jdbc:sqlite:" + getProjectDirectory(project).getPath() + "/analytics.sqlite");
                createLRSExportableTable(connection);
            } catch (SQLException e) {
                e.printStackTrace();
            }
        }
        return connection;
    }

    void createLRSExportableTable(Connection connection) throws SQLException {
        Statement statement = connection.createStatement();
        statement.executeUpdate(
                "CREATE TABLE IF NOT EXISTS LRS (" +
                        "`Actor` VARCHAR NOT NULL," +
                        "`Verb` VARCHAR NOT NULL," +
                        "`ObjectType` VARCHAR NOT NULL," +
                        "`ObjectId` VARCHAR NOT NULL," +
                        "`Timestamp` DATETIME NOT NULL" +
                        ")");
    }

    File getProjectDirectory(String project) {
        File projectDir = new File("projects/" + project + "/");
        if (!projectDir.exists())
            projectDir.mkdirs();
        return projectDir;
    }

    void tableToJSON(StringBuilder str, ResultSet table) throws SQLException {
        ResultSetMetaData md = table.getMetaData();
        int numCols = md.getColumnCount();
        JSONArray result = new JSONArray();
        while (table.next()) {
            JSONObject row = new JSONObject();
            for (int i = 1; i <= md.getColumnCount(); i++) {
                row.put(md.getColumnName(i), table.getObject(i));
            }
            result.add(row);
        }
        str.append(result.toJSONString());
    }

    String tableToCSV(ResultSet table) throws SQLException {
        StringBuilder str = new StringBuilder();
        ResultSetMetaData md = table.getMetaData();
        int numCols = md.getColumnCount();

        // get column names
        for (int i = 1; i <= md.getColumnCount(); i++) {
            str.append(getEscapedCSV(md.getColumnName(i)));
            str.append(',');
        }
        str.append('\n');

        // get data
        while (table.next()) {
            for (int i = 1; i <= md.getColumnCount(); i++) {
                str.append(getEscapedCSV(table.getString(i)));
                str.append(',');
            }
            str.append('\n');
        }
        return str.toString();
    }

    String getEscapedCSV(String text) {
        return "\"" + text.replace("\"", "\"\"") + "\"";
    }
}


