using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
   private void CheckLedgeClimb()
    {
        if(ledgeDetected && !canClimbLedge)
        {
            
            canClimbLedge = true;
            if(canClimbLedge)
            {
                isWallSliding = false;
            }
           
            if (facingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallDistance) - ledgeXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallDistance) + ledgeXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallDistance) + ledgeXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallDistance) - ledgeXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeYOffset2);
            }
            ChangeAnimationState(ledgeHang);
            canMove = false;
            canFlip = false;

            rb.linearVelocity = new Vector2(0,0);
            transform.position = new Vector2(ledgePos1.x + (facingRight ? .2f : -.2f), ledgePos1.y + (facingRight ? .5f : .5f));
            rb.gravityScale = 0;
           
        }

        if(canClimbLedge && Input.GetKeyDown(KeyCode.Space))
        {
            ChangeAnimationState(ledgeClimb);
        }
        if(canClimbLedge && Input.GetKeyDown(KeyCode.S))
        {
            FinishLedgeClimb();
        }   
    }

    public void FinishLedgeClimb()
    {
        transform.position = new Vector2(transform.position.x + (facingRight ?  -.1f : .1f), transform.position.y);
        canClimbLedge = false;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        rb.gravityScale = gravity;
    }
    public void ChangePlayerPosition()
    {
        canClimbLedge = false;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        rb.gravityScale = gravity;
        transform.position = new Vector2(ledgePos2.x, ledgePos2.y + .4f);
        if (isGrounded)
            ChangeAnimationState(idle);
    }
    void Move()
    {
        if (canMove)
        {
            if (!isWallJumping)
            {
                if (!isRolling && !isAttacking && !isPraying && !playerManager.isHealing && !isStunned && !isFallAttacking)
                {
                    if (!isGuarding)
                    {
                        if(!walkToggle)
                            rb.linearVelocity = new Vector2(xAxis * runSpeed, rb.linearVelocity.y);
                        else
                            rb.linearVelocity = new Vector2(xAxis * walkSpeed, rb.linearVelocity.y);
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(xAxis * runSpeed / 2, rb.linearVelocity.y);
                    }
                }
            }
            else
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(xAxis * runSpeed, rb.linearVelocity.y)), wallJumpLerp * Time.deltaTime);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0,0);
            return;
        }
    }
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGuarding = false;
        /*if (isJumping && jumpTimer < 1) //isGuarding eklenebilir
        {
            jumpTimer += Time.fixedDeltaTime;
            jumpForce = Math.Round(jumpTimer,2);
        }*/
        
    }
    void WallJump()
    {
        if (wallJumpPressed)
        {
            if((wallDirection == 1 && !facingRight) || (wallDirection == -1 && facingRight))
            {
                Flip();
            }

            rb.linearVelocity = new Vector2(xWallForce * wallDirection, 10);
            wallJumpPressed = false;

            StartCoroutine(WallJumpWaiter());
        }
    }

    IEnumerator WallJumpWaiter()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(0.4f);
        isWallJumping = false;
    }

    void WallSlide()
    {
        if (canGrab && !isGrounded && !canClimbLedge && xAxis != 0 && !isFallAttacking)
        {
            isWallSliding = true;
            jumpTime = Time.time + wallJumpTime;
        }
        else if (jumpTime < Time.time)
        {
            isWallSliding = false;
        }  
        else if (canClimbLedge == true)
        {
            isWallSliding = false;
        }
        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
    }
    
    IEnumerator Roll()
    {
        isRolling = true;
        playerManager.damageable = false;
        rollColl.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        StaminaBar.instance.useStamina(rollStaminaCost);
        Debug.Log((rollStaminaCost) + "stamina kullanildi");

        if (facingRight)
            rb.linearVelocity = new Vector2(rollSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(-rollSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(rollSeconds);
        isRolling = false;
        playerManager.damageable = true;
        rollColl.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = true;
    }
    void FlipPlayer()
    {
        if(!isRolling && !isPraying && !isAttacking && !isStunned)
        {
            if(xAxis < 0 && facingRight)
            {
                Flip();
            }
            else if(xAxis > 0 && !facingRight)
            {
                Flip();
            }
        }
    }
    
    void Flip(){
        if(canFlip)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingRight = !facingRight;
        }
    }
}
