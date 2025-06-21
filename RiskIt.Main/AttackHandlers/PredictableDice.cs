namespace RiskIt.Main.AttackHandlers
{
    public class PredictableDice : IDice
    {
        private LinkedList<int> _ints;
        private LinkedListNode<int> _current;

        public PredictableDice(IEnumerable<int> ints)
        {
            if (ints.Count() < 1) throw new Exception("Do exceptions later");

            _ints = new LinkedList<int>(ints);
            _current = _ints.First;
        }

        public int Next()
        {
            var ret = _current;

            _current = ret.Next;
            if (_current == null)
                _current = ret.List.First;

            return ret.Value;
        }
    }
}
