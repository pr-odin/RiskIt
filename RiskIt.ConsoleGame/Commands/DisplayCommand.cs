using System.Text;

namespace RiskIt.ConsoleGame.Commands
{
    public class DisplayCommand : ICommand
    {
        public DisplayCommandType DisplayCommandType;
        public string? Text;

        public static DisplayCommand CreateUnknownCommand()
        {
            var comm = new DisplayCommand { DisplayCommandType = DisplayCommandType.Unknown };
            comm.SetUnknownText();
            return comm;
        }

        public void Parse(string[] args)
        {
            switch (args[0])
            {
                case "?":
                    DisplayCommandType = DisplayCommandType.Help;
                    SetHelpText();
                    break;
                case "map":
                    DisplayCommandType = DisplayCommandType.Map;
                    break;
                case "replays":
                    DisplayCommandType = DisplayCommandType.Replays;
                    break;
                default:
                    // TODO: This is actually unreachable. Due to.. 
                    // technical difficulties... yeah...
                    DisplayCommandType = DisplayCommandType.Unknown;
                    SetUnknownText();
                    break;
            }
        }

        // TODO: Make help cover all cases by including into compiler check
        // somehow... probably not prio 0. Or 1.. or 10
        private void SetHelpText()
        {
            StringBuilder sb = new StringBuilder();

            Text =
                @"
Help text:

----------------------------DISPLAY ACTIONS---------------------------
Prefix 'disp'                   - when using a display command, prefix
                                - it with 'disp'. A valid command is
                                - 'disp map' whereas 'map' is not valid

'map'                      - displays current map
'replays'                       - displays available replay files
                                - can be used to feed 'startreplay'

----------------------------SERVER ACTIONS----------------------------
Prefix 'server'                 - when using a server command, prefix
                                - it with 'server'. A valid command is
                                - 'server startgame' whereas 
                                - 'startgame' is not valid

'startgame'                     - starts a new game
'endgame'                       - ends current game
'startreplay [filename]'        - start the replay with the name 
                                - [filename]. Remember the extension
                                - Can be advantageously used with 
                                - 'disp replays'

-----------------------------GAME ACTIONS-----------------------------
Placement phase:
[Area] [Troops]                 - Place [Troops] on [Area]
                                - '0 4' places 4 troops on 
                                - area 0

Attack phase:
[AreaFrom] [Troops] [AreaTo]    - Attacks [AreaTo] with [Troops]
                                - from [AreaFrom]
                                - '2 9 1' attacks from area 2 to 
                                - area 1 with 9 troops

Fortify phase:
[AreaFrom] [Troops] [AreaTo]    - Transfers [Troops] from [AreaFrom]
                                - to [AreaTo]
                                - '2 0 5' transfers 5 troops from 2
                                - to 0
Finishing turn:
s OR skip                       - Skips the current phase when in
                                - attack or fortify phase

-----------------------------REPLAY ACTIONS----------------------------
Next action taken:
n OR next                       - Plays the next action in the game
";
        }

        private void SetUnknownText()
        {
            Text = "You have entered an unknown command. Try ? for help";
        }
    }
    public enum DisplayCommandType
    {
        Unknown,
        Help,
        Map,
        Display,
        Replays,
    }
}
