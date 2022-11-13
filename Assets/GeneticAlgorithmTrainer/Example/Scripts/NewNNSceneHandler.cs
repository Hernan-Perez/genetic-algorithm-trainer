using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GeneticAlgorithmTrainer.Example
{
    public class NewNNSceneHandler : MonoBehaviour
    {

        public GameObject PauseMenu;
        public FreeMovementCamera CameraController;
        public List<GameObject> Maps;
        public GameObject prefabCar;

        public bool IsPaused
        {
            get { return pause; }
        }

        
        private List<CarHandler> carList, orderedCarList;
        private List<Color> colorGradients;

        private int generation = 1;
        private float lastBestScore = 0f, beforeLastScore = 0f, deltaScore = 0f;
        private float bestCurrentFitness = 0f;

        private bool allDead = false;

        private GUIStyle label1;
        private bool pause = true;
        private BrainAI lastBestNN;

        private bool hideUI = false;

        // Use this for initialization
        void Start()
        {
            Maps[0].SetActive(false);
            Maps[Parameters.currentMap].SetActive(true);

            label1 = new GUIStyle("label");
            label1.alignment = TextAnchor.UpperLeft;
            label1.fontSize = 24;
            label1.normal.textColor = Color.black;
            label1.hover.textColor = Color.black;

            var oldCars = GameObject.FindGameObjectsWithTag("Car");
            foreach (var o in oldCars)
                Destroy(o.gameObject);

            carList = new List<CarHandler>();
            orderedCarList = new List<CarHandler>();
            colorGradients = new List<Color>();
            CarHandler cAux;
            for (int i = 0; i < Parameters.genePool; i++)
            {
                cAux = ((GameObject)Instantiate(prefabCar)).GetComponent<CarHandler>();
                cAux.InitializeBrain(Parameters.brainStructure);
                carList.Add(cAux);
                colorGradients.Add(new Color(i / (float)Parameters.genePool, 1f - i / (float)Parameters.genePool, 0f));  //From green to red
            }

            for (int i = 0; i < carList.Count; i++)
            {
                carList[i].enabled = true;
            }

            SetPause(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SetPause(!pause);

            if (Input.GetKeyDown(KeyCode.F1))
                hideUI = !hideUI;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (pause)
                return;

            ProcessCars();
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

        private void OnGUI()
        {
            if (hideUI)
                return;
            GUI.Label(new Rect(Screen.width * 0.0f, Screen.height * 0.70f, 500, 50), "Generation: " + generation, label1);
            GUI.Label(new Rect(Screen.width * 0.0f, Screen.height * 0.75f, 500, 50), "Current best: " + bestCurrentFitness.ToString("0.00"), label1);
            GUI.Label(new Rect(Screen.width * 0.0f, Screen.height * 0.80f, 500, 50), "Best of last generation: " + lastBestScore.ToString("0.00") + " (" + ((deltaScore < 0) ? ("") : ("+")) + deltaScore.ToString("0.00") + ")", label1);
            GUI.Label(new Rect(Screen.width * 0.0f, Screen.height * 0.85f, 500, 50), "Alive: " + (Parameters.genePool - DeadCount()) + "/" + Parameters.genePool, label1);

            GUI.Label(new Rect(0, 0, Screen.width, 50), "Pool: " + Parameters.genePool + "\tMutation prob.: " + (Parameters.mutationProbability * 100f).ToString("0.00") + "%\t Elitism: " + Parameters.elitismQuantity + "\t Crossover: " + Parameters.crossoverMethod.ToString(), label1);

            GUI.Label(new Rect(10 + Screen.width * 0f, Screen.height * 0.15f, 500, 50), "TOP 10: ", label1);

            for (int i = 0; i < 10 && i < orderedCarList.Count; i++)
                GUI.Label(new Rect(10 + Screen.width * 0f, Screen.height * (0.20f + 0.04f * i), 500, 50), (i + 1) + ": [" + orderedCarList[i].GetID + "] " + orderedCarList[i].Fitness.ToString("0.00"), label1);
        }

        private void ProcessCars()
        {
            foreach (var car in carList)
                car.Process();
        }



        /// <summary>
        /// Order the cars from best to worst according to their fitness, and then gives them a color from green to red gradient.
        /// </summary>
        private void OrderCars()
        {
            List<CarHandler> listAux = new List<CarHandler>(carList);
            /*for (int i = 0; i < Parameters.genePool; i++)
                listAux.Add(carList[i]);*/

            CarHandler aux = null;
            orderedCarList = new List<CarHandler>();


            int count = 0;
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
                aux.displayID = (count < 10); //only the current top 10 will display their ID.
                count++;
                
            }
            bestCurrentFitness = orderedCarList[0].Fitness;
        }

        private void NewGeneration()
        {
            lastBestNN = orderedCarList[0].GetBrain();

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

        private void SetPause(bool cond)
        {
            pause = cond;
            PauseMenu.SetActive(cond);
            CameraController.SetEnabled(!cond);
        }

        #region PAUSE_BUTTON_EVENTS
        public void ResumeButton()
        {
            SetPause(false);
        }

        public void SaveNNButton(InputField inf)
        {
            if (inf.text == "" || lastBestNN == null)
            {
                inf.GetComponent<Image>().color = Color.red;

            }
            else
            {
                if (Parameters.SavedNNs.ContainsKey(inf.text))
                    Parameters.SavedNNs[inf.text] = lastBestNN;
                else
                    Parameters.SavedNNs.Add(inf.text, lastBestNN);

                inf.GetComponent<Image>().color = Color.green;
            }
        }

        public void ChangeButton()
        {

        }

        public void MainMenuButton()
        {
            SceneManager.LoadScene(0);
        }

        #endregion
    }
}