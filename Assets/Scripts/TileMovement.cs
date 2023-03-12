using UnityEngine;
using UnityEngine.InputSystem;

public class TileMovement : MonoBehaviour
{
    [SerializeField] private string attachmentId = "NA";
    //public InputMaster controls;
    private bool isMoving = false;
    private Vector3 nextPosition;
    float timeToMove = 0.5f;
    float distance = 1f;
    float speed;
    float tileStep;

    private int noOfTilesToMove = 0; // comes from attachment ID
    private char directionToMove = '\0'; // comes from attachment ID

    // Less Important Variables
    int dynamicDirection = 0;
    int stepCount = 1;

    private void Awake()
    {
        //controls = new InputMaster();
        //controls.TileMovement.Move.performed += context => Move(context.ReadValue<Vector2>());
        speed = distance / timeToMove;
    }

    //MovementConfiguration(); // this should run before animation starts for everytime

    public void AttachmentMove()
    {
        //MovementConfiguration();
        if (stepCount == noOfTilesToMove && directionToMove == 'R') dynamicDirection = -1;
        if (stepCount == 1 && directionToMove == 'R') dynamicDirection = 1;

        if (stepCount == noOfTilesToMove * -1 && directionToMove == 'L') dynamicDirection = 1;
        if (stepCount == -1 && directionToMove == 'L') dynamicDirection = -1;

        nextPosition = transform.position + (transform.right * dynamicDirection);
        isMoving = true;

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
        if (directionToMove == 'R')
        {
            dynamicDirection = 1;
            stepCount = 1;
        }
        if (directionToMove == 'L')
        {
            dynamicDirection = -1;
            stepCount = -1;
        }
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
        tileStep = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, tileStep);
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
}
