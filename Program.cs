using System.Diagnostics;

namespace Multi_threaded_File_Processing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The number of processors " +
                "on this computer is {0}.",
                Environment.ProcessorCount);

            try
            {
                FileAnalyzer analyzer = new FileAnalyzer();

                Thread countFiles = new Thread(analyzer.CountFiles);
                Thread countLines = new Thread(analyzer.CountLines);
                Thread countWords = new Thread(analyzer.CountWords);

                ManualResetEvent[] manualResetEvents = new ManualResetEvent[3];
                manualResetEvents[0] = new ManualResetEvent(false);
                manualResetEvents[1] = new ManualResetEvent(false);
                manualResetEvents[2] = new ManualResetEvent(false);
                countFiles.Start(manualResetEvents[0]);
                countLines.Start(manualResetEvents[1]);
                countWords.Start(manualResetEvents[2]);

                ManualResetEvent.WaitAll(manualResetEvents);

                Console.WriteLine("\nEnter a word you want search");
                string? word = Console.ReadLine();
                Console.WriteLine();

                if (word != null)
                {
                    analyzer.SearchWord(word);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
