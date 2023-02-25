using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovement : MonoBehaviour
{
    private char movingDirection = 'U'; // U L R D UL UR DL DR
    private Vector3 nextPosition;
    private Vector3 adjustPosition;
    private float timeToMove = 0.5f;
    private float adjustTimeToMove = 0.1f;
    float distance = 1f;
    float speed;
    float adjustSpeed;
    float speedControl = 2f;
    float timeVar = 0.5f;
    private bool isMoving;
    private bool isPositionAdjusting;
    Vector3 presentCoordinate;
    Vector3 previousCoordinate;
    GridManager gridManager;
    private MovementMaster movementMaster;
    private List<GameObject> collidedAttachments;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        presentCoordinate = gridManager.GetTileCoordinate(transform.position);
        collidedAttachments = new List<GameObject>();
    }
    void Start()
    {
        speed = distance / timeToMove;
        adjustSpeed = distance / adjustTimeToMove;
        isMoving = false;
        isPositionAdjusting = false;
        movementMaster = FindObjectOfType<MovementMaster>();
        previousCoordinate = Vector3.zero;

        DisableCollider();
    }

    public void HeroMove()
    {
        speedControl = 2f;
        CheckGameOverStatus();
        UpdateHeroNextPosition();
        isMoving = true;
    }
    void UpdateHeroNextPosition()
    {
        if (movingDirection == 'U')
            nextPosition = transform.position + transform.up;
        else if (movingDirection == 'R')
            nextPosition = transform.position + transform.right;
        else if (movingDirection == 'L')
            nextPosition = transform.position + (transform.right * -1);
        else if (movingDirection == 'D')
            nextPosition = transform.position + (transform.up * -1);
    }

    public void StartHeroRoutine()
    {
        StopHeroRoutine();
        InvokeRepeating("HeroMove", timeToMove, timeToMove + 0.2f);
    }
    public void StopHeroRoutine()
    {
        isMoving = false;
        CancelInvoke();
    }

    private void Update()
    {
        // MOVING
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            step *= speedControl;
            if (speedControl >= 1f) speedControl -= (0.3f * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, step);
            timeVar -= Time.deltaTime;
            if (timeVar < 0f || Equals(transform.position, nextPosition))
            {
                ReorderHeroPosition();
                isMoving = false;
                timeVar = 0.5f;
            }
        }


        // ADJUSTING
        if (!isPositionAdjusting) return;
        float adjustStep = adjustSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, adjustPosition, adjustStep);
        if (Equals(transform.position, adjustPosition))
            isPositionAdjusting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        char directionCode;
        if (collision.gameObject.tag == "Director")
        {
            Director director = collision.gameObject.GetComponent<Director>();
            directionCode = director.GetDirectionCode();
            movingDirection = directionCode;
        }
        else if (collision.gameObject.tag == "Attachment") { } // not used
        else if(collision.gameObject.tag == "End")
        {
            movementMaster.GameWin();
        }
        if(collidedAttachments.Count > 1)
        {
            StopMovement();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Attachment")
            collidedAttachments.Add(collision.gameObject);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Attachment")
            collidedAttachments.Remove(collision.gameObject);
    }

    public void ReorderHeroPosition()
    {
        Vector3 newPosition = gridManager.ReorderedPosition(transform.position);
        adjustPosition = newPosition;
        isPositionAdjusting = true;
    }

    void CheckGameOverStatus()
    {
        if(!gridManager)
        {
            Debug.Log("Grid Manager is null -- CheckGameOverStatus()");
        }

        presentCoordinate = gridManager.GetTileCoordinate(transform.position);
        if(Equals(previousCoordinate, presentCoordinate) || !gridManager.IsCoordinateInsideTileGrid(presentCoordinate))
        {
            StopMovement();
        }
        previousCoordinate = presentCoordinate;
    }

    public void StopMovement()
    {
        movementMaster.GameFailed();
    }

    public void DisableCollider()
    {
        if (gameObject != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
    public void EnableCollider()
    {
        if (gameObject != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }
}
