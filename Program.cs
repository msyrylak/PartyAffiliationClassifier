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

            Classifier classifier = new Classifier();
            List<List<WordMetrics>> trainedFiles = classifier.Train(FileManager.FileReaderTraining(folderName));

            Console.WriteLine("Specify a file path");
            string newFile = Console.ReadLine();
            classifier.Classify(trainedFiles, newFile);

            Console.WriteLine("Specify a file path");
            string newFile2 = Console.ReadLine();
            classifier.Classify(trainedFiles, newFile2);

            Console.WriteLine("Specify a file path");
            string newFile3 = Console.ReadLine();
            classifier.Classify(trainedFiles, newFile3);


            Console.ReadLine();
        }
    }
}
