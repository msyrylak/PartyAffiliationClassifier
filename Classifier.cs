using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    class Classifier
    {
        public static void Train(Dictionary<string, int> trainingFile)
        {
            string[] categories = { "conservative", "labour", "coalition" };
            int allWords = 0;
            int uniqueWords = trainingFile.Count();

            foreach (KeyValuePair<string, int> wordPair in trainingFile)
            {
                allWords += wordPair.Value;
            }
        }

        public static void Classify()
        {

        }

    }
}
