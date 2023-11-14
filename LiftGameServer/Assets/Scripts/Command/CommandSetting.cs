using UnityEngine;

namespace Lift.Command
{
    [CreateAssetMenu(menuName = "Lift/CommandSetting", fileName = "CommandSetting")]
    public class CommandSetting : ScriptableObject
    {
        public enum BuildModeKind
        {
            Release,
            Debug
        }

        [SerializeField]
        BuildModeKind buildMode;

        public BuildModeKind BuildMode => buildMode;
    }
}
