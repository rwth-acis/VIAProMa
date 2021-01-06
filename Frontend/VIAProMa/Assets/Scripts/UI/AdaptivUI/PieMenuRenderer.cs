using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;

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
    public int currentlyHighlighted { private set; get;}
    int test = 0;

    GameObject mainCamera;
    [SerializeField]
    GameObject menuCursor;

    GameObject instiatedMenuCursor;

    IMixedRealityPointer pointer;

    public void constructor(IMixedRealityPointer pointer, GameObject mainCamera)
    {
        this.pointer = pointer;
        this.mainCamera = mainCamera;
        currentlyHighlighted = int.MinValue;

        //Positioning
        transform.LookAt(mainCamera.transform);

        //Generation
        menuEntries = new List<MenuEntry>(PieMenuManager.Instance.menuEntries);
        int numberItems = menuEntries.Count;
        pieMenuPieces = new List<GameObject>();
        for (int i = 0; i < numberItems; i++)
        {
            GameObject piece = Instantiate(piemenuPiecePrefab, transform);
            Image pieceImage = piece.transform.Find("CanvasPiePiece/PiePiece").GetComponent<Image>();
            pieceImage.fillAmount = 1f / numberItems;
            pieceImage.color = normalColor;
            piece.transform.Rotate(new Vector3(0, 0, entryNumberToRotation(i)), Space.Self);
            pieMenuPieces.Add(piece);
            placeIcon(i, 0.5f);
        }

        instiatedMenuCursor = Instantiate(menuCursor, transform);
    }

    float entryNumberToRotation(int number)
    {
        return ((float)number / menuEntries.Count) * 360;
    }

    int CalculatePieceID(Vector2 projectedPointer)
    {
        float angle = Vector2.SignedAngle(Vector2.down, projectedPointer);
        if (angle < 0)
        {
            angle = 360 + angle;
        }
        int i = (int)(angle / 360 * menuEntries.Count);
        return i;
    }

    public void highlightPiece(int i)
    {
        if (i != currentlyHighlighted)
        {
            Image image = pieMenuPieces[i].transform.Find("CanvasPiePiece/PiePiece").GetComponent<Image>();
            image.color = highlightColor;
            Transform piece = pieMenuPieces[i].transform;
            piece.localScale = Vector3.Scale(piece.localScale, new Vector3(1.2f, 1.2f, 1));

            if (currentlyHighlighted >= 0 && currentlyHighlighted < pieMenuPieces.Count)
            {
                Image oldImage = pieMenuPieces[currentlyHighlighted].transform.Find("CanvasPiePiece/PiePiece").GetComponent<Image>();
                oldImage.color = normalColor;
                piece = oldImage.transform.parent.parent;
                piece.localScale = Vector3.Scale(piece.localScale, new Vector3(1 / 1.2f, 1 / 1.2f, 1));
            }

            currentlyHighlighted = i;
        }
    }

    void placeIcon(int entryNumber, float menuRadius)
    {
        //Place the icon in the middle of the piece
        Image icon = pieMenuPieces[entryNumber].transform.Find("CanvasIcon/Icon").GetComponent<Image>();
        icon.sprite = menuEntries[entryNumber].icon;
        icon.rectTransform.localPosition = Quaternion.Euler(0, 0, 0.5f * entryNumberToRotation(1)) * new Vector3(0, -1, 0) * menuRadius / 2;

        //The icons are rotate wrong in the worldspace because the pieces they are attached to were rotated in the positioning process. This reverses the unwanted rotation of the icons.
        icon.rectTransform.Rotate(0, 0, -entryNumberToRotation(entryNumber), Space.Self);
    }



    void Update()
    {
        instiatedMenuCursor.transform.position = pointer.Position;
        Vector3 localPosition = instiatedMenuCursor.transform.localPosition;
        instiatedMenuCursor.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0);
        highlightPiece(CalculatePieceID(instiatedMenuCursor.transform.localPosition));
    }
}
