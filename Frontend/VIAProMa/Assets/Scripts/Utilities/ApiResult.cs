using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiResult<T>
{
    public ApiResult(T value) : this(value, 200)
    {
    }

    public ApiResult(T value, int responseCode)
    {
        this.ResponseCode = responseCode;
        this.Value = value;
        this.ErrorMessage = "";
    }

    public ApiResult(long responseCode, string errorMessage)
    {
        this.ResponseCode = responseCode;
        this.ErrorMessage = errorMessage;
    }

    public bool Successful
    {
        get { return string.IsNullOrEmpty(ErrorMessage) && ResponseCode == 200; }
    }

    public bool HasError
    {
        get { return !Successful; }
    }

    public T Value
    {
        get; private set;
    }

    public long ResponseCode
    {
        get; private set;
    }

    public string ErrorMessage
    {
        get; private set;
    }
}
