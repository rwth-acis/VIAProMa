using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container with the result and response code of an API call
/// </summary>
/// <typeparam name="T">The type of the value</typeparam>
public class ApiResult<T>
{
    /// <summary>
    /// Creates a new APIResult object
    /// </summary>
    /// <param name="value">The result of the API call</param>
    public ApiResult(T value) : this(value, 200)
    {
    }

    /// <summary>
    /// Creates a new APIResult object
    /// </summary>
    /// <param name="value">The result of the API call</param>
    /// <param name="responseCode">The response code of the API call</param>
    public ApiResult(T value, int responseCode)
    {
        this.ResponseCode = responseCode;
        this.Value = value;
        this.ErrorMessage = "";
    }

    /// <summary>
    /// Creates a new APIResult object which is initialized with an error message
    /// </summary>
    /// <param name="responseCode">The response code of the API call</param>
    /// <param name="errorMessage">The error message of the call</param>
    public ApiResult(long responseCode, string errorMessage)
    {
        this.ResponseCode = responseCode;
        this.ErrorMessage = errorMessage;
    }

    /// <summary>
    /// True if the API call was successful and no error message is stored in this object
    /// </summary>
    public bool Successful
    {
        get { return string.IsNullOrEmpty(ErrorMessage) && ResponseCode == 200; }
    }

    /// <summary>
    /// True if the API call returned an error
    /// </summary>
    public bool HasError
    {
        get { return !Successful; }
    }

    /// <summary>
    /// The result of the API call
    /// </summary>
    public T Value
    {
        get; private set;
    }

    /// <summary>
    /// The HTTP response code of the API call
    /// </summary>
    public long ResponseCode
    {
        get; private set;
    }

    /// <summary>
    /// The error message which was returned by the API call
    /// </summary>
    public string ErrorMessage
    {
        get; private set;
    }
}
