using GeneticAlgorithmTrainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmTrainer.Example
{
    public static class Parameters
    {
        public static int currentMap = 0;

        //Parameters for New NN Scene
        public static int genePool;
        public static float mutationProbability;
        public static int elitismQuantity;
        public static BrainStructure brainStructure;
        public static BrainAI.CrossoverMethod crossoverMethod;


        //Parameters for Test NN Scene
        public static Dictionary<string, BrainAI> SavedNNs;
        public static string SelectedNN = "";

        
    }
}