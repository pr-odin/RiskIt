namespace RiskIt.ConsoleGame
{
    public class FileHandler
    {
        public static void WriteToFile(string path, string content)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(content);
            }
        }

        public static string ReadFromFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}
