using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class NetworkManager2D : MonoBehaviour
{
    [Header("References")]
    public PlayerController2D playerController2D;

    [Header("Controls")]
    public int populationCount = 50;
    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.015f;

    [Header("Crossover Controls")]
    public int bestSelection = 8;
    public int numberToCrossover = 10;

    [Header("Public View")]
    public int currentGeneration = 0;
    public int currentGenome = 0;

    public List<NeuralNetwork2D> networks = new List<NeuralNetwork2D>();
    public int startIndex = 0;

    public Text text1, text2, bestFitness, farthestText;

    private void Awake()
    {
        CreatePopulation();
    }

    public void CreatePopulation()
    {
        while (startIndex < populationCount)
        {
            NeuralNetwork2D network = playerController2D.gameObject.AddComponent<NeuralNetwork2D>();
            network.Initialise(playerController2D.layers, playerController2D.neurons);
            networks.Add(network);
            startIndex++;
        }

        startIndex = 0;

        text1.text = "Generation: " + currentGeneration;
        text2.text = "Genome: " + currentGenome;

        playerController2D.network = networks[currentGenome];
    }

    public void FeedDataOnDeath(float fitness, Vector3 lastPos)
    {
        networks[currentGenome].fitness = fitness;

        if (System.Convert.ToDouble(bestFitness.text.Substring(14)) < fitness)
        {
            bestFitness.text = "Best Fitness: " + fitness;
            farthestText.text = "Farthest Position: " + "(" + lastPos.x + "," + lastPos.y + "," + lastPos.z + ")";
        }

        currentGenome++;

        if (currentGenome > populationCount - 1)
        {
            Repopulate();
            playerController2D.network = networks[currentGenome];

            text1.text = "Generation: " + currentGeneration;
            text2.text = "Genome: " + currentGenome;
        }
        else
        {
            text2.text = "Genome: " + currentGenome;
            playerController2D.network = networks[currentGenome];
        }
    }

    public void Repopulate()
    {
        SortNetworksAccordingToFitness();

        BetterGenerationCreation();
    }

    public void SortNetworksAccordingToFitness()
    {
        networks = networks.OrderByDescending(x => x.fitness).ToList();
    }

    public void BetterGenerationCreation()
    {
        List<NeuralNetwork2D> betterNetworks = new List<NeuralNetwork2D>();

        for (int i = 0; i < bestSelection; i++)
        {
            betterNetworks.Add(networks[i].InitialiseCopy(playerController2D.layers, playerController2D.neurons));
        }

        CrossOver(betterNetworks);

        Mutate(betterNetworks);

        FillUpRestSpots(betterNetworks);

        networks = betterNetworks;

        currentGenome = 0;
        currentGeneration++;
    }

    public void CrossOver(List<NeuralNetwork2D> betterNet)
    {
        for (int i = 0; i < numberToCrossover - 1; i += 2)
        {
            NeuralNetwork2D Child1 = playerController2D.gameObject.AddComponent<NeuralNetwork2D>();
            NeuralNetwork2D Child2 = playerController2D.gameObject.AddComponent<NeuralNetwork2D>();

            Child1.Initialise(playerController2D.layers, playerController2D.neurons);
            Child2.Initialise(playerController2D.layers, playerController2D.neurons);

            for (int j = 0; j < betterNet[i].weights.Count; j++)
            {
                if (Random.Range(0, 2) == 0)
                {
                    Child1.weights[j] = betterNet[i].weights[j];
                    Child2.weights[j] = betterNet[i + 1].weights[j];
                }
                else
                {
                    Child2.weights[j] = betterNet[i].weights[j];
                    Child1.weights[j] = betterNet[i + 1].weights[j];
                }
            }

            for (int j = 0; j < betterNet[i].biases.Count; j++)
            {
                if (Random.Range(0, 2) == 0)
                {
                    Child1.biases[j] = betterNet[i].biases[j];
                    Child2.biases[j] = betterNet[i + 1].biases[j];
                }
                else
                {
                    Child2.biases[j] = betterNet[i].biases[j];
                    Child1.biases[j] = betterNet[i + 1].biases[j];
                }
            }

            betterNet.Add(Child1);
            betterNet.Add(Child2);
        }
    }

    public void Mutate(List<NeuralNetwork2D> betterNetworks)
    {
        foreach (var item in betterNetworks)
        {
            for (int i = 0; i < item.weights.Count; i++)
            {
                if (Random.Range(0f, 1f) < mutationRate)
                {
                    int randomRow = Random.Range(0, item.weights[i].RowCount);
                    int randomColumn = Random.Range(0, item.weights[i].ColumnCount);

                    item.weights[i][randomRow, randomColumn] = Mathf.Clamp(item.weights[i][randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
                }
            }
        }
    }

    public void FillUpRestSpots(List<NeuralNetwork2D> betterNetworks)
    {
        while (betterNetworks.Count < populationCount)
        {
            NeuralNetwork2D newNetwork = playerController2D.gameObject.AddComponent<NeuralNetwork2D>();
            newNetwork.Initialise(playerController2D.layers, playerController2D.neurons);
            betterNetworks.Add(newNetwork);
        }
    }
}
