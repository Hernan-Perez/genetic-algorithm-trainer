using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandlerScript : MonoBehaviour {

    private GameObject prefabCar;
    private List<CarHandler> carList, orderedCarList;
    private List<Color> colorGradients;

    private int generation = 1;
    private float lastBestScore = 0f, beforeLastScore = 0f, deltaScore = 0f;
    private float mejorFitnessActual = 0f;

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
            colorGradients.Add(new Color(i / (float)Parameters.genePool, 1f - i/(float)Parameters.genePool, 0f));  //desde verde a rojo
        }

        for (int i = 0; i < carList.Count; i++)
        {
            carList[i].enabled = true;
        }
	}
	
    /// <summary>
    /// Ordena los autos de mejor a peor segun su fitness en la lista listaCarOrdenados.
    /// Ademas les cambia el color a los autos de verde a rojo segun su fitness.
    /// </summary>
    private void OrdenarAutos()
    {
        List<CarHandler> listaAux = new List<CarHandler>();
        for (int i = 0; i < Parameters.genePool; i++)
            listaAux.Add(carList[i]);

        CarHandler aux = null;
        orderedCarList = new List<CarHandler>();

        while (listaAux.Count > 0)  //en cada ciclo voy sacando el mejor de la lista y lo pongo en la de ordenados
        {
            aux = null;
            for (int i = 0; i < listaAux.Count; i++)
            {
                if (aux == null || aux.Fitness < listaAux[i].Fitness)
                {
                    aux = listaAux[i];

                }
            }
            listaAux.Remove(aux);
            orderedCarList.Add(aux);
            aux.CambiarColor(colorGradients[orderedCarList.IndexOf(aux)]); //cambia el color de aux segun su posicion en la lista ordenada
        }
        mejorFitnessActual = orderedCarList[0].Fitness;
    }

	// Update is called once per frame
	void Update ()
    {
        if (!allDead)
        {
            if (Parameters.genePool == deadCount())
            {
                allDead = true;
            }
            else
            {
                OrdenarAutos();
            }
        }
        else
        {
            NuevaGeneracion();
        }
    }

    private void NuevaGeneracion()
    {
        List<CarHandler> nl = new List<CarHandler>();

        //Transfiere el elitismo
        if (Parameters.elitismQuantity > 0)
        {
            for (int i = 0; i < Parameters.elitismQuantity && i < Parameters.genePool; i++)
            {
                nl.Add(orderedCarList[i]);
                orderedCarList[i].Resetear();
            }
        }

        CarHandler cAux;

        //selection
        for (int i = nl.Count; i < Parameters.genePool; i++)
        {
                
            cAux = Instantiate(prefabCar).GetComponent<CarHandler>();
            cAux.Crossover(orderedCarList.GetRange(0, Parameters.elitismQuantity));
            cAux.Mutate(Parameters.mutationProbability, Parameters.crossoverMethod);
            nl.Add(cAux);
        }


        for (int i = 0; i < carList.Count; i++)
        {
            //que no elimine a los que pasaron de generacion
            //ya se resetearon asi que son los que no estan meurtos
            if (carList[i].isDead)
                GameObject.Destroy(carList[i].gameObject);
        }
            
        carList = nl;
        orderedCarList = new List<CarHandler>();
        allDead = false;
        beforeLastScore = lastBestScore;
        lastBestScore = mejorFitnessActual;
        deltaScore = lastBestScore - beforeLastScore;
        generation++;
    }

    private int deadCount()
    {
        

        int c = 0;
        for (int i = 0; i < carList.Count; i++)
        {
            if (carList[i].isDead)
                c++;
        }
            

        return c;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.40f, 500, 50), "Generation: " + generation, label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.45f, 500, 50), "Current best: " + mejorFitnessActual.ToString("0.00"), label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.50f, 500, 50), "Best of last generation: " + lastBestScore.ToString("0.00") + " (" + ((deltaScore<0)?(""):("+")) + deltaScore.ToString("0.00") + ")", label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.55f, 500, 50), "Alive: " + (Parameters.genePool - deadCount()) + "/" + Parameters.genePool, label1);

        GUI.Label(new Rect(0, 0, Screen.width, 50), "Pool: " + Parameters.genePool + "\tMutation prob.: " + (Parameters.mutationProbability *100f).ToString("0.00") + "%\t Elitism: " + Parameters.elitismQuantity + "\t Crossover: " + Parameters.crossoverMethod.ToString(), label1);

        GUI.Label(new Rect(10 + Screen.width * 0f, Screen.height * 0.25f, 500, 50), "TOP 10: ", label1);

        for (int i = 0; i < 10 && i < orderedCarList.Count; i++)
            GUI.Label(new Rect(10 + Screen.width * 0f, Screen.height * (0.30f + 0.04f * i), 500, 50), (i+1) + ": [" + orderedCarList[i].getID + "] " + orderedCarList[i].Fitness.ToString("0.00"), label1);
    }
}
