using System.IO;

namespace Renamer
{
    public class Item
    {
        public string tempPath;
        public FileSystemWatcher watcher;

        public Item(string[] files)
        {
            string[] originalFiles = files;

            string fileName = Path.GetRandomFileName() + ".txt";
            tempPath = Path.Combine(Path.GetTempPath(), fileName);

            File.WriteAllLines(tempPath, originalFiles);

            watcher = new FileSystemWatcher()
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                Path = Path.GetTempPath(),
                Filter = fileName,
                EnableRaisingEvents = true,
            };

            watcher.Changed += (s, e) =>
            {
                if (e.FullPath != tempPath)
                {
                    return;
                }

                try
                {
                    string[] newFiles = File.ReadAllLines(tempPath);
                    for (int i = 0; i < originalFiles.Length; i++)
                    {
                        File.Move(originalFiles[i], newFiles[i]);
                    }

                    File.Delete(tempPath);
                }
                catch
                {
                }

                watcher.Dispose();
            };
        }
    }
}