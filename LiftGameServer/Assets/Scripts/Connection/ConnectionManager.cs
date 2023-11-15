using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;
using System.Linq;

namespace Lift
{
    public class ConnectionManager : NetworkBehaviour
    {
        [SerializeField]
        ConnectionSetting connectionSetting;

        bool hasGuid;
        System.Guid ownGuid;

        readonly Dictionary<System.Guid, Session> sessionMap = new();
        readonly Dictionary<ulong, System.Guid> idMap = new();

        public int SessionCount => sessionMap.Count;

        public int ActiveSessionCount => sessionMap.Count((kv) => kv.Value.isActive);

        void Awake()
        {
            Assert.IsNotNull(connectionSetting);
        }

        public void InitializeServer(BootManager.BootEventParams param)
        {
            var nm = NetworkManager.Singleton;
            nm.ConnectionApprovalCallback += OnConnectinApproval;
            nm.OnClientConnectedCallback += OnClientConnectedServer;
            nm.OnClientDisconnectCallback += OnClientDisconnectedServer;
        }

        public void InitializeClient(BootManager.BootEventParams param)
        {
            var nm = NetworkManager.Singleton;
            nm.OnClientConnectedCallback += OnClientConnectedSelf;
            nm.OnClientDisconnectCallback += OnClientDisconnectedSelf;
        }

        public void OnClientConnectedSelf(ulong connId)
        {
            byte[] buff = hasGuid switch
            {
                true => ownGuid.ToByteArray(),
                false => new byte[] { 0 },
            };

            RequestSessionServerRpc(buff);
        }

        public void OnClientDisconnectedSelf(ulong connId)
        {
            ErrorManager.Singleton.Error(NetworkManager.DisconnectReason);
        }

        public void OnClientConnectedServer(ulong connId)
        {
            Debug.Log($"new client connection id: {connId}");
            idMap.Add(connId, System.Guid.Empty);
            Assert.IsTrue(NetworkManager.Singleton.ConnectedClientsIds.Count == idMap.Count);
        }

        public void OnClientDisconnectedServer(ulong connId)
        {
            Debug.Log($"client disconnected id: {connId}");
            var guid = idMap[connId];
            idMap.Remove(connId);
            Assert.IsTrue(NetworkManager.Singleton.ConnectedClientsIds.Count == idMap.Count);

            if (guid == System.Guid.Empty || !sessionMap.ContainsKey(guid))
            {
                return;
            }

            var sess = sessionMap[guid];
            sess.isActive = false;
            sess.timeDisconnected = UnixTime.Now;
        }

        public void OnConnectinApproval(
            NetworkManager.ConnectionApprovalRequest req,
            NetworkManager.ConnectionApprovalResponse res)
        {
            // clientId is just a index starts from '1'
            // they are unique for this game server instance
            // including disapproved clients
            // if connection is like OOXXO the last id is 5 not 3
            var connected = NetworkManager.Singleton.ConnectedClientsIds;
            var len = connected.Count;
            Debug.Log($"current connected clients: '{len}'");
            if (len+1 > connectionSetting.MaxConnection)
            {
                var reason = "exceed max connections";
                Debug.LogWarning("connection disapproved: " + reason);
                res.Reason = reason;
                res.Approved = false;
                return;
            }

            res.CreatePlayerObject = true;
            res.Approved = true;
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        public void RequestSessionServerRpc(byte[] raw, ServerRpcParams param = default)
        {
            var connId = param.Receive.SenderClientId;
            if (raw.Length == 1 && raw[0] == 0)
            {
                StartNewSession(connId);
                return;
            }

            if (raw.Length != 16)
            {
                NetworkManager.Singleton.DisconnectClient(connId);
                Debug.LogWarning("disconnected clinet: invalid guid");
                return;
            }

            var guid = new System.Guid(raw);
            if (!sessionMap.ContainsKey(guid))
            {
                NetworkManager.Singleton.DisconnectClient(connId);
                Debug.LogWarning("disconnected clinet: unknown guid");
                return;
            }

            RestartSession(guid, connId);
        }

        void StartNewSession(ulong connId)
        {
            var guid = System.Guid.NewGuid();
            idMap[connId] = guid;
            sessionMap.Add(guid, Session.NewActiveSession(connId));
            InformNewGuidClientRpc(guid.ToByteArray());
            Debug.Log($"starting new sesiion guid: {guid}, connId: {connId}");
        }

        void RestartSession(System.Guid guid, ulong newConnId)
        {
            idMap[newConnId] = guid;
            var sess = sessionMap[guid];
            sess.connectionId = newConnId;
            sess.isActive = true;
            sess.timeReconnected = UnixTime.Now;
            Debug.Log($"restarting session for guid: {guid}, connId: {newConnId}");
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void InformNewGuidClientRpc(byte[] raw, ClientRpcParams param = default)
        {
            if (raw.Length != 16)
            {
                return;
            }

            ownGuid = new System.Guid(raw);
            hasGuid = true;
        }
    }
}
