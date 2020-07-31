using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ProgressOrganizer : MonoBehaviour
{
    [SerializeField] private GameObject ProgressList;
    private Vector3 newpos = new Vector3(-0.78f, 1.5f, -0.5f);

    public void myClear()
    {
        int num = ProgressList.transform.childCount;

        if (num != 0)
        {
            GameObject n = ProgressList.transform.GetChild(num-1).gameObject;
            if (n.transform.position == newpos)
            {
                PhotonNetwork.Destroy(n);
            }
        }
    }

    public void Newspace()
    {
        Vector3 pos = new Vector3(-0.78f, 1.5f, -0.5f);
        int num = ProgressList.transform.childCount;
        pos.y += num*0.8f;

        foreach (Transform child in ProgressList.transform)
        {
            child.transform.position = pos;
            pos.y -= 0.8f;
        }
    }

    public void Compress()
    {
        Vector3 pos = new Vector3(-0.78f, 0.7f, -0.5f);
        int num = ProgressList.transform.childCount;
        pos.y += num*0.8f;

        foreach(Transform child in ProgressList.transform)
        {
            child.transform.position = pos;
            pos.y -= 0.8f;
        }
    }

}

