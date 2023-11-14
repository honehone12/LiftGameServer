using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Netcode;

namespace Lift
{
    public class ConnectionManager : NetworkBehaviour
    {
        [SerializeField]
        ConnectionSetting connectionSetting;

        bool hasGuid;
        System.Guid ownGuid;

        readonly Dictionary<System.Guid, Session> sessionMap = new();

        void Awake()
        {
            Assert.IsNotNull(connectionSetting);
        }

        public void InitializeServer(BootManager.BootEventParams param)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectinApproval;
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

            RequestOrInformGuidServerRpc(buff);
        }

        public void OnClientDisconnectedSelf(ulong connId)
        {
            ErrorManager.Singleton.Error(NetworkManager.DisconnectReason);

            BootManager.Singleton.Reboot();
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
        public void RequestOrInformGuidServerRpc(byte[] raw, ServerRpcParams param = default)
        {
            var connId = param.Receive.SenderClientId;
            if (raw.Length == 1 && raw[0] == 0)
            {
                ownGuid = System.Guid.NewGuid();
                hasGuid = true;
                sessionMap.Add(ownGuid, Session.NewActiveSession(connId));
                InformNewGuidClientRpc(ownGuid.ToByteArray());
                Debug.Log($"starting new sesiion guid: {ownGuid}, connId: {connId}");
                return;
            }

            if (raw.Length != 16)
            {
                return;
            }

            var guid = new System.Guid(raw);
            if (!sessionMap.ContainsKey(guid))
            {
                NetworkManager.Singleton.DisconnectClient(connId);
                Debug.LogWarning("disconnected clinet: unknown guid");
                return;
            }

            var sess = sessionMap[guid];
            sess.connectionId = connId;
            sess.isActive = true;
            sess.timeReconnected = UnixTime.Now;
            sessionMap[guid] = sess;
            ownGuid = guid;
            Debug.Log($"restarting session for guid: {ownGuid}, connId: {connId}");
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
