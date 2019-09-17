using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkObjectDictionary
{
    private static readonly Dictionary<int, string> _dict =
        new Dictionary<int, string>
        {
            { 1, "BouncyBall" }
        };

    public static string GetResourcePath(int id)
        => _dict.ContainsKey(id) 
            ? _dict[id] 
            : null;

    public static bool HasResource(int id)
        => _dict.ContainsKey(id);
}
