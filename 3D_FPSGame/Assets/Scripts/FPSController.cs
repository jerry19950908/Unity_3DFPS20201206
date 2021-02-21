using System.Collections;
using UnityEngine;

public class FPSController : MonoBehaviour
{
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


    private Animator ani;
    private Rigidbody rig;

    private void Awake()
    {
        Cursor.visible = false;  // 隱藏滑鼠
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
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
