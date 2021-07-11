using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager{

    public static bool PAUSA = false, PAUSA_FORZADA = false;

    public int genePool = 20;
    public float mutationProbability = 0.015f;
    public int elitismQuantity = 2;
    public bool enableTournament = false;
    public int tournamentSize = 5;
    public BrainAI.CrossoverMethod MetodoCrossover = BrainAI.CrossoverMethod.Uniform;
    private GameObject prefabCar;
    private List<CarHandler> listaCar, listaCarOrdenados;
    private List<Color> gradienteColores;

    private int generacion = 1;
    private float ultimoMejorScore = 0f, anteultimoMejorScore = 0f, mejoraEvolutiva = 0f;
    private float mejorFitnessActual = 0f;

    private bool todosMuertos = false;

    private GUIStyle label1;


    public GenerationManager(int gPool = 20, float mutProb = 0.03f, int elitism = 2, bool enTournament = false, int tournamentQ = 5)
    {
        genePool = gPool;
        mutationProbability = mutProb;
        elitismQuantity = elitism;
        tournamentSize = tournamentQ;
        enableTournament = enTournament;

        //VALIDO VARIABLES PUBLICAS////////
        if (genePool < 2)
            genePool = 2;   //por el algoritmo de seleccion necesito por lo menos 2
        if (mutationProbability < 0)
            mutationProbability = 0;
        if (mutationProbability > 1)
            mutationProbability = 1;
        if (elitismQuantity < 0)
            elitismQuantity = 0;
        if (elitismQuantity > genePool)
            elitismQuantity = genePool;
        if (tournamentSize < 0)
            tournamentSize = 0;
        if (tournamentSize > genePool)
            tournamentSize = genePool;
        //////////////////////////////////

        label1 = new GUIStyle("label");
        label1.alignment = TextAnchor.UpperLeft;
        label1.fontSize = 24;
        label1.normal.textColor = Color.black;

        //destruyo el primer auto que esta como "modelo"
        //GameObject.Destroy(GameObject.Find("Car"));
        prefabCar = Resources.Load("Personaje") as GameObject;

        listaCar = new List<CarHandler>();
        listaCarOrdenados = new List<CarHandler>();
        gradienteColores = new List<Color>();

        for (int i = 0; i < genePool; i++)
        {
            listaCar.Add(MonoBehaviour.Instantiate(prefabCar).GetComponent<CarHandler>());
            gradienteColores.Add(new Color(i / (float)genePool, 1f - i / (float)genePool, 0f));  //desde verde a rojo
        }
    }

    public void Reiniciar()
    {

    }

    /// <summary>
    /// Ordena los autos de mejor a peor segun su fitness en la lista listaCarOrdenados.
    /// Ademas les cambia el color a los autos de verde a rojo segun su fitness.
    /// </summary>
    private void OrdenarAutos()
    {
        List<CarHandler> listaAux = new List<CarHandler>();
        for (int i = 0; i < genePool; i++)
            listaAux.Add(listaCar[i]);

        CarHandler aux = null;
        listaCarOrdenados = new List<CarHandler>();

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
            listaCarOrdenados.Add(aux);
            aux.CambiarColor(gradienteColores[listaCarOrdenados.IndexOf(aux)]); //cambia el color de aux segun su posicion en la lista ordenada
        }
        mejorFitnessActual = listaCarOrdenados[0].Fitness;
    }

    // Update is called once per frame
    public void Update()
    {
        if (!todosMuertos)
        {
            if (genePool == cantMuertos())
            {
                todosMuertos = true;
                PAUSA_FORZADA = true;
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
        if (elitismQuantity > 0)
        {
            for (int i = 0; i < elitismQuantity && i < genePool; i++)
            {
                nl.Add(listaCarOrdenados[i]);
                listaCarOrdenados[i].Resetear();
            }
        }

        CarHandler cAux;

        //Hace la seleccion en forma directa o por torneo
        if (!enableTournament)
        {
            for (int i = nl.Count; i < genePool; i++)
            {

                cAux = MonoBehaviour.Instantiate(prefabCar).GetComponent<CarHandler>();
                cAux.Crossover(listaCarOrdenados[0], listaCarOrdenados[1]);
                cAux.Mutate(mutationProbability, MetodoCrossover);
                nl.Add(cAux);


                /*//PRUEBA
                cAux = Instantiate(prefabCar).GetComponent<CarHandler>();
                cAux.CrossBreed(listaCarOrdenados[0], listaCarOrdenados[0]);
                cAux.Mutate(mutationProbability, BrainAI.CrossBreedMethod.Uniforme);
                nl.Add(cAux);
                cAux.ShowDebugConsoleSerializedBrain();*/
            }
        }
        else
        {
            //FALTA CODEAR
        }

        for (int i = 0; i < listaCar.Count; i++)
        {
            //que no elimine a los que pasaron de generacion
            //ya se resetearon asi que son los que no estan meurtos
            if (!listaCar[i].isDead)
                continue;
            GameObject.Destroy(listaCar[i].gameObject);
        }

        listaCar = nl;
        listaCarOrdenados = new List<CarHandler>();
        todosMuertos = false;
        anteultimoMejorScore = ultimoMejorScore;
        ultimoMejorScore = mejorFitnessActual;
        mejoraEvolutiva = ultimoMejorScore - anteultimoMejorScore;
        generacion++;
        PAUSA_FORZADA = false;
    }

    private int cantMuertos()
    {
        int c = 0;
        for (int i = 0; i < listaCar.Count; i++)
            if (listaCar[i].isDead)
                c++;

        return c;
    }

    public void Draw()
    {
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.25f, 500, 50), "Generacion: " + generacion, label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.30f, 500, 50), "Mejor act.: " + mejorFitnessActual.ToString("0.00"), label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.35f, 500, 50), "Mejor ult. gen.: " + ultimoMejorScore.ToString("0.00") + " (" + ((mejoraEvolutiva < 0) ? ("") : ("+")) + mejoraEvolutiva.ToString("0.00") + ")", label1);
        GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.40f, 500, 50), "Vivos: " + (genePool - cantMuertos()) + "/" + genePool, label1);

        GUI.Label(new Rect(0, 0, Screen.width, 50), "Pool: " + genePool + "\tProb. mutacion: " + (mutationProbability * 100f).ToString("0.00") + "%\t Elitismo: " + elitismQuantity + "\t Crossover: " + ((MetodoCrossover == BrainAI.CrossoverMethod.Uniform) ? ("Uniforme") : ("Single point")), label1);

        GUI.Label(new Rect(Screen.width * 0f, Screen.height * 0.25f, 500, 50), "TOP 10: ", label1);
        for (int i = 0; i < 10 && i < listaCarOrdenados.Count; i++)
            GUI.Label(new Rect(Screen.width * 0f, Screen.height * (0.30f + 0.04f * i), 500, 50), (i + 1) + ": [" + listaCarOrdenados[i].getID + "] " + listaCarOrdenados[i].Fitness.ToString("0.00"), label1);
    }
}
