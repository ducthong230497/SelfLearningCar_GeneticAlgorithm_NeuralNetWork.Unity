using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    public int inputNodes;
    public int hiddenNodes;
    public int outputNodes;

    public Matrix ihWeights;
    public Matrix hoWeights;

    public Matrix biasH;
    public Matrix biasO;

    public NeuralNetwork(int input, int hidden, int output)
    {
        inputNodes = input;
        hiddenNodes = hidden;
        outputNodes = output;

        ihWeights = new Matrix(hiddenNodes, inputNodes);
        hoWeights = new Matrix(outputNodes, hiddenNodes);

        ihWeights.Randomize();
        hoWeights.Randomize();

        biasH = new Matrix(hiddenNodes, 1);
        biasO = new Matrix(outputNodes, 1);

        biasH.Randomize();
        biasO.Randomize();
    }

    public float[] FeedForward(float[] inputArray)
    {
        // Generate hidden layer
        Matrix inputs = Matrix.FromArray(inputArray);
        Matrix hidden = ihWeights * inputs;
        hidden += biasH;
        // Apply activatate function for hidden layer
        hidden.map(sigmoid);

        // Generate output layer
        Matrix output = hoWeights * hidden;
        output += biasO;
        output.map(sigmoid);

        return output.ToArray();
    }

    private float sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }
}
