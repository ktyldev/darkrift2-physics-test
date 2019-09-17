using DarkRift;

public class SpawnMessage : IDarkRiftSerializable
{
    public int ID { get; set; }
    public int ResourceID { get; set; }
    public float X { get; set; }
    public float Y { get; set; }

    public void Deserialize(DeserializeEvent e)
    {
        ID = e.Reader.ReadInt32();
        ResourceID = e.Reader.ReadInt32();
        X = e.Reader.ReadSingle();
        Y = e.Reader.ReadSingle();
    }

    public void Serialize(SerializeEvent e)
    {
        e.Writer.Write(ID);
        e.Writer.Write(ResourceID);
        e.Writer.Write(X);
        e.Writer.Write(Y);
    }
}
