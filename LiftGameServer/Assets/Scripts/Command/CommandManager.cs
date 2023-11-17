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

            var required = commandSetting.RequiredOptions;
            for (int i = 0, length = required.Count; i < length; i++)
            {
                var op = required[i];
                var err = $"command line option {op} does not found";
                if (!commandList.ContainsOption(op))
                {
                    switch (commandSetting.BuildMode)
                    {
                        case CommandSetting.BuildModeKind.Debug:
                            throw new System.Exception(err);
                        case CommandSetting.BuildModeKind.Developement:
                            Debug.LogWarning(err);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
