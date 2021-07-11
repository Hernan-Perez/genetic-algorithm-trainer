using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix
{
    public int filas, columnas;
    public float[,] m;

    public Matrix(int f, int c)
    {
        filas = f;
        columnas = c;
        m = new float[f,c];
    }

    //ENTRE -1 y 1 -- ESTO NO SE SI DEBERIA SER ASI
    public void Randomizar()
    {
        for (int i = 0; i < filas; i++)
            for (int j = 0; j < columnas; j++)
                m[i, j] = Random.Range(-1f, 1f);
    }

    public int getSize()
    {
        return filas * columnas;
    }

    //FUNCIONES ESTATICAS////////////////////////////////////////////////
    public static float sigmoid(float x)
    {
        float y = 1f / (1f + Mathf.Pow((float)System.Math.E, -x));
        return y;
    }

    public static Matrix SigmoidMatrix (Matrix m)
    {
        Matrix n = new Matrix(m.filas, m.columnas);
        for (int i = 0; i < m.filas; i++)
        {
            for (int j = 0; j < m.columnas; j++)
            {
                n.m[i,j] = sigmoid(m.m[i,j]);
            }
        }
        return n;
    }

    public static Matrix Multiplicar(Matrix m, float escalar)
    {
        Matrix aux = new Matrix(m.filas, m.columnas);
        for (int i = 0; i < m.filas; i++)
            for (int j = 0; j < m.columnas; j++)
                aux.m[i, j] = m.m[i, j] * escalar;
        return aux; 
    }

    public static Matrix Suma(Matrix m1, Matrix m2)
    {
        Matrix aux = new Matrix(m1.filas, m1.columnas);
        for (int i = 0; i < m1.filas; i++)
            for (int j = 0; j < m1.columnas; j++)
                aux.m[i, j] = m1.m[i, j] + m2.m[i, j];
        return aux;
    }

    public static Matrix Resta(Matrix m1, Matrix m2)
    {
        Matrix aux = new Matrix(m1.filas, m1.columnas);
        for (int i = 0; i < m1.filas; i++)
            for (int j = 0; j < m1.columnas; j++)
                aux.m[i, j] = m1.m[i, j] - m2.m[i, j];
        return aux;
    }

    //RECORDATORIO: SE MULTIPLICAN LAS FILAS DE M1 POR LAS COLUMNAS DE M2
    // TIENEN QUE COINCIDIR LA CANT DE COLUMNAS DE M1 CON LA CANTIDAD DE FILAS DE M2
    public static Matrix Cross(Matrix m1, Matrix m2)
    {
        if (m1.columnas != m2.filas)
        {
            return null;
        }
            
        Matrix aux = new Matrix(m1.filas, m2.columnas);
        for (int j = 0; j < aux.columnas; j++)
            for (int i = 0; i < aux.filas; i++)
            {
                aux.m[i, j] = 0;
                //k es el offset para multiplicar una determinada fila por columna ELEMENTO A ELEMENTO
                for (int k = 0; k < m1.columnas; k++)
                {
                    aux.m[i, j] += m1.m[i, k] * m2.m[k, j];
                }
            }
        return aux;
    }
}
