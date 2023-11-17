using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Lift.Command
{
    [CreateAssetMenu(menuName = "Lift/CommandSetting", fileName = "CommandSetting")]
    public class CommandSetting : ScriptableObject
    {
        public enum BuildModeKind
        {
            Release,
            Debug,
            Developement
        }

        [SerializeField]
        BuildModeKind buildMode;
        [SerializeField]
        List<string> requiredOptionList = new();

        public BuildModeKind BuildMode => buildMode;

        public ReadOnlyCollection<string> RequiredOptions => requiredOptionList.AsReadOnly();
    }
}
