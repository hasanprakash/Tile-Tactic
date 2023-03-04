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
    private int freezeCount = 0;
    float distance = 1f;
    float speed;
    float adjustSpeed;
    float adjustStep;
    float speedControl = 2f;
    float timeVar = 0.5f;

    private bool isFirstAdjusting;
    private bool isMoving;
    private bool isLastAdjusting;

    private bool isGameOver = false;

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
        isFirstAdjusting = false;
        isMoving = false;
        isLastAdjusting = false;
        movementMaster = FindObjectOfType<MovementMaster>();
        previousCoordinate = Vector3.zero;

        DisableCollider();
    }

    public void HeroMove()
    {
        if (isGameOver)
        {
            movementMaster.GameWin();
            return;
        }

        bool freezeStatus = CheckFreezerStatus();
        if (!freezeStatus) return;
        speedControl = 2f;
        /*CheckGameOverStatus();*/
        /*UpdateHeroNextPosition();*/  // changed to update the position after first adjustment
        ReorderHeroPosition();
        isFirstAdjusting = true;
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

    private void Update()
    {

        // FIRST ADJUSTMENT
        if (isFirstAdjusting)
        {
            previousCoordinate = gridManager.GetTileCoordinate(transform.position);
            adjustStep = adjustSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, adjustPosition, adjustStep);
            if (Equals(transform.position, adjustPosition))
            {
                isFirstAdjusting = false;
                UpdateHeroNextPosition();
                isMoving = true;
            }
        }

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
                timeVar = 0.5f;
                isMoving = false;
                isLastAdjusting = true;
            }
        }


        // LAST ADJUSTMENT
        if (!isLastAdjusting) return;
        adjustStep = adjustSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, adjustPosition, adjustStep);
        if (Equals(transform.position, adjustPosition))
        {
            isLastAdjusting = false;
            CheckGameOverStatus();
        }
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
        else if(collision.gameObject.tag == "End")
        {
            isGameOver = true;
        }
        else if(collision.gameObject.tag == "Freezer")
        {
            freezeCount++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Attachment") { } // not used
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Attachment" || collision.gameObject.tag == "Blocker")
            collidedAttachments.Add(collision.gameObject);
        if (collidedAttachments.Count > 1)
            StopMovement();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Attachment" || collision.gameObject.tag == "Blocker")
            collidedAttachments.Remove(collision.gameObject);
    }

    public void ReorderHeroPosition()
    {
        Vector3 newPosition = gridManager.ReorderedPosition(transform.position);
        adjustPosition = newPosition;
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
    }

    public bool CheckFreezerStatus()
    {
        if(freezeCount > 0)
        {
            freezeCount--;
            return false;
        }
        return true;
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
