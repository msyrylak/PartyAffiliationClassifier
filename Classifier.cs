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
        public List<List<WordMetrics>> Train(List<Dictionary<string, int>> trainingFiles)
        {
            // categories match so that the program can assign files into a category
            Regex rx_conservative = new Regex(@"\bconservative", RegexOptions.IgnoreCase);
            Regex rx_coalition = new Regex(@"\bcoalition", RegexOptions.IgnoreCase);
            Regex rx_labour = new Regex(@"\blabour", RegexOptions.IgnoreCase);

            string categoryName1 = " ";
            string categoryName2 = " ";
            string categoryName3 = " ";

            int noOfTrainingFiles = trainingFiles.Count();

            // lists for each category 
            List<Dictionary<string, int>> l_conservative = new List<Dictionary<string, int>>();
            List<Dictionary<string, int>> l_coalition = new List<Dictionary<string, int>>();
            List<Dictionary<string, int>> l_labour = new List<Dictionary<string, int>>();
            List<string> vocabulary = new List<string>();

            int allWords = 0;

            // copies
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
                        categoryName1 = "Conservative";
                    }
                    else if (rx_coalition.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        entriesToRemove.Add(wordPair.Key);
                        l_coalition.Add(file);
                        categoryName2 = "Coalition";
                    }
                    else if (rx_labour.IsMatch(wordPair.Key) && wordPair.Value == 0)
                    {
                        entriesToRemove.Add(wordPair.Key);
                        l_labour.Add(file);
                        categoryName3 = "Labour";
                    }
                    vocabulary.Add(wordPair.Key);
                }
            }

            List<List<WordMetrics>> wordMetrics = new List<List<WordMetrics>>();
            wordMetrics.Add(NaiveBayes(categoryName3, vocabulary, l_labour, entriesToRemove, noOfTrainingFiles));
            wordMetrics.Add(NaiveBayes(categoryName1, vocabulary, l_conservative, entriesToRemove, noOfTrainingFiles));
            wordMetrics.Add(NaiveBayes(categoryName2, vocabulary, l_coalition, entriesToRemove, noOfTrainingFiles));

            FileManager fm = new FileManager();
            fm.WriteTraining(wordMetrics);

            return wordMetrics;
        }

        private List<WordMetrics> NaiveBayes(string categoryName,
            List<string> vocabulary,
            List<Dictionary<string, int>> category,
            List<string> listToRemove,
            int numberOfTrainingFiles)
        {
            // training
            List<string> uniqueVocabulary = vocabulary.Distinct().ToList();
            List<WordMetrics> probabilities = new List<WordMetrics>();

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
                    probabilities = CalculateProbability(categoryName, uniqueVocabulary, categoryFile, allCatWords, numberOfTrainingFiles);
                }
                else
                {
                    probabilities = CalculateProbability(categoryName, uniqueVocabulary, category, allCatWords, numberOfTrainingFiles);
                }
            }
            return probabilities;
        }

        // probability for just one file 
        private List<WordMetrics> CalculateProbability(string categoryName, List<string> uniqueVocabulary,
            Dictionary<string, int> categoryFile, int allCatWords, int numberOfTrainingFiles)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();
            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();
            float categoryProbability = 1 / (float)numberOfTrainingFiles;

            WordMetrics categoryWord = new WordMetrics(categoryName, 1, categoryProbability);
            l_wordMetrics.Add(categoryWord);

            foreach (KeyValuePair<string, int> word in categoryFile)
            {
                float probabilityResult = ((word.Value + 1) / (float)(uniqueWords + allCatWords));
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult);
                l_wordMetrics.Add(wordMetrics);
            }

            return l_wordMetrics;
        }

        // probability for more than one file in the category
        private List<WordMetrics> CalculateProbability(string categoryName, List<string> uniqueVocabulary,
            List<Dictionary<string, int>> categoryFiles, int allCatWords, int numberOfTrainingFiles)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();
            Dictionary<string, int> dictCopy = new Dictionary<string, int>();
            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();
            float categoryProbability = categoryFiles.Count() / (float)numberOfTrainingFiles;

            WordMetrics categoryWord = new WordMetrics(categoryName, categoryFiles.Count(), categoryProbability);
            l_wordMetrics.Add(categoryWord);

            // if the keys are the same just add their values
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
                float probabilityResult = (word.Value + 1) / (float)(uniqueWords + allCatWords);
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult);
                l_wordMetrics.Add(wordMetrics);
            }

            return l_wordMetrics;
        }

        public void Classify(List<List<WordMetrics>> trainedWords /* wordMetrics*/, string newFile)
        {
            FileManager fileManager = new FileManager();
            Dictionary<string, int> fileToClassify = fileManager.FileReaderClassification(newFile);
            float probabilityConservative = 0;
            float probabilityLabour = 0;
            float probabilityCoalition = 0;

            foreach (var list in trainedWords)
            {
                if (list[0].Value == "Conservative")
                {
                    foreach (var newWord in fileToClassify)
                    {
                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.Key == list[i].Value)
                            {
                                probabilityConservative += list[i].Probability;
                            }
                        }
                    }
                    probabilityConservative *= list[0].Probability;
                }

                if (list[0].Value == "Labour")
                {
                    foreach (var newWord in fileToClassify)
                    {
                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.Key == list[i].Value)
                            {
                                probabilityLabour += list[i].Probability;
                            }
                        }
                    }
                    probabilityLabour *= list[0].Probability;
                }

                if (list[0].Value == "Coalition")
                {
                    foreach (var newWord in fileToClassify)
                    {

                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.Key == list[i].Value)
                            {
                                probabilityCoalition += list[i].Probability;
                            }
                        }
                    }
                    probabilityCoalition *= list[0].Probability;
                }
            }
            Console.WriteLine("Coalition: " + probabilityCoalition);
            Console.WriteLine("Conservative: " + probabilityConservative);
            Console.WriteLine("Labour: " + probabilityLabour);

            // comparison
            if (probabilityConservative > probabilityLabour && probabilityConservative > probabilityCoalition)
            {
                Console.WriteLine("File belongs to Conservative category");
                Console.WriteLine();
            }
            if (probabilityLabour > probabilityConservative && probabilityLabour > probabilityCoalition)
            {
                Console.WriteLine("File belongs to Labour category");
                Console.WriteLine();
            }
            if (probabilityCoalition > probabilityLabour && probabilityCoalition > probabilityConservative)
            {
                Console.WriteLine("File belongs to Coalition category");
                Console.WriteLine();
            }
        }

    }
}
