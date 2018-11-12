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
        /// <summary>
        /// Creates a list of files(which are now dictionaries) for each category,
        /// does a regex match with the first entries in the dictionaries with 0 assigned as key
        /// values and then adds those first entries to a list to remove from the vocabulary later on as it is not needed there. 
        /// After assigning those dictionaries to category lists, passes them into NaiveBayes method
        /// that will then return a list of WordMetrics objects and adds it to a list of list of WordMetrics.
        /// Writes the objects into a binary file upon finishing the training phase.
        /// </summary>
        /// <param name="trainingFiles"></param>
        /// <returns></returns>
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
                // if the first entry in the dictionary matches the category name regex, add that dictionary
                // to the according list 
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


        /// <summary>
        /// Gets the unique words from the vocabulary and puts it into a list, removes category names
        /// from it and then passes the dictionaries for each category into a CalculateProbability method. 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="vocabulary"></param>
        /// <param name="category"></param>
        /// <param name="listToRemove"></param>
        /// <param name="numberOfTrainingFiles"></param>
        /// <returns></returns>
        private List<WordMetrics> NaiveBayes(string categoryName,
            List<string> vocabulary,
            List<Dictionary<string, int>> category,
            List<string> listToRemove,
            int numberOfTrainingFiles)
        {
            List<string> uniqueVocabulary = vocabulary.Distinct().ToList();
            List<WordMetrics> wordsProbabilities = new List<WordMetrics>();

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

            // based on the number of files in the category, pass into one of the overloads of the CalculateProbability method
            foreach (Dictionary<string, int> categoryFile in category)
            {
                if (category.Count() < 2)
                {
                    wordsProbabilities = CalculateProbability(categoryName, uniqueVocabulary, categoryFile, numberOfTrainingFiles);
                }
                else
                {
                    wordsProbabilities = CalculateProbability(categoryName, uniqueVocabulary, category, numberOfTrainingFiles);
                }
            }
            return wordsProbabilities;
        }

       
        // probability for just one file 
        private List<WordMetrics> CalculateProbability(string categoryName, List<string> uniqueVocabulary,
            Dictionary<string, int> categoryFile, int numberOfTrainingFiles)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();
            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();
            double categoryProbability = 1 / (double)numberOfTrainingFiles;

            // create the first object for the category and its probability 
            WordMetrics categoryWord = new WordMetrics(categoryName, 1, categoryProbability);
            l_wordMetrics.Add(categoryWord);

            int allCatWords = 0;

            // calculate number of all the words in the category
            foreach (KeyValuePair<string, int> word in categoryFile)
            {
                allCatWords += word.Value;
            }

            // calculate probability for each word and then create a new object with this information
            foreach (KeyValuePair<string, int> word in categoryFile)
            {
                double probabilityResult = ((word.Value + 1) / (double)(uniqueWords + allCatWords));
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult);
                l_wordMetrics.Add(wordMetrics);
            }

            return l_wordMetrics;
        }

        
        // probability for more than one file in the category
        private List<WordMetrics> CalculateProbability(string categoryName, List<string> uniqueVocabulary,
            List<Dictionary<string, int>> categoryFiles, int numberOfTrainingFiles)
        {
            // number of unique words in the training set
            int uniqueWords = uniqueVocabulary.Count();
            Dictionary<string, int> dictCopy = new Dictionary<string, int>();
            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();
            double categoryProbability = categoryFiles.Count() / (double)numberOfTrainingFiles;

            // creates the first object for the category and its probability 
            WordMetrics categoryWord = new WordMetrics(categoryName, categoryFiles.Count(), categoryProbability);
            l_wordMetrics.Add(categoryWord);

            // merge two or more dictionaries for the category together
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

            int allCatWords = 0;

            // calculate number of all the words in the category
            foreach (KeyValuePair<string, int> frequency in dictCopy)
            {
                allCatWords += frequency.Value;
            }

            // calculate probability for each word and then create a new object with this information
            foreach (KeyValuePair<string, int> word in dictCopy)
            {
                double probabilityResult = (word.Value + 1) / (double)(uniqueWords + allCatWords);
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult);
                l_wordMetrics.Add(wordMetrics);
            }

            return l_wordMetrics;
        }


        /// <summary>
        /// Classifies a given text file:
        /// adds up the logs of each word probability for each category and then compares the results
        /// to give the classification.
        /// </summary>
        /// <param name="trainedWords"></param>
        /// <param name="newFile"></param>
        public void Classify(List<List<WordMetrics>> trainedWords /* wordMetrics*/, string newFile)
        {
            FileManager fileManager = new FileManager();

            // prepare the text file for classification
            List<string> fileToClassify = fileManager.FileReaderClassification(newFile);

            double probabilityConservative = 0;
            double probabilityLabour = 0;
            double probabilityCoalition = 0;

            // add up probabilities logs for each category 
            foreach (var list in trainedWords)
            {
                if (list[0].Value == "Conservative")
                {
                    foreach (var newWord in fileToClassify)
                    {
                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.ToLower() == list[i].Value)
                            {
                                probabilityConservative += Math.Log(list[i].Probability);
                            }
                        }
                    }
                    probabilityConservative += Math.Log(list[0].Probability);
                }

                if (list[0].Value == "Labour")
                {
                    foreach (var newWord in fileToClassify)
                    {
                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.ToLower() == list[i].Value)
                            {
                                probabilityLabour += Math.Log(list[i].Probability);
                            }
                        }
                    }
                    probabilityLabour += Math.Log(list[0].Probability);
                }

                if (list[0].Value == "Coalition")
                {
                    foreach (var newWord in fileToClassify)
                    {

                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.ToLower() == list[i].Value)
                            {
                                probabilityCoalition += Math.Log(list[i].Probability);
                            }
                        }
                    }
                    probabilityCoalition += Math.Log(list[0].Probability);
                }
            }
            Console.WriteLine("Coalition: " + probabilityCoalition);
            Console.WriteLine("Conservative: " + probabilityConservative);
            Console.WriteLine("Labour: " + probabilityLabour);

            // comparison
            if (probabilityConservative < probabilityLabour && probabilityConservative < probabilityCoalition)
            {
                Console.WriteLine("File belongs to Conservative category");
                Console.WriteLine();
            }
            if (probabilityLabour < probabilityConservative && probabilityLabour < probabilityCoalition)
            {
                Console.WriteLine("File belongs to Labour category");
                Console.WriteLine();
            }
            if (probabilityCoalition < probabilityLabour && probabilityCoalition < probabilityConservative)
            {
                Console.WriteLine("File belongs to Coalition category");
                Console.WriteLine();
            }
        }

    }
}
