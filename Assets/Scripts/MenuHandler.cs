using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{

    private GUIStyle buttons, textbox, label, labelError;
    private string pool_size, mutation;

    private bool display_error = false;

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


        pool_size = Parameters.genePool.ToString();
        mutation = (Parameters.mutationProbability * 100).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {

        GUI.Label(new Rect(100, 100, 200, 50), "Pool size:", label);
        pool_size = GUI.TextField(new Rect(100, 150, 200, 50), pool_size, textbox);

        GUI.Label(new Rect(400, 100, 400, 50), "Mutation probability (%):", label);
        mutation = GUI.TextField(new Rect(400, 150, 200, 50), mutation, textbox);

        if (display_error)
        {
            GUI.Label(new Rect(10, 10, 500, 50), "ERROR: INVALID PARAMETERS", labelError);
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
        try
        {
            Parameters.genePool = int.Parse(pool_size);
            Parameters.mutationProbability = float.Parse(mutation)/100f;
        }
        catch
        {
            return false;
        }

        return true;

        /*PONER ESTA VALIDACION
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
         
         
         */
    }
}
