using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] private Transform craneTop;
    [SerializeField] private Transform slider;
    [SerializeField] private Transform cable;

    public float turnProbability = 0.05f;

    public float turnSpeed = 1f;

    private CraneStatus status;
    private float timeSinceLastBehaviourUpdate;
    private float maxCableLength = 1.5f;

    private float targetCableLength;
    private Vector3 targetRotation;

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

    private void BehaviourUpdate()
    {
        if (status == CraneStatus.IDLE)
        {
            DetermineNextAction();
        }
    }

    private void DetermineNextAction()
    {
        float rand = Random.value;
        if (rand < turnProbability)
        {
            status = CraneStatus.TURNING;
            targetRotation = new Vector3(0, Random.Range(0, 359), 0);
        }
    }

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

public enum CraneStatus
{
    IDLE, TURNING
}
