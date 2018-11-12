using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    // class that stores the probability and frequency in a category for each word
    [Serializable()]
    class WordMetrics
    {
        private string value;
        private int frequency;
        private double probability;

        public string Value { get => value; set => this.value = value; }
        public int Frequency { get => frequency; set => frequency = value; }
        public double Probability { get => probability; set => probability = value; }

        public WordMetrics(string word, int recurrence, double calculation)
        {
            value = word;
            frequency = recurrence;
            probability = calculation;
        }
    }
}
