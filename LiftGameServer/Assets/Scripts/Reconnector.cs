using UnityEngine;
using UnityEngine.UI;

namespace Lift.Dummy
{
    public class Reconnector : MonoBehaviour
    {
        [SerializeField]
        Button reconnectButton;

        void Start()
        {
            switch (BootManager.Singleton.Setting.BootMode)
            {
                case BootSetting.BootModeKind.Client:
                    reconnectButton.interactable = false;
                    break;
                default:
                    gameObject.SetActive(false);
                    break;
            }
        }

        public void OnDisconnect()
        {
            reconnectButton.interactable = true;
        }

        public void Recconect()
        {
            reconnectButton.interactable = false;
            BootManager.Singleton.Reboot();
        }
    }
}
