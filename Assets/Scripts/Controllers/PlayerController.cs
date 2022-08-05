using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    [Header("Data Infomation")]
    public CharacterStats characterStats;

    [Space]
    public NavMeshAgent agent;
    public Animator anim;
    public GameObject attackTarget;//攻击的目标
    public float lastAttackTime; //计时器
    public bool isDead;
    public int hitRockFroce = 15;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void OnEnable() {
        GameManager.Instance.RegisterPlayer(characterStats);
        MouseManager.Instance.onMouseClicked += MoveToTarget;
        MouseManager.Instance.onEnemyClicked += EventEnemy;
        SaveManager.Instance.LoadPlayerData();
    }

    void Start()
    {
        SaveManager.Instance.SavePlayerData();
    }

    void Update()
    {
        ChangeAnimations();
        if (isDead)
        {
            Dead();
        }
        //计时器
        lastAttackTime -= Time.deltaTime;
    }

    private void OnDisable() {
        if (!MouseManager.IsInitial)
            return;
        MouseManager.Instance.onMouseClicked -= MoveToTarget;
        MouseManager.Instance.onEnemyClicked -= EventEnemy;
    }

    //切换动画
    public void ChangeAnimations()
    {
        isDead = characterStats.CurrentHealth <= 0;
        anim.SetBool("Dead",isDead);
        anim.SetFloat("Speed",agent.velocity.sqrMagnitude);
    }

    public void MoveToTarget(Vector3 target)
    {
        agent.isStopped = false;
        //打断协程
        StopAllCoroutines();
        agent.destination = target;
    }

    public void EventEnemy(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    private IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        //转向敌人
        transform.LookAt(attackTarget.transform.position);

        //逐渐走向敌人
        //TODO:根据武器的不同修改攻击范围
        var range = characterStats.baseAttackData.attackRange;
        if (attackTarget.GetComponent<NavMeshAgent>())
        {
            range += attackTarget.GetComponent<NavMeshAgent>().radius;
        }else{
            range += 0;
        }
        
        while(Vector3.Distance(attackTarget.transform.position,transform.position) >= range)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        //距离为1停止移动
        agent.isStopped = true;

        //攻击
        if (lastAttackTime < 0)
        {
            bool isCritical = Random.value < characterStats.baseAttackData.criticalChance;
            characterStats.isCritical = isCritical;
            anim.SetTrigger("Attack");
            anim.SetBool("Critical",isCritical);
            lastAttackTime = characterStats.baseAttackData.coolDown;//重置攻击时间
        }
    }

    private void Dead()
    {
        agent.isStopped = true;
        agent.radius = 0;
        GetComponent<Collider>().enabled = false;
        GameManager.Instance.Notify();
        MouseManager.Instance.onMouseClicked -= MoveToTarget;
        MouseManager.Instance.onEnemyClicked -= EventEnemy;
    }

    //Animation Event Function
    public void HitEvent()
    {
        if (attackTarget != null)
        {
            if (attackTarget.CompareTag("Attack"))
            {
                if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockstate==Rock.RockStates.HitNothing)
                {
                    var rock = attackTarget.GetComponent<Rock>();
                    rock.rb.velocity = Vector3.one;
                    rock.rockstate = Rock.RockStates.HitEnmey;
                    rock.rb.AddForce(transform.forward * hitRockFroce,ForceMode.Impulse);
                }
            }else if (transform.IsFacingTarget(attackTarget.transform))
            {
                var defender = attackTarget.GetComponent<CharacterStats>();
                characterStats.TakeDamage(defender);
            }
        }
    }
}
