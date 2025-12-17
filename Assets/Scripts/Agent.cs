using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public int mark;

    [Header("随机选择概率")]
    public double epsilon = 0.1;

    [Header("学习率")]
    [SerializeField] double alpha = 0.1;

    public ValueMatrix valueMatrix = new ValueMatrix(new int[] {3, 3, 3, 3, 3, 3, 3, 3, 3});
    int[] storedOutcome = null;

    // Start is called before the first frame update
    void Awake()
    {
        storedOutcome = new int[9];
    }

    public void Init()
    {
        System.Array.Clear(storedOutcome, 0, storedOutcome.Length);
    }

    public int[] Move(int[] state)
    {
        int[] outcome = (int[])state.Clone();

        // 所有能走的格子
        List<int> posAvailable = new List<int>();

        for (int i = 0; i < 9; ++i)
        {
            if (outcome[i] == 0)
            {
                posAvailable.Add(i);
            }
        }

        // 有一定的概率做随机选择
        if (Random.value < epsilon)
        {
            int pos = posAvailable[Random.Range(0, posAvailable.Count)];
            outcome[pos] = mark;
        }
        else
        {
            // 做最优选择
            double optimalValue = double.NegativeInfinity;

            foreach (int pos in posAvailable)
            {
                int[] outcomeCandidate = (int[])state.Clone();
                outcomeCandidate[pos] = mark;
                double value = valueMatrix[outcomeCandidate];

                if (value > optimalValue)
                {
                    outcome = outcomeCandidate;
                    optimalValue = value;
                }
            }
        }

        double error = valueMatrix[outcome] - valueMatrix[storedOutcome];
        valueMatrix[storedOutcome] += alpha * error;
        storedOutcome = (int[])outcome.Clone();

        return outcome;
    }
}

public class ValueMatrix
{
    double[] data;
    int[] shape;

    public ValueMatrix(int[] shape)
    {
        int totalSize = 1;

        foreach (int dim in shape)
        {
            totalSize *= dim;
        }

        data = new double[totalSize];
        this.shape = (int[])shape.Clone();
    }

    int Index(int[] outcome)
    {
        int index = 0;
        int stride = 1;

        for (int i = shape.Length - 1; i >= 0; --i)
        {
            index += outcome[i] * stride;
            stride *= shape[i];
        }

        return index;
    }

    public double this[int[] outcome]
    {
        get => data[Index(outcome)];

        set => data[Index(outcome)] = value;
    }
}