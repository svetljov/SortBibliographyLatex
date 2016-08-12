namespace SortBibliography
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class CitationBuilder : ICitationBuilder
    {
        private const string BibitemPhrase = @"\bibitem{";
        private const string BeginBibliographyPhrase = @"\begin{thebibliography}";
        private const string EndOfBibliographyPhrase = @"\end{thebibliography}";
        private const string RegexConst = @"\\cite{([\s\S]+?)}";

        private IList<string> orderedListOfUniqueCitations;
        private IDictionary<string, string> bibliographyDictionary;

        private string outputFileName;
        private string inputFileName;
        private string body;
        private string bibliography;
        
        public CitationBuilder(string inputFileName, string outputFileName)
        {
            this.inputFileName = inputFileName;
            this.outputFileName = outputFileName;

            this.orderedListOfUniqueCitations = new List<string>();
            this.bibliographyDictionary = new Dictionary<string, string>();

            this.body = null;
            this.bibliography = null;
        }

        public void BuildOrderedBibliography()
        {
            this.ExtractBodyAndBibliography();
            this.ExtractOrderedListOfUniqueCitationsFromFile();
            this.ExtractBibliographyDictionary();

            StringBuilder sb = new StringBuilder();

            foreach (var item in orderedListOfUniqueCitations)
            {
                try
                {
                    sb.Append(bibliographyDictionary[item.Trim()]);
                }
                catch (Exception)
                {
                    throw new ArgumentException($"\\bibitem{{{item.Trim()}}} is undefined. Press ctrl-c...");
                }
            }

            File.WriteAllText(outputFileName, sb.ToString());
        }

        private void ExtractBodyAndBibliography()
        {
            string[] lines = File.ReadAllLines(inputFileName);
            StringBuilder bodyBuilder = new StringBuilder();
            StringBuilder bibliographyBuilder = new StringBuilder();
            bool isInBibliography = false;

            foreach (var line in lines)
            {
                if (line.StartsWith(BeginBibliographyPhrase))
                {
                    isInBibliography = true;
                }

                if (line.StartsWith(EndOfBibliographyPhrase) && isInBibliography)
                {
                    bibliographyBuilder.AppendLine(line);
                    break;
                }

                if (!isInBibliography)
                {
                    bodyBuilder.AppendLine(line);
                }
                else
                {
                    bibliographyBuilder.AppendLine(line);
                }
            }

            this.body = bodyBuilder.ToString();
            this.bibliography = bibliographyBuilder.ToString();
        }

        private void ExtractBibliographyDictionary()
        {
            string[] lines = this.bibliography.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            bool isInBibItem = false;
            string citationName = null;

            StringBuilder sb = null;

            foreach (var line in lines)
            {
                if (line.StartsWith(BibitemPhrase) || line.StartsWith(EndOfBibliographyPhrase))
                {
                    if (isInBibItem)
                    {
                        isInBibItem = false;

                        bibliographyDictionary.Add(citationName, sb.ToString());
                    }

                    isInBibItem = true;
                    sb = new StringBuilder();

                    int startIndex = line.IndexOf("{") + 1;
                    int endIndex = line.IndexOf("}");
                    citationName = line.Substring(startIndex, endIndex - startIndex);
                }

                if (isInBibItem)
                {
                    sb.AppendLine(line);
                }
            }
        }

        private void RemoveCommentedTextFromBody()
        {
            StringBuilder sb = new StringBuilder();

            string[] lines = this.body.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (line.StartsWith("%"))
                {
                    continue;
                }

                string lineWithoutComment = RemoveCommentedTextFromLine(line);
                sb.Append(lineWithoutComment);
            }

            this.body = sb.ToString();
        }

        private string RemoveCommentedTextFromLine(string line)
        {
            int indexOfSymbol = line.IndexOf('%');

            if (indexOfSymbol != -1)
            {
                return line.Substring(0, indexOfSymbol);
            }

            return line;
        }

        private void ExtractOrderedListOfUniqueCitationsFromFile()
        {
            this.RemoveCommentedTextFromBody();

            Regex regex = new Regex(RegexConst);

            Match match = regex.Match(body);
            while (match.Success)
            {
                var collection = match.Groups;
                string citationItems = collection[1].Value;
                string[] items = citationItems.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in items)
                {
                    this.orderedListOfUniqueCitations.Add(item.Trim());
                }

                match = match.NextMatch();
            }

            this.orderedListOfUniqueCitations = this.orderedListOfUniqueCitations.Distinct().ToList();
        }
    }
}