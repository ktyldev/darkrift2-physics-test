using DarkRift;
using DarkRift.Client;
using DarkRift.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkBouncyBall : NetworkObject
{

    private Rigidbody _rb;
    private int _clientTick = -1;
    private BouncyBallSyncMessage _lastMessage = null;
    private List<BouncyBallSyncMessage> _reconciliationInfo;

    protected override void Start()
    {
        base.Start();

        _rb = GetComponent<Rigidbody>();

        if (GameClientManager.IsInitialised)
        {
            GameClientManager.Instance.client.MessageReceived += UpdateFromServer;
            _reconciliationInfo = new List<BouncyBallSyncMessage>();
        }
    }

    private void UpdateFromServer(object sender, DarkRift.Client.MessageReceivedEventArgs e)
    {
        if (e.Tag == NetworkTags.InGame.BouncyBallSyncPos)
        {
            // Get message data
            var syncMessage = e.GetMessage().Deserialize<BouncyBallSyncMessage>();

            // First time receiving message
            if (_lastMessage == null)
            {
                _rb.velocity = syncMessage.Velocity;
                _rb.transform.position = syncMessage.Position;
                _clientTick = syncMessage.ServerTick;
                _lastMessage = syncMessage;
            }

            if (ID == syncMessage.ID && syncMessage.ServerTick > _lastMessage.ServerTick)
            {
                _lastMessage = syncMessage;
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameServerManager.IsInitialised)
        {
            if (GameServerManager.Instance.Tick % 10 == 0)
            {
                SendPositionToClients();
            }
        }
        else if (GameClientManager.IsInitialised && _clientTick != -1)
        {
            _clientTick++;
            _reconciliationInfo.Add(new BouncyBallSyncMessage
            {
                Position = transform.position,
                ServerTick = _clientTick,
                Velocity = _rb.velocity
            });

            Reconcile();
        }
    }

    private void Reconcile()
    {
        float threshold = 0.05f;
        float maxFrames = 50;

        if (_reconciliationInfo.Count > 0)
        {
            BouncyBallSyncMessage clientInfo = null;
            for (int i = 0; i < _reconciliationInfo.Count; i++)
            {
                if (_reconciliationInfo[i].ServerTick == _lastMessage.ServerTick)
                {
                    clientInfo = _reconciliationInfo[i];
                }
            }

            // If 50 ticks have passed, reconcile anyway
            if (_reconciliationInfo.Count > maxFrames)
            {
                _rb.velocity = _lastMessage.Velocity;
                _rb.transform.position = _lastMessage.Position;
                _clientTick = _lastMessage.ServerTick;
                clientInfo = _lastMessage;
            }

            if (clientInfo != null)
            {
                if (Vector3.Distance(clientInfo.Position, _lastMessage.Position) >= threshold)
                {
                    _rb.velocity = _lastMessage.Velocity;
                    _rb.transform.position = _lastMessage.Position;
                }

                _reconciliationInfo.Clear();
            }
        }
    }

    private void SendPositionToClients()
    {
        BouncyBallSyncMessage syncMessage = new BouncyBallSyncMessage
        {
            ID = ID,
            ServerTick = GameServerManager.Instance.Tick,
            Position = _rb.transform.position,
            Velocity = _rb.velocity
        };

        using (Message m = Message.Create(NetworkTags.InGame.BouncyBallSyncPos, syncMessage))
        {
            foreach (IClient client in GameServerManager.Instance.ClientManager.GetAllClients())
            {
                // Does this really need to be reliable?
                client.SendMessage(m, SendMode.Reliable);
            }
        }
    }

    private void OnDestroy()
    {
        if (GameClientManager.IsInitialised)
        {
            GameClientManager.Instance.client.MessageReceived -= UpdateFromServer;
        }
    }
}
