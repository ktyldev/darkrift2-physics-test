using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkExtensions 
{
    public static Vector3 ReadVector3(this DarkRiftReader reader) 
        => new Vector3(
            reader.ReadSingle(), 
            reader.ReadSingle(), 
            reader.ReadSingle());

    public static void Write(this DarkRiftWriter writer, Vector3 v)
    {
        writer.Write(v.x);
        writer.Write(v.y);
        writer.Write(v.z);
    }
}

