using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [Header("符号")]
    [Tooltip("符号的int表示")]
    public int mark;
    [Tooltip("符号对应的GameObject")]
    public GameObject markObj;

    [Header("随机选择概率")]
    [SerializeField] double epsilon = 0.1;

    [Header("学习率")]
    [SerializeField] double alpha = 0.1;

    public ValueMatrix valueMatrix;
    int[] storedOutcome;
    OutcomeCandidateGen outcomeCandidateGen;

    // Start is called before the first frame update
    void Awake() => EventBus.Subscribe<TrainingCompletedEvent>(OnTrainingCompleted);

    private void OnDestroy() => EventBus.Unsubscribe<TrainingCompletedEvent>(OnTrainingCompleted);

    public void Init(int[] valueMatrixShape)
    {
        valueMatrix = new ValueMatrix(valueMatrixShape);
        outcomeCandidateGen = GetComponent<OutcomeCandidateGen>();
    }

    public void Clear() => storedOutcome = null;

    public int[] Move(int[] state, out int pos)
    {
        int[] outcome = (int[])state.Clone();

        // 所有可能的结果
        List<(int[], int)> outcomeCandidates = outcomeCandidateGen.Apply(state, mark);

        // 有一定的概率做随机选择
        if (Random.value < epsilon)
        {
            (outcome, pos) = outcomeCandidates[Random.Range(0, outcomeCandidates.Count)];
        }
        else
        {
            // 做最优选择
            (outcome, pos) = outcomeCandidates[0];
            double optimalValue = valueMatrix[outcome];

            foreach ((int[] outcomeCandidate, int posCandidate) in outcomeCandidates)
            {
                double value = valueMatrix[outcomeCandidate];

                if (value > optimalValue)
                {
                    (outcome, pos) = (outcomeCandidate, posCandidate);
                    optimalValue = value;
                }
            }
        }

        if (storedOutcome != null)
        {
            double error = valueMatrix[outcome] - valueMatrix[storedOutcome];
            valueMatrix[storedOutcome] += alpha * error;
        }

        storedOutcome = (int[])outcome.Clone();

        return outcome;
    }

    public int[] Move(int[] state) => Move(state, out _);

    void OnTrainingCompleted(TrainingCompletedEvent _)
    {
        epsilon = 0;
        Clear();
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