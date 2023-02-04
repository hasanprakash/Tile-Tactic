using UnityEngine;
using UnityEngine.InputSystem;

public class TileMovement : MonoBehaviour
{
    [SerializeField] private string attachmentId = "NA";
    public InputMaster controls;
    private bool isMoving = false;
    private Vector3 nextPosition;
    float timeToMove = 0.5f;
    float distance = 1f;
    float speed;

    private int noOfTilesToMove = 0;
    private char directionToMove = '\0';

    // Not Important Variables
    int dynamicDirection = 0;
    int stepCount = 1;

    private void Awake()
    {
        controls = new InputMaster();
        controls.TileMovement.Move.performed += context => Move(context.ReadValue<Vector2>());
        speed = distance / timeToMove;
    }

    private void Start()
    {
        //MovementConfiguration(); // this should run before animation starts for everytime
    }

    void Move(Vector2 direction)
    {
        if (!isMoving)
        {
            nextPosition = transform.position + new Vector3(direction.x, direction.y, 0f);
            isMoving = true;
        }
    }
    public void AttachmentMove()
    {
        //MovementConfiguration();
        if (stepCount == noOfTilesToMove && directionToMove == 'R') dynamicDirection = -1;
        if (stepCount == 1 && directionToMove == 'R') dynamicDirection = 1;
        if (directionToMove == 'R')
        {
            nextPosition = transform.position + (transform.right * dynamicDirection);
            isMoving = true;
        }
        if (directionToMove == 'L')
        {
            nextPosition = transform.position + (transform.right * dynamicDirection * -1f);
            isMoving = true;
        }
        stepCount += dynamicDirection;
    }
    public void StartAttachmentRoutine()
    {
        StopAttachmentRoutine();
        InvokeRepeating("AttachmentMove", timeToMove, timeToMove + 0.2f);
    }
    public void StopAttachmentRoutine()
    {
        CancelInvoke();
        isMoving = false;
    }
    public void MovementConfiguration()
    {
        if (attachmentId == "NA") return;
        noOfTilesToMove = attachmentId[0] - 48;
        directionToMove = attachmentId[1];
        if (directionToMove != 'R' && directionToMove != 'L') return;
        if (directionToMove == 'R') dynamicDirection = 1;
        if (directionToMove == 'L') dynamicDirection = -1;
        stepCount = 1;
    }
    public void ClearConfiguration()
    {
        noOfTilesToMove = 0;
        directionToMove = '\0';
        stepCount = 1;
    }
    private void Update()
    {
        if (!isMoving) return;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, step);
        if(transform.position == nextPosition)
            isMoving = false;
    }

    public void StopMovement()
    {
        isMoving = false;
    }
    public void SetNextPosition(Vector3 nextPos)
    {
        nextPosition = nextPos;
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
