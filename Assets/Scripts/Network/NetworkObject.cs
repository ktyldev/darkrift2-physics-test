using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkObject : MonoBehaviour
{
    public int resourceId;

    public int ID { get; set; }

    protected virtual void Start()
    {
        // If we are on the client and the ID is not set, destroy the object
        if (!GameServerManager.IsInitialised && ID == 0)
        {
            Destroy(gameObject);
        }
        else if (GameServerManager.IsInitialised)
        {
            ID = GetInstanceID();

            GameServerManager.Instance.RegisterNetworkObject(this);

            if (!NetworkObjectDictionary.HasResource(resourceId))
                throw new System.Exception("No stored resource for ID: " + resourceId);
        }
    }
}
