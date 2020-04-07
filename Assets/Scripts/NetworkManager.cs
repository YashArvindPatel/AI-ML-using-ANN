using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController3D playerController3D;

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

    public List<NeuralNetwork> networks = new List<NeuralNetwork>();
    public int startIndex = 0;

    public Text text1, text2, bestFitness;

    private void Awake()
    {
        CreatePopulation();
    }

    public void CreatePopulation()
    {
        while (startIndex < populationCount)
        {
            NeuralNetwork network = new NeuralNetwork();
            network.Initialise(playerController3D.layers, playerController3D.neurons);
            networks.Add(network);
            startIndex++;
        }

        startIndex = 0;

        text1.text = "Generation: " + currentGeneration;
        text2.text = "Genome: " + currentGenome;

        playerController3D.network = networks[currentGenome];        
    }

    public void FeedDataOnDeath(float fitness)
    {
        networks[currentGenome].fitness = fitness;

        if (System.Convert.ToDouble(bestFitness.text.Substring(14)) < fitness)
        {
            bestFitness.text = "Best Fitness: " + fitness;
        }

        currentGenome++;

        if (currentGenome > populationCount - 1)
        {
            Repopulate();
            playerController3D.network = networks[currentGenome];

            text1.text = "Generation: " + currentGeneration;
            text2.text = "Genome: " + currentGenome;
        }
        else
        {
            text2.text = "Genome: " + currentGenome;
            playerController3D.network = networks[currentGenome];
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
        List<NeuralNetwork> betterNetworks = new List<NeuralNetwork>();

        for (int i = 0; i < bestSelection; i++)
        {
            NeuralNetwork net = networks[i].InitialiseCopy(playerController3D.layers,playerController3D.neurons);
            net.fitness = 0;
            betterNetworks.Add(net);
        }

        CrossOver(betterNetworks);

        Mutate(betterNetworks);

        FillUpRestSpots(betterNetworks);

        networks = betterNetworks;

        currentGenome = 0;
        currentGeneration++;
    }

    public void CrossOver(List<NeuralNetwork> betterNet)
    {
        for (int i = 0; i < numberToCrossover -  1; i += 2)
        {
            NeuralNetwork Child1 = new NeuralNetwork();
            NeuralNetwork Child2 = new NeuralNetwork();

            Child1.Initialise(playerController3D.layers, playerController3D.neurons);
            Child2.Initialise(playerController3D.layers, playerController3D.neurons);

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

    public void Mutate(List<NeuralNetwork> betterNetworks)
    {
        foreach (var item in betterNetworks)
        {
            for (int i = 0; i < item.weights.Count; i++)
            {
                if (Random.Range(0f,1f) < mutationRate)
                {
                    int randomRow = Random.Range(0, item.weights[i].RowCount);
                    int randomColumn = Random.Range(0, item.weights[i].ColumnCount);

                    item.weights[i][randomRow, randomColumn] = Mathf.Clamp(item.weights[i][randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
                }
            }
        }
    }

    public void FillUpRestSpots(List<NeuralNetwork> betterNetworks)
    {
        while (betterNetworks.Count < populationCount)
        {
            NeuralNetwork newNetwork = new NeuralNetwork();
            newNetwork.Initialise(playerController3D.layers, playerController3D.neurons);
            betterNetworks.Add(newNetwork);
        }
    }
}
