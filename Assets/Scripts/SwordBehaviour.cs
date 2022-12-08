using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            other.GetComponent<EnemyHealthSystem>().Damage(_playerController.CurrentCombatState.DamageAmount);
        }
    }
}
