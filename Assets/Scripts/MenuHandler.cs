using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

    private GUIStyle buttons, textbox, label, labelError;
    private string pool_size, mutation, elitismSize;
    private BrainAI.CrossoverMethod cm;

    private bool display_error = false;
    private string error_msg = "";

    // Start is called before the first frame update
    void Start()
    {
        buttons = new GUIStyle("button");
        buttons.fontSize = 32;

        textbox = new GUIStyle("textfield");
        textbox.fontSize = 32;

        label = new GUIStyle("label");
        label.fontSize = 32;

        labelError = new GUIStyle("label");
        labelError.fontSize = 32;
        labelError.normal.textColor = Color.red;

        //load default values to parameters
        pool_size = Parameters.genePool.ToString();
        mutation = (Parameters.mutationProbability * 100).ToString();
        elitismSize = Parameters.elitismQuantity.ToString();
        cm = Parameters.crossoverMethod;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(100, 100, 200, 50), "Pool size:", label);
        pool_size = GUI.TextField(new Rect(100, 150, 200, 50), pool_size, textbox);

        GUI.Label(new Rect(600, 100, 400, 50), "Mutation probability (%):", label);
        mutation = GUI.TextField(new Rect(600, 150, 200, 50), mutation, textbox);

        GUI.Label(new Rect(100, 250, 400, 50), "Elitism selection size:", label);
        elitismSize = GUI.TextField(new Rect(100, 300, 200, 50), elitismSize, textbox);

        GUI.Label(new Rect(600, 250, 400, 50), "Crossover method:", label);
        if (GUI.Button(new Rect(600, 300, 200, 50), cm.ToString()))
        {
            cm++;
            if ((int)cm == Enum.GetValues(typeof(BrainAI.CrossoverMethod)).Length)
                cm = 0;
        }

        if (display_error)
        {
            GUI.Label(new Rect(10, 10, 500, 50), error_msg, labelError);
        }

        if (GUI.Button(new Rect(Screen.width * 0.20f, Screen.height * 0.80f, Screen.width * 0.20f, Screen.height * 0.07f), "Start", buttons))
        {
            if (ValidateParams())
                SceneManager.LoadScene(1);
            else
                display_error = true;
        }

        if (GUI.Button(new Rect(Screen.width * 0.60f, Screen.height * 0.80f, Screen.width * 0.20f, Screen.height * 0.07f), "Exit", buttons))
        {
            Application.Quit();
        }
    }

    private bool ValidateParams()
    {
        int pool, elit;
        float mut;

        try
        {
            pool = int.Parse(pool_size);
            mut = float.Parse(mutation)/100f;
            elit = int.Parse(elitismSize);
        }
        catch
        {
            error_msg = "Error: One or more parameters contain an invalid number";
            return false;
        }

        if (pool < 2)
            pool = 2;

        if (mut < 0)
            mut = 0;

        if (mut > 1)
            mut = 1;

        if (elit < 0)
            elit = 0;

        if (elit > pool)
            elit = pool;


        Parameters.genePool = pool;
        Parameters.mutationProbability = mut;
        Parameters.elitismQuantity = elit;
        Parameters.crossoverMethod = cm;

        return true;
    }
}
