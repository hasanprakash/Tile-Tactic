using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Attachment : MonoBehaviour
{
    [SerializeField] private string attachmentId;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Color baseColor;
    [SerializeField] public bool isDraggable;
    [SerializeField] private GameObject countObject;
    TMP_Text countText;
    private bool isDrag = false;
    GridManager gridManager;
    TileMovement tileMovement;
    MovementMaster movementMaster;
    Vector3 previousCoordinate;
    Vector3 currentCoordinate;
    Vector3 tilePosition;

    private Vector3 startPosition;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Start()
    {
        startPosition = Vector3.zero;
        movementMaster = FindObjectOfType<MovementMaster>();
        tileMovement = GetComponent<TileMovement>();
        renderer = GetComponent<SpriteRenderer>();
        currentCoordinate = GetCurrentCoordinate();
        tilePosition = transform.position;
    }

    public void Init()
    {
        renderer.color = baseColor;
    }

    private void OnMouseDown()
    {
        if (!isDraggable) return;
        isDrag = true;
        renderer.sortingLayerName = "ActiveAttachment";
        countObject.gameObject.SetActive(false);

        if (gridManager.IsCoordinateInsideTileGrid(currentCoordinate))
        {
            Debug.Log(currentCoordinate + " is inside the grid");
            SetOccupiedStatus(currentCoordinate, false);
        }
    }
    private void OnMouseDrag()
    {
        if (isDrag)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            transform.position = worldPosition;
        }
    }
    private void OnMouseUp()
    {
        if (!isDraggable) return;

        bool status = ValidateAttachmentDestinationPosition(gridManager.ConvertWorldPositionToTileCoordinate(transform.position));
        if(!status)
        {
            movementMaster.StopGameRoutine(gameObject);
            SetOccupiedStatus(currentCoordinate, false);
            transform.position = tilePosition;
            isDrag = false;
            renderer.sortingLayerName = "Attachment";
            float tileCoordinateX, tileCoordinateY;
            gridManager.ConvertWorldPositionToTileCoordinate(tilePosition, out tileCoordinateX, out tileCoordinateY);
            RefreshAllAttachmentCount(currentCoordinate, new Vector3(tileCoordinateX, tileCoordinateY));

            currentCoordinate = new Vector3(tileCoordinateX, tileCoordinateY);
            ValidateCountTextVisibility();
            return;
        }

        movementMaster.StopGameRoutine(gameObject); // resetting it's movement

        int gridWidth = gridManager.GetGridWidth(), gridHeight = gridManager.GetGridHeight();
        isDrag= false;
        float x, y;
        gridManager.ConvertInputPositionToTileCoordinate(Input.mousePosition, out x, out y);
        //transform.position = gridManager.d[new Vector3(x, y)].position;
        float worldX, worldY;
        gridManager.ConvertTileCoordinateToWorldPosition(new Vector3(x, y), out worldX, out worldY);

        previousCoordinate = currentCoordinate;
        currentCoordinate = new Vector3(x, y);
        transform.position = new Vector3(worldX, worldY);
        SetOccupiedStatus(previousCoordinate, false);
        SetOccupiedStatus(currentCoordinate, true);

        RefreshAllAttachmentCount(previousCoordinate, currentCoordinate);

        startPosition = transform.position;
        /*if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight - 2)
        {
            tileMovement.MovementConfiguration();
            movementMaster.StartGameRoutine(false, true);
        }
        else
        {
            tileMovement.ClearConfiguration();
        }*/

        renderer.sortingLayerName = "Attachment";
        ValidateCountTextVisibility();
    }

    void RefreshAllAttachmentCount(Vector3 previousCoordinate, Vector3 currentCoordinate)
    {
        //Debug.Log(previousCoordinate + " " + currentCoordinate);
        Level level = FindObjectOfType<LevelInfo>().level;
        AttachmentTracker prevAttachmentTracker = level.attachmentTracker[(int)previousCoordinate.x, (int)previousCoordinate.y];
        prevAttachmentTracker.attachmentsCount--;
        AttachmentTracker currAttachmentTracker = level.attachmentTracker[(int)currentCoordinate.x, (int)currentCoordinate.y];
        currAttachmentTracker.attachmentsCount++;

        currAttachmentTracker.attachments.Add(GetComponent<Attachment>());
        prevAttachmentTracker.attachments.RemoveAt(prevAttachmentTracker.attachments.Count - 1);

        foreach (Attachment att in currAttachmentTracker.attachments)
            att.RefreshAttachmentCount();
        foreach (Attachment att in prevAttachmentTracker.attachments)
            att.RefreshAttachmentCount();
    }

    public void RestoreToStartPosition()
    {
        if(!Equals(startPosition, Vector3.zero))
        {
            transform.position = startPosition;
            tileMovement.SetNextPosition(startPosition);
        }
    }

    public Vector3 GetCurrentCoordinate()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        float x, y;
        gridManager.ConvertWorldPositionToTileCoordinate(transform.position, out x, out y);
        return new Vector3(x, y);
    }

    public void RefreshAttachmentCount()
    {
        countText = countObject.GetComponentInChildren<TMP_Text>();
        Level level = FindObjectOfType<LevelInfo>().level;
        int count = -1;
        Vector3 currCoordinate = GetCurrentCoordinate();
        // Debug.Log("Refreshing coordinate for " + currCoordinate);
        count = level.attachmentTracker[(int)currCoordinate.x, (int)currCoordinate.y].attachmentsCount;
        if (count != -1)
        {
            countText.text = count.ToString();
            ValidateCountTextVisibility();
        }
        else
        {
            Debug.Log("Attachment Count not Found for "+gameObject.name);
        }
    }

    void ValidateCountTextVisibility()
    {
        bool isInsideAttachmentGrid = gridManager.IsPositionInsideAttachmentGrid(transform.position);
        if (isInsideAttachmentGrid) tilePosition = transform.position;
        if (!isInsideAttachmentGrid || Convert.ToInt32(countText.text) <= 1)
        {
            countObject.gameObject.SetActive(false);
        }
        else
            countObject.gameObject.SetActive(true);
    }

    bool ValidateAttachmentDestinationPosition(Vector3 coordinate)
    {
        if (!gridManager.IsCoordinateInsideAttachmentGrid(coordinate) &&
            !gridManager.IsCoordinateInsideTileGrid(coordinate)) return false;
        Level level = FindObjectOfType<LevelInfo>().level;
        int x = (int)coordinate.x, y = (int)coordinate.y;
        int noOfSteps = attachmentId[0] - 48;
        char direction = attachmentId[1];
        var attTrackerData = level.attachmentTracker;
        AttachmentTracker attTracker;
        if (direction == 'R')
        {
            for (int i = x; i < x + noOfSteps; i++)
            {
                if (i >= gridManager.GetGridWidth()) return false;
                attTracker = attTrackerData[i, y];
                if (attTracker.occupied) return false;
            }
        }
        else if (direction == 'L')
        {
            for (int i = x; i > x - noOfSteps; i--)
            {
                if (i < 0) return false;
                attTracker = attTrackerData[i, y];
                if (attTracker.occupied) return false;
            }
        }

        return true;
    }

    void SetOccupiedStatus(Vector3 coordinate, bool status)
    {
        if (!gridManager.IsCoordinateInsideTileGrid(coordinate))
        {
            Debug.Log(coordinate + " is not inside the grid -- SetOccupiedStatus()");
            return;
        }
        Debug.Log(coordinate + " " + status);

        Level level = FindObjectOfType<LevelInfo>().level;
        int x = (int)coordinate.x, y = (int)coordinate.y;

        int noOfSteps = attachmentId[0] - 48;
        char direction = attachmentId[1];
        var attTrackerData = level.attachmentTracker;
        AttachmentTracker attTracker;
        if(direction == 'R')
        {
            for(int i=x;i<x + noOfSteps;i++)
            {
                attTracker = attTrackerData[i, y];
                attTracker.occupied = status;
            } 
        }
        else if(direction == 'L')
        {
            for(int i=x;i>x-noOfSteps;i--)
            {
                attTracker = attTrackerData[i, y];
                attTracker.occupied = status;
            }
        }

    }

    Vector3 ConvertCoordinateToPosition(Vector3 coordinate)
    {
        float positionX, positionY;
        gridManager.ConvertTileCoordinateToWorldPosition(coordinate, out positionX, out positionY);
        return new Vector3(positionX, positionY);
    }

    public string GetAttachmentId()
    {
        return attachmentId;
    }
    public Vector3 GetInitialPosition()
    {
        return startPosition;
    }
}
