using System;
using UltimateCC;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private PlayerMain player;
    public static event Action OnEnemyKilled;

    private void Start()
    {
        player = PlayerMain.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<EnemyHealthSystem>(out var enemy))
        {
            enemy.Damage(player.PlayerData.Attack.AttackColliders.Find(x => x.Collider.Equals(GetComponent<Collider2D>())).Damage,0.5f);
            if (enemy.currentHealth <= 0)
            {
                OnEnemyKilled?.Invoke();
            }
        }
    }

}
