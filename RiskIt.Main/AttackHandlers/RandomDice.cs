namespace RiskIt.Main.AttackHandlers
{
    public class RandomDice : IDice
    {
        private Random _random;

        public RandomDice(int seed)
        {
            _random = new Random(seed);
        }

        public int Next()
        {
            return _random.Next(1, 7);
        }
    }
}
