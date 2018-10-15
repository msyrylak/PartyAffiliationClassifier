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
        public static Dictionary<string, int> FileReader(string filePath)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t', '\r', '\n' };

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

                Console.WriteLine("All of the words: {0}", queensSpeech.Count());
                Console.WriteLine("Unique words: {0}", queensSpeech.Distinct(StringComparer.CurrentCultureIgnoreCase).Count());


                //string speechString = string.Join(" ", queensSpeech.ToArray());
                //Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

                //foreach (var word in queensSpeech.Distinct(StringComparer.CurrentCultureIgnoreCase))
                //{
                //    string dupSearch = word;
                //    int count = new Regex(dupSearch, RegexOptions.IgnoreCase).Matches(speechString).Count;

                //    wordFrequency.Add(word.ToLower(), count);

                //}

                int count = 0;

                Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

                foreach (var uniqueWord in queensSpeech.Distinct(StringComparer.CurrentCultureIgnoreCase).ToArray())
                {
                    for (int i = 0; i < queensSpeech.Count; i++)
                    {
                        if (uniqueWord.ToLower() == queensSpeech[i].ToLower())
                        {
                            count++;
                        }
                    }
                    wordFrequency.Add(uniqueWord.ToLower(), count);
                    count = 0;
                }

                foreach (KeyValuePair<string, int> wordCount in wordFrequency) //.OrderBy(i => i.Value))
                {
                    Console.WriteLine("Word = {0}, Count = {1}", wordCount.Key, wordCount.Value);
                }


                // write stats to file
                string path = @"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\Vocabulary.txt";

                StreamWriter sw = File.AppendText(path);

                //sw.WriteLine("File name: {0}", filePath);
                //sw.WriteLine("All of the words: {0}", queensSpeech.Count());
                //sw.WriteLine("Unique words: {0}", queensSpeech.Distinct().Count());
                foreach (KeyValuePair<string, int> pair in wordFrequency.OrderBy(i => i.Value))
                {
                    sw.WriteLine(pair.Key);
                }

                sw.Close();

                return wordFrequency;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
