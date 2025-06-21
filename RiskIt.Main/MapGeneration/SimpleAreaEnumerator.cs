using RiskIt.Main.Models;

namespace RiskIt.Main.MapGeneration
{
    public class SimpleAreaEnumerator<T> : IAreaEnumerator<T> where T : IComparable<T>
    {
        private IList<Area<T>> _areas;
        private IEnumerator<Area<T>> _emptyAreas;
        private bool _hasEmpty;

        private int _index;

        public SimpleAreaEnumerator(ICollection<Area<T>> areas)
        {
            _areas = areas.ToList();

            var emptyAreas = areas.Where(area => area.Player is null);

            _emptyAreas = emptyAreas.GetEnumerator();
            _hasEmpty = emptyAreas.FirstOrDefault() != null;

        }

        private Area<T>? NextEmpty()
        {
            var ret = _emptyAreas.MoveNext();

            if (!ret)
            {
                _hasEmpty = false;
                return null;
            }

            return _emptyAreas.Current;
        }

        public Area<T> Next(Player player)
        {
            if (_hasEmpty)
            {
                var nextEmpty = NextEmpty();
                if (nextEmpty is not null)
                    return nextEmpty;

                _hasEmpty = false;
            }

            Area<T> area;
            do
            {
                area = _areas[_index];
                _index++;

                // circular
                if (_index == _areas.Count) _index = 0;

            } while (!area.Player.Equals(player));

            return area;
        }
    }
}
