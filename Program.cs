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
            string filePath = @"C:\Users\Maja\Documents\Visual Studio 2017\Projects\PartyAffiliationClassifier\Conservative27thMay2015.txt";
            FileManager.FileReader(filePath);
            Console.ReadLine();

        }
    }
}
