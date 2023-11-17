using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Unity.Netcode;
using NativeWebSocket;
using Lift.Command;

namespace Lift
{
    public class MonitorClient : MonoBehaviour
    {
        [SerializeField]
        UrlBuilder urlBuilder;
        [SerializeField]
        MonitorSetting monitorSetting;
        [SerializeField]
        ConnectionManager connectionManager;

        WebSocket webSocket;
        System.Guid serverGuid;
        
        void Awake()
        {
            Assert.IsNotNull(urlBuilder);
            Assert.IsNotNull(monitorSetting);
            Assert.IsNotNull(connectionManager);
            urlBuilder.OnInitialAssertion();
            monitorSetting.OnInitialAssertion();
        }

        public void InitializeClient(BootManager.BootEventParams param)
        {
            if (param.bootSetting.BootMode == BootSetting.BootModeKind.Client)
            {
                Destroy(gameObject);
            }
        }

        public async void InitializeServer(BootManager.BootEventParams param)
        {
            if (CommandManager.Singleton.CommandLineArgs.OptionValue("-u", out var idStr))
            {
                if (!System.Guid.TryParse(idStr, out var serverGuid))
                {
                    ErrorManager.Singleton.Error("failed to parse guid");
                }
                Debug.Log($"server guid is set as {serverGuid}");
            }

            var connRouteIdx = 0;
            if (!urlBuilder.BuildWebSocketUrl(connRouteIdx, out var url))
            {
                ErrorManager.Singleton.Error("invalid route index");
                return;
            }

            webSocket = new WebSocket(url);
            webSocket.OnOpen += () => Debug.Log("websocket opened");
            webSocket.OnClose += (code) => Debug.Log($"websocket closed with code: {code}");
            webSocket.OnError += (err) => ErrorManager.Singleton.Error(err);
            webSocket.OnMessage += (raw) => OnWebSocketMessage(raw);

            if (monitorSetting.DisableWebSocket)
            {
                _ = StartCoroutine(MonitoringLoop(() =>
                {
                    Debug.Log(
                        $"connection count: {NetworkManager.Singleton.ConnectedClientsIds.Count}, " +
                        $"session count: {connectionManager.SessionCount}, " +
                        $"active session count: {connectionManager.ActiveSessionCount}"
                    );
                }));
            }
            else
            {
                await webSocket.Connect();
                _ = StartCoroutine(WebSocketUpdateLoop());
                _ = StartCoroutine(MonitoringLoop(async () =>
                {
                    var msg = new MonitoringMessage(
                        serverGuid.ToByteArray(),
                        NetworkManager.Singleton.ConnectedClientsIds.Count,
                        connectionManager.SessionCount,
                        connectionManager.ActiveSessionCount
                    );
                    await webSocket.SendText(JsonUtility.ToJson(msg));
                }));
            }
        }

        void OnWebSocketMessage(byte[] raw)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator WebSocketUpdateLoop()
        {
            var ticker = new WaitForSecondsRealtime(monitorSetting.WSUpdateInterval);
            while (true)
            {
                yield return ticker;

                webSocket.DispatchMessageQueue();
            }
        }

        IEnumerator MonitoringLoop(UnityAction action)
        {
            var ticker = new WaitForSecondsRealtime(monitorSetting.MonitorInterval);
            while (true)
            {
                yield return ticker;

                action?.Invoke();
            }
        }

        async void OnDestroy()
        {
            if (webSocket != null)
            {
                await webSocket.Close();
            }
        }
    }
}
