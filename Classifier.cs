using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    class Classifier
    {
        enum Categories { Conservative, Coalition, Labour};

        string[,,] probabilitesTable;
        List<string[,,]> listOfProbabilities = new List<string[,,]>();

        public static void Train(List<Dictionary<string, int>> trainingFiles)
        {
            int allWords = 0;

            foreach (Dictionary<string, int> file in trainingFiles)
            {
                int uniqueWords = file.Count();

                foreach (KeyValuePair<string, int> wordPair in file)
                {
                    allWords += wordPair.Value;
                }

                NaiveBayes(uniqueWords, allWords, file);
            }
        }

        public static void Classify()
        {

        }


        private static void NaiveBayes(int uniqueWords, int allWords, Dictionary<string, int> wordFrequency)
        {

        }

        private static void Calculate()
        {

        }


    }
}
