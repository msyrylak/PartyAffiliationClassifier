using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Party Affiliation Classifier!");

            Console.WriteLine("File number 1: ");
            string filePath1 = Console.ReadLine();

            Console.WriteLine("File number 2: ");
            string filePath2 = Console.ReadLine();

            Console.WriteLine("File number 3: ");
            string filePath3 = Console.ReadLine();

            Console.WriteLine("File number 4: ");
            string filePath4 = Console.ReadLine();

            Console.WriteLine("File number 5: ");
            string filePath5 = Console.ReadLine();

            FileManager.FileReader(@"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\" + filePath1);
            FileManager.FileReader(@"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\" + filePath2);
            FileManager.FileReader(@"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\" + filePath3);
            FileManager.FileReader(@"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\" + filePath4);
            FileManager.FileReader(@"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\" + filePath5);


            Console.ReadLine();
        }
    }
}
