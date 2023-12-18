using DG.Tweening;
using System;
using UltimateCC;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    private PlayerMain player;
    public static event Action OnEnemyKilled;
    [SerializeField] private float knockbackDistance;

    private void Start()
    {
        player = PlayerMain.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<EnemyHealthSystem>(out var enemy))
        {
            SlowDownTime(0.06f, 0.06f, 0.02f);
            enemy.Damage(player.PlayerData.Attack.AttackColliders.Find(x => x.Collider.Equals(GetComponent<Collider2D>())).Damage, knockbackDistance);

            if (enemy.currentHealth <= 0)
            {
                OnEnemyKilled?.Invoke();
            }
        }
    }

    private void SlowDownTime(float scale, float duration1, float duration2)
    {
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, scale, duration1)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(duration2, () => Time.timeScale = 1f).SetUpdate(true);
            }).SetUpdate(true);
    }

}
