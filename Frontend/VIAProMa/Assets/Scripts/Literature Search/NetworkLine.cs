using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] private float speed = 2;


    // Update is called once per frame
    void Update()
    {
        lineRenderer.material.mainTextureOffset = - Vector2.right * Time.time * speed;
    }

    public void SetLine(Vector3 start, Vector3 target)
    {

        lineRenderer = GetComponent<LineRenderer>(); 

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, target);

        //float distance = Vector3.Distance(start, target);
        //lineRenderer.material.mainTextureScale = new Vector2(distance * 2, 1);


        float width = lineRenderer.startWidth;
        lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
    }
}
