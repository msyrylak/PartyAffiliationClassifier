using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    class WordMetrics
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
}
