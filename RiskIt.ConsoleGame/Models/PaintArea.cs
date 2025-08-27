using RiskIt.Main.Models;

namespace RiskIt.ConsoleGame.Models
{
    public class PaintArea
    {
        public ConsoleColor Background;
        public ConsoleColor Foreground;
        public char Char;
        public int Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }
        private int _length;

        public string Word
        {
            get
            {
                if (_word is null)
                    return string.Join("", Enumerable.Repeat(Char, Length));
                else
                    return _word;
            }
            set
            {
                _word = value;
                _length = _word.Length;
            }
        }
        private string? _word;

        public Area<string>? Area;

        public override string ToString()
        {
            return $"{Char} ({Length}) - {Foreground}";
        }
    }
}
