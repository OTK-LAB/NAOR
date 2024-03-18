using UnityEngine;

public class Archer_StartingMove: ArcherState
{
    private Archer.State nextState; // Next state variable
    
    private Vector2 playerPos; // Updated each frame
    private float verticalTolerance = 1.5f;
    private float followDistance = 15.0f;
    private int moveDirection = 1;
    
    public Archer_StartingMove(GameObject _archerGameObject, GameObject _playerGameObject)
        : base(Archer.State.STATE_STARTINGMOVE, _archerGameObject, _playerGameObject)
    {}

    public override void EnterState()
    {
        nextState = Archer.State.STATE_STARTINGMOVE;
        archerComponent.ChangeAnimationState("run");
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        playerPos = playerGameObject.transform.position;
        checkPlayer();
        
        float moveDirectionX = moveDirection;
        float step = archerComponent.moveSpeed * moveDirectionX;
        archerRigidBody.velocity = new Vector3(step, archerRigidBody.velocity.y);
    }

    public override Archer.State GetNextState()
    {
        return nextState;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("wall"))
        {
            moveRight = !moveRight;
            moveDirection *= -1;
            playerGameObject.transform.Rotate(0f, 180f, 0f);
        }
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        
    }
    
    void checkPlayer()
    {
        Vector2 enemyPosition = new Vector2(archerRigidBody.transform.position.x, archerRigidBody.transform.position.y);
        
        float distanceToPlayer = Vector2.Distance(enemyPosition, playerPos);
        if (distanceToPlayer < followDistance && Mathf.Abs(enemyPosition.y - playerPos.y) < verticalTolerance)
        {
            nextState = Archer.State.STATE_ATTACK;
            flip();
        }
    }
    
    void flip()
    {
        if (playerPos.x > (archerGameObject.transform.position.x + 0.5f))
        {
            if (!moveRight)
            {
                archerGameObject.transform.Rotate(0f, 180f, 0f);
                moveRight = true;
                moveDirection *= -1;
            }
        }
        else
        {
            if (moveRight)
            {
                archerGameObject.transform.Rotate(0f, 180f, 0f);
                moveRight = false;
                moveDirection *= -1;
            }
        }
    }
}
