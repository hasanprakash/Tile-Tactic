using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Attachment attachment;
    private bool isDrag = false;
    private Vector3 worldPosition;
    private GridManager gridManager;
    private Vector3 tilePositionOnAttGrid;
    private Vector3 previousCoordinate;
    private Vector3 currentCoordinate;
    private SpriteRenderer spriteRenderer;
    private LevelInfo levelInfo;
    private AudioManager audioManager;
    public bool isDraggable = true;

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }
    void Start()
    {
        TryGetComponent<Attachment>(out attachment);
        gridManager = FindObjectOfType<GridManager>();
        tilePositionOnAttGrid = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        levelInfo = FindObjectOfType<LevelInfo>();
    }

    private void OnMouseDown()
    {
        if(attachment)
        {
            attachment.MouseDown();
            return;
        }
        if(!isDraggable) return;
        isDrag = true;
        spriteRenderer.sortingLayerName = "ActiveAttachment";

        if (gridManager.IsCoordinateInsideTileGrid(currentCoordinate))
        {
            SetTileOccupiedStatus(currentCoordinate, false);
        }
    }

    private void OnMouseDrag()
    {
        if(attachment )
        {
            attachment.MouseDrag();
            return;
        }
        if (!isDraggable) return;
        if (isDrag)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            transform.position = worldPosition;
        }
    }

    private void OnMouseUp()
    {
        if (attachment)
        {
            attachment.MouseUp();
            return;
        }

        if (!isDraggable) return;
        bool status = ValidateDestinationPosition(gridManager.ConvertWorldPositionToTileCoordinate(transform.position));
        if(!status)
        {
            transform.position = tilePositionOnAttGrid;
            audioManager.Play("TileAttachment");
            isDrag = false;
            currentCoordinate = gridManager.ConvertWorldPositionToTileCoordinate(tilePositionOnAttGrid);
            return;
        }
        float x, y;
        gridManager.ConvertInputPositionToTileCoordinate(Input.mousePosition, out x, out y);
        previousCoordinate = currentCoordinate;
        currentCoordinate = new Vector3(x, y);
        transform.position = gridManager.ConvertTileCoordinateToWorldPosition(currentCoordinate);
        audioManager.Play("TileAttachment");
        SetTileOccupiedStatus(previousCoordinate, false);
        SetTileOccupiedStatus(currentCoordinate, true);
        if (gridManager.IsCoordinateInsideAttachmentGrid(currentCoordinate))
            tilePositionOnAttGrid = transform.position;

        spriteRenderer.sortingLayerName = "Director";
    }

    bool ValidateDestinationPosition(Vector3 destinationCoordinate)
    {
        if (!gridManager.IsCoordinateInsideAttachmentGrid(destinationCoordinate) &&
            !gridManager.IsCoordinateInsideTileGrid(destinationCoordinate)) return false;
        if (levelInfo.level.attachmentTracker[(int)destinationCoordinate.x, (int)destinationCoordinate.y].occupied) return false;
        return true;
    }

    void SetTileOccupiedStatus(Vector3 coordinate, bool status)
    {
        levelInfo.level.attachmentTracker[(int)coordinate.x, (int)coordinate.y].occupied = status;
    }
}
