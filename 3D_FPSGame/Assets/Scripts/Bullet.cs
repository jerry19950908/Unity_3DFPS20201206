using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float attack;  // 子彈傷害

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

}
