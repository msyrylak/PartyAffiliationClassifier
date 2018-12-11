using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;


namespace PartyAffiliationClassifier
{
    class FileManager
    {
        /// <summary>
        /// This function is for training part of the program,
        /// It takes in the name of the folder with training files, iterates through it 
        /// and for each file removes the stop words and then creates a dictionary with 
        /// a frequency of each word in the file and then adds it to a list of dictionaries and that list 
        /// is returned.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public List<Dictionary<string, int>> FileReaderTraining(string folderName)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', ';', '\'', '\t', '\r', '\n' };

            string[] stopWords = File.ReadLines(@".\stopwords.txt").ToArray();
            List<Dictionary<string, int>> listOfProcessedFiles = new List<Dictionary<string, int>>();
            List<string> listOfFileNames = new List<string>();

            try
            {
                // take in the folder name and iterate through it to look for .txt files
                string path = @"./" + folderName;
                var files = from file in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories)
                            select new
                            {
                                File = file,
                            };

                // foreach txt file in the folder
                foreach (var f in files)
                {
                    StreamReader streamReader = new StreamReader(f.File);

                    List<string> queensSpeech = new List<string>();
                    queensSpeech = streamReader.ReadToEnd().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList();

                    // remove stopwords
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

                    int count = 0;

                    Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

                    // add name of the file as the first entry in the dictionary (file name = category name)
                    wordFrequency.Add(Path.GetFileNameWithoutExtension(f.File), 0);

                    foreach (var uniqueWord in queensSpeech.Distinct(StringComparer.CurrentCultureIgnoreCase).ToArray())
                    {
                        // for each unique word in the file count how many times it shows up 
                        for (int i = 0; i < queensSpeech.Count; i++)
                        {
                            // if they match just add one to the count variable
                            if (uniqueWord.ToLower() == queensSpeech[i].ToLower())
                            {
                                count++;
                            }
                        }
                        // add the unique word and the frequency to the dictionary
                        wordFrequency.Add(uniqueWord.ToLower(), count);
                        count = 0;
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


        /// <summary>
        /// Used for the classification part of the assignment.
        /// Takes in the name of the file that needs to be classified,
        /// removes stop words and returns a list of words.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<string> FileReaderClassification(string fileName)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', ';', '\t', '\r', '\n', '\'' };

            string[] stopWords = File.ReadLines(@".\stopwords.txt").ToArray();
            List<string> listOfFileNames = new List<string>();

            try
            {
                string path = @"./" + fileName;
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

                return queensSpeech;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// Write all the word objects into a binary file after the training has been finished.
        /// </summary>
        /// <param name="trainedWords"></param>
        public void WriteTraining(List<List<WordMetrics>> trainedWords)
        {
            try
            {
                string fileToWrite = @".\training.bin";

                Stream stream = File.Open(fileToWrite, FileMode.Create);
                BinaryFormatter binFormatter = new BinaryFormatter();

                binFormatter.Serialize(stream, trainedWords);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Read the word objects from the binary file.
        /// </summary>
        /// <returns></returns>
        public List<List<WordMetrics>> ReadTraining()
        {
            string fileToRead = @".\training.bin";

            try
            {
                Stream stream = File.Open(fileToRead, FileMode.Open);
                BinaryFormatter binFormatter = new BinaryFormatter();

                List<List<WordMetrics>> trainedWords = (List<List<WordMetrics>>)binFormatter.Deserialize(stream);
                return trainedWords;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

    }

}
