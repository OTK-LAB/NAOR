using System;
using UltimateCC;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private PlayerMain player;
    public static event Action OnEnemyKilled;
    public static event Action<EnemyHealthSystem> OnEnemyDamaged;

    private void Start()
    {
        player = PlayerMain.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<EnemyHealthSystem>(out var enemy))
        {
            enemy.Damage(player.PlayerData.Attack.AttackColliders.Find(x => x.Collider.Equals(GetComponent<Collider2D>())).Damage);
            OnEnemyDamaged?.Invoke(enemy);
            if (enemy.currentHealth <= 0)
            {
                OnEnemyKilled?.Invoke();
            }
        }
    }

}
