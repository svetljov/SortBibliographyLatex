namespace SortBibliography
{
    using System.IO;

    internal class Streamreader : StreamReader
    {
        public Streamreader(string path) : base(path)
        {
        }
    }
}