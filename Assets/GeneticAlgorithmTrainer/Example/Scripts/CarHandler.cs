using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmTrainer.Example
{
    public class CarHandler : MonoBehaviour
    {

        public static int nextID = 0;
        public bool displayID = true;
        private int ID;
        public int GetID
        {
            get
            {
                return ID;
            }
        }

        // Change the max distance that the car can 'see'.
        public float maxVisionDistance = 10f;

        public Action<List<Matrix>> ShowNNVisuallyCallback;

        private const float speedMultiplier = 10f;
        private const float speedBase = 1f;
        private const float angleMultiplier = 1000f;
        private const float maxAngleLoop = 360f;

        private float currentAngleLoop;

        private float speed = 0f;
        private float turnAngle = 0f;
        private float totalDistance = 0f;
        private BrainAI brain;
        private bool dead = false;
        private Vector3 initialPos;
        private Vector3 initialRotation;

        private GUIStyle label1;

        public bool IsDead
        {
            get
            {
                return dead;
            }
        }

        public float Fitness
        {
            get
            {
                return totalDistance;
            }
        }

        // Use this for initialization
        void Start()
        {
            ID = nextID;
            nextID++;
            initialPos = transform.position;
            initialRotation = transform.eulerAngles;

            label1 = new GUIStyle("label");
            label1.alignment = TextAnchor.UpperCenter;
            label1.fontSize = 24;
            label1.normal.textColor = Color.black;
            label1.hover.textColor = Color.black;

        }

        public void InitializeBrain(BrainStructure b)
        {
            brain = new BrainAI(b);
        }

        public void Process()
        {
            if (brain == null)
                return;

            if (!dead)
            {
                ProcessBrain(GetInputs());
                Move();
            }

        }

        private void OnGUI()
        {
            if (displayID)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1f, 0));
                if (pos.z > 0)
                    GUI.Label(new Rect(pos.x - 50, Screen.height - pos.y, 100, 130), ID.ToString(), label1);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wall")
            {
                dead = true;
            }
            else if (other.tag == "Goal")
            {
                dead = true;
                totalDistance += 1000f;
            }
        }

        private void ProcessBrain(Matrix inputs)
        {
            Matrix output;
            if (ShowNNVisuallyCallback == null)
                output = brain.Process(inputs);
            else
            {
                List<Matrix> lm;
                output = brain.ProcessVerbose(inputs, out lm);
                ShowNNVisuallyCallback(lm);
            }

            speed = output.m[0, 0];

            turnAngle = (output.m[1, 0] * 2f) - 1f; //this normalizes the output values from 1 to -1

        }

        private Matrix GetInputs()
        {
            Matrix m = new Matrix(5, 1);

            m.m[0, 0] = GetInputDirection((Quaternion.Euler(0, -45f, 0) * transform.forward).normalized);
            m.m[1, 0] = GetInputDirection((Quaternion.Euler(0, -22.5f, 0) * transform.forward).normalized);
            m.m[2, 0] = GetInputDirection(transform.forward.normalized);    //mira adelante
            m.m[3, 0] = GetInputDirection((Quaternion.Euler(0, +22.5f, 0) * transform.forward).normalized);
            m.m[4, 0] = GetInputDirection((Quaternion.Euler(0, +45f, 0) * transform.forward).normalized);

            return m;
        }

        private float GetInputDirection(Vector3 dir)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(transform.position, dir, maxVisionDistance, -1, QueryTriggerInteraction.Ignore);
            float res = -1f;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.CompareTag("Wall"))
                {
                    if (Vector3.Distance(hits[i].point, transform.position) < res || res == -1)
                    {
                        res = Vector3.Distance(hits[i].point, transform.position);
                    }
                }
            }

            if (res < 0.01f)
                res = 0.01f;
            if (res > maxVisionDistance)
                res = maxVisionDistance;

            return res;
        }

        private void Move()
        {
            transform.RotateAround(transform.position, Vector3.up, turnAngle * angleMultiplier * Time.fixedDeltaTime);
            transform.position += transform.forward * (speedBase + speed) * speedMultiplier * Time.fixedDeltaTime;
            totalDistance += (speedBase + speed) * speedMultiplier * Time.fixedDeltaTime;
            currentAngleLoop += turnAngle * angleMultiplier * Time.fixedDeltaTime;

            if (currentAngleLoop > maxAngleLoop || currentAngleLoop < -maxAngleLoop)
                dead = true;
        }

        public void Reset()
        {
            transform.position = initialPos;
            transform.eulerAngles = initialRotation;
            dead = false;
            speed = turnAngle = totalDistance = 0f;
            currentAngleLoop = 0f;
        }

        public void ChangeColor(Color col)
        {
            if (this == null)
                return;

            transform.GetChild(0).GetComponent<Renderer>().material.color = col;
        }

        public BrainAI GetBrain()
        {
            return brain;
        }

        public void Crossover(List<CarHandler> l)
        {
            CarHandler s1, s2;
            
            //gets random parent for crossover
            s1 = l[UnityEngine.Random.Range(0, l.Count)];

            //get random parent for the 2° one, but check that its a different one than s1
            do
            {
                s2 = l[UnityEngine.Random.Range(0, l.Count)];
            } while (s2 == s1);

            brain = BrainAI.Crossover(s1.GetBrain(), s2.GetBrain());
        }

        public void Mutate(float mutProb, BrainAI.CrossoverMethod method)
        {
            brain = BrainAI.Mutate(brain, mutProb, method);
        }

        public void ShowDebugConsoleSerializedBrain()
        {
            float[] b = BrainAI.BrainToArray(brain);
            string output = "(ID" + ID + ") ";

            for (int i = 0; i < b.Length; i++)
                output += b[i] + " ";

            Debug.Log(output);
        }

        public void SetBrain(float[] b, BrainStructure bStruct)
        {
            brain = BrainAI.ArrayToBrain(b, bStruct);
        }

        public void SetBrain(BrainAI b)
        {
            brain = b;
        }
    }
}