using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PartyAffiliationClassifier
{
    class Classifier
    {
        struct WordMetrics
        {
            private string value;
            private int frequency;
            private float probability;

            public string Value { get => value; set => this.value = value; }
            public int Frequency { get => frequency; set => frequency = value; }
            public float Probability { get => probability; set => probability = value; }

            public WordMetrics(string word, int recurrence, float calculation)
            {
                value = word;
                frequency = recurrence;
                probability = calculation;
            }
        }

        public static void Train(List<Dictionary<string, int>> trainingFiles)
        {
            // categories match so that the program can assign files into a category
            Regex rx_conservative = new Regex(@"\bconservative\b", RegexOptions.IgnoreCase);
            Regex rx_coalition = new Regex(@"\bcoalition\b", RegexOptions.IgnoreCase);
            Regex rx_labour = new Regex(@"\blabour\b", RegexOptions.IgnoreCase);

            // lists for each category 
            List<Dictionary<string, int>> l_conservative = new List<Dictionary<string, int>>();
            List<Dictionary<string, int>> l_coalition = new List<Dictionary<string, int>>();
            List<Dictionary<string, int>> l_labour = new List<Dictionary<string, int>>();
            List<string> vocabulary = new List<string>();

            int allWords = 0;
            int allConservativeWords = 0;
            int allCoalitionWords = 0;
            int allLabourWords = 0;

            foreach (Dictionary<string, int> file in trainingFiles)
            {
                int uniqueWords = file.Count();

                foreach (KeyValuePair<string, int> wordPair in file)
                {
                    if (rx_conservative.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        file.Remove(wordPair.Key);
                        l_conservative.Add(file);
                        allConservativeWords += wordPair.Value;
                    }
                    else if (rx_coalition.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        file.Remove(wordPair.Key);
                        l_coalition.Add(file);
                        allCoalitionWords += wordPair.Value;
                    }
                    else if(rx_labour.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        file.Remove(wordPair.Key);
                        l_labour.Add(file);
                        allLabourWords += wordPair.Value;
                    }
                    vocabulary.Add(wordPair.Key);
                }
                NaiveBayes(vocabulary, allLabourWords, l_labour);
            }
        }

        public static void Classify()
        {

        }

        private static void NaiveBayes(List<string> vocabulary, int allCatWords, List<Dictionary<string, int>> category)
        {
            List<string> uniqueVocabulary = vocabulary.Distinct().ToList();

            foreach (Dictionary<string, int> categoryFile in category)
            {
                if (category.Count() < 2)
                {
                    foreach (KeyValuePair<string, int> categoryPair in categoryFile)
                    {
                        WordMetrics w = new WordMetrics(categoryPair.Key, categoryPair.Value, CalculateProbability(uniqueVocabulary, categoryFile, allCatWords));
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, int> categoryPair in categoryFile)
                    {
                        WordMetrics w = new WordMetrics(categoryPair.Key, categoryPair.Value, CalculateProbability(uniqueVocabulary, categoryFile, allCatWords));
                    }
                }
                
            }
        }

        private static float CalculateProbability(List<string> uniqueVocabulary, Dictionary<string, int> categoryFile, int allCatWords)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();

            

            float probability;

            return 0;
        }

        private static float CalculateProbability(List<string> uniqueVocabulary, List<Dictionary<string, int>> categoryFile, int allCatWords)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();


            float probability;

            return 0;
        }


    }
}
