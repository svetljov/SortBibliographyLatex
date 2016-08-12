namespace SortBibliography
{
    using System;
    using System.IO;

    public static class Startup
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                string InputFileName = args[0];
                string OutputFileName = InputFileName + "-OrderedBibliography.tex";

                ICitationBuilder citationBuilder = new CitationBuilder(InputFileName, OutputFileName);
                try
                {
                    citationBuilder.BuildOrderedBibliography();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadLine();
                }
            }
            else
            {
                throw new ArgumentException("Drag and drop a valid text file.");
            }
        }
    }
}
