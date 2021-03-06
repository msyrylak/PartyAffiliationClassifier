﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PartyAffiliationClassifier
{
    class NaiveBayes
    {
        /// <summary>
        /// Creates a list of files(which are now dictionaries) for each category,
        /// does a regex match with the first entries in the dictionaries with 0 assigned as key
        /// values and then adds those first entries to a list to remove from the vocabulary later on as it is not needed there. 
        /// After assigning those dictionaries to category lists, passes them into NaiveBayes method
        /// that will then return a list of WordMetrics objects and adds it to a list of lists of WordMetrics.
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

            // lists for each category and the vocabulary
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
            wordMetrics.Add(WordProbabilitiesCalculator(categoryName1, vocabulary, l_conservative, entriesToRemove, noOfTrainingFiles));
            wordMetrics.Add(WordProbabilitiesCalculator(categoryName2, vocabulary, l_coalition, entriesToRemove, noOfTrainingFiles));
            wordMetrics.Add(WordProbabilitiesCalculator(categoryName3, vocabulary, l_labour, entriesToRemove, noOfTrainingFiles));

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
        /// <param name="categoryList"></param>
        /// <param name="listToRemove"></param>
        /// <param name="numberOfTrainingFiles"></param>
        /// <returns></returns>
        private List<WordMetrics> WordProbabilitiesCalculator(string categoryName,
            List<string> vocabulary,
            List<Dictionary<string, int>> categoryList,
            List<string> listToRemove,
            int numberOfTrainingFiles)
        {
            List<string> uniqueVocabulary = vocabulary.Distinct().ToList();
            List<WordMetrics> wordsProbabilities = new List<WordMetrics>();

            for (int i = 0; i < listToRemove.Count; i++)
            {
                vocabulary.Remove(listToRemove[i]);
            }

            foreach (Dictionary<string, int> item in categoryList)
            {
                for (int i = 0; i < listToRemove.Count; i++)
                {
                    item.Remove(listToRemove[i]);
                }
            }

            // based on the number of files in the category, pass into one of the overloads of the CalculateProbability method
            foreach (Dictionary<string, int> categoryFile in categoryList)
            {
                if (categoryList.Count() < 2)
                {
                    wordsProbabilities = CalculateProbability(categoryName, uniqueVocabulary, categoryFile, numberOfTrainingFiles);
                }
                else
                {
                    wordsProbabilities = CalculateProbability(categoryName, uniqueVocabulary, categoryList, numberOfTrainingFiles);
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
            int allCatWords = 0;

            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();
            double categoryProbability = 1 / (double)numberOfTrainingFiles;


            // calculate number of all the words in the category
            foreach (KeyValuePair<string, int> word in categoryFile)
            {
                allCatWords += word.Value;
            }

            // create the first object for the category and its probability 
            WordMetrics categoryWord = new WordMetrics(categoryName, 1, categoryProbability, uniqueWords, allCatWords);
            l_wordMetrics.Add(categoryWord);



            // calculate probability for each word and then create a new object with this information
            foreach (KeyValuePair<string, int> word in categoryFile)
            {
                double probabilityResult = ((word.Value + 1) / (double)(uniqueWords + allCatWords));
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult, uniqueWords, allCatWords);
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
            int allCatWords = 0;

            Dictionary<string, int> dictCopy = new Dictionary<string, int>();
            List<WordMetrics> l_wordMetrics = new List<WordMetrics>();
            double categoryProbability = categoryFiles.Count() / (double)numberOfTrainingFiles;

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

            // calculate number of all the words in the category
            foreach (KeyValuePair<string, int> frequency in dictCopy)
            {
                allCatWords += frequency.Value;
            }

            // creates the first object for the category and its probability 
            WordMetrics categoryWord = new WordMetrics(categoryName, categoryFiles.Count(), categoryProbability, uniqueWords, allCatWords);
            l_wordMetrics.Add(categoryWord);

            // calculate probability for each word and then create a new object with this information
            foreach (KeyValuePair<string, int> word in dictCopy)
            {
                double probabilityResult = (word.Value + 1) / (double)(uniqueWords + allCatWords);
                WordMetrics wordMetrics = new WordMetrics(word.Key, word.Value, probabilityResult, uniqueWords, allCatWords);
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
            List<string> fileToClassify = fileManager.ClassificationFileReader(newFile);

            double probabilityConservative = 0;
            double probabilityLabour = 0;
            double probabilityCoalition = 0;

            // add up probabilities logs for each category 
            foreach (var list in trainedWords)
            {
                if (list[0].Value == "Conservative")
                {
                    double uniqueVocabulary = list[0].UniqueVocab;
                    double allCatWords = list[0].AllCatWords;
                    int match;

                    foreach (var newWord in fileToClassify)
                    {
                        match = 0;
                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.ToLower() == list[i].Value)
                            {
                                probabilityConservative += Math.Log(list[i].Probability);
                                match++;
                            }
                        }
                        if (match == 0)
                        {
                            probabilityConservative += Math.Log((0 + 1) / (uniqueVocabulary + allCatWords));
                        }
                    }
                    probabilityConservative += Math.Log(list[0].Probability);
                }

                if (list[0].Value == "Labour")
                {
                    double uniqueVocabulary = list[0].UniqueVocab;
                    double allCatWords = list[0].AllCatWords;
                    int hits;

                    foreach (var newWord in fileToClassify)
                    {
                        hits = 0;
                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.ToLower() == list[i].Value)
                            {
                                probabilityLabour += Math.Log(list[i].Probability);
                                hits++;
                            }
                        }
                        if (hits == 0)
                        {
                            probabilityLabour += Math.Log((0 + 1) / (uniqueVocabulary + allCatWords));
                        }
                    }
                    probabilityLabour += Math.Log(list[0].Probability);
                }

                if (list[0].Value == "Coalition")
                {
                    double uniqueVocabulary = list[0].UniqueVocab;
                    double allCatWords = list[0].AllCatWords;
                    int hits;

                    foreach (var newWord in fileToClassify)
                    {
                        hits = 0;
                        for (int i = 1; i < list.Count(); i++)
                        {
                            if (newWord.ToLower() == list[i].Value)
                            {
                                probabilityCoalition += Math.Log(list[i].Probability);
                                hits++;
                            }
                        }
                        if (hits == 0)
                        {
                            probabilityCoalition += Math.Log((0 + 1) / (uniqueVocabulary + allCatWords));
                        }
                    }
                    probabilityCoalition += Math.Log(list[0].Probability);
                }
            }
            Console.WriteLine("Coalition: " + probabilityCoalition);
            Console.WriteLine("Conservative: " + probabilityConservative);
            Console.WriteLine("Labour: " + probabilityLabour);

            // comparison
            if (probabilityConservative > probabilityLabour && probabilityConservative > probabilityCoalition)
            {
                Console.WriteLine("File belongs to CONSERVATIVE category");
                Console.WriteLine();
            }
            if (probabilityLabour > probabilityConservative && probabilityLabour > probabilityCoalition)
            {
                Console.WriteLine("File belongs to LABOUR category");
                Console.WriteLine();
            }
            if (probabilityCoalition > probabilityLabour && probabilityCoalition > probabilityConservative)
            {
                Console.WriteLine("File belongs to COALITION category");
                Console.WriteLine();
            }
        }

    }
}
