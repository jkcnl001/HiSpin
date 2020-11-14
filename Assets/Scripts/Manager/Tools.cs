using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{

}
public class Range
{
    private int min;
    private int max;
    public int Min { get { return min; } }
    public int Max { get { return max; } }
    public Range(int min,int max)
    {
        this.min = min;
        this.max = max;
    }
    public int RandomIncludeMax()
    {
        return Random.Range(min, max + 1);
    }
    public int RandomExculdeMax()
    {
        return Random.Range(min, max);
    }
}
