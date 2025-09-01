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
            // Open the text file using a stream reader.
            using StreamReader reader = new(path);

            // Read the stream as a string.
            return reader.ReadToEnd();
        }
    }
}
