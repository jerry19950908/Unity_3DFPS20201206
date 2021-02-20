using System.Collections;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("移動速度"), Range(0, 2000)]
    public float speed;
    [Header("旋轉"), Range(0, 2000)]
    public float turn;

    private Animator ani;
    private Rigidbody rig;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// 移動方法
    /// </summary>
    private void Move()
    {
        float v = Input.GetAxis("Vertical");     // W S 上 下 - 前進值 = 1  後退值 = -1
        float h = Input.GetAxis("Horizontal");   // A D 左 右

        rig.MovePosition(transform.position + transform.forward * v * speed * Time.deltaTime + transform.right * h * speed * Time.deltaTime);
    }
}
