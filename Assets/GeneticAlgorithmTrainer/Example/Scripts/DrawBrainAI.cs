using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticAlgorithmTrainer.Example
{
    public class DrawBrainAI:MonoBehaviour
    {
        public GameObject NeuronPrefab;

        private LineRenderer line1;
        private List<GameObject> neurons;
        private void Start()
        {
            
        }
        public void Init(BrainStructure b)
        {
            int offsetX = -100;
            int offsetXInc = 40;
            int offsetY = 15;
            int i = 0;

            neurons = new List<GameObject>();
            List<GameObject> column_aux1 = new List<GameObject>();
            List<GameObject> column_aux2 = new List<GameObject>();

            for (i = 0; i < b.inputs; i++)
            {
                column_aux1.Add(InstanciatePrefab(new Vector3(offsetX, 30 - i * offsetY, 0), true));
            }
            offsetX += offsetXInc;

            GameObject go;

            foreach (int c in b.hiddenLayers)
            {
                for (i = 0; i < c; i++)
                {
                    go = InstanciatePrefab(new Vector3(offsetX, 30 - i * offsetY, 0));
                    column_aux2.Add(go);
                    DrawLineFromList(column_aux1, go); //draw lines between the objects from the last column and the new object.
                }

                column_aux1 = column_aux2;
                column_aux2 = new List<GameObject>();
                offsetX += offsetXInc;
            }

            for (i = 0; i < b.outputs; i++)
            {
                go = InstanciatePrefab(new Vector3(offsetX, 30 - i * offsetY, 0));
                DrawLineFromList(column_aux1, go);
            }
            offsetX += offsetXInc;

        }

        private void DrawLineFromList(List<GameObject> lg, GameObject go)
        {
            foreach (var a in lg)
            {
                DrawLine(a.transform.position, go.transform.position);
            }
        }
        private void DrawLine(Vector3 p1, Vector3 p2)
        {
            GameObject go = new GameObject();
            go.transform.parent = this.transform;
            line1 = go.AddComponent<LineRenderer>();
            line1.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            line1.positionCount = 2;
            line1.SetPosition(0, p1 - new Vector3(0, 0, 1));
            line1.SetPosition(1, p2 - new Vector3(0, 0, 1));

            line1.startColor = Color.red;
            line1.endColor = Color.red;

            line1.startWidth = 1f;
            line1.endWidth = 1f;
        }

        private GameObject InstanciatePrefab(Vector3 v, bool isInput = false)
        {
            GameObject go = Instantiate(NeuronPrefab);
            go.transform.parent = this.transform;
            go.transform.localPosition = v;

            if (!isInput)
                neurons.Add(go);
            else
                go.GetComponent<MeshRenderer>().material.color = Color.green;
            return go;
        }

        private void Update()
        {
            

        }

        public void DrawCallback(List<Matrix> lm)
        {
            int c = 0;
            float[] fAux;
            foreach (var m in lm)
            {
                fAux = m.ToArray();
                foreach (var f in fAux)
                {
                    neurons[c].GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.gray, Color.yellow, f);
                    neurons[c].transform.localScale = new Vector3(5, 5, 5) * (1f + f);
                    c++;
                }
            }
        }
    }
}