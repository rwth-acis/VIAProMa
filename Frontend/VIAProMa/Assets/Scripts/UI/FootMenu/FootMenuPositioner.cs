using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMenuPositioner : MonoBehaviour
{
    [SerializeField] private HeightChanger heightChanger;

    public float maxDegreeDeviation = 90f;
    public float lerpSpeed = 1f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Awake()
    {
        if (heightChanger == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(heightChanger));
        }
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = new Vector3(
            Camera.main.transform.position.x,
            heightChanger.Height,
            Camera.main.transform.position.z
            );
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);


        float angleDifference = Mathf.Abs(Camera.main.transform.eulerAngles.y - transform.eulerAngles.y);
        if (angleDifference > maxDegreeDeviation)
        {
            targetRotation = Quaternion.Euler(new Vector3(
                0f,
                Camera.main.transform.eulerAngles.y,
                0f
                ));
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);

    }
}
