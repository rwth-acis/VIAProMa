﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieMenuRenderer : MonoBehaviour
{
    int numberItems = 15;
    [SerializeField]
    GameObject piemenuPiecePrefab;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color highlightColor;

    List<Image> pieceImages;
    Image currentlyHighlighted;
    int test = 0;
    // Start is called before the first frame update
    void Start()
    {
        pieceImages = new List<Image>();
        for (int i = 0; i < numberItems; i++)
        {
            GameObject piece = Instantiate(piemenuPiecePrefab, transform);
            Image pieceImage = piece.GetComponentInChildren<Image>();
            pieceImage.fillAmount = 1f / numberItems;
            pieceImage.color = normalColor;
            piece.transform.Rotate(new Vector3(0,0, entryNumberToRotation(i)),Space.Self);
            pieceImages.Add(pieceImage);
        }
        highlightPiece(0);
    }

    float entryNumberToRotation(int number)
    {
        return ((float)number / numberItems) * 360;
    }

    void highlightPiece(int i)
    {
        pieceImages[i].color = highlightColor;
        Transform piece = pieceImages[i].transform.parent.parent;
        piece.localScale = Vector3.Scale(piece.localScale, new Vector3(1.2f,1.2f,1));

        if (currentlyHighlighted != null)
        {
            currentlyHighlighted.color = normalColor;
            piece = currentlyHighlighted.transform.parent.parent;
            piece.localScale = Vector3.Scale(piece.localScale, new Vector3(1/1.2f, 1/1.2f, 1));
        }
        currentlyHighlighted = pieceImages[i];
    }

    bool alreadyUpdated = false;
    // Update is called once per frame
    void Update()
    {
        if (System.DateTime.Now.Second % 2 == 0 && !alreadyUpdated)
        {
            highlightPiece(test);
            test++;
            if (test >= numberItems)
            {
                test = 0;
            }
            alreadyUpdated = true;
        }
        else if(System.DateTime.Now.Second % 2 != 0)
        {
            alreadyUpdated = false;
        }
    }
}
