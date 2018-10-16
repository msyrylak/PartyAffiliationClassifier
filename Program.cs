using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PartyAffiliationClassifier
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Welcome to the Party Affiliation Classifier!");
            Console.WriteLine("Name of the folder you wish to classify/train on");
            string folderName = Console.ReadLine();

            Classifier.Train(FileManager.FileReader(folderName));

            Console.ReadLine();
        }
    }
}
