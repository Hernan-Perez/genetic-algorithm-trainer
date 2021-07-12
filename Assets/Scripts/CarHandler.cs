using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour {

    private static int nextID = 0;
    public static bool displayID = true;
    private int ID;
    public int getID
    {
        get
        {
            return ID;
        }
    }

    /// <summary>
    /// Hasta donde "ve" el auto. Sirve para los inputs
    /// Intentar no cambiar este valor despues de tener un brain entrenado.
    /// </summary>
    public float distanciaMaximaVision = 10f;

    private float speedMultiplier = 10f;
    private float speedBase = 1f;
    private float angleMultiplier = 1000f;

    private float currentAngleLoop, maxAngleLoop = 10720f;  //para evitar que se quede girando en circulos

    private float speed = 0f;
    private float turnAngle = 0f;
    private float totalRecorrido = 0f;
    private BrainAI brain;
    private bool muerto = false;
    private Vector3 posInicial;
    private Vector3 rotacionInicial;

    private GUIStyle label1;

    public bool isDead
    {
        get
        {
            return muerto;
        }
    }

    public float Fitness
    {
        get
        {
            return totalRecorrido;
        }
    }

	// Use this for initialization
	void Start () {
        ID = nextID;
        nextID++;
        posInicial = transform.position;
        rotacionInicial = transform.eulerAngles;
        if (brain == null)
            brain = new BrainAI(5, 4, 4, 2);

        label1 = new GUIStyle("label");
        label1.alignment = TextAnchor.UpperCenter;
        label1.fontSize = 24;
        label1.normal.textColor = Color.black;

        if (ID == 0)    //para eliminar el de prueba que esta en escena
            GameObject.Destroy(this.gameObject);
    }

    // Update is called once per frame
    private void FixedUpdate ()
    {
        if (!muerto)
        {
            ProcesarBrain(getInputs());
            Moverse();
        }
        
	}

    private void OnGUI()
    {
        if (displayID)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1f, 0));
            GUI.Label(new Rect(pos.x - 50, Screen.height - pos.y, 100, 130), ID.ToString(), label1);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            muerto = true;
        }
    }

    private void ProcesarBrain(Matrix inputs)
    {
        
        Matrix output = brain.Procesar(inputs);

        //Debug.Log("M: " + output.m[0, 0] + " " + output.m[1, 0]);

        speed = output.m[0, 0];

        turnAngle = (output.m[1, 0] * 2f) - 1f; //para que los valores esten entre 1 y -1
        //Debug.Log(turnAngle);

    }

    private Matrix getInputs()
    {
        Matrix m = new Matrix(5, 1);

        m.m[0, 0] = getInputDireccion((Quaternion.Euler(0, -45f, 0) * transform.forward).normalized);
        m.m[1, 0] = getInputDireccion((Quaternion.Euler(0, -22.5f, 0) * transform.forward).normalized);
        m.m[2, 0] = getInputDireccion(transform.forward.normalized);    //mira adelante
        m.m[3, 0] = getInputDireccion((Quaternion.Euler(0, +22.5f, 0) * transform.forward).normalized);
        m.m[4, 0] = getInputDireccion((Quaternion.Euler(0, +45f, 0) * transform.forward).normalized);
        //Debug.Log("....................");
        return m;
    }

    private float getInputDireccion(Vector3 dir)
    {
        //Debug.Log(dir);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, dir, distanciaMaximaVision);
        float res = -1f;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.tag == "Wall")
            {
                if (Vector3.Distance(hits[i].point, transform.position) < res || res == -1)
                {
                    res = Vector3.Distance(hits[i].point, transform.position);
                }
            }
        }

        if (res < 0.01f)
            res = 0.01f;
        if (res > distanciaMaximaVision)
            res = distanciaMaximaVision;

        return res;
        //return 1f - res/distanciaMaximaVision; //si esta a dist = 0 -> ret = 1f // si esta a dist max -> ret = 0f
    }

    private void Moverse()
    {
        transform.RotateAround(transform.position, Vector3.up, turnAngle * angleMultiplier * Time.fixedDeltaTime);
        transform.position += transform.forward * (speedBase + speed) * speedMultiplier * Time.fixedDeltaTime;
        totalRecorrido += (speedBase + speed) * speedMultiplier * Time.fixedDeltaTime;// - Time.fixedDeltaTime;
        currentAngleLoop += turnAngle * angleMultiplier * Time.fixedDeltaTime;

        if (currentAngleLoop > maxAngleLoop || currentAngleLoop < -maxAngleLoop)
            muerto = true;
    }

    public void Resetear()
    {
        transform.position = posInicial;
        transform.eulerAngles = rotacionInicial;
        muerto = false;
        speed = turnAngle = totalRecorrido = 0f;
    }

    public void CambiarColor(Color col)
    {
        if (this == null)
            return;

        transform.GetChild(0).GetComponent<Renderer>().material.color = col;
    }

    public BrainAI getBrain()
    {
        return brain;
    }

    public void Crossover(List<CarHandler> l)
    {
        CarHandler s1, s2;

        //gets random parent for crossover
        s1 = l[Random.Range(0, l.Count)];

        //get random parent for the 2° one, but check that its a different one than s1
        do
        {
            s2 = l[Random.Range(0, l.Count)];
        } while (s2 == s1);


        brain = new BrainAI();
        brain = BrainAI.Crossover(s1.getBrain(), s2.getBrain());
    }

    public void Mutate(float mutProb, BrainAI.CrossoverMethod metodo = BrainAI.CrossoverMethod.Uniform)
    {
        brain = BrainAI.Mutate(brain, mutProb);
    }

    public void ShowDebugConsoleSerializedBrain()
    {
        float[] b = BrainAI.Serialize(brain);
        string output = "(ID" + ID + ") ";

        for (int i = 0; i < b.Length; i++)
            output += b[i] + " ";

        Debug.Log(output);
    }

}
