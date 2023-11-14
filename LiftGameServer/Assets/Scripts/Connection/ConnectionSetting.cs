using UnityEngine;

namespace Lift
{
    [CreateAssetMenu(menuName = "Lift/ConnectionSetting", fileName = "ConnectionSetting")]
    public class ConnectionSetting : ScriptableObject
    {
        [SerializeField]
        uint maxConnection;

        public uint MaxConnection => maxConnection;
    }
}