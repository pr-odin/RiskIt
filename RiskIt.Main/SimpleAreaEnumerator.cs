using RiskIt.Main.Models;
using System.Collections;

namespace RiskIt.Main
{
    public class SimpleAreaEnumerator<T> where T : IComparable<T>
    {
        private IList<Area<T>> _areas;
        private IEnumerator<Area<T>> _emptyAreas;
        private bool _hasEmpty;

        private Area<T> _current;
        private int _index;

        public SimpleAreaEnumerator(ICollection<Area<T>> areas)
        {
            _areas = areas.ToList();

            var emptyAreas = areas.Where(area => area.Player is null);

            _emptyAreas = emptyAreas.GetEnumerator();
            _hasEmpty = emptyAreas.FirstOrDefault() != null;

        }

        public Area<T> Current => _current;

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


        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
