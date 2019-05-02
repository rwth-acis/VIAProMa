using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAway : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 0.4f;
    [SerializeField] private float spread = 0.2f;

    private float speed;
    private Vector3 direction;
    private Vector3 originalPosition;

    private void OnEnable()
    {
        speed = Random.value * maxSpeed;
        direction = RandomDirectionInCone(spread);
        originalPosition = transform.localPosition;
    }

    private void OnDisable()
    {
        transform.localPosition = originalPosition;
    }

    private void Update()
    {
        transform.localPosition += speed * Time.deltaTime * direction;
    }

    /// <summary>
    /// Computes a random direction in an upwards facing cone
    /// </summary>
    /// <param name="maxRadius"></param>
    /// <returns></returns>
    private Vector3 RandomDirectionInCone(float maxRadius)
    {
        float radius = Random.Range(0, maxRadius);
        float angle = Random.Range(0, 2 * Mathf.PI);
        Vector3 dir = new Vector3(
            radius * Mathf.Cos(angle),
            1f,
            radius * Mathf.Sin(angle)
            );

        dir = transform.TransformDirection(dir);
        return dir;
    }
}
