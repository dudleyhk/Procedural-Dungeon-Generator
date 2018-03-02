using System.Collections;


public class IntRange 
{
    public int MinRange { get; private set; }
    public int MaxRange { get; private set; }

    public IntRange(int min, int max)
    {
        MinRange = min;
        MaxRange = max;
    }

    public int Random
    {
        get { return UnityEngine.Random.Range(MinRange, MaxRange); }
    }
}
