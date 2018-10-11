using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace PartyAffiliationClassifier
{
    class FileManager
    {
        public static void FileReader(string filePath)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\r', '\n' };

            string[] stopWords = File.ReadLines("C:\\Users\\Maja\\Documents\\Visual Studio 2017\\Projects\\PartyAffiliationClassifier\\stopwords.txt").ToArray();

            try
            {
                StreamReader sr = new StreamReader(filePath);
                List<string> queensSpeech = new List<string>();
                queensSpeech = sr.ReadToEnd().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var item in queensSpeech.ToList())
                {
                    for (int j = 0; j < stopWords.Length; j++)
                    {
                        Match match = Regex.Match(stopWords[j], item, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            queensSpeech.Remove(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
