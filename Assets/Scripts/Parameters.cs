using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Parameters
{
    public static int genePool = 20;
    public static float mutationProbability = 0.03f;
    public static int elitismQuantity = 2;
    public static bool enableTournament = false;
    public static int tournamentSize = 5;
    public static BrainAI.CrossoverMethod MetodoCrossover = BrainAI.CrossoverMethod.Uniform;
}
