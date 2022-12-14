using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates {GUARD,PATROL,CHASE,DEAD}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    public NavMeshAgent agent;
    private EnemyStates enemyState;

    [Header("Base Setting")]
    private Vector3 initalPos;
    private Quaternion initRotation;
    public float sightRadius;
    protected GameObject attackTarget;
    private float speed;
    public bool isGuard;
    private Collider coll;

    [Space]

    [Header("Animations Settings")]
    private Animator anim;
    public bool isWalk;
    public bool isChase;
    public bool isFollow;
    public bool isDead;

    [Header("Patrol Settings")]
    public float patrolRadius;
    private Vector3 randomPatrolPoint;
    private float lookAtTime;
    public float maxLookTime;

    [Header("Attack Setting")]
    public CharacterStats characterStats;
    private float _lastAttackTime;
    public float lastAttackTime
    {
        get
        {
            return _lastAttackTime;
        }
        set
        {
            if (value >= -1)
                _lastAttackTime = value;
            else
                _lastAttackTime = -1;
        }
    }

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        initalPos = transform.position;
        initRotation = transform.rotation;
        lookAtTime = maxLookTime;
        characterStats = GetComponent<CharacterStats>();
    }

    private void OnEnable() {
        GameManager.Instance.AddObserver(this);
    }

    private void Start() {
        lastAttackTime = 0;
        if (isGuard)
        {
            enemyState = EnemyStates.GUARD;
        }else{
            enemyState = EnemyStates.PATROL;
            GetRandomPatrolPoint();
        }
    }

    private void Update() {
        lastAttackTime -= Time.deltaTime;
        ChangeEnemyState();
        ChangeAnimations();
    }

    private void OnDisable() {
        if (!GameManager.IsInitial)//??????????????????????????????????????????OnDisable???????????????????????????????????????
        {
            return;
        }
        GameManager.Instance.RemoveObserver(this);
    }

    public void ChangeEnemyState()
    {
        //????????????
        if (isDead)
        {
            enemyState = EnemyStates.DEAD;
        }else if (IsFoundPlayer()){
            enemyState = EnemyStates.CHASE;
        }

        switch(enemyState)
        {
            case EnemyStates.GUARD:
                Guard();
                break;
            case EnemyStates.PATROL:
                Patrol();
                break;
            case EnemyStates.CHASE:
                Chase();
                break;
            case EnemyStates.DEAD:
                Dead();
                break;
        }
    }

    private void Guard()
    {
        isChase = false;
        agent.speed = speed * 0.5f;

        //????????????????????????
        if (Vector3.SqrMagnitude(transform.position - initalPos) <= Mathf.Pow(agent.stoppingDistance,2))
        {
            //??????????????????
            isWalk = false;
            transform.rotation = Quaternion.Slerp(transform.rotation,initRotation,.02f);
        }else{
            agent.destination = initalPos;
        }
    }

    private void Chase()
    {
        isWalk = false;
        isChase = true;

        agent.speed = speed;

        if (IsFoundPlayer())
        {
            //????????????
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;
            //???????????????????????????
            if (IsAttackRange() || IsRemoteRange())
            {
                isFollow = false;
                agent.isStopped = true;
                Attack();
            }
        }else{
            //?????????
            isFollow = false;
            agent.isStopped = false;
            agent.destination = transform.position;//???????????????????????????????????????????????????????????????Player?????????????????????
            
            //??????????????????????????????
            if (lookAtTime > 0)
            {
                lookAtTime -= Time.deltaTime;
            }else if(isGuard){
                enemyState = EnemyStates.GUARD;
            }else{
                enemyState = EnemyStates.PATROL;
            }
        }
    }

    private void Patrol()
    {
        isChase = false;
        agent.speed = speed * 0.5f;

        //??????????????????????????????
        if (Vector3.Distance(transform.position,randomPatrolPoint) <= agent.stoppingDistance)
        {
            isWalk = false;
            if (lookAtTime > 0)
            {
                lookAtTime -= Time.deltaTime;
            }else{
                lookAtTime = maxLookTime;
                GetRandomPatrolPoint();
            }
        }else{
            isWalk = true;
            agent.destination = randomPatrolPoint;
        }
    }

    private void Dead()
    {
        agent.radius = 0;
        coll.enabled = false;
        Destroy(gameObject,2f);
    }

    //??????????????????????????????
    private void ChangeAnimations()
    {
        isDead = characterStats.CurrentHealth <= 0;
        anim.SetBool("Dead",isDead);
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase",isChase);
        anim.SetBool("Follow",isFollow);
    }

    //???????????????????????????
    private void GetRandomPatrolPoint()
    {
        while(true)
        {
            float x = Random.Range(-patrolRadius,patrolRadius);
            float z = Random.Range(-patrolRadius,patrolRadius);

            Vector3 pos = new Vector3(initalPos.x + x,transform.position.y,initalPos.z + z);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos,out hit,1.0f,1))
            {
                randomPatrolPoint = hit.position;
                break;
            }
        }
        
    }

    //??????????????????
    private bool IsFoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position,sightRadius);

        foreach(var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                attackTarget = collider.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    //????????????
    private void Attack()
    {
        transform.LookAt(attackTarget.transform.position);

        if (IsAttackRange())
        {
            //??????
            if (lastAttackTime < 0)
            {
                lastAttackTime = characterStats.baseAttackData.coolDown;
                bool isCritical = Random.value < characterStats.baseAttackData.criticalChance;
                characterStats.isCritical = isCritical;
                anim.SetTrigger("Attack");
                anim.SetBool("Critical",isCritical);
            }
        }else if (IsRemoteRange()){
            //??????
            if (lastAttackTime < 0)
            {
                lastAttackTime = characterStats.baseAttackData.coolDown;
                anim.SetTrigger("Kick");
            }
        }
    }

    //????????????????????????
    private bool IsAttackRange()
    {
        if (Vector3.Distance(transform.position,attackTarget.transform.position) <= characterStats.baseAttackData.attackRange)
        {
            return true;
        }else{
            return false;
        }
    }

    //??????????????????????????????
    private bool IsRemoteRange()
    {
        if (Vector3.Distance(transform.position,attackTarget.transform.position) <= characterStats.baseAttackData.remoteRange)
        {
            return true;
        }else{
            return false;
        }
    }

    //????????????
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,sightRadius);
    }

    //Animation Event Fountion
    public void HitEvent()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var defender = attackTarget.GetComponent<CharacterStats>();
            characterStats.TakeDamage(defender);
        }
    }

    public void EndNotify()
    {
        isChase = false;
        isWalk = false;
        agent.isStopped = true;
        anim.SetTrigger("Win");
    }
}
