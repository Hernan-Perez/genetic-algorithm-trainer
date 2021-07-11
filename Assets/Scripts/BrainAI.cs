using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainAI
{
    public enum CrossoverMethod { SinglePoint, Uniform };

    private Matrix L0, B0, L1, B1, L2, B2;
    public int SerialCount
    {
        get
        {
            return L0.getSize() + B0.getSize() + L1.getSize() + B1.getSize() + L2.getSize() + B2.getSize();
        }
    }
    //L0 pesos entre input y capa 1, L1 pesos entre capa 1 y capa2, L2 pesos entre capa2 y output
    private int nodosL1 = 4;
    private int nodosL2 = 4;
    private int inputs = 5;
    private int outputs = 2;

    //Crear nuevo Brain COMPLETAMENTE aleatorio
	public BrainAI(int _inputs = 5, int _nodosL1 = 4, int _nodosL2 = 4, int _outputs = 2)
    {

        inputs = _inputs;
        nodosL1 = _nodosL1;
        nodosL2 = _nodosL2;
        outputs = _outputs;

        L0 = new Matrix(nodosL1, inputs);
        L0.Randomizar();

        B0 = new Matrix(nodosL1, 1);
        B0.Randomizar();

        L1 = new Matrix(nodosL2, nodosL1);
        L1.Randomizar();

        B1 = new Matrix(nodosL2, 1);
        B1.Randomizar();

        L2 = new Matrix(outputs, nodosL2);
        L2.Randomizar();

        B2 = new Matrix(outputs, 1);
        B2.Randomizar();

    }

	public Matrix Procesar (Matrix input)
    {
        //Debug.Log("Input: " + input.filas + " " + input.columnas);
        Matrix aux = Matrix.SigmoidMatrix(Matrix.Suma(Matrix.Cross(L0, input), B0));
        //Debug.Log("aux(0): " + aux.filas + " " + aux.columnas);
        aux = Matrix.SigmoidMatrix(Matrix.Suma(Matrix.Cross(L1, aux), B1));
        //Debug.Log("aux(1): " + aux.filas + " " + aux.columnas);
        aux = Matrix.SigmoidMatrix(Matrix.Suma(Matrix.Cross(L2, aux), B2));
        //Debug.Log("aux(2): " + aux.filas + " " + aux.columnas);
        return aux;
	}

    /// <summary>
    /// En realidad NO DEVUELVE hidden layer. Devuelve los pesos entre capa y capa
    /// 0 -> entre input y capa 1
    /// 1 -> entre capa 1 y capa 2
    /// 2 -> entre capa 2 y output
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Matrix getHiddenLayer(int i)
    {
        switch (i)
        {
            case 0:
                return L0;
            case 1:
                return L1;
            case 2:
                return L2;
            default:
                return null;
        }
    }

    public Matrix getBiasLayer(int i)
    {
        switch (i)
        {
            case 0:
                return B0;
            case 1:
                return B1;
            case 2:
                return B2;
            default:
                return null;
        }
    }

    public void setHiddenLayer(int i, Matrix m)
    {
        switch (i)
        {
            case 0:
                L0 = m;
            break;
            case 1:
                L1 = m;
            break;
            case 2:
                L2 = m;
            break;
        }
    }

    public void setBiasLayer(int i, Matrix m)
    {
        switch (i)
        {
            case 0:
                B0 = m;
                break;
            case 1:
                B1 = m;
                break;
            case 2:
                B2 = m;
                break;
        }
    }

    /// <summary>
    /// Matrix (L0 B0 L1 B1 L2 B2) -> float[] (L0 B0 L1 B1 L2 B2)
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float[] Serialize(BrainAI b)
    {
        float[] serial = new float[b.SerialCount];

        //orden: l0 b0 l1 b1 l2 b2

        int c = 0;
        int i, j;
        Matrix m;
        for (int z = 0; z < 3; z++)
        {
            m = b.getHiddenLayer(z);
            for (i = 0; i < m.filas; i++)
                for (j = 0; j < m.columnas; j++)
                {
                    serial[c] = m.m[i, j];
                    c++;
                }
            //b.setHiddenLayer(z, m);
            m = b.getBiasLayer(z);
            for (i = 0; i < m.filas; i++)
                for (j = 0; j < m.columnas; j++)
                {
                    serial[c] = m.m[i, j];
                    c++;
                }
            //b.setBiasLayer(z, m);
        }
        


        return serial;
    }

    /// <summary>
    /// Matrix (L0 B0 L1 B1 L2 B2) -> float[] (L0 B0 L1 B1 L2 B2)
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static BrainAI Deserialize(float[] serial)
    {
        //orden: l0 b0 l1 b1 l2 b2
        BrainAI b = new BrainAI();
        int c = 0;
        int i, j;
        Matrix m;
        for (int z = 0; z < 3; z++)
        {
            m = b.getHiddenLayer(z);
            for (i = 0; i < m.filas; i++)
                for (j = 0; j < m.columnas; j++)
                {
                    m.m[i, j] = serial[c];
                    c++;
                }
            m = b.getBiasLayer(z);
            for (i = 0; i < m.filas; i++)
                for (j = 0; j < m.columnas; j++)
                {
                    m.m[i, j] = serial[c];
                    c++;
                }
        }

        //Debug.Log(b.L0);

        return b;
    }

    public static BrainAI Crossover(BrainAI b1, BrainAI b2, CrossoverMethod cbm = CrossoverMethod.Uniform)
    {
        float[] f1, f2, ff;
        BrainAI aux = new BrainAI();

        f1 = BrainAI.Serialize(b1);
        f2 = BrainAI.Serialize(b2);
        ff = BrainAI.Serialize(aux);

        switch (cbm)
        {
            case CrossoverMethod.SinglePoint:
                //METODO CROSSOVER SINGLE POINT
                int r = Random.Range(0, f1.Length);
                for (int i = 0; i < f1.Length; i++)
                {
                    if (i < r)
                    {
                        ff[i] = f1[i];
                    }
                    else
                    {
                        ff[i] = f2[i];
                    }
                }
                break;

            case CrossoverMethod.Uniform:
                // METODO CROSSOVER UNIFORME
                for (int i = 0; i < f1.Length; i++)
                {
                    if (Random.Range(0f, 1f) < 0.5f)
                    {
                        ff[i] = f1[i];
                    }
                    else
                    {
                        ff[i] = f2[i];
                    }
                }
                break;

        }

        aux = BrainAI.Deserialize(ff);

        return aux;
    }

    public static BrainAI Mutate(BrainAI b, float mutProb)
    {
        float[] f = BrainAI.Serialize(b);

        for (int i = 0; i < f.Length; i++)
        {
            //muta si el random es menor a mutProb
            //EJ: si la prob es de 1.5% / 0.015, el random da un nro entre 0 y 1
            // necesito que sea menor a 0.015 para que entre en la probabilidad
            if (Random.Range(0f, 1f) < mutProb)
            {
                //float aux = f[i];
                //f[i] += Random.Range(-1f, 1f)/5f;///5f;
                f[i] = Random.Range(-2f, 2f);
                //Debug.Log("Mutacion: " + aux + " -> " + f[i]);
            }
        }
        b = BrainAI.Deserialize(f);
        return b;
    }

}
