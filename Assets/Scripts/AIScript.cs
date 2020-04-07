using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AIScript : MonoBehaviour
{
    public float speed;
    public float length = 10;
    RaycastHit hit1,hit2,hit3,hit4;

    void Update()
    { 
        Physics.Raycast(transform.position, (transform.forward + transform.right).normalized, out hit1, length);
        Physics.Raycast(transform.position, (transform.forward - transform.right).normalized, out hit2, length);
        Physics.Raycast(transform.position, -transform.right, out hit3, length);
        Physics.Raycast(transform.position, transform.right, out hit4, length);

        float distance1 = (hit1.point - transform.position).magnitude;
        float distance2 = (hit2.point - transform.position).magnitude;
        float distance3 = (hit3.point - transform.position).magnitude;
        float distance4 = (hit4.point - transform.position).magnitude;

        if(distance3 < 1)
        {
            distance3 = 1;
        }

        if(distance4 < 1)
        {
            distance4 = 1;
        }

        if(Physics.Raycast(transform.position, transform.forward, 5)){
            if(Physics.Raycast(transform.position, (transform.forward - transform.right).normalized, 10))
            {
                transform.Rotate(0, 45, 0);
            }
            else if(Physics.Raycast(transform.position,(transform.forward+transform.right).normalized, 10))
            {
                transform.Rotate(0, -45, 0);
            }
        }
        else
        {
            transform.Rotate(0, -45 / distance1 + 45 / distance2 + 15 / distance3 - 15 / distance4, 0);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            transform.position = new Vector3(-55.8f, 1, -72.95f);
            transform.localRotation = Quaternion.identity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 10);
        Gizmos.DrawRay(transform.position, transform.right * 10);
        Gizmos.DrawRay(transform.position, transform.right * -10);
        Gizmos.DrawRay(transform.position, (transform.forward + transform.right).normalized * 10);
        Gizmos.DrawRay(transform.position, (transform.forward - transform.right).normalized * 10);

        Vector3 size = new Vector3(2, 2, 2);

        Gizmos.DrawWireCube(hit1.point, size);
        Gizmos.DrawWireCube(hit2.point, size);
        Gizmos.DrawWireCube(hit3.point, size);
        Gizmos.DrawWireCube(hit4.point, size);
    }
}
