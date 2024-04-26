using System.Diagnostics;

namespace Kq1Pathing.FileIO
{
    public class FileWriter : IDisposable
    {
        private StreamWriter sw;
        private bool showOutputToConsole;
        private bool swOpen = true;

        public FileWriter(string outputFilenamepath, bool showOutputToConsole = false)
        {
            string fullPath = Path.GetFullPath(outputFilenamepath);
            string dirName = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
                Debug.WriteLine($"Created directory: {dirName}");
            }
            Debug.WriteLine($"Writing to {fullPath}");
            sw = new StreamWriter(fullPath);
            this.showOutputToConsole = showOutputToConsole;
        }

        public void CloseFileWriter()
        {
            sw.Close();
            swOpen = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && sw != null)
            {
                if (swOpen)
                {
                    CloseFileWriter();
                }
                sw.Dispose();
                sw = null;
            }
        }

        public void Write(string text)
        {
            sw.Write(text);
            if (showOutputToConsole)
            {
                Debug.Write($"{text}");
            }
        }

        public void WriteLine(string text)
        {
            sw.WriteLine(text);
            if (showOutputToConsole)
            {
                Debug.WriteLine($"{text}");
            }
        }
    }
}
