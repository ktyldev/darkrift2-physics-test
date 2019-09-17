using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBallSyncMessage : IDarkRiftSerializable
{
    public int ID { get; set; }
    public int ServerTick { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }

    public void Deserialize(DeserializeEvent e)
    {
        ID = e.Reader.ReadInt32();
        ServerTick = e.Reader.ReadInt32();
        Position = e.Reader.ReadVector3();
        Velocity = e.Reader.ReadVector3();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ID);
        e.Writer.Write(ServerTick);
        e.Writer.Write(Position);
        e.Writer.Write(Velocity);
    }
}
