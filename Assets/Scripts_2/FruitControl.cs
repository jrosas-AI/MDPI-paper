using UnityEngine;

public class FruitControl : MonoBehaviour
{
    private bool onConveyor = false;
    private Rigidbody rb;
    private float velocity = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (onConveyor)
        {
            // Continuously set velocities to zero while on the conveyor

            if(SystemModel.getVariableValue("Q5")=="0") {
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                //print("on conveyor");
            }
            if (SystemModel.getVariableValue("Q5") == "1")
            {
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = Vector3.left*velocity;
            }
        } else
        {
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Conveyor")
        {
            onConveyor = true;
            // Optional: immediately set velocities to zero on collision
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            print("on collision");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Conveyor")
        {
            onConveyor = false;
        }
    }
}
