using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CompetenceDisplayVisualController))]
public class CompetenceDisplaySynchronizer : TransformSynchronizer
{
    private CompetenceDisplayVisualController competenceController;

    private void Awake()
    {
        competenceController = GetComponent<CompetenceDisplayVisualController>();
    }
}
