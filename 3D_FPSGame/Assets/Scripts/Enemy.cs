using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    private Transform player;  //玩家變形資訊
    private NavMeshAgent nav;  //代理器
    private Animator ani;  //動畫



    [Header("移動速度"), Range(0, 30)]
    public float speed = 2.5f;
    [Header("攻擊範圍"), Range(2, 100)]
    public float rangeAttack = 5f;
    [Header("生成子彈位置")]
    public Transform point;
    [Header("子彈")]
    public GameObject bullet;
    [Header("子彈速度"), Range(0,3000)]
    public float bulletspeed = 500;
    [Header("開槍間隔"), Range(0, 5)]
    public float interval = 0.5f;

    private float timer;



    private void Awake()
    {
        player = GameObject.Find("玩家").transform;  //取得玩家變形資訊
        nav = GetComponent<NavMeshAgent>();         //取得導覽代理器
        ani = GetComponent<Animator>();
        nav.speed = speed;                          //速度
        nav.stoppingDistance = rangeAttack;         //停止攻擊距離 
    }

    private void Update()
    {
        track();
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, rangeAttack);
    }


    /// <summary>
    /// 追蹤方法
    /// </summary>
    private void track()
    {
        nav.SetDestination(player.position);

        if (nav.remainingDistance > rangeAttack)
        {
            ani.SetBool("跑步開關", true);
        }
        else
        {
            Fire();
        }
    }


    /// <summary>
    /// 開槍方法
    /// </summary>
    private void Fire()
    {
        ani.SetBool("跑步開關", false);

        if (timer >= interval)   //如果計時器 大於等於 開槍間隔
        {
            timer = 0;
           GameObject temp = Instantiate(bullet, point.position, point.rotation);   //生成暫存子彈
            temp.GetComponent<Rigidbody>().AddForce(point.right * -bulletspeed);   //子彈施加推力
        }
        else
        {
            timer += Time.deltaTime;
        }

    }



}
