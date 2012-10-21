using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndexComparer.BusinessObjects;

namespace IndexComparer.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate { Console.WriteLine(String.Format("{0}Goodbye.{0}", Environment.NewLine)); };

            string PrimaryServerName, SecondaryServerName, PrimaryDatabaseName, SecondaryDatabaseName, OutputFileName;

            if (args.Count() == 5)
            {
                PrimaryServerName = args[0];
                SecondaryServerName = args[1];
                PrimaryDatabaseName = args[2];  
                SecondaryDatabaseName = args[3];
                OutputFileName = args[4];
            }
            else
            {
                if (args.Count() > 0)
                {
                    Console.WriteLine("Invalid argument listing passed.  A valid call looks like:");
                    Console.WriteLine("IndexComparer.exe PrimaryServerName SecondaryServerName PrimaryDatabaseName SecondaryDatabaseName OutputFileName");
                    Console.WriteLine("");
                    Console.WriteLine("");
                }

                #region Console Activity

                do
                {
                    Console.Write("Give the primary server name. ");
                    PrimaryServerName = Console.ReadLine();
                } while (String.IsNullOrWhiteSpace(PrimaryServerName));

                do
                {
                    Console.Write("Give the primary database name. ");
                    PrimaryDatabaseName = Console.ReadLine();
                } while (String.IsNullOrWhiteSpace(PrimaryDatabaseName));

                Console.Write("Give the secondary server name.  If this is the same as the primary server, just hit Enter. ");
                SecondaryServerName = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(SecondaryServerName))
                    SecondaryServerName = PrimaryServerName;

                Console.Write("Give the secondary database name.  If this is the same as the primary database, just hit Enter. ");
                SecondaryDatabaseName = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(SecondaryDatabaseName))
                    SecondaryDatabaseName = PrimaryDatabaseName;

                Console.Write("Tell where you would like the output file to go.  Default: C:\\Temp\\IndexComparisonLog.txt  -- ");
                OutputFileName = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(OutputFileName))
                    OutputFileName = @"C:\Temp\IndexComparisonLog.txt";

                #endregion
            }

            List<IndexSet> PrimaryResults = IndexSet.RetrieveIndexData(PrimaryServerName, PrimaryDatabaseName);
            List<IndexSet> SecondaryResults = IndexSet.RetrieveIndexData(SecondaryServerName, SecondaryDatabaseName);

            using (System.IO.StreamWriter writer = System.IO.File.CreateText(OutputFileName))
            {
                DataStreamer.StreamFile(true, true, true, writer, PrimaryServerName, PrimaryDatabaseName, SecondaryServerName, SecondaryDatabaseName, PrimaryResults, SecondaryResults);
            }
        }
    }
}
