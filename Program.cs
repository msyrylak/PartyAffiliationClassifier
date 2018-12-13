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
            bool quit = false;
            string fileToClassify;
            string answer;
            List<List<WordMetrics>> bayesianNetwork = new List<List<WordMetrics>>();
            List<Dictionary<string, int>> trainedFiles = new List<Dictionary<string, int>>();

            NaiveBayes naiveBayes = new NaiveBayes();
            FileManager fileManager = new FileManager();

            do
            {
                Console.WriteLine("Welcome to the Party Affiliation Classifier!");
                Console.WriteLine("Choose the action you want to perform: " + '\n' + "a) train" + '\n' +
                    "b) classify a document" + '\n' + "c) close the program");
                answer = Console.ReadLine();

                switch (answer.ToLower())
                {
                    case "a":
                        Console.WriteLine("You chose training!");
                        Console.WriteLine();
                        Console.WriteLine("Specify a folder name within the program's directory from which you wish to train the machine.");

                        string folderName = Console.ReadLine();

                        trainedFiles = fileManager.TrainingFileReader(folderName);
                        bayesianNetwork = naiveBayes.Train(trainedFiles);

                        Console.WriteLine(" ");
                        Console.WriteLine("Training finished! Do you want to classify a document now? (Y/N)");

                        string classificationAnswer = Console.ReadLine();

                        Console.WriteLine();
                        if (classificationAnswer.ToLower() == "y")
                        {
                            Console.WriteLine("Type in a file name.");
                            fileToClassify = Console.ReadLine();

                            naiveBayes.Classify(bayesianNetwork, fileToClassify);
                        }
                        else if (classificationAnswer.ToLower() == "n")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Sorry, the answer not recognised.");
                        }
                        break;

                    case "b":
                        Console.WriteLine("You chose file classification!");
                        Console.WriteLine("Type in a file name");

                        fileToClassify = Console.ReadLine();
                        bayesianNetwork = fileManager.ReadTraining();
                        naiveBayes.Classify(bayesianNetwork, fileToClassify);
                        break;

                    case "c":
                        quit = true;
                        break;

                    default:
                        Console.WriteLine("Sorry, the answer not recognised.");
                        break;
                }
            } while (!quit);
        }
    }
}
