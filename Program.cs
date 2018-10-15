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
            //List<List<Dictionary<string, int>>> list = new List<List<Dictionary<string, int>>>();

            Console.WriteLine("Welcome to the Party Affiliation Classifier!");
            //Console.WriteLine("How many files do you want to provide?");
            //int noOfFiles = int.Parse(Console.ReadLine());

            //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"../../QueensSpeech");
            string path = @"./QueensSpeech";
            var files = from file in Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories)
                        //from line in File.ReadLines(file)
                        select new
                        {
                            File = file,
                            //Line = line
                        };

            foreach (var f in files)
            {
                Console.WriteLine("{0}", f.File);
            }
            Console.WriteLine("{0} files found.", files.Count().ToString());
      
            //for (int i = 0; i < noOfFiles; i++)
            //{
            //    Console.WriteLine("File number {0}: ", i + 1);
            //    string filePath1 = Console.ReadLine();
            //    Classifier.Train(FileManager.FileReader(@"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\QueensSpeech\" + filePath1));
            //}
            // add results from the training function to a big array outside
            Console.ReadLine();
        }
    }
}
