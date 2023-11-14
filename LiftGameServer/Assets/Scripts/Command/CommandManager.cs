using UnityEngine;
using UnityEngine.Assertions;

namespace Lift.Command
{
    public class CommandManager : MonoBehaviour
    {
        public static CommandManager Singleton { get; private set; }

        [SerializeField]
        CommandSetting commandSetting;

        readonly CommandList commandList = new();

        public CommandList CommandLineArgs => commandList;

        void Awake()
        {
            Assert.IsNotNull(commandSetting);

            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            commandList.CollectCommandLineArgs();

            if (commandSetting.BuildMode == CommandSetting.BuildModeKind.Debug)
            {
                var list = commandList.List;
                
                for (int i = 0, length = list.Count; i < length; i++)
                {
                    var cmd = list[i];
                    Debug.Log($"command line arg {cmd.Option} {cmd.Value}");
                }
            }
        }
    }
}
