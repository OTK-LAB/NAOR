using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    void performGuard()
    {
        if(!isRolling && isGrounded && !isPraying && !isAttacking && !isStunned)
        {
            if(!parryStamina)
            {
                StaminaBar.instance.useStamina(shieldStamina);
                parryStamina = true;
            }
            guardTimer += Time.deltaTime;
            if(guardTimer > 1)
            {
                guardTimer = 0;
                StaminaBar.instance.useStamina(shieldStamina*5/4);
            }
            ChangeAnimationState(parry);
            isGuarding = true;
        }
    }
    private void CheckAttack()
    {
        attackTime += Time.deltaTime;
        if (attackTime > 0.6f)
            isCombo = false;
        if (Input.GetButtonDown("Fire1") && !Input.GetKey(KeyCode.S))
        {
            if((isGrounded && stamina >= 15) || (!isGrounded && stamina >= 25))
            {
                if (!isPraying && !isGuarding && !isRolling && !playerManager.isHealing && !isStunned && !isFallAttacking && !canClimbLedge)
                {
                    if (isCombo && attackTime > 0.3f)
                        Attack();
                    else if (!isCombo)
                        Attack();
                }
            }
        }
        else if (!isGrounded && Input.GetButtonDown("Fire1") && Input.GetKey(KeyCode.S) && stamina >= 50 && rb.linearVelocity.y <= 6 && !isFallAttacking && !isWallSliding)
        {
            StaminaBar.instance.useStamina(50);
            isFallAttacking = true;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y - 1f);
            //rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            FallAttack();
        }
    }
    void Attack()
    {
        isAttacking = true;
        if(isGrounded)
        {
            StaminaBar.instance.useStamina(15);
            rb.linearVelocity = new Vector2(0,0);
            attackDamage += 2;
        }
        else
        {
            StaminaBar.instance.useStamina(25);
            attackDamage += 1;
        }
        attackCount++;
        isCombo = true;

        if ((attackCount > 3 || attackTime > 0.6f))
        {
            attackCount = 1;
            if(isGrounded)
                attackDamage = 20;
            else
                attackDamage = 10;
        }
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if(enemy.CompareTag("Enemy"))
                enemy.GetComponent<Minion_wfireball>().TakeDamage(attackDamage);
            if(enemy.CompareTag("Villager"))
                enemy.GetComponent<VillagerHealthManager>().TakeDamage(attackDamage);
            if(enemy.CompareTag("Sword"))
                enemy.GetComponent<Sword_Behaviour>().TakeDamage(attackDamage);
            if(enemy.CompareTag("MinionwPoke"))
                enemy.GetComponent<Minion_wpoke>().TakeDamage(attackDamage);
            if(enemy.CompareTag("Legolas"))
                enemy.GetComponent<Legolas>().TakeDamage(attackDamage);
            if (enemy.CompareTag("Miniboss"))
                enemy.GetComponent<Boss_Manager>().TakeDamage(attackDamage);
            if (enemy.CompareTag("MinibossShield"))
                enemy.transform.parent.GetComponent<Boss_Manager>().Parry();
            if (enemy.CompareTag("Spearman"))
                enemy.GetComponent<Spearman_Manager>().TakeDamage(attackDamage);
            if (enemy.CompareTag("SpearmanShield"))
                enemy.transform.parent.GetComponent<Spearman_Manager>().Guard();
            if (enemy.CompareTag("FakeWall"))
                enemy.GetComponent<FakeWallScript>().TakeDamage(attackDamage);
        }
        attackTime = 0f;
    }
    void FallAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(fallAttackBox.position, fallAttackSize, enemyLayers);
        this.GetComponent<PlayerManager>().damageable = false;
        foreach (Collider2D enemy in hitEnemies)
        {
            if(enemy.CompareTag("Enemy"))
                enemy.GetComponent<Minion_wfireball>().TakeDamage(40);
            if(enemy.CompareTag("Villager"))
                enemy.GetComponent<VillagerHealthManager>().TakeDamage(40);
            if(enemy.CompareTag("Sword"))
                enemy.GetComponent<Sword_Behaviour>().TakeDamage(40);
            if(enemy.CompareTag("MinionwPoke"))
                enemy.GetComponent<Minion_wpoke>().TakeDamage(40);
            if(enemy.CompareTag("Legolas"))
                enemy.GetComponent<Legolas>().TakeDamage(40);
            if (enemy.CompareTag("Miniboss"))
                enemy.GetComponent<Boss_Manager>().TakeDamage(attackDamage);
            if (enemy.CompareTag("MinibossShield"))
                enemy.transform.parent.GetComponent<Boss_Manager>().Parry();
            if (enemy.CompareTag("Spearman"))
                enemy.GetComponent<Spearman_Manager>().TakeDamage(attackDamage);
            if (enemy.CompareTag("SpearmanShield"))
                enemy.transform.parent.GetComponent<Spearman_Manager>().Guard();
        }
    }
    public void FallAttackTransition()
    {
        if(isGrounded)
        {
            ChangeAnimationState("PlayerFallAttackLanding");
        }
    }
    public void FallAttackDone()
    {
        isFallAttacking = false;
        //rb.constraints = ~RigidbodyConstraints2D.FreezePositionX;
        this.GetComponent<PlayerManager>().damageable = true;
    }
    public void StealLife(float stealRate)
    {
        if (playerManager.CurrentHealth < playerManager.MaxHealth)
        {
            playerManager.CurrentHealth += attackDamage * stealRate;
            Debug.Log(attackDamage * stealRate + "Can calindi");
            Actions.OnHealthChanged();
            if (playerManager.CurrentHealth > 100)
            {
                playerManager.CurrentHealth = 100;
            }
        }
    }
    public void ThrowDagger()
    {
        //Stack'ten gir dagger çıkar ve dagger objesine ata
        GameObject dagger = daggerStack.PopFromStack();

        if (dagger != null)
        {
            if (GetComponent<PlayerController>().facingRight)
            {
                dagger.transform.position = firePoint.position;
                dagger.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                dagger.GetComponent<Dagger>().Initialize(Vector2.right);
                dagger.SetActive(true);
                StartCoroutine(startDaggerLifeTime());
                //Stack'ten çıkarmış dagger objesini Queue'ya yerleştir
                daggerCooldownController.EnqueueItem(dagger);
            }
            else
            {
                dagger.transform.position = firePoint.position;
                dagger.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                dagger.GetComponent<Dagger>().Initialize(Vector2.left);
                dagger.SetActive(true);
                StartCoroutine(startDaggerLifeTime());
                //Stack'ten çıkarmış dagger objesini Queue'ya yerleştir
                daggerCooldownController.EnqueueItem(dagger);
            }
            //Daggerların 3 saniye sonra sahneden çıkmasına yarayan coroutine
            IEnumerator startDaggerLifeTime()
            {
                yield return new WaitForSeconds(10f);
                dagger.SetActive(false);
            }
        }
    }
}
