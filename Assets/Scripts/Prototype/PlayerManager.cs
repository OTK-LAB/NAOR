using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public GameObject currentCheckPoint;
    private PlayerController player;
    private Rigidbody2D rb;
    public GameObject reviveEffect;
    private SpriteRenderer spriteRenderer;
    public float flickerSpeed;
    private bool flickering;
    public GameObject crown;
    [SerializeField] private SceneChanger scene;

    public static PlayerManager instance;

    public int lives = 4;
    public float MaxHealth = 100;
    public float CurrentHealth = 100f;
    public bool isHealing;
    //[HideInInspector] 
    public bool damageable = true;
    //[HideInInspector] 
    public bool dead = false;
    [HideInInspector] public bool isReviving;
    public int status;
    private bool revived = false;


    //Animations
    const string hit = "PlayerHit";
    const string death = "PlayerDeath";
    const string deathDD = "PlayerDeathDD";
    const string revive = "PlayerRevive";
    const string counter = "PlayerCounter";
    const string heal = "PlayerHeal";
    [HideInInspector] public bool hitAnimRunning;

    //HealthGate
    public HealthBar healthBar;

    //Gemler icin eklediklerim
    public float defenceRate=0;
    public float shieldDefenceRate = 0.4f;
    public bool isRegen = false;
    public float regenHealth = 5f;


    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        player = GetComponent<PlayerController>(); 
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar.SetMaxHealth(MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(flickering)
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.PingPong(Time.time * flickerSpeed, 1));
        else
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, 1);
        if (status == 1 || status == 2) // continue moving after a parry
            player.canMove = true;

        switch(lives){
             case 4:
                 if (CurrentHealth >= 100)
            {
                CurrentHealth = 100;
            }
                break;
            case 3:
                if (CurrentHealth >= 1)
            {
                CurrentHealth = 1;
            }
                break;
            case 2:
                if (CurrentHealth >= 1)
            {
                CurrentHealth = 1;
            }
                break;
            case 1:
                if (CurrentHealth >= 1)
            {
                CurrentHealth = 1;
            }
                break;
        }
        }
    
    //TODO REGEN KISMINI DUZELT SUAN CALISMIYOR
    public IEnumerator Regen()
    {
        while (isRegen != false)
        {
            Debug.Log("regen calisti");
            yield return new WaitForSeconds(10f);
            CurrentHealth += regenHealth;
            Debug.Log(regenHealth +"eklendi");
            Actions.OnHealthChanged();
        }

        yield return null;
    }

    private void Awake()
    {
        instance = this;
    }
    public void HealthPotion(float health)
    {
        if(!revived)
            CurrentHealth += health;
        player.ChangeAnimationState(heal);
        isHealing = true;
        rb.velocity = new Vector2(0,0);
        Invoke("CancelHealState", 0.8f);
        if (CurrentHealth > 100)
        {
            CurrentHealth = 100;
        }
        Actions.OnHealthChanged();
    }
    void CancelHealState()
    {
        isHealing = false;
    } 
    public virtual void DamagePlayer(float damage)
    {
        if (damageable)
        {
            if ((CurrentHealth - damage) >= 0)
            {
                switch (status)
                {
                    //normal damage status
                    case 1:
                        CurrentHealth -= (damage*(1-defenceRate));
                        player.ChangeAnimationState(hit);
                        hitAnimRunning = true;
                        Invoke("CancelHitState", .33f);
                        Actions.OnHealthChanged();
                        break;
                    //blocking damage status
                    case 2:
                        CurrentHealth -= (damage*(1-defenceRate)) * (1-shieldDefenceRate);
                        player.ChangeAnimationState(hit);
                        hitAnimRunning = true;
                        Invoke("CancelHitState", .33f);
                        Actions.OnHealthChanged();
                        break;
                    //parry status
                    case 3:
                        player.canMove = false;             // stop when parrying
                        player.ChangeAnimationState(counter);
                        //invoke?
                        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(player.attackPoint.position, player.attackRange, player.enemyLayers);
                        foreach (Collider2D enemy in hitEnemies)
                        {
                            if(enemy.CompareTag("Enemy"))
                                enemy.GetComponent<Minion_wfireball>().TakeDamage(player.attackDamage * 1.25f);
                            if(enemy.CompareTag("Villager"))
                                enemy.GetComponent<VillagerHealthManager>().TakeDamage(player.attackDamage * 1.25f);
                            if(enemy.CompareTag("Sword"))
                                enemy.GetComponent<Sword_Behaviour>().TakeDamage(player.attackDamage * 1.25f);
                            if(enemy.CompareTag("MinionwPoke"))
                                enemy.GetComponent<Minion_wpoke>().TakeDamage(player.attackDamage * 1.25f);
                            if(enemy.CompareTag("Legolas"))
                                enemy.GetComponent<Legolas>().TakeDamage(player.attackDamage * 1.25f);
                            if(enemy.CompareTag("Spearman"))
                                enemy.GetComponent<Spearman_Manager>().TakeDamage(player.attackDamage * 1.25f);
                        }
                        break;
                }
            }
            else
            {
                CurrentHealth = 0;

            }
            Die();
        }
    }

    public void StunPlayer(float stuntime)
    {
        player.ChangeAnimationState(hit);
        if (player.facingRight)
              rb.AddForce(new Vector2(-100,0));
        else
              rb.AddForce(new Vector2(100,0));
        player.isStunned = true;
        StartCoroutine(Stunned(stuntime));
    }

    IEnumerator Stunned(float stuntime)
    {
        player.ChangeAnimationState(hit);
        yield return new WaitForSeconds(0.3f);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(stuntime);
        rb.constraints = ~RigidbodyConstraints2D.FreezeAll;
        player.isStunned = false;
    }
    void CancelHitState()
    {
        hitAnimRunning = false;
    }
    public void Die()
    {
        if (CurrentHealth <= 0)
        {
            lives--;
            if (lives == 3)
            {
                dead = true;
                //rb.simulated = false; character stays in air when he dies if these lines are active
                player.enabled = false;
                rb.velocity = new Vector2(0,0);                
                player.ChangeAnimationState(death);
                StartCoroutine(DeathDefiance());
                CurrentHealth = 1;
                healthBar.DeathDefienceGem(lives);
            }
            if (lives == 2)
            {
                CurrentHealth = 1;
                healthBar.DeathDefienceGem(lives);
            }
            if (lives == 1)
            {
                CurrentHealth = 1;
                healthBar.DeathDefienceGem(lives);
            }
            if (lives == 0)
            {
                dead = true;
                damageable = false;
                player.ChangeAnimationState(deathDD);
                CurrentHealth = MaxHealth;
                healthBar.DeathDefienceGem(lives);
                lives = 4;
                //rb.simulated = false; character stays in air when he dies if these lines are active
                player.enabled = false;
                crown.SetActive(false);
                healthBar.RevertHealthBar();
                Potion.instance.CheckPoint();
                StartCoroutine(RespawnPlayer());
            }
        }
    }

    void StatusChanger(int a)
    {
        status = a;
    }

    IEnumerator DeathDefiance()
    {
        damageable = false;
        yield return new WaitForSeconds(1f);
        isReviving = true;
        Instantiate(reviveEffect, transform.position, Quaternion.identity);
        player.ChangeAnimationState(revive);
        yield return new WaitForSeconds(1f);
        revived = true;
        crown.SetActive(true);
        isReviving = false;
        dead = false;
        //rb.simulated = true; character stays in air when he dies if these lines are active
        player.enabled = true;
        flickering = true;
        yield return new WaitForSeconds(2f);
        damageable = true;
        flickering = false;
    }

    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(1f);
        revived = false;
        damageable = true;
        transform.position = new Vector3(currentCheckPoint.transform.position.x + 1, currentCheckPoint.transform.position.y, 0);
        rb.velocity = new Vector2(0,0);
        dead = false;
        //rb.simulated = true; character stays in air when he dies if these lines are active
        player.enabled = true;
        currentCheckPoint.GetComponent<CheckPointController>().currentVCam.SetActive(true);
        //StartCoroutine(scene.WelcomeToScene());
    }

}
