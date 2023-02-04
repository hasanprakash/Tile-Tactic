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
    void Start()
    {
        speed = distance / timeToMove;
        adjustSpeed = distance / adjustTimeToMove;
        isMoving = false;
        isPositionAdjusting = false;
        gridManager = FindObjectOfType<GridManager>();
        movementMaster = FindObjectOfType<MovementMaster>();
        presentCoordinate = gridManager.GetTileCoordinate(transform.position);
        previousCoordinate = Vector3.zero;
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
        else if (collision.gameObject.tag == "Attachment") { }
        else if(collision.gameObject.tag == "End")
        {
            movementMaster.GameWin();
        }
    }

    public void ReorderHeroPosition()
    {
        Vector3 newPosition = gridManager.ReorderedPosition(transform.position);
        adjustPosition = newPosition;
        isPositionAdjusting = true;
    }

    void CheckGameOverStatus()
    {
        presentCoordinate = gridManager.GetTileCoordinate(transform.position);
        if(Equals(previousCoordinate, presentCoordinate))
        {
            StopMovement();
        }
        previousCoordinate = presentCoordinate;
    }

    public void StopMovement()
    {
        movementMaster.GameFailed();
    }
}
