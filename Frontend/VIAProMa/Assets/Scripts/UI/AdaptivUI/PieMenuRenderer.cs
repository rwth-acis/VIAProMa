using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieMenuRenderer : MonoBehaviour
{
    
    [SerializeField]
    GameObject piemenuPiecePrefab;
    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color highlightColor;

    List<MenuEntry> menuEntries;
    List<GameObject> pieMenuPieces;
    Image currentlyHighlighted;
    int test = 0;
    // Start is called before the first frame update
    void Start()
    {
        menuEntries = new List<MenuEntry>(PieMenuManager.Instance.menuEntries);
        int numberItems = menuEntries.Count;
        pieMenuPieces = new List<GameObject>();
        for (int i = 0; i < numberItems; i++)
        {
            GameObject piece = Instantiate(piemenuPiecePrefab, transform);
            Image pieceImage = piece.transform.Find("CanvasPiePiece/PiePiece").GetComponent<Image>();
            pieceImage.fillAmount = 1f / numberItems;
            pieceImage.color = normalColor;
            piece.transform.Rotate(new Vector3(0,0, entryNumberToRotation(i)),Space.Self);
            pieMenuPieces.Add(piece);
            placeIcon(i, 0.5f);
        }
    }

    float entryNumberToRotation(float number)
    {
        return (number / menuEntries.Count) * 360;
    }

    public void highlightPiece(int i)
    {
        Image image = pieMenuPieces[i].transform.Find("CanvasPiePiece/PiePiece").GetComponent<Image>();
        if (image != currentlyHighlighted)
        {
            image.color = highlightColor;
            Transform piece = pieMenuPieces[i].transform;
            piece.localScale = Vector3.Scale(piece.localScale, new Vector3(1.2f, 1.2f, 1));

            if (currentlyHighlighted != null)
            {
                currentlyHighlighted.color = normalColor;
                piece = currentlyHighlighted.transform.parent.parent;
                piece.localScale = Vector3.Scale(piece.localScale, new Vector3(1 / 1.2f, 1 / 1.2f, 1));
            }
            currentlyHighlighted = image;
        }
    }

    void placeIcon(int entryNumber, float menuRadius)
    {
        Image icon = pieMenuPieces[entryNumber].transform.Find("CanvasIcon/Icon").GetComponent<Image>();
        icon.sprite = menuEntries[entryNumber].icon;
        float rotation = entryNumberToRotation(entryNumber);
        icon.rectTransform.localPosition = Quaternion.Euler(0, 0, entryNumberToRotation(0.5f)) * new Vector3(0, 1, 0) * menuRadius / 2;
    }

    //bool alreadyUpdated = false;
    // Update is called once per frame
    void Update()
    {
        //if (System.DateTime.Now.Second % 2 == 0 && !alreadyUpdated)
        //{
        //    highlightPiece(test);
        //    test++;
        //    if (test >= menuEntries.Count)
        //    {
        //        test = 0;
        //    }
        //    alreadyUpdated = true;
        //}
        //else if(System.DateTime.Now.Second % 2 != 0)
        //{
        //    alreadyUpdated = false;
        //}
    }
}
