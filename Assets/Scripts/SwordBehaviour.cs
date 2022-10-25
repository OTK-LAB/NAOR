using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<HealthSystem>().Damage(_playerController.CurrentCombatState.DamageAmount);
        }
    }
}
