using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Manager : MonoBehaviour
{
	                                                                    //small note for miniboss: the animations must be long in order to damage the miniboss
															         	//if the animation is done it will revert to idle (if nearby) and change direction towards us.
    public PlayerManager player;  //do not try this at home
    public float health = 500;
	private bool alive = true;

	public GameObject deathEffect;

	private bool isInvulnerable = false;

	[HideInInspector] public float attackTimer;
	public float autoAttackTimer;

	public int attackDamage = 35;
	public int shieldDamage;
	public int enragedAttackDamage = 40;

	public Vector3 attackOffset;
	public float attackRange = 1f;
	public LayerMask attackMask;

    public bool inRange;
	public GameObject shieldcoll;
	Animator anim;

	[HideInInspector] public float trainAttack;
	public float trainAttackTimer = 1f;

	//batuhanin ekledikleri
	public static Boss_Manager instance;
	//

	private void Awake()
	{
		instance = this;
		anim = GetComponent<Animator>();
		trainAttack = trainAttackTimer;
	}

	void Update()
    {
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;
		if (trainAttack > 0 && attackTimer <= 0)
			trainAttack -= Time.deltaTime;
    }



	public void Attack()
	{
		Vector3 pos = transform.position;
		pos += transform.right * attackOffset.x;
		pos += transform.up * attackOffset.y;

		Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
		if (colInfo != null)
		{
			player.DamagePlayer(attackDamage);
			//colInfo.GetComponent<PlayerManager>().DamagePlayer(attackDamage);
		}
	}

    	void OnDrawGizmosSelected()
	{
		Vector3 pos = transform.position;
		pos += transform.right * attackOffset.x;
		pos += transform.up * attackOffset.y;

		Gizmos.DrawWireSphere(pos, attackRange);
	}


	public void TakeDamage(float damage)
	{
		if (isInvulnerable)
			return;

		health -= damage;
		

		if (health <= 130)
		{
			anim.SetBool("IsEnraged", true);
		}

		if (health <= 0)
		{
			Die();
		}else
        {
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Miniboss_hit"))
				anim.Play("Miniboss_hit");

        }
	}

	public void Parry()
	{
		Vector3 pos2 = transform.position;
		pos2 += transform.right * attackOffset.x;
		pos2 += transform.up * attackOffset.y;

		Collider2D colInfo2 = Physics2D.OverlapCircle(pos2, attackRange, attackMask);
		if (colInfo2 != null)
		{
			anim.Play("Miniboss_shield");
            player.DamagePlayer(shieldDamage);
			player.StunPlayer(2f);
			//colInfo2.GetComponent<PlayerManager>().DamagePlayer(shieldDamage);
			//colInfo2.GetComponent<PlayerManager>().StunPlayer(2f);
		}
	}

    void Die()
    {
        GetComponent<Collider2D>().enabled = false;
		shieldcoll.SetActive(false);
		GetComponent<Rigidbody2D>().simulated = false;
        this.enabled = false;
		alive = false;
        anim.Play("Miniboss_dead");
        Invoke("Eliminate", 5f);     
    }
    private void Eliminate()
    {
         Destroy(gameObject);
    }

	/*public void Timer(float decrease)
	{
		if (decrease > 0)                                                   does not work for some reason?
			decrease -= Time.deltaTime;
	}*/

	public bool isAlive()
	{
		return alive;
	}
}
