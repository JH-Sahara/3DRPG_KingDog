using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Extend Animation")]
    public int kickForce = 8;

    public void KickEvent()
    {
        if (attackTarget != null)
        {
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            var agent = attackTarget.GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            agent.velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
