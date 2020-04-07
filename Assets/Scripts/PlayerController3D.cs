using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(NeuralNetwork))]
public class PlayerController3D : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 startRotation;
    public Vector3 lastPosition;
    public NeuralNetwork network;
    public NetworkManager manager;

    public float aSensor, bSensor, cSensor, dSensor, eSensor;

    [Range(-1f, 1f)]
    public float a, t;

    [Header("Fitness")]
    public float overallFitness;
    public float distanceMultiplier = 1.4f;
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 1f;

    [Header("Network Options")]
    public int layers = 1;
    public int neurons = 5;

    [Header("Infomation")]
    public float distanceTravelled = 0;
    public float avgSpeed = 0;
    public float timeSinceStart = 0;

    public Text currentFitness;

    public void Start()
    {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    void Update()
    {
        Vector3 aS = -transform.right;
        Vector3 bS = (transform.forward - transform.right).normalized;
        Vector3 cS = transform.forward;
        Vector3 dS = (transform.forward + transform.right).normalized;
        Vector3 eS = transform.right;

        Ray ray = new Ray(transform.position, aS);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            aSensor = hit.distance / 20;
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }

        ray.direction = bS;

        if (Physics.Raycast(ray, out hit))
        {
            bSensor = hit.distance / 20;
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }

        ray.direction = cS;

        if (Physics.Raycast(ray, out hit))
        {
            cSensor = hit.distance / 20;
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }

        ray.direction = dS;

        if (Physics.Raycast(ray, out hit))
        {
            dSensor = hit.distance / 20;
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }

        ray.direction = eS;

        if (Physics.Raycast(ray, out hit))
        {
            eSensor = hit.distance / 20;
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }

        lastPosition = transform.position;

        (a,t) = network.RunNetwork(aSensor, bSensor, cSensor, dSensor, eSensor);

        timeSinceStart += Time.deltaTime;

        MovePlayer(a, t);

        CalculateFitness();
    }

    public void CalculateFitness()
    {
        distanceTravelled += Vector3.Distance(lastPosition, transform.position);
        avgSpeed = distanceTravelled / timeSinceStart;

        overallFitness = distanceTravelled * distanceMultiplier + avgSpeed * avgSpeedMultiplier + ((aSensor + bSensor + cSensor + dSensor + eSensor) / 5) * sensorMultiplier;
        currentFitness.text = "Current Fitness: " + overallFitness;

        //if (timeSinceStart > 5 && Vector3.Distance(startPosition,transform.position) < 10)
        //{
        //    overallFitness = 0;
        //    Death();
        //}

        if (overallFitness > 1000)
        {
            Death();
        }
    }

    private Vector3 input;
    public void MovePlayer(float a, float t)
    {
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, a * 11.4f), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;
        transform.eulerAngles += new Vector3(0, t * 90 * 0.02f, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Finish")
        {
            Death();
        }      
    }

    public void Death()
    {
        manager.FeedDataOnDeath(overallFitness);
        Reset();
    }

    public void Reset()
    {
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
        timeSinceStart = 0f;
        distanceTravelled = 0;
        avgSpeed = 0;
        lastPosition = startPosition;
        overallFitness = 0;
    }
}
