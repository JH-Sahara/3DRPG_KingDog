using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Extend Animation")]
    public int kickForce = 10;
    public GameObject rockPrefab;
    private GameObject rock;
    public Transform rockFollow;

    public void KickEvent()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            Vector3 dir = attackTarget.transform.position - transform.position;
            dir.Normalize();

            var agent = attackTarget.GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            agent.velocity = dir * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }

    public void CreateRock()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
        }

        rock = Instantiate(rockPrefab,rockFollow.position,Quaternion.identity);
        rock.GetComponent<Rock>().follow = rockFollow;
    }

    public void ThrowRock()
    {
        rock.GetComponent<Rock>().isThrow = true;
    }
}
