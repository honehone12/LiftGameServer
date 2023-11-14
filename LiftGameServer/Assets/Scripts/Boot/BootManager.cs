using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Unity.Netcode;
using Lift.Command;
using Unity.Netcode.Transports.UTP;

namespace Lift
{
    public class BootManager : MonoBehaviour
    {
        public class BootEventParams
        {
            public BootSetting bootSetting;

            public BootEventParams(BootSetting bootSetting)
            {
                this.bootSetting = bootSetting;
            }
        }

        public static BootManager Singleton { get; private set; }

        [SerializeField]
        BootSetting bootSetting;
        [Space]
        public UnityEvent<BootEventParams> OnClientInitialize = new();
        public UnityEvent<BootEventParams> OnServerInitialize = new();
        public UnityEvent<BootEventParams> OnClientBoot = new();
        public UnityEvent<BootEventParams> OnServerBoot = new();

        void Awake()
        {
            Assert.IsNotNull(bootSetting);
        
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            var param = new BootEventParams(bootSetting);
            var nm = NetworkManager.Singleton;
            var commands = CommandManager.Singleton.CommandLineArgs;
            if (commands.OptionValue("-p", out var p) && ushort.TryParse(p, out var port))
            {
                if (commands.OptionValue("-a", out var address))
                {
                    nm.GetComponent<UnityTransport>().SetConnectionData(address, port);
                    Debug.Log($"transport url is set as {address}:{port}");
                }
            }

            switch (bootSetting.BootMode)
            {
                case BootSetting.BootModeKind.Host:
                    OnServerInitialize.Invoke(param);
                    OnClientInitialize.Invoke(param);
                    nm.StartHost();
                    OnServerBoot.Invoke(param);
                    OnClientBoot.Invoke(param);
                    Debug.Log("booted as 'host'");
                    break;
                case BootSetting.BootModeKind.Client:
                    OnClientInitialize.Invoke(param);
                    nm.StartClient();
                    OnClientBoot.Invoke(param);
                    Debug.Log("booted as 'client'");
                    break;
                case BootSetting.BootModeKind.Server:
                    OnServerInitialize.Invoke(param);
                    nm.StartServer();
                    OnServerBoot.Invoke(param);
                    Debug.Log("booted as 'server'");
                    break;
                default:
                    ErrorManager.Singleton.Error("unexpected boot mode");
                    break;
            }
        }
        
        public void Reboot()
        {
            var param = new BootEventParams(bootSetting);

            switch (bootSetting.BootMode)
            {
                case BootSetting.BootModeKind.Host:
                    ErrorManager.Singleton.Exception(new System.NotImplementedException());
                    break;
                case BootSetting.BootModeKind.Client:
                    NetworkManager.Singleton.StartClient();
                    OnClientBoot.Invoke(param);
                    Debug.Log("rebooted as 'client'");
                    break;
                case BootSetting.BootModeKind.Server:
                    ErrorManager.Singleton.Exception(new System.NotImplementedException());
                    break;
                default:
                    ErrorManager.Singleton.Error("unexpected boot mode");
                    break;
            }
        }
    }
}
