using UnityEngine;

public static class JsonHelper
{
    public static string FixJson(string value)
    {
        return "{\"entries\":" + value + "}";
    }
}