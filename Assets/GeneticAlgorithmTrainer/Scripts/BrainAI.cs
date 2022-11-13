using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneticAlgorithmTrainer
{
    public struct BrainStructure
    {
        public int inputs;
        public int outputs;
        public int[] hiddenLayers;

        public BrainStructure(int inputs, int[] hiddenLayers, int outputs)
        {
            this.inputs = inputs;
            this.hiddenLayers = (int[])hiddenLayers.Clone();
            this.outputs = outputs;
        }

        public static bool Compare(BrainStructure a, BrainStructure b)
        {
            return (a.inputs == b.inputs && a.outputs == b.outputs && a.hiddenLayers.SequenceEqual(b.hiddenLayers));
        }
    }

    /// <summary> 
    /// REESCRIBIR
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

        public int TotalElements
        {
            get
            {
                int res = 0;
                foreach (Matrix mAux in L)
                    res += mAux.getSize();

                foreach (Matrix mAux in B)
                    res += mAux.getSize();
                return res;
            }
        }

        public BrainStructure brainStructure
        {
            get { return bStructure; }
        }

        //private Matrix L0, B0, L1, B1, L2, B2;
        private Matrix[] L, B;
        /* 
        L -> weight matrix,  B -> bias matrix
        Example with 2 hidden layers:
            L0 are weights between input and hidden layer 1
            L1 are weights between hidden layer 1 and 2
            L2 are weights between hidden layer 2 and outputs
            B0 are the bias values of the nodes in hidden layer 1
            B1 are the bias values of the nodes in hidden layer 2
            B2 are the bias values of the nodes in output layer
        */

        private BrainStructure bStructure;

        /// <summary>
        /// Creates a new Brain using the structure of another Brain
        /// </summary>
        /// <param name="b"></param>
        public BrainAI(BrainAI b)
        {
            bStructure = b.brainStructure;

            InitBrain();
        }
        /// <summary>
        /// Create a new Brain with a defined structure
        /// </summary>
        public BrainAI(BrainStructure bStruct)
        {

            bStructure = bStruct;

            InitBrain();
        }

        private void InitBrain()
        {
            //Creates the matrices
            L = new Matrix[1 + bStructure.hiddenLayers.Length];
            B = new Matrix[1 + bStructure.hiddenLayers.Length];

            L[0] = new Matrix(bStructure.hiddenLayers[0], bStructure.inputs);
            B[0] = new Matrix(bStructure.hiddenLayers[0], 1);
            int i;
            for (i = 1; i < bStructure.hiddenLayers.Length; i++)
            {
                L[i] = new Matrix(bStructure.hiddenLayers[i], bStructure.hiddenLayers[i - 1]);
                B[i] = new Matrix(bStructure.hiddenLayers[i], 1);
            }

            L[i] = new Matrix(bStructure.outputs, bStructure.hiddenLayers[i - 1]);
            B[i] = new Matrix(bStructure.outputs, 1);


            //Randomize matrices
            foreach (Matrix mAux in L)
                mAux.Randomize();

            foreach (Matrix mAux in B)
                mAux.Randomize();
        }

        /// <summary>
        /// Process an input through the neural network.
        /// Activation function: Sigmoid function
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Matrix Process(Matrix input)
        {
            Matrix aux;

            try
            {
                aux = Matrix.SigmoidMatrix(Matrix.Sum(Matrix.Cross(L[0], input), B[0]));
                for (int i = 1; i < L.Length; i++)
                    aux = Matrix.SigmoidMatrix(Matrix.Sum(Matrix.Cross(L[i], aux), B[i]));
            }
            catch (Exception)
            {
                throw new BrainAIException("Invalid input format");
            }
            return aux;
        }


        /// <summary>
        /// Process an input through the neural network, and give you aux values.
        /// This function could be used to display visually the hidden layers of a neural network.
        /// Activation function: Sigmoid function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aux_values"></param>
        /// <returns></returns>
        public Matrix ProcessVerbose(Matrix input, out List<Matrix> aux_values)
        {
            Matrix aux;
            aux_values = new List<Matrix>();
            try
            {
                aux = Matrix.SigmoidMatrix(Matrix.Sum(Matrix.Cross(L[0], input), B[0]));
                aux_values.Add(aux.Clone());
                for (int i = 1; i < L.Length; i++)
                {
                    aux = Matrix.SigmoidMatrix(Matrix.Sum(Matrix.Cross(L[i], aux), B[i]));
                    aux_values.Add(aux.Clone());
                }
                    
            }
            catch (Exception)
            {
                throw new BrainAIException("Invalid input format");
            }
            return aux;
        }

        /// <summary>
        /// Transform all the matrix to a unique float array.
        /// Order: l0 b0 l1 b1 l2 b2
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float[] BrainToArray(BrainAI b)
        {
            float[] serial = new float[b.TotalElements];

            int c = 0;
            int i, j;
            Matrix m;
            for (int z = 0; z < 3; z++)
            {
                m = b.L[z];
                for (i = 0; i < m.rows; i++)
                    for (j = 0; j < m.columns; j++)
                    {
                        serial[c] = m.m[i, j];
                        c++;
                    }

                m = b.B[z];
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
        public static BrainAI ArrayToBrain(float[] serial, BrainAI brain)
        {
            return ArrayToBrain(serial, brain.bStructure);
        }
            public static BrainAI ArrayToBrain(float[] serial, BrainStructure bStruct)
        {
            BrainAI b = new BrainAI(bStruct);
            if (b.TotalElements != serial.Length)
                throw new BrainAIException("Invalid array length for the current BrainAI structure.");

            int c = 0;
            int i, j;
            Matrix m;

            for (int z = 0; z < b.L.Length; z++)
            {
                m = b.L[z];
                for (i = 0; i < m.rows; i++)
                    for (j = 0; j < m.columns; j++)
                    {
                        m.m[i, j] = serial[c];
                        c++;
                    }
                m = b.B[z];
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

            if (!BrainAI.CompareBrainStructure(b1, b2))
                throw new BrainAIException("Different Brain structure detected during Crossover");

            BrainAI aux = new BrainAI(b1);

            f1 = BrainAI.BrainToArray(b1);
            f2 = BrainAI.BrainToArray(b2);
            ff = BrainAI.BrainToArray(aux);

            switch (cbm)
            {
                case CrossoverMethod.SinglePoint:
                    int r = UnityEngine.Random.Range(1, f1.Length);
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
                        if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
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

            aux = BrainAI.ArrayToBrain(ff, aux);

            return aux;
        }

        /// <summary>
        /// Applies to each value in each weight and bias matrix a probability of mutation.
        /// If a value get mutated it will get a new random value.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="mutProb"></param>
        /// <returns></returns>
        public static BrainAI Mutate(BrainAI b, float mutProb, BrainAI.CrossoverMethod method = BrainAI.CrossoverMethod.Uniform)
        {
            float[] f = BrainAI.BrainToArray(b);

            for (int i = 0; i < f.Length; i++)
            {
                //the value mutates if the random number generated is less or equal to the mutation probability
                if (UnityEngine.Random.Range(0f, 1f) <= mutProb)
                {
                    f[i] = UnityEngine.Random.Range(-100f, 100f); //these are arbitrary number. Changing these may produce better or worst results.
                }
            }
            b = BrainAI.ArrayToBrain(f, b);
            return b;
        }

        /// <summary>
        /// Returns true if boths brains have the same number of inputs, outputs and hidden layer structure.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CompareBrainStructure(BrainAI a, BrainAI b)
        {
            return BrainStructure.Compare(a.brainStructure, b.brainStructure);
        }
    }

    public class BrainAIException : Exception
    {
        public BrainAIException(string message) : base(message)
        {
        }
    }
}