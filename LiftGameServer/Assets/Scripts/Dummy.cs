using UnityEngine;
using Unity.Netcode;

namespace Lift.Dummy
{
    public class Dummy : NetworkBehaviour
    {
        
        public override void OnNetworkSpawn()
        {
            if (IsClient && IsOwner)
            {
                HelloServerRpc();
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = true)]
        public void HelloServerRpc(ServerRpcParams param = default)
        {
            Debug.Log($"hello, i am '{param.Receive.SenderClientId}'");
        }
    }
}
