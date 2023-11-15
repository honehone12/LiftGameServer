using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using NativeWebSocket;
using Unity.Netcode;

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

        void Awake()
        {
            Assert.IsNotNull(urlBuilder);
            Assert.IsNotNull(monitorSetting);
            Assert.IsNotNull(connectionManager);
            urlBuilder.OnInitialAssertion();
            monitorSetting.OnInitialAssertion();
        }

        public void OnBootClient(BootManager.BootEventParams param)
        {
            Destroy(gameObject);
        }

        public async void OnBootServer(BootManager.BootEventParams param)
        {
            var rooIdx = 0;
            if (!urlBuilder.BuildWebSocketUrl(rooIdx, out var url))
            {
                ErrorManager.Singleton.Error("invalid route index");
                return;
            }

            webSocket = new WebSocket(url);
            webSocket.OnOpen += () => Debug.Log("websocket opened");
            webSocket.OnClose += (code) => Debug.Log($"websocket closed with code: {code}");
            webSocket.OnError += (err) => ErrorManager.Singleton.Error(err);
            webSocket.OnMessage += (raw) => OnWebSocketMessage(raw);
            await webSocket.Connect();

            _ = StartCoroutine(WebSocketUpdateLoop());
            _ = StartCoroutine(MonitoringLoop());
        }

        void OnWebSocketMessage(byte[] raw)
        {

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

        IEnumerator MonitoringLoop()
        {
            var ticker = new WaitForSecondsRealtime(monitorSetting.MonitorInterval);
            while (true)
            {
                yield return ticker;

                
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
