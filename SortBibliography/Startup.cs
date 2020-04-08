namespace SortBibliography
{
    using System;
    using System.IO;

    public static class Startup
    {
        public static void Main(string[] args)
        {
            string InputFileName = @"D:\SvetPAPERS\0. SortBibliographyC#\SortBibliography - repository\SortBibliography\bin\Release\un-Sort.tex";
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
    }
}
