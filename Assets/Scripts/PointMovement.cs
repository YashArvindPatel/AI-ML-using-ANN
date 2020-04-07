using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMovement : MonoBehaviour
{
    public GameObject p1,p2,p3,p4,p5,p6,p7,p8,p9;
    public GameObject[] pp;
    public float speed;
    public int counter = 0;
    public Vector3 position;
    public Vector3 dir = Vector3.zero;

    private void Start()
    {
        pp = new GameObject[] { p1, p2, p3, p4, p5, p6, p7, p8, p9 };
        position = p1.transform.position;
    }

    private void Update()
    {
        dir = Vector3.Lerp(dir, position, speed);
    }

    private void FixedUpdate()
    {      
        if ((position - transform.position).magnitude > 0.5)
        {
            GetComponent<Rigidbody>().MovePosition(dir);
        }
        else
        {          
            position = pp[counter].transform.position;
            if(counter == 8)
            {
                counter = 0;
            }
            else
            {
                counter++;
            }
        }   
    }
}
