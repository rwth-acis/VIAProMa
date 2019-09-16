using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the crane on top of the building progress bar
/// </summary>
public class CraneController : MonoBehaviour
{
    [Tooltip("The top part of the crane which contains the arm; This part will be rotated")]
    [SerializeField] private Transform craneTop;
    [Tooltip("The slider on the crane which moves along the arm")]
    [SerializeField] private Transform slider;
    [Tooltip("The cable object which lifts object")]
    [SerializeField] private Transform cable;

    /// <summary>
    /// The probability that the crane will start turning 
    /// </summary>
    [Tooltip("The probability that the crane will start turning ")]
    public float turnProbability = 0.05f;

    /// <summary>
    /// The speed at which the crane is turning
    /// </summary>
    [Tooltip("The speed at which the crane is turning")]
    public float turnSpeed = 1f;

    private CraneStatus status;
    private float timeSinceLastBehaviourUpdate;
    private float maxCableLength = 1.5f;

    private float targetCableLength;
    private Vector3 targetRotation;

    /// <summary>
    /// Checks if the component was set up correctly
    /// </summary>
    private void Awake()
    {
        if (craneTop == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(craneTop));
        }
        if (slider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(slider));
        }
        if (cable == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(cable));
        }
    }

    /// <summary>
    /// Updates the behaviour of the crane
    /// If the crane is idle: every second, it is determined if the crane should start turning
    /// If the crane is turning: the turning movement is executed
    /// </summary>
    private void Update()
    {
        // determine the behaviour every second
        timeSinceLastBehaviourUpdate += Time.deltaTime;

        if (timeSinceLastBehaviourUpdate >= 1)
        {
            timeSinceLastBehaviourUpdate = 0;
            BehaviourUpdate();
        }

        // execute behaviour
        switch (status)
        {
            case CraneStatus.IDLE:
                break;
            case CraneStatus.TURNING:
                TurnTop();
                break;
        }
    }

    /// <summary>
    /// Updates the behaviour status
    /// </summary>
    private void BehaviourUpdate()
    {
        if (status == CraneStatus.IDLE)
        {
            DetermineNextAction();
        }
    }

    /// <summary>
    /// Determines the next action based on a random number and the turnProbability
    /// </summary>
    private void DetermineNextAction()
    {
        float rand = Random.value;
        if (rand < turnProbability)
        {
            status = CraneStatus.TURNING;
            targetRotation = new Vector3(0, Random.Range(0, 359), 0);
        }
    }

    /// <summary>
    /// Turns the top part towards the target rotation
    /// </summary>
    private void TurnTop()
    {
        craneTop.localRotation = Quaternion.RotateTowards(
            craneTop.localRotation,
            Quaternion.Euler(targetRotation),
            Time.deltaTime * turnSpeed
            );

        float delta = craneTop.localEulerAngles.y - targetRotation.y;
        if (Mathf.Abs(delta) < 0.1f)
        {
            status = CraneStatus.IDLE;
        }
    }
}

/// <summary>
/// The behaviour states of the crane
/// </summary>
public enum CraneStatus
{
    IDLE, TURNING
}
