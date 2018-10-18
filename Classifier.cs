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
            Regex rx_conservative = new Regex(@"\bconservative", RegexOptions.IgnoreCase);
            Regex rx_coalition = new Regex(@"\bcoalition", RegexOptions.IgnoreCase);
            Regex rx_labour = new Regex(@"\blabour", RegexOptions.IgnoreCase);

            // lists for each category 
            List<Dictionary<string, int>> l_conservative = new List<Dictionary<string, int>>();
            List<Dictionary<string, int>> l_coalition = new List<Dictionary<string, int>>();
            List<Dictionary<string, int>> l_labour = new List<Dictionary<string, int>>();
            List<string> vocabulary = new List<string>();

            int allWords = 0;

            //copies
            List<string> entriesToRemove = new List<string>();

            foreach (Dictionary<string, int> file in trainingFiles)
            {
                int uniqueWords = file.Count();

                foreach (KeyValuePair<string, int> wordPair in file)
                {
                    if (rx_conservative.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        entriesToRemove.Add(wordPair.Key);
                        l_conservative.Add(file);
                    }
                    else if (rx_coalition.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        entriesToRemove.Add(wordPair.Key);
                        l_coalition.Add(file);
                    }
                    else if(rx_labour.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        entriesToRemove.Add(wordPair.Key);
                        l_labour.Add(file);
                    }
                    vocabulary.Add(wordPair.Key);
                }
            }
            NaiveBayes(vocabulary, l_labour, entriesToRemove);
            NaiveBayes(vocabulary, l_conservative, entriesToRemove);
            NaiveBayes(vocabulary, l_coalition, entriesToRemove);

        }

        public static void Classify()
        {

        }

        private static void NaiveBayes(List<string> vocabulary, List<Dictionary<string, int>> category, List<string> listToRemove)
        {
            List<string> uniqueVocabulary = vocabulary.Distinct().ToList();

            for (int i = 0; i < listToRemove.Count; i++)
            {
                vocabulary.Remove(listToRemove[i]);
            }

            foreach (Dictionary<string, int> item in category)
            {
                for (int i = 0; i < listToRemove.Count; i++)
                {
                    item.Remove(listToRemove[i]);
                }
            }

            int allCatWords = 0;

            foreach (Dictionary<string, int> categoryFile in category)
            {
                foreach (KeyValuePair<string, int> valuePair in categoryFile)
                {
                    allCatWords += valuePair.Value;
                }
            }

            foreach (Dictionary<string, int> categoryFile in category)
            {
                if (category.Count() < 2)
                {
                    CalculateProbability(uniqueVocabulary, categoryFile, allCatWords);
                }
                else
                {
                    CalculateProbability(uniqueVocabulary, category, allCatWords);
                }
            }
        }

        private static List<WordMetrics> CalculateProbability(List<string> uniqueVocabulary, Dictionary<string, int> categoryFile, int allCatWords)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();
            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();

            foreach (KeyValuePair<string, int> word in categoryFile)
            {
                float probabilityResult = ((float)(word.Value + 1) / (float)(uniqueWords + allCatWords));
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult);
                l_wordMetrics.Add(wordMetrics);
            }
            
            return l_wordMetrics;
        }

        private static List<WordMetrics> CalculateProbability(List<string> uniqueVocabulary, List<Dictionary<string, int>> categoryFiles, int allCatWords)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();
            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();
            Dictionary<string, int> dictCopy = new Dictionary<string, int>();
            
            // if the keys are the same just add their values
            ILookup<string, int> lookup = null;
            int frequencyValue = 0;
            foreach (Dictionary<string, int> dictionary in categoryFiles)
            {
                foreach (KeyValuePair<string, int> kvp in dictionary)
                {
                    if (dictCopy.ContainsKey(kvp.Key))
                    {
                        dictCopy[kvp.Key] += kvp.Value; 
                    }
                    else
                    {
                        dictCopy.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            foreach (KeyValuePair<string, int> word in dictCopy)
            {
                float probabilityResult = ((float)(word.Value + 1) / (float)(uniqueWords + allCatWords));
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult);
                l_wordMetrics.Add(wordMetrics);
            }

            return l_wordMetrics;
        }


    }
}
