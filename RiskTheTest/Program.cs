//using RiskTheTest.CommandHandlers;
//using RiskTheTest.Actions;
//using System.Data;
//using RiskTheTest.Models;

//namespace RiskTheTest
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            Console.WriteLine("Hello, World!");
//            Area a1 = new Area { Id = 1, Troops = 4 };
//            Area a2 = new Area { Id = 2, Troops = 1 };

//            var attCommand = new AttackAction { Attacker = a1, Defender = a2, AttackingTroops = a1.Troops-1, DefenderTroops = a2.Troops };

//            new AttackHandler().HandleCommand(attCommand);

//            Console.WriteLine($"New counts: Attacker has {a1.Troops} and defender has {a2.Troops}");

//            var placeCommand = new PlacementAction { Area = a1, Troops = 4 };

//            new PlacementHandler().HandleCommand(placeCommand);

//            Console.WriteLine($"Placed {placeCommand.Troops} on area {a1.Id}. New troop cound on area: {a1.Troops}");

//            var fortifyCommand = new FortifyAction { From = a1, To = a2, Amount = 1 };

//            new FortifyHandler().HandleCommand(fortifyCommand);

//            Console.WriteLine($"Fortified {fortifyCommand.Amount} from {fortifyCommand.From.Id} to {fortifyCommand.To.Id}");
//        }
//    }
//}
