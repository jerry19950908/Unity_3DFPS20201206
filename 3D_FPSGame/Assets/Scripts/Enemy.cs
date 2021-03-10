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
    [Header("面向玩家速度"), Range(0, 100)]
    public float speedface = 5f;
    [Header("彈夾目前數量")]
    public int bulletcount = 30;
    [Header("彈夾數量")]
    public int bulletclip = 30;
    [Header("補充子彈時間"), Range(0, 5)]
    public float addbulletTime = 1;

    private float timer;
    private bool isaddbullet;
    private float hp = 100; //血量設定



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
        if (isaddbullet) return;
       
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
            FaceToPlayer();
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
            Managebulletcount();
        }
        else
        {
            timer += Time.deltaTime;
        }

    }


    /// <summary>
    /// 管理子彈數量方法
    /// </summary>
    private void Managebulletcount()
    {
        bulletcount--;
        if (bulletcount <= 0)
        {
            StartCoroutine(addbullet());
        }
    }

    /// <summary>
    /// 添加子彈協程方法
    /// </summary>
    /// <returns></returns>
    private IEnumerator addbullet()
    {
        ani.SetTrigger("換彈夾觸發");
        isaddbullet = true;
        yield return new WaitForSeconds(addbulletTime);
        isaddbullet = false;
        bulletcount += bulletclip;
    }

    /// <summary>
    /// 面向玩家方法
    /// </summary>
    private void FaceToPlayer()
    {
        Quaternion faceAngle = Quaternion.LookRotation(player.position - transform.position);   //面向向量
        transform.rotation = Quaternion.Lerp(transform.rotation, faceAngle, Time.deltaTime * speedface);  // 利用差值(敵人角度, 面向向量角度, 速度)

    }

    /// <summary>
    /// 受傷方法
    /// </summary>
    /// <param name="getDamage"></param>
    private void Damage(float getDamage)
    {
        hp -= getDamage;\

        if (hp <= 0) Dead();
        
    }

    /// <summary>
    /// 死亡方法
    /// </summary>
    private void Dead()
    {
        ani.SetTrigger("死亡觸發");
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        enabled = false;
    }

    /// <summary>
    /// 敵人碰到子彈事件
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "子彈")
        {
            //區域變數 =  碰撞.物件.取得子彈的傷害值
            float damage = collision.gameObject.GetComponent<Bullet>().attack;
            Damage(damage);
        }
    }

}
