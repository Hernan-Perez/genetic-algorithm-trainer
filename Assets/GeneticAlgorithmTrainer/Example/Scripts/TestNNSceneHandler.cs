using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GeneticAlgorithmTrainer.Example
{
    public class TestNNSceneHandler : MonoBehaviour
    {

        public GameObject PauseMenu;
        public FreeMovementCamera CameraController;
        public List<GameObject> Maps;
        public DrawBrainAI NNRenderer;

        public bool IsPaused
        {
            get { return pause; }
        }


        private CarHandler car;

        private GUIStyle label1;
        private bool pause = true;
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


            //Init car
            car = GameObject.FindGameObjectWithTag("Car").GetComponent<CarHandler>();
            car.enabled = true;
            car.ChangeColor(Color.cyan);
            car.SetBrain(Parameters.SavedNNs[Parameters.SelectedNN]);
            car.displayID = false;

            NNRenderer.Init(car.GetBrain().brainStructure);

            car.ShowNNVisuallyCallback = NNRenderer.DrawCallback;

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

            car.Process();

            if (car.IsDead)
            {
                car.Reset();
            }
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

        public void RestartButton()
        {
            car.Reset();
            SetPause(false);
        }

        public void MainMenuButton()
        {
            SceneManager.LoadScene(0);
        }

        #endregion
    }
}