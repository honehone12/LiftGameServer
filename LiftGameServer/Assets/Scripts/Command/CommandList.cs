using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lift.Command
{
    public class CommandList
    {
        readonly List<Command> commandList = new();

        public ReadOnlyCollection<Command> List => commandList.AsReadOnly();

        public void CollectCommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs();
            // arg[0] should be name of the program
            for (int i = 1, length = args.Length; i < length; i++)
            {
                if (args[i].StartsWith('-'))
                {
                    var option = args[i].ToLower();
                    var value = string.Empty;
                    if (i < length-1 && !args[i + 1].StartsWith('-'))
                    {
                        value = args[i + 1].ToLower();
                        i++;
                    }
                    commandList.Add(new Command(option, value));
                }
            }
        }

        public bool ContainsOption(string option)
        {
            return commandList.Exists((cmd) => cmd.Option == option);
        }

        public bool OptionValue(string option, out string value)
        {
            var cmd = commandList.Find((cmd) => cmd.Option == option);
            if (cmd == null)
            {
                value = string.Empty;
                return false;
            }

            value = cmd.Value;
            return true;
        }
    }
}
