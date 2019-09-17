using DarkRift.Client.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DarkRift.Client;

public class GameClientManager : Singleton<GameClientManager>
{
    public UnityClient client;

    void Start()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);

        client.MessageReceived += MessageReceived;

        Debug.Log("Attempting to connect to server...");
        client.ConnectInBackground(
            client.Address, 
            client.Port, 
            client.IPVersion, 
            e => 
            {
                if (e != null)
                {
                    Debug.LogError(e);
                    return;
                }

                Debug.Log("Connection successful.");
            });
    }

    private void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        SpawnGameObjects(sender, e);
    }

    private void SpawnGameObjects(object sender, MessageReceivedEventArgs e)
    {
        if (e.Tag == NetworkTags.InGame.SpawnObject)
        {
            SpawnMessage spawnMessage = e.GetMessage().Deserialize<SpawnMessage>();

            string resourcePath = NetworkObjectDictionary.GetResourcePath(spawnMessage.ResourceID);

            var ball = Instantiate(
                Resources.Load(resourcePath) as GameObject, 
                new Vector3(spawnMessage.X, spawnMessage.Y, 0), 
                Quaternion.identity)
                .GetComponent<NetworkObject>();
            ball.ID = spawnMessage.ID;
        }
    }
}
