using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// Handels all visual parts of the PieMenu
/// </summary>
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

    [SerializeField]
    GameObject menuCursor;

    GameObject instiatedMenuCursor;

    IMixedRealityPointer pointer;

    /// <summary>
    /// The contructor
    /// </summary>
    /// <param name="pointer"></param> The pointer of the input source that opend this PieMenu
    public void Constructor(IMixedRealityPointer pointer)
    {
        this.pointer = pointer;
        currentlyHighlighted = int.MinValue;

        //Positioning
        transform.LookAt(Camera.main.transform);
        transform.Rotate(new Vector3(0, 180, 0),Space.Self);

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
            piece.transform.Rotate(new Vector3(0, 0, EntryNumberToRotation(i)), Space.Self);
            pieMenuPieces.Add(piece);
            PlaceIcon(i, 0.5f);
        }

        instiatedMenuCursor = Instantiate(menuCursor, transform);
    }

    /// <summary>
    /// Convert an index of the MenuEntry array to the corrosponding rotation on the PieMenu
    /// </summary>
    /// <param name="number"></param> The index from the MenuEntry array
    /// <returns></returns> The corresponding rotation on the PieMenu
    float EntryNumberToRotation(int number)
    {
        return ((float)number / menuEntries.Count) * 360;
    }

    /// <summary>
    /// Convert the position of the pointer to the corresponding index from the MenuEntry array
    /// </summary>
    /// <param name="projectedPointer"></param> The position of the pointer projected on the plane of the pie menu
    /// <returns></returns> The corresponding index from the MenuEntry array
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

    /// <summary>
    /// Highlight the i'th piece on the PieMenu
    /// </summary>
    /// <param name="i"></param> The number of the entry that should be highlighted
    public void HighlightPiece(int i)
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

    /// <summary>
    /// Place the icon from the menu entry with the number entryNumber on the correct position in the PieMenu
    /// </summary>
    /// <param name="entryNumber"></param> The number of the menu entry
    /// <param name="menuRadius"></param> The radius of the PieMenu
    void PlaceIcon(int entryNumber, float menuRadius)
    {
        //Place the icon in the middle of the piece
        Image icon = pieMenuPieces[entryNumber].transform.Find("CanvasIcon/Icon").GetComponent<Image>();
        icon.sprite = menuEntries[entryNumber].iconTool;
        icon.rectTransform.localPosition = Quaternion.Euler(0, 0, 0.5f * EntryNumberToRotation(1)) * new Vector3(0, -1, 0) * menuRadius / 2;

        //The icons are rotate wrong in the worldspace because the pieces they are attached to were rotated in the positioning process. This reverses the unwanted rotation of the icons.
        icon.rectTransform.Rotate(0, 0, -EntryNumberToRotation(entryNumber), Space.Self);
    }


    /// <summary>
    /// Calculate the new position of the pointer and highlight/dehighlight correspondingly
    /// </summary>
    void Update()
    {
        instiatedMenuCursor.transform.position = pointer.Position;
        Vector3 localPosition = instiatedMenuCursor.transform.localPosition;
        instiatedMenuCursor.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0);
        HighlightPiece(CalculatePieceID(instiatedMenuCursor.transform.localPosition));
    }
}
