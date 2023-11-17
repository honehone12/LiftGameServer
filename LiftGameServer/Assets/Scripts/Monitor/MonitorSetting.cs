using UnityEngine;
using UnityEngine.Assertions;

namespace Lift
{
    [CreateAssetMenu(menuName = "Lift/MonitorSetting", fileName = "MonitorSetting")]
    public class MonitorSetting : ScriptableObject
    {
        [SerializeField]
        bool disableWebSocket;
        [SerializeField]
        float websocketUpdateInterval;
        [SerializeField]
        float monitorInterval;

        public bool DisableWebSocket => disableWebSocket;

        public float WSUpdateInterval => websocketUpdateInterval;

        public float MonitorInterval => monitorInterval;

        public void OnInitialAssertion()
        {
            Assert.IsFalse(websocketUpdateInterval < 0.0f);
            Assert.IsFalse(monitorInterval < 0.0f);
            if (disableWebSocket)
            {
                Debug.LogWarning("websocket is disabled");
            }
        }
    }
}
