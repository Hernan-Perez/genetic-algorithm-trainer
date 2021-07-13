using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandlerScript : MonoBehaviour {

    private GameObject prefabCar;
    private List<CarHandler> carList, orderedCarList;
    private List<Color> colorGradients;

    private int generation = 1;
    private float lastBestScore = 0f, beforeLastScore = 0f, deltaScore = 0f;
    private float bestCurrentFitness = 0f;

    private bool allDead = false;

    private GUIStyle label1;

	// Use this for initialization
	void Start ()
    {

        label1 = new GUIStyle("label");
        label1.alignment = TextAnchor.UpperLeft;
        label1.fontSize = 24;
        label1.normal.textColor = Color.black;

        prefabCar = Resources.Load("carPrefab") as GameObject;

        carList = new List<CarHandler>();
        orderedCarList = new List<CarHandler>();
        colorGradients = new List<Color>();

        for (int i = 0; i < Parameters.genePool; i++)
        {
            carList.Add( ((GameObject)Instantiate(prefabCar)).GetComponent<CarHandler>() );
            colorGradients.Add(new Color(i / (float)Parameters.genePool, 1f - i/(float)Parameters.genePool, 0f));  //From green to red
        }

        for (int i = 0; i < carList.Count; i++)
        {
            carList[i].enabled = true;
        }
	}
	
    /// <summary>
    /// Order the cars from best to worst according to their fitness, and then gives them a color from green to red gradient.
    /// </summary>
    private void OrderCars()
    {
        List<CarHandler> listAux = new List<CarHandler>();
        for (int i = 0; i < Parameters.genePool; i++)
            listAux.Add(carList[i]);

        CarHandler aux = null;
        orderedCarList = new List<CarHandler>();

        //on each loop 1 car is removed from listAux and put inside the orderedList
        while (listAux.Count > 0)
        {
            aux = null;
            for (int i = 0; i < listAux.Count; i++)
            {
                if (aux == null || aux.Fitness < listAux[i].Fitness)
                {
                    aux = listAux[i];

                }
            }
            listAux.Remove(aux);
            orderedCarList.Add(aux);
            aux.ChangeColor(colorGradients[orderedCarList.IndexOf(aux)]);
        }
        bestCurrentFitness = orderedCarList[0].Fitness;
    }

	// Update is called once per frame
	void Update ()
    {
        if (!allDead)
        {
            if (Parameters.genePool == DeadCount())
            {
                allDead = true;
            }
            else
            {
                OrderCars();
            }
        }
        else
        {
            NewGeneration();
        }
    }

    private void NewGeneration()
    {
        List<CarHandler> nl = new List<CarHandler>();

        //Transfer selected parents to next generation
        if (Parameters.elitismQuantity > 0)
        {
            for (int i = 0; i < Parameters.elitismQuantity && i < Parameters.genePool; i++)
            {
                nl.Add(orderedCarList[i]);
                orderedCarList[i].Reset();
            }
        }

        CarHandler cAux;

        //crossover from the selected parents
        for (int i = nl.Count; i < Parameters.genePool; i++)
        {
                
            cAux = Instantiate(prefabCar).GetComponent<CarHandler>();
            cAux.Crossover(orderedCarList.GetRange(0, Parameters.elitismQuantity));
            cAux.Mutate(Parameters.mutationProbability, Parameters.crossoverMethod);
            nl.Add(cAux);
        }

        //destroy the cars from the old list that won't get transfered to the new generation
        //selected parents won't get destroyed because they have been reseted, therefore they don't count as dead.
        for (int i = 0; i < carList.Count; i++)
        {
            if (carList[i].IsDead)
                GameObject.Destroy(carList[i].gameObject);
        }
            
        carList = nl;
        orderedCarList = new List<CarHandler>();
        allDead = false;
        beforeLastScore = lastBestScore;
        lastBestScore = bestCurrentFitness;
        deltaScore = lastBestScore - beforeLastScore;
        generation++;
    }

    private int DeadCount()
    {
        

        int c = 0;
        for (int i = 0; i < carList.Count; i++)
        {
            if (carList[i].IsDead)
                c++;
        }
            

        return c;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.40f, 500, 50), "Generation: " + generation, label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.45f, 500, 50), "Current best: " + bestCurrentFitness.ToString("0.00"), label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.50f, 500, 50), "Best of last generation: " + lastBestScore.ToString("0.00") + " (" + ((deltaScore<0)?(""):("+")) + deltaScore.ToString("0.00") + ")", label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.55f, 500, 50), "Alive: " + (Parameters.genePool - DeadCount()) + "/" + Parameters.genePool, label1);

        GUI.Label(new Rect(0, 0, Screen.width, 50), "Pool: " + Parameters.genePool + "\tMutation prob.: " + (Parameters.mutationProbability *100f).ToString("0.00") + "%\t Elitism: " + Parameters.elitismQuantity + "\t Crossover: " + Parameters.crossoverMethod.ToString(), label1);

        GUI.Label(new Rect(10 + Screen.width * 0f, Screen.height * 0.25f, 500, 50), "TOP 10: ", label1);

        for (int i = 0; i < 10 && i < orderedCarList.Count; i++)
            GUI.Label(new Rect(10 + Screen.width * 0f, Screen.height * (0.30f + 0.04f * i), 500, 50), (i+1) + ": [" + orderedCarList[i].GetID + "] " + orderedCarList[i].Fitness.ToString("0.00"), label1);
    }
}
