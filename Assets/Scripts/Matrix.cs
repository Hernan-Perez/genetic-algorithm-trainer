using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix
{
    public int rows, columns;
    public float[,] m;

    public Matrix(int f, int c)
    {
        rows = f;
        columns = c;
        m = new float[f,c];
    }

    /// <summary>
    /// Randomize all values from -1f to 1f
    /// </summary>
    public void Randomize()
    {
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                m[i, j] = Random.Range(-1f, 1f);
    }

    public int getSize()
    {
        return rows * columns;
    }


    /// <summary>
    /// Returns a value from 0f to 1f
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Sigmoid(float x)
    {
        float y = 1f / (1f + Mathf.Pow((float)System.Math.E, -x));
        return y;
    }

    public static Matrix SigmoidMatrix (Matrix m)
    {
        Matrix n = new Matrix(m.rows, m.columns);
        for (int i = 0; i < m.rows; i++)
        {
            for (int j = 0; j < m.columns; j++)
            {
                n.m[i,j] = Sigmoid(m.m[i,j]);
            }
        }
        return n;
    }

    public static Matrix Multiply(Matrix m, float escalar)
    {
        Matrix aux = new Matrix(m.rows, m.columns);
        for (int i = 0; i < m.rows; i++)
            for (int j = 0; j < m.columns; j++)
                aux.m[i, j] = m.m[i, j] * escalar;
        return aux; 
    }

    public static Matrix Sum(Matrix m1, Matrix m2)
    {
        Matrix aux = new Matrix(m1.rows, m1.columns);
        for (int i = 0; i < m1.rows; i++)
            for (int j = 0; j < m1.columns; j++)
                aux.m[i, j] = m1.m[i, j] + m2.m[i, j];
        return aux;
    }

    public static Matrix Substract(Matrix m1, Matrix m2)
    {
        Matrix aux = new Matrix(m1.rows, m1.columns);
        for (int i = 0; i < m1.rows; i++)
            for (int j = 0; j < m1.columns; j++)
                aux.m[i, j] = m1.m[i, j] - m2.m[i, j];
        return aux;
    }

    /// <summary>
    /// Only valid if the num of columns of m1 y the same as the num of rows of m2.
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static Matrix Cross(Matrix m1, Matrix m2)
    {
        if (m1.columns != m2.rows)
        {
            return null;
        }
            
        Matrix aux = new Matrix(m1.rows, m2.columns);
        for (int j = 0; j < aux.columns; j++)
            for (int i = 0; i < aux.rows; i++)
            {
                aux.m[i, j] = 0;

                for (int k = 0; k < m1.columns; k++)
                {
                    aux.m[i, j] += m1.m[i, k] * m2.m[k, j];
                }
            }
        return aux;
    }
}
