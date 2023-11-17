using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Lift.Dummy
{
    public class RandomDisconnector : MonoBehaviour
    {
        [SerializeField]
        float interval;

        void Start()
        {
            switch (BootManager.Singleton.Setting.BootMode)
            {
                case BootSetting.BootModeKind.Server:
                    _ = StartCoroutine(DisconnectorLoop());
                    break;
                default:
                    gameObject.SetActive(false);
                    break;
            }
        }

        IEnumerator DisconnectorLoop()
        {
            var ticker = new WaitForSecondsRealtime(interval);
            while (true)
            {
                yield return ticker;

                var nm = NetworkManager.Singleton;
                var clientIds = nm.ConnectedClientsIds;
                var length = clientIds.Count;
                if (length > 0)
                {
                    var idx = Random.Range(0, length);
                    var id = clientIds[idx];
                    nm.DisconnectClient(id);
                }
            }
        }
    }
}
