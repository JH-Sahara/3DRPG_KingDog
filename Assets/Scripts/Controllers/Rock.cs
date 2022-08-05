using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{

    public enum RockStates {HitPlayer,HitEnmey,HitNothing}
    public RockStates rockstate;
    public Rigidbody rb;

    [Header("Base Setting")]
    public Transform follow;
    public int froce = 15;
    public bool isThrow;
    private bool isFly; //石头是否在飞行中
    private Vector3 dir; //石头飞行的方向
    public ParticleSystem rockDamage;

    [Header("Base Attack")]
    public int damage; //石头的攻击
    private float damageTime;
    public float maxDamageTime = 20;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rockstate = RockStates.HitPlayer;
        damageTime = maxDamageTime;
    }

    private void FixedUpdate() {
        if (isFly && rb.velocity.sqrMagnitude < 1f)
        {
            rockstate = RockStates.HitNothing;
        }
    }

    private void Update() {
        damageTime -= Time.deltaTime;
        if (isFly || follow==null)
            return;

        if (!isThrow)
        {
            transform.position = follow.position;
        }else{
            FlyToTarget();
            rb.velocity = Vector3.one;
            isFly = true;
        }
    }

    private void FlyToTarget()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        dir = (player.transform.position - transform.position).normalized;

        //抛出石头
        rb.AddForce(dir * froce,ForceMode.Impulse);
        Debug.Log("抛出石头");
    }

    private void OnCollisionEnter(Collision other) {
        switch(rockstate)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    var agent = other.gameObject.GetComponent<NavMeshAgent>();
                    agent.isStopped = true;
                    agent.velocity = dir * froce;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    //造成伤害
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage);
                    rockstate = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnmey:
                if (other.gameObject.GetComponent<Golem>())
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Hit");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage);
                    //播放粒子
                    Instantiate(rockDamage,transform.position,Quaternion.identity);
                    Destroy(gameObject);
                }else if (damageTime < 0)
                {
                    Instantiate(rockDamage,transform.position,Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
