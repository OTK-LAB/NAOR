using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearman_Manager : MonoBehaviour
{
	//spearman reqires 2 walls (from the minion) to patrol

    public GameObject player;
    public float health = 500;

	public GameObject deathEffect;

	private bool isInvulnerable = false;

	[HideInInspector] public float attackTimer;
	public float autoAttackTimer;

	public int attackDamage = 35;
	public bool shieldBroke;

	public Vector3 attackOffset;
	public float damageRange = 1f;   //there are 2 attack ranges idk why , this is for gizmoz, attack range in "spearman_run" real range for the behaivour!!!
	public LayerMask attackMask;

	public float triggerRange;
	[HideInInspector] public bool playerNear;
	[HideInInspector] public bool Moveright = true;
	public bool inRange;
	public GameObject shieldcoll;
	Animator anim;


	void Awake()
	{
		//player = GameObject.FindGameObjectWithTag("Player");
	    anim = GetComponent<Animator>();
	}

	void Update()
    {
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;

		if (player.transform.position.x < transform.position.x)
        {
			if(transform.rotation.y == 180f)  //k�saca spearmen player a bak�yor
				isInvulnerable = true;
			else
				isInvulnerable = false;
		}
		else if (player.transform.position.x > transform.position.x)
        {
			if (transform.rotation.y == 0f)  //k�saca spearmen player a bak�yor
				isInvulnerable = true;
			else
				isInvulnerable = false;

		}

    }


	public void Attack()
	{
		isInvulnerable = false;                          //bad way to fix it, works tho (animation event could be used)

		Vector3 pos = transform.position;
		pos += transform.right * attackOffset.x;
		pos += transform.up * attackOffset.y;

		Collider2D colInfo = Physics2D.OverlapCircle(pos, damageRange, attackMask);
		if (colInfo != null)
		{
			player.GetComponent<PlayerManager>().DamagePlayer(attackDamage);
		}
	}

    void OnDrawGizmosSelected()
	{
		Vector3 pos = transform.position;
		pos += transform.right * attackOffset.x;
		pos += transform.up * attackOffset.y;

		Gizmos.DrawWireSphere(pos, damageRange);
	}


	public void TakeDamage(float damage)
	{
		if (isInvulnerable)
			return;

		health -= damage;
				
		if (health <= 0)
		{
			Die();
		}else
        {
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Spearman_hit"))
				anim.Play("Spearman_hit");

        }
	}

	public void Guard()
	{
		Vector3 pos2 = transform.position;
		pos2 += transform.right * attackOffset.x;
		pos2 += transform.up * attackOffset.y;

		Collider2D colInfo2 = Physics2D.OverlapCircle(pos2, damageRange, attackMask);
		if (colInfo2 != null)
		{
			isInvulnerable = true;
			anim.Play("Spearman_shield");
		}
	}
	private void OnTriggerEnter2D(Collider2D trig)
	{
		if (trig.CompareTag("turn") && !playerNear)
		{
			if (Moveright) Moveright = false;
			else Moveright = true;
			transform.Rotate(0f, 180f, 0f);
		}
	}

	void Die()
    {
        GetComponent<Collider2D>().enabled = false;
		ColiseumKey.instance.deadEnemyCount++;
        this.enabled = false;
        anim.Play("Spearman_dead");
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
}
