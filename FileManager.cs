using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace PartyAffiliationClassifier
{
    class FileManager
    {
        public static void FileReader(string filePath)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\r', '\n' };

            string[] stopWords = File.ReadLines("C:\\Users\\Maja\\Documents\\Visual Studio 2017\\Projects\\PartyAffiliationClassifier\\stopwords.txt").ToArray();

            try
            {
                StreamReader streamReader = new StreamReader(filePath);
                List<string> queensSpeech = new List<string>();
                queensSpeech = streamReader.ReadToEnd().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var word in queensSpeech.ToList())
                {
                    for (int j = 0; j < stopWords.Length; j++)
                    {
                        Match match = Regex.Match(stopWords[j], word, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            queensSpeech.Remove(word);
                        }
                    }
                }

                int uniqueWords = queensSpeech.Distinct().Count();
                Console.WriteLine("All of the words: {0}", queensSpeech.Count());
                Console.WriteLine("Unique words: {0}", uniqueWords);

                string speechString = string.Join(" ", queensSpeech.ToArray());
                Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

                foreach (var word in queensSpeech.Distinct())
                {
                    string dupSearch = word;
                    int count = new Regex(dupSearch, RegexOptions.IgnoreCase).Matches(speechString).Count;

                    wordFrequency.Add(word, count);
                }

                foreach (KeyValuePair<string, int> wordCount in wordFrequency)
                {
                    Console.WriteLine("Word = {0}, Count = {1}", wordCount.Key, wordCount.Value);
                }

                // write stats to file
                string path = @"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\BayesNetwork.txt";

                StreamWriter sw = File.AppendText(path);

                sw.WriteLine("File name: {0}", filePath);
                sw.WriteLine("All of the words: {0}", queensSpeech.Count());
                sw.WriteLine("Unique words: {0}", queensSpeech.Distinct().Count());
                foreach (KeyValuePair<string, int> pair in wordFrequency)
                {
                    sw.WriteLine("Word = {0}, Count = {1}", pair.Key, pair.Value);
                }

                sw.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
