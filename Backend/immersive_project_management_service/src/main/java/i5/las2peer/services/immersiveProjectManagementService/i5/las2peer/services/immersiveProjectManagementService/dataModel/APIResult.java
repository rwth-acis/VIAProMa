package i5.las2peer.services.immersiveProjectManagementService.i5.las2peer.services.immersiveProjectManagementService.dataModel;

/**
 * Created by bened on 10.05.2019.
 */
public class APIResult <T> {
    private int code;
    private T value;
    private String errorMessage;

    public int getCode() {
        return code;
    }

    public APIResult(int code, T value)
    {
        this.code = code;
        this.value = value;
    }

    public APIResult(String errorMessage, int code)
    {
        this.code = code;
        this.errorMessage = errorMessage;
    }

    public T getValue() {
        return value;
    }

    public boolean hasError()
    {
        return code != 200 && code != 201;
    }

    public boolean successful()
    {
        return !hasError();
    }

    public String getErrorMessage() {
        return errorMessage;
    }
}
