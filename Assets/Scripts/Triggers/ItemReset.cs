using UnityEngine;

public class ItemReset : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -5)
        {
            transform.position = Vector3.up * 5;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
