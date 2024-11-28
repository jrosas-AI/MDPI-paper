using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveConveyor : MonoBehaviour
{
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private List<GameObject> onBelt;
    //private bool control = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Fixed update for physics
    void FixedUpdate()
    {
        

        float speed = SliderControl.value;
        if (SystemModel.InputPin(17) == 1) {
            // For every item on the belt, add force to it in the direction given

            for (int i = 0; i <= onBelt.Count - 1; i++)
            {
                if (onBelt[i] != null)
                {
                    onBelt[i].GetComponent<Rigidbody>().linearVelocity = Vector3.right * Time.deltaTime * speed;
                    print("moving....");
                }
                else
                    break;
            }
            //control = false;
        }
        else if (SystemModel.InputPin(17) == 0)
        {
            for (int i = 0; i <= onBelt.Count - 1; i++)
            {
                if (onBelt[i] != null)
                {
                    onBelt[i].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    //print("Entrei no stop conv");
                }
                else
                    break;
            }
        }
        
    }

    // When something collides with the belt
    private void OnCollisionEnter(Collision collision)
    {
        onBelt.Add(collision.gameObject);
        print("fruit got in the conveyor");
    }

    // When something leaves the belt
    private void OnCollisionExit(Collision collision)
    {
        onBelt.Remove(collision.gameObject);
        print("fruit left the conveyor");
    }
}