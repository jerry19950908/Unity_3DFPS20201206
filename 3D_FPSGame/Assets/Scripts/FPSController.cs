using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour
{
    #region 基本欄位
    [Header("移動速度"), Range(0, 2000)]
    public float speed;
    [Header("滑鼠旋轉速度"), Range(0, 2000)]
    public float turn;
    [Header("跳躍高度"), Range(0, 2000)]
    public float jump = 100;
    [Header("地板偵測範圍")]
    public Vector3 flooroffset;
    [Header("地板偵測半徑"), Range(0, 20)]
    public float floorRadious = 1;
    [Header("文字 : 子彈目前數量")]
    public Text textbulletcurrent;
    [Header("文字 : 子彈總數量")]
    public Text textbullettotal;

    private Animator ani;
    private Rigidbody rig;
    #endregion

    #region 開槍欄位
    [Header("子彈生成位置")]
    public Transform pointFire;
    [Header("子彈")]
    public GameObject bullet;
    [Header("子彈目前數量")]
    public int bulletcurrent = 30;
    [Header("彈夾補充數量")]
    public int bulletclip = 30;
    [Header("子彈總數量")]
    public int bulletTotal = 150;
    [Header("子彈速度"), Range(0, 1000)]
    public float bulletspeed = 450;
    [Header("補充子彈時間"), Range(0, 5)]
    public float addbulletTime = 1;
    [Header("開槍音效")]
    public AudioClip soundfire;
    [Header("換彈夾音效")]
    public AudioClip soundAddbullet;
    [Header("開槍間隔時間"), Range(0f, 1f)]
    public float fireInterval = 0.1f;

    private AudioSource aud;
    private float timer; //計時器

    private bool isAddbullet;
    #endregion

    private void Awake()
    {
        Cursor.visible = false;  // 隱藏滑鼠
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        aud = GetComponent<AudioSource>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position + flooroffset, floorRadious);
    }

    private void Update()
    {
        Move();
        Jump();
        Fire();
        Addbullet();
    }

    /// <summary>
    /// 開槍方法
    /// </summary>
    private void Fire()
    {

        // GetKey等於一直按住也有效
        //如果按下左鍵 並且 目前子彈數量大於0 並且 不是在補充子彈(防止補充子彈時還能發射)
        if (Input.GetKey(KeyCode.Mouse0) && bulletcurrent > 0 && !isAddbullet)
        {
            if (timer >= fireInterval)
            {
                ani.SetTrigger("開槍觸發");
                timer = 0;
                aud.PlayOneShot(soundfire, Random.Range(0.8f, 1.2f));

                //按下左鍵扣除子彈數量，並更新介面
                bulletcurrent--;
                textbulletcurrent.text = bulletcurrent.ToString();

                //暫存子彈(區域變數) = 生成(物件, 座標, 角度)
                GameObject temp = Instantiate(bullet, pointFire.position, pointFire.rotation);
                //暫存子彈.取得剛體元件.添加推力(生成點前方 * 速度)
                temp.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
            }
            else timer += Time.deltaTime;
     
        }
    }

    /// <summary>
    /// 補充子彈方法
    /// </summary>
    private void Addbullet()
    {
        aud.PlayOneShot(soundAddbullet, Random.Range(0.8f, 1.2f));
        //符合條件邏輯 才能補充子彈
        //1. 按下R
        //2. 不是在補充子彈
        //3. 子彈總數 大於 0
        //4. 目前子彈數量 小於 彈夾數量
        if (Input.GetKeyDown(KeyCode.R) && !isAddbullet && bulletTotal > 0 &&  bulletcurrent < bulletclip) // GetKey等於一直按住也有效
        {
            //啟動協成(補充子彈協程方法);
            StartCoroutine(Delayaddbullet());
        }
    }

    /// <summary>
    /// 補充子彈延遲時間方法(協程)
    /// </summary>
    /// <returns></returns>
    private IEnumerator Delayaddbullet()
    {
        isAddbullet = true;
        yield return new WaitForSeconds(addbulletTime);
        isAddbullet = false;

        //如果 子彈目前數量 小於 彈夾數量
        if (bulletcurrent < bulletclip)
        {
            ani.SetTrigger("換彈夾觸發");
            //計算裝填子彈 = 彈夾數量 - 子彈目前數量
            int add = bulletclip - bulletcurrent;

            if ( bulletTotal >= add)   //如果 子彈總數 大於等於 要裝填的子彈數量
            {
                bulletcurrent += add;  //補充完畢 = 子彈目前數量 + 計算裝填子彈數量
                bulletTotal -= add;    //子彈總數需扣除 計算裝填子彈數量
            }
            else             //否則若子彈總數 小於 要裝填的子彈數量 (裝填的子彈總數不夠一個彈夾數量)
            {
                bulletcurrent += bulletTotal;  //子彈目前數量 + 子動剩餘總數量 
                bulletTotal = 0;               //子彈總數歸零  
            }
            textbulletcurrent.text = bulletcurrent.ToString(); //更新數字資訊
            textbullettotal.text = bulletTotal.ToString();
        }
    }


    /// <summary>
    /// 跳躍方法
    /// </summary>
    private void Jump()
    {
       //3D 模式霧裡碰撞偵測
       //碰撞物件[] = 物理.覆蓋球體(中心點+位移, 半徑, 1 << 圖層編號)
       Collider[] hits = Physics.OverlapSphere(transform.position + flooroffset, floorRadious, 1 << 8 );

        //如果 碰撞物件有一個以上 並且 碰撞物件是存在的 並且 按下空白鍵
        if (hits.Length > 0 &&  hits[0] && Input.GetKeyDown(KeyCode.Space))
        {
            //添加向上的推力
            rig.AddForce(0, jump, 0);
        }
    }


    /// <summary>
    /// 移動方法
    /// </summary>
    private void Move()
    {
        float v = Input.GetAxis("Vertical");     // W S 上 下 - 前進值 = 1  後退值 = -1
        float h = Input.GetAxis("Horizontal");   // A D 左 右

        rig.MovePosition(transform.position + transform.forward * v * speed * Time.deltaTime + transform.right * h * speed * Time.deltaTime);

        float x = Input.GetAxis("Mouse X");  //滑鼠左右方向的值

        transform.Rotate(0, x * Time.deltaTime * turn, 0);
    }
}
