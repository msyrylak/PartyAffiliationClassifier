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
        public static List<Dictionary<string, int>> FileReaderTraining(string folderName)
        {
            // TODO overload for one file 

            char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t', '\r', '\n' };

            string[] stopWords = File.ReadLines(@".\stopwords.txt").ToArray();
            List<Dictionary<string, int>> listOfProcessedFiles = new List<Dictionary<string, int>>();
            List<string> listOfFileNames = new List<string>();

            try
            {
                string path = @"./" + folderName;
                var files = from file in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories)
                            select new
                            {
                                File = file,
                            };

                Console.WriteLine("{0} files found.", files.Count().ToString());

                foreach (var f in files)
                {
                    Console.WriteLine("{0}", f.File);
                    //listOfFileNames.Add(Path.GetFileNameWithoutExtension(f.File));

                    StreamReader streamReader = new StreamReader(f.File);


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
                    wordFrequency.Add(Path.GetFileNameWithoutExtension(f.File), 0);

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

                    listOfProcessedFiles.Add(wordFrequency);
                }

                return listOfProcessedFiles;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public static Dictionary<string, int> FileReaderClassification(string fileName)
        {
            // TODO overload for one file 

            char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t', '\r', '\n' };

            string[] stopWords = File.ReadLines(@".\stopwords.txt").ToArray();
            List<string> listOfFileNames = new List<string>();
            Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

            try
            {
                string path = @"./" + fileName;
                //var files = from file in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories)
                //            select new
                //            {
                //                File = file,
                //            };

                //Console.WriteLine("{0} files found.", files.Count().ToString());

//                foreach (var f in files)
                //{
                    //Console.WriteLine("{0}", f.File);
                    //listOfFileNames.Add(Path.GetFileNameWithoutExtension(f.File));

                    StreamReader streamReader = new StreamReader(path);


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
                    int count = 0;

                    //wordFrequency.Add(Path.GetFileNameWithoutExtension(f.File), 0);

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

                    //foreach (KeyValuePair<string, int> wordCount in wordFrequency) //.OrderBy(i => i.Value))
                    //{
                    //    Console.WriteLine("Word = {0}, Count = {1}", wordCount.Key, wordCount.Value);
                    //}

                //}

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
