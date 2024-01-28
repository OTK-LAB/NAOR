using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
     public void speedReduction(Collider2D enemy,float reduction_time)
     {
        if (enemy.GetComponent<ShieldEnemy>() != null)
            enemy.GetComponent<ShieldEnemy>().speedReduction(reduction_time);
        else if (enemy.GetComponent<SwordEnemy>() != null)
            enemy.GetComponent<SwordEnemy>().speedReduction(reduction_time);
        else if (enemy.GetComponent<Archer>() != null)
            enemy.GetComponent<Archer>().speedReduction(reduction_time);
     }
    public void frozenState(Collider2D enemy)
    {
        if (enemy.GetComponent<ShieldEnemy>() != null)
            enemy.GetComponent<ShieldEnemy>().setFrozenState();
        else if (enemy.GetComponent<SwordEnemy>() != null)
            enemy.GetComponent<SwordEnemy>().setFrozenState();
        else if (enemy.GetComponent<Archer>() != null)
            enemy.GetComponent<Archer>().setFrozenState();
     }
    public void breakFreeze(Collider2D enemy)
    {
        if (enemy.GetComponent<ShieldEnemy>() != null)
            enemy.GetComponent<ShieldEnemy>().breakFreeze();
        else if (enemy.GetComponent<SwordEnemy>() != null)
            enemy.GetComponent<SwordEnemy>().breakFreeze();
        else if (enemy.GetComponent<Archer>() != null)
            enemy.GetComponent<Archer>().breakFreeze();
     }
}
