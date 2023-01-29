package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel.LogPoint;

import java.sql.Timestamp;

public class LogPointLRS {
    private String actor;
    private String verb;
    private String objectType;
    private String objectId;
    private Timestamp timestamp;

    public String getActor() {
        return actor;
    }

    public void setActor(String actor) {
        this.actor = actor;
    }

    public String getVerb() {
        return verb;
    }

    public void setVerb(String verb) {
        this.verb = verb;
    }

    public String getObjectType() {
        return objectType;
    }

    public void setObjectType(String objectType) {
        this.objectType = objectType;
    }

    public String getObjectId() {
        return objectId;
    }

    public void setObjectId(String objectId) {
        this.objectId = objectId;
    }

    public Timestamp getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(Timestamp timestamp) {
        this.timestamp = timestamp;
    }
}
