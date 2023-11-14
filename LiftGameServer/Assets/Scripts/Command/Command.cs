namespace Lift.Command
{
    public class Command
    {
        public string Option { get; private set; }
        public string Value { get; private set; }

        public Command(string option, string value)
        {
            if (!option.StartsWith('-'))
            {
                throw new System.Exception("unexpected command format");
            }

            Option = option;
            Value = value;
        }
    }
}
