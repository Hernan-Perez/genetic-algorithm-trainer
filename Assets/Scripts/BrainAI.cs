using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Neural Network default composition:
/// 
/// Input layer: 5 nodes
/// Hidden layer 1: 4 nodes
/// Hidden layer 2: 4 nodes
/// Output layer: 2 nodes
/// 
/// Each node in the hidden layers and output has a bias value associated.
/// 
/// </summary>
public class BrainAI
{
    public enum CrossoverMethod { SinglePoint, Uniform };

    private Matrix L0, B0, L1, B1, L2, B2;// L -> weight matrix,  B -> bias matrix
    //L0 are weights between input and hidden layer 1
    //L1 are weights between hidden layer 1 and 2
    //L2 are weights between hidden layer 2 and outputs
    //B0 are the bias values of the nodes in hidden layer 1
    //B1 are the bias values of the nodes in hidden layer 2
    //B2 are the bias values of the nodes in output layer

    /// <summary>
    /// Returns the serialized length.
    /// </summary>
    public int SerialCount
    {
        get
        {
            return L0.getSize() + B0.getSize() + L1.getSize() + B1.getSize() + L2.getSize() + B2.getSize();
        }
    }

    

    private int nodesL1 = 4;
    private int nodesL2 = 4;
    private int inputs = 5;
    private int outputs = 2;

    /// <summary>
    /// Create a new Brain with completely random values
    /// </summary>
    public BrainAI(int _inputs = 5, int _nodesL1 = 4, int _nodesL2 = 4, int _outputs = 2)
    {

        inputs = _inputs;
        nodesL1 = _nodesL1;
        nodesL2 = _nodesL2;
        outputs = _outputs;

        L0 = new Matrix(nodesL1, inputs);
        L0.Randomize();

        B0 = new Matrix(nodesL1, 1);
        B0.Randomize();

        L1 = new Matrix(nodesL2, nodesL1);
        L1.Randomize();

        B1 = new Matrix(nodesL2, 1);
        B1.Randomize();

        L2 = new Matrix(outputs, nodesL2);
        L2.Randomize();

        B2 = new Matrix(outputs, 1);
        B2.Randomize();

    }

    /// <summary>
    /// Process an input through the neural network.
    /// Activation function: Sigmoid function
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
	public Matrix Process (Matrix input)
    {
        Matrix aux = Matrix.SigmoidMatrix(Matrix.Sum(Matrix.Cross(L0, input), B0));

        aux = Matrix.SigmoidMatrix(Matrix.Sum(Matrix.Cross(L1, aux), B1));
        aux = Matrix.SigmoidMatrix(Matrix.Sum(Matrix.Cross(L2, aux), B2));
        return aux;
	}


    public Matrix GetWeightLayer(int i)
    {
        switch (i)
        {
            case 0:
                return L0;
            case 1:
                return L1;
            case 2:
                return L2;
            default:
                return null;
        }
    }

    public Matrix GetBiasLayer(int i)
    {
        switch (i)
        {
            case 0:
                return B0;
            case 1:
                return B1;
            case 2:
                return B2;
            default:
                return null;
        }
    }

    public void SetWeightLayer(int i, Matrix m)
    {
        switch (i)
        {
            case 0:
                L0 = m;
            break;
            case 1:
                L1 = m;
            break;
            case 2:
                L2 = m;
            break;
        }
    }

    public void SetBiasLayer(int i, Matrix m)
    {
        switch (i)
        {
            case 0:
                B0 = m;
                break;
            case 1:
                B1 = m;
                break;
            case 2:
                B2 = m;
                break;
        }
    }

    /// <summary>
    /// Transform all the matrix to a unique float array.
    /// Order: l0 b0 l1 b1 l2 b2
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float[] Serialize(BrainAI b)
    {
        float[] serial = new float[b.SerialCount];

        int c = 0;
        int i, j;
        Matrix m;
        for (int z = 0; z < 3; z++)
        {
            m = b.GetWeightLayer(z);
            for (i = 0; i < m.rows; i++)
                for (j = 0; j < m.columns; j++)
                {
                    serial[c] = m.m[i, j];
                    c++;
                }

            m = b.GetBiasLayer(z);
            for (i = 0; i < m.rows; i++)
                for (j = 0; j < m.columns; j++)
                {
                    serial[c] = m.m[i, j];
                    c++;
                }
        }
        


        return serial;
    }

    /// <summary>
    /// Transform a float array to matrix
    /// Order: l0 b0 l1 b1 l2 b2
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static BrainAI Deserialize(float[] serial)
    {
        BrainAI b = new BrainAI();
        int c = 0;
        int i, j;
        Matrix m;
        for (int z = 0; z < 3; z++)
        {
            m = b.GetWeightLayer(z);
            for (i = 0; i < m.rows; i++)
                for (j = 0; j < m.columns; j++)
                {
                    m.m[i, j] = serial[c];
                    c++;
                }
            m = b.GetBiasLayer(z);
            for (i = 0; i < m.rows; i++)
                for (j = 0; j < m.columns; j++)
                {
                    m.m[i, j] = serial[c];
                    c++;
                }
        }

        //Debug.Log(b.L0);

        return b;
    }

    /// <summary>
    /// Crossover of 2 parents to produce a unique child using the chosen method.
    /// </summary>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <param name="cbm"></param>
    /// <returns></returns>
    public static BrainAI Crossover(BrainAI b1, BrainAI b2, CrossoverMethod cbm = CrossoverMethod.Uniform)
    {
        float[] f1, f2, ff;
        BrainAI aux = new BrainAI();

        f1 = BrainAI.Serialize(b1);
        f2 = BrainAI.Serialize(b2);
        ff = BrainAI.Serialize(aux);

        switch (cbm)
        {
            case CrossoverMethod.SinglePoint:
                int r = Random.Range(1, f1.Length);
                for (int i = 0; i < f1.Length; i++)
                {
                    if (i < r)
                    {
                        ff[i] = f1[i];
                    }
                    else
                    {
                        ff[i] = f2[i];
                    }
                }
                break;

            case CrossoverMethod.Uniform:
                for (int i = 0; i < f1.Length; i++)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        ff[i] = f1[i];
                    }
                    else
                    {
                        ff[i] = f2[i];
                    }
                }
                break;

        }

        aux = BrainAI.Deserialize(ff);

        return aux;
    }

    /// <summary>
    /// Applies to each value in each weight and bias matrix a probability of mutation.
    /// If a value get mutated it will get a new random value.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="mutProb"></param>
    /// <returns></returns>
    public static BrainAI Mutate(BrainAI b, float mutProb)
    {
        float[] f = BrainAI.Serialize(b);

        for (int i = 0; i < f.Length; i++)
        {
            //the value mutates if the random number generated is less or equal to the mutation probability
            if (Random.Range(0f, 1f) <= mutProb)
            {
                f[i] = Random.Range(-100f, 100f); //these are arbitrary number. Changing these may produce better or worst results.
            }
        }
        b = BrainAI.Deserialize(f);
        return b;
    }

}
