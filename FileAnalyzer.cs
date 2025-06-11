using System.Text.RegularExpressions;

namespace Multi_threaded_File_Processing;

internal class FileAnalyzer
{
    public readonly string filesDirectory = "InputFiles";

    public FileAnalyzer()
    {
        if (!Directory.Exists(filesDirectory))
        {
            throw new DirectoryNotFoundException("Directory with files doesn't exist!");
        }
    }

    public void CountFiles(object? resetEvent)
    {
        if (resetEvent is not ManualResetEvent manualResetEvent)
        {
            throw new TypeAccessException($"ManualResetEvent type expected. {resetEvent.GetType()} is given.");
        }
        string[] files = Directory.GetFiles(filesDirectory);
        Console.WriteLine("Count of files is - {0}", files.Length);

        manualResetEvent.Set();
    }

    public void CountLines(object? resetEvent)
    {
        if (resetEvent is not ManualResetEvent manualResetEvent)
        {
            throw new TypeAccessException($"ManualResetEvent type expected. {resetEvent.GetType()} is given.");
        }

        string[] files = Directory.GetFiles(filesDirectory);

        int lineCounter = 0;

        foreach (string file in files)
        {
            StreamReader sr = new StreamReader(file);
            try
            {
                while(!sr.EndOfStream)
                {
                    sr.ReadLine();
                    lineCounter++;
                }
            }
            finally
            {
                sr.Close();
            }
        }

        Console.WriteLine("Count of lines in all files is - {0}", lineCounter);

        manualResetEvent.Set();
    }

    public void CountWords(object? resetEvent)
    {
        if (resetEvent is not ManualResetEvent manualResetEvent)
        {
            throw new TypeAccessException($"ManualResetEvent type expected. {resetEvent.GetType()} is given.");
        }

        string[] files = Directory.GetFiles(filesDirectory);

        int wordCounter = 0;

        foreach (string file in files)
        {
            StreamReader sr = new StreamReader(file);
            try
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    wordCounter += line.Split(new char[] { ' ' }).Length;
                }
            }
            finally
            {
                sr.Close();
            }
        }

        Console.WriteLine("Count of words in all files is - {0}", wordCounter);

        manualResetEvent.Set();
    }

    private record SearchOptions(string searchingWord, string readingFile);

    public void SearchWord(string word)
    {
        string[] files = Directory.GetFiles(filesDirectory);

        foreach (string file in files)
        {
            SearchOptions options = new SearchOptions(word, file);
            Thread searchThread = new Thread(ScanFile);
            searchThread.Start(options);
        }
    }

    private void ScanFile(object? options)
    {
        if (options is SearchOptions searchingOptions)
        {
            StreamReader sr = new StreamReader(searchingOptions.readingFile);
            int lineCounter = 0;
            bool matchFound = false;
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            try
            {
                while (!sr.EndOfStream)
                {
                    lineCounter++;
                    string line = sr.ReadLine();
                    string[] lineOfWords =  line.Split(new char[] { ' ' });
                    int wordCounter = 0;
                    foreach(string word in lineOfWords)
                    {
                        string finishedWord = rgx.Replace(word, "");
                        wordCounter++;
                        if (finishedWord.Equals(searchingOptions.searchingWord))
                        {
                            matchFound = true;
                            Console.WriteLine("Word found in {0} file, line - {1}, position - {2}", searchingOptions.readingFile, lineCounter, wordCounter);
                        }
                    }
                }
                if (!matchFound)
                {
                    Console.WriteLine("Word \"{0}\" was not found in file {1}", searchingOptions.searchingWord, searchingOptions.readingFile);
                }
            }
            finally
            {
                sr.Close();
            }
        }
    }
}
