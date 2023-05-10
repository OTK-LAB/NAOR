using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehaviour : MonoBehaviour
{
    //[SerializeField] private PlayerController _playerController; //add implemantation for new player

    private GameObject Stick;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            //other.GetComponent<EnemyHealthSystem>().Damage(_playerController.CurrentCombatState.DamageAmount);
        }
        if (other.gameObject.name == "Rope")
        {
            Stick = GameObject.Find("Stick");
            Stick.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            other.gameObject.SetActive(false);
        }
    }
}
