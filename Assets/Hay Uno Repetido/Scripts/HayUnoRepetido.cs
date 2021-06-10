using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayUnoRepetido : MonoBehaviour
{
    private int mistakes;
    private int successes;
    private float[] timeBetweenSuccesses;
    private float totalTime;

    public HayUnoRepetido(int mistakes, int successes, float[] timeBetweenSuccesses, float totalTime)
    {
        this.mistakes = mistakes;
        this.successes = successes;
        this.timeBetweenSuccesses = timeBetweenSuccesses;
        this.totalTime = totalTime;
    }

    public int Mistakes { get => mistakes; set => mistakes = value; }
    public int Successes { get => successes; set => successes = value; }
    public float[] TimeBetweenSuccesses { get => timeBetweenSuccesses; set => timeBetweenSuccesses = value; }
    public float TotalTime { get => totalTime; set => totalTime = value; }
}
