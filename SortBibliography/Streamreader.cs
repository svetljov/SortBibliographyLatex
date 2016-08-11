using System.IO;

namespace SortBibliography
{
    internal class Streamreader : StreamReader
    {
        public Streamreader(string path) : base(path)
        {
        }
    }
}