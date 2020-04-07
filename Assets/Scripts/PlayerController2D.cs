using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NeuralNetwork2D))]
public class PlayerController2D : MonoBehaviour
{
    public GameObject reward;
    public GameObject panel;
    public Text text, fitnessText;
    public NetworkManager2D manager;
    public NeuralNetwork2D network;

    public float left, right, up, down;
    public float aSensor, bSensor, cSensor, dSensor;

    public float timer = 1f;
    public float resetTimer = 1.5f;
    private float timerS, resetTimerS;
    public bool reset = false;

    [Header("Network Options")]
    public int layers;
    public int neurons;

    public float fitness = 0;
    public float timeSurvived = 0;
    public float distanceFromReward = 0;
    public float distanceTravelled = 0;

    [Header("Fitness Options")]
    public float distanceMultiplier = 1.5f;
    public float timeMultiplier = 1.5f;
    public float distanceLeft = 10f;

    public Vector3 lastPosition;

    private void Update()
    {
        if(Convert.ToDouble(FindObjectOfType<NetworkManager2D>().bestFitness.text.Substring(14)) > 58)
        {
            panel.SetActive(true);
            Time.timeScale = 0;
        }

        if(!reset)
        {
            if (timerS < 0)
            {
                Vector3 a = -transform.right;
                Vector3 b = transform.up;
                Vector3 c = transform.right;
                Vector3 d = -transform.up;

                Ray ray = new Ray(transform.position, a);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit))
                {
                    aSensor = hit.distance;
                    Debug.DrawLine(ray.origin, hit.point);
                }

                ray.direction = b;

                if(Physics.Raycast(ray,out hit))
                {
                    bSensor = hit.distance;
                    Debug.DrawLine(ray.origin, hit.point);
                }

                ray.direction = c;

                if (Physics.Raycast(ray, out hit))
                {
                    cSensor = hit.distance;
                    Debug.DrawLine(ray.origin, hit.point);
                }

                ray.direction = d;

                if (Physics.Raycast(ray, out hit))
                {
                    dSensor = hit.distance;
                    Debug.DrawLine(ray.origin, hit.point);
                }

                //Call function from Neural Network providing hit1,2,3,4 distances and return movement;

                (left, up, right, down) = network.RunNetwork(aSensor, bSensor, cSensor, dSensor);
                
                timeSurvived += Time.deltaTime;

                lastPosition = transform.position;

                MovePlayer(left, up, right, down);
                CalculateDistance();
                timerS = timer;

                if (timeSurvived > 10)
                {
                    timeSurvived = 0;
                    distanceTravelled = 0;
                    Death();
                }

            }
            else
            {
                timeSurvived += Time.deltaTime;
                timerS -= Time.deltaTime;
            }
        }
        else
        {
            resetTimerS -= Time.deltaTime;
        }
        
        if(resetTimerS < 0)
        {
            reset = false;
            resetTimerS = resetTimer;
        }
    }

    public void CalculateDistance()
    {
        distanceTravelled += (transform.position - lastPosition).magnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        Death();
    }

    public void Death()
    {
        fitness = distanceTravelled * distanceMultiplier + timeSurvived * timeMultiplier + distanceLeft / (transform.position - reward.transform.position).magnitude;
        manager.FeedDataOnDeath(fitness, lastPosition);
        Reset(fitness);
    }

    public void Reset(float fit)
    {

        reset = true;
        text.text = "Reset";
        fitnessText.text = "Last Fitness: " + fit;
        fitness = 0;
        timeSurvived = 0;
        distanceFromReward = 0;
        distanceTravelled = 0;
        transform.position = new Vector3(-4, 4, -5.1f);
    }

    public void MovePlayer(float l, float u, float r, float d)
    {
        List<float> max = new List<float> { l, u, r, d };
        max.Sort();
        if (max[3] == l)
        {
            transform.Translate(Vector3.left);
        }
        else if (max[3] == u)
        {
            transform.Translate(Vector3.up);
        }
        else if (max[3] == r)
        {
            transform.Translate(Vector3.right);
        }
        else if (max[3] == d)
        {
            transform.Translate(Vector3.down);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = color;
    //    Gizmos.DrawLine(transform.position, hit1.point);
    //    Gizmos.DrawLine(transform.position, hit2.point);
    //    Gizmos.DrawLine(transform.position, hit3.point);
    //    Gizmos.DrawLine(transform.position, hit4.point);
    //}
}
