using UnityEngine;

namespace Lift
{
    [CreateAssetMenu(menuName = "Lift/BootSetting", fileName = "BootSetting")]
    public class BootSetting : ScriptableObject
    {
        public enum BootModeKind
        {
            Host,
            Client,
            Server
        }

        [SerializeField]
        BootModeKind bootMode;

        public BootModeKind BootMode => bootMode;
    }
}
