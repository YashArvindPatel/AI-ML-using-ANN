﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;

public class NeuralNetwork2D : MonoBehaviour
{
    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 4);

    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();

    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 4);

    public List<Matrix<float>> weights = new List<Matrix<float>>();

    public List<float> biases = new List<float>();

    public float fitness;

    public void Initialise(int hiddenLayerCount, int hiddenNeuronCount)
    {

        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++)
        {

            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuronCount);

            hiddenLayers.Add(f);

            biases.Add(Random.Range(-1f, 1f));

            //WEIGHTS
            if (i == 0)
            {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(4, hiddenNeuronCount);
                weights.Add(inputToH1);
            }

            Matrix<float> HiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
            weights.Add(HiddenToHidden);

        }

        Matrix<float> OutputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 4);
        weights.Add(OutputWeight);
        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();

    }

    public NeuralNetwork2D InitialiseCopy(int hiddenLayerCount, int hiddenNeuronCount)
    {
        NeuralNetwork2D n = FindObjectOfType<PlayerController2D>().gameObject.AddComponent<NeuralNetwork2D>();

        List<Matrix<float>> newWeights = new List<Matrix<float>>();

        for (int i = 0; i < this.weights.Count; i++)
        {
            Matrix<float> currentWeight = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);

            for (int x = 0; x < currentWeight.RowCount; x++)
            {
                for (int y = 0; y < currentWeight.ColumnCount; y++)
                {
                    currentWeight[x, y] = weights[i][x, y];
                }
            }

            newWeights.Add(currentWeight);
        }

        List<float> newBiases = new List<float>();

        newBiases.AddRange(biases);

        n.weights = newWeights;
        n.biases = newBiases;

        for (int i = 0; i < hiddenLayerCount + 1; i++)
        {
            Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            n.hiddenLayers.Add(newHiddenLayer);
        }

        n.fitness = 0;

        return n;

    }

    public void RandomiseWeights()
    {

        for (int i = 0; i < weights.Count; i++)
        {

            for (int x = 0; x < weights[i].RowCount; x++)
            {

                for (int y = 0; y < weights[i].ColumnCount; y++)
                {

                    weights[i][x, y] = Random.Range(-1f, 1f);

                }

            }

        }

    }

    public (float, float, float, float) RunNetwork(float a, float b, float c, float d)
    {
        inputLayer[0, 0] = a;
        inputLayer[0, 1] = b;
        inputLayer[0, 2] = c;
        inputLayer[0, 3] = d;

        inputLayer = inputLayer.PointwiseTanh();

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

        //First output is acceleration and second output is steering
        return (Sigmoid(outputLayer[0, 0]), Sigmoid(outputLayer[0, 1]), Sigmoid(outputLayer[0, 2]), Sigmoid(outputLayer[0, 3]));
    }

    private float Sigmoid(float s)
    {
        return (1 / (1 + Mathf.Exp(-s)));
    }

}
