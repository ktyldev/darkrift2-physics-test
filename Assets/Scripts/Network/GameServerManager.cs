using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DarkRift.Server;
using DarkRift.Server.Unity;
using DarkRift;

public class GameServerManager : Singleton<GameServerManager>
{
    public XmlUnityServer server;
    public IClientManager ClientManager => server.Server.ClientManager;

    private List<int> _clientIds = new List<int>();
    private List<NetworkObject> _networkObjects = new List<NetworkObject>();

    public int Tick { get; private set; } = -1;

    void Start()
    {
        server.Server.ClientManager.ClientConnected += ClientConnected;
        server.Server.ClientManager.ClientDisconnected += ClientDisconnected;

        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    private void FixedUpdate()
    {
        Tick++;
    }

    private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        Debug.Log("Client [" + e.Client.ID + "] disconnected");
        _clientIds.Remove(e.Client.ID);
    }

    private void ClientConnected(object sender, ClientConnectedEventArgs e)
    {
        _clientIds.Add(e.Client.ID);
        Debug.Log("Client [" + e.Client.ID + "] connected");
        SpawnAllObjectsOnClient(e.Client);
    }

    public void RegisterNetworkObject(NetworkObject networkObject)
    {
        _networkObjects.Add(networkObject);
        Debug.Log("Registered NetworkObject: " + networkObject.gameObject);
    }

    public void SpawnObjectOnClient(NetworkObject networkObject, IClient client)
    {
        SpawnMessage spawnMessage = new SpawnMessage
        {
            ID = networkObject.ID,
            ResourceID = networkObject.resourceId,
            X = networkObject.gameObject.transform.position.x,
            Y = networkObject.gameObject.transform.position.y
        };

        using (Message message = Message.Create(NetworkTags.InGame.SpawnObject, spawnMessage))
        {
            client.SendMessage(message, SendMode.Reliable);
        }
    }

    public void SpawnAllObjectsOnClient(IClient client)
    {
        foreach (NetworkObject networkObject in _networkObjects)
        {
            SpawnObjectOnClient(networkObject, client);
        }
    }
}
