﻿using System;
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
            Classifier classifier = new Classifier();
            List<List<WordMetrics>> trainedFiles = new List<List<WordMetrics>>();
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
                        Console.WriteLine("Specify a folder from which you wish to train the machine.");

                        string folderName = Console.ReadLine();

                        trainedFiles = classifier.Train(fileManager.FileReaderTraining(folderName));

                        Console.WriteLine(" ");
                        Console.WriteLine("Training finished! Do you want to classify a document now? (Y/N)");
                        string classificationAnswer = Console.ReadLine();
                        Console.WriteLine();
                        if (classificationAnswer.ToLower() == "y")
                        {
                            Console.WriteLine("Type in a file name.");
                            fileToClassify = Console.ReadLine();
                            classifier.Classify(trainedFiles, fileToClassify);
                        }
                        else if (classificationAnswer.ToLower() == "n")
                        {
                            quit = true;
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
                        trainedFiles = fileManager.ReadTraining();
                        classifier.Classify(trainedFiles, fileToClassify);
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
