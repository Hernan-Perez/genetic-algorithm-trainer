using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace GeneticAlgorithmTrainer.Example
{
    public class UIHandler : MonoBehaviour
    {
        [Header("Menues")]
        public GameObject MainMenu;
        public GameObject NewNN;
        public GameObject TestNN;
        public GameObject Help;

        [Space(10)]
        [Header("New NN menu parameters")]
        public InputField PoolSize;
        public InputField ElitismSelectionSize;
        public InputField MutationProbability;
        public Dropdown CrossoverMethod;
        public Dropdown TrainingCourse;
        public Dropdown NNArch;
        public GameObject ErrorText;

        [Space(10)]
        [Header("Test NN menu parameters")]
        public Dropdown DropdownNN;
        public Dropdown DropdownCourse;
        public GameObject ErrorTextTest;

        private List<int[]> arch_list;

        // Start is called before the first frame update
        void Start()
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
            CarHandler.nextID = 0;

            MainMenu.SetActive(true);
            NewNN.SetActive(false);
            TestNN.SetActive(false);
            Help.SetActive(false);

            if (Parameters.SavedNNs == null)
                Parameters.SavedNNs = new Dictionary<string, BrainAI>();

            DropdownNN.ClearOptions();
            List<string> lAux = new List<string>();
            foreach (var n in Parameters.SavedNNs.Keys)
            {
                lAux.Add(n);
            }

            DropdownNN.AddOptions(lAux);



            arch_list = new List<int[]>();
            //CUSTOM ARCHITECTURE PRESETS
            arch_list.Add(new int[] { 4, 4 });
            arch_list.Add(new int[] { 3, 3 });
            arch_list.Add(new int[] { 4, 6, 4 });
            arch_list.Add(new int[] { 2, 4, 4, 2});
            arch_list.Add(new int[] { 3, 8, 8, 3 });

            //convert arch_list into strings to show on dropdown
            lAux = new List<string>();
            NNArch.ClearOptions();
            string sAux = "";
            foreach (var a in arch_list)
            {
                sAux = "[ ";

                foreach (var b in a)
                {
                    sAux += b.ToString();
                    sAux += " ";
                }
                sAux += "]";
                lAux.Add(sAux);
            }
            NNArch.AddOptions(lAux);
        }

        private bool ValidateParamsNewNN()
        {
            int pool, elit;
            float mut;

            string pool_size = PoolSize.text;
            string elitismSize = ElitismSelectionSize.text;
            string mutation = MutationProbability.text;

            try
            {
                pool = int.Parse(pool_size);
                mut = float.Parse(mutation) / 100f;
                elit = int.Parse(elitismSize);
            }
            catch
            {
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
            Parameters.currentMap = TrainingCourse.value;
            Parameters.brainStructure = new BrainStructure(5, arch_list[NNArch.value], 2);
            Parameters.crossoverMethod = (CrossoverMethod.value == 0) ? (BrainAI.CrossoverMethod.Uniform) : (BrainAI.CrossoverMethod.SinglePoint);

            return true;
        }


        #region UI_BUTTON_EVENTS

        #region MAIN_MENU
        public void Open_NewNN()
        {
            MainMenu.SetActive(false);
            NewNN.SetActive(true);
        }

        public void Open_TestNN()
        {
            MainMenu.SetActive(false);
            TestNN.SetActive(true);
        }

        public void Open_Help()
        {
            MainMenu.SetActive(false);
            Help.SetActive(true);
        }

        public void Exit_action()
        {
            Application.Quit();
        }
        #endregion

        #region NEW_NN_MENU
        public void NewNN_Start()
        {
            if (ValidateParamsNewNN())
                SceneManager.LoadScene(1);
            else
                ErrorText.SetActive(true);
        }

        public void NewNN_Back()
        {
            MainMenu.SetActive(true);
            NewNN.SetActive(false);
        }

        #endregion

        #region TEST_NN_MENU
        public void TestNN_Start()
        {
            if (Parameters.SavedNNs.Count == 0)
            {
                ErrorTextTest.SetActive(true);
                return;
            }
            Parameters.SelectedNN = DropdownNN.options[DropdownNN.value].text;
            Parameters.currentMap = DropdownCourse.value;

            SceneManager.LoadScene(2);
        }

        public void TestNN_Back()
        {
            MainMenu.SetActive(true);
            TestNN.SetActive(false);
        }
        #endregion

        #region HELP

        public void Help_Back()
        {
            MainMenu.SetActive(true);
            Help.SetActive(false);
        }
        #endregion


        #endregion
    }
}