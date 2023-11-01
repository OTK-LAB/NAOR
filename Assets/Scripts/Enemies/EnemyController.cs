using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int reduction_num;
     private void speedReduction(Collider2D enemy)
     {
        if (enemy.GetComponent<ShieldEnemy>() != null)
            enemy.GetComponent<ShieldEnemy>().speedReduction(reduction_num);
        else if (enemy.GetComponent<SwordEnemy>() != null)
            enemy.GetComponent<SwordEnemy>().speedReduction(reduction_num);
        else if (enemy.GetComponent<Archer>() != null)
            enemy.GetComponent<Archer>().speedReduction(reduction_num);
     }
     private void frozenState(Collider2D enemy)
     {

        if (enemy.GetComponent<ShieldEnemy>() != null)
            enemy.GetComponent<ShieldEnemy>().setFrozenState();
        else if (enemy.GetComponent<SwordEnemy>() != null)
            enemy.GetComponent<SwordEnemy>().setFrozenState();
        else if (enemy.GetComponent<Archer>() != null)
            enemy.GetComponent<Archer>().setFrozenState();
     }
}
