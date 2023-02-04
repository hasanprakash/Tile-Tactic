using UnityEngine;

public class Attachment : MonoBehaviour
{
    [SerializeField] private string attachmentId;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Color baseColor;
    [SerializeField] public bool isDraggable;
    private bool isDrag = false;
    GridManager gridManager;
    TileMovement tileMovement;
    MovementMaster movementMaster;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = Vector3.zero;
        gridManager = FindObjectOfType<GridManager>();
        movementMaster = FindObjectOfType<MovementMaster>();
        tileMovement = GetComponent<TileMovement>();
    }

    public void Init()
    {
        renderer.color = baseColor;
    }

    private void OnMouseDown()
    {
        if(isDraggable)
            isDrag = true;
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

        movementMaster.StopGameRoutine(gameObject); // resetting it's movement

        int gridWidth = gridManager.GetGridWidth(), gridHeight = gridManager.GetGridHeight();
        isDrag= false;
        float x, y;
        gridManager.ConvertInputPositionToTileCoordinate(Input.mousePosition, out x, out y);
        //transform.position = gridManager.d[new Vector3(x, y)].position;
        float worldX, worldY;
        gridManager.ConvertTileCoordinateToWorldPosition(new Vector3(x, y), out worldX, out worldY);
        transform.position = new Vector3(worldX, worldY);

        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight - 2)
        {
            startPosition = transform.position;
            tileMovement.MovementConfiguration();
            movementMaster.StartGameRoutine(false, true);
        }
        else
        {
            tileMovement.ClearConfiguration();
        }
    }

    public void RestoreToStartPosition()
    {
        if(!Equals(startPosition, Vector3.zero))
        {
            transform.position = startPosition;
            tileMovement.SetNextPosition(startPosition);
        }
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
