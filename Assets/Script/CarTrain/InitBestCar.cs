using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class InitBestCar : MonoBehaviour
{
    public string fileName;

    // Start is called before the first frame update
    void Start()
    {
        ReadBestCarTrainedData($"Assets/Training_Result/{fileName}.txt", ref GetComponent<CarDNA>().neuralNetwork);
    }

    private void ReadBestCarTrainedData(string filePath, ref NeuralNetwork neuralNetwork)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        string str = Encoding.ASCII.GetString(bytes);
        string[] data = str.Split('\n');
        int i = 0;
        for (; i < neuralNetwork.ihWeights.rowNb; i++)
        {
            int j = 0;
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (; j < neuralNetwork.ihWeights.columnNb; j++)
            {
                neuralNetwork.ihWeights[i][j] = float.Parse(number[j]);
            }
        }
        int t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < 5; j++)
            {
                neuralNetwork.hoWeights[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb + neuralNetwork.biasH.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < neuralNetwork.biasH.columnNb; j++)
            {
                neuralNetwork.biasH[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb + neuralNetwork.biasH.rowNb + neuralNetwork.biasO.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < neuralNetwork.biasO.columnNb; j++)
            {
                neuralNetwork.biasO[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        int b = 2;
    }
}
