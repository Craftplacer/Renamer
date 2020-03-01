using System;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Renamer
{
    public class Item
    {
        public string TempPath { get; }
        private FileSystemWatcher Watcher { get; }
        
        private string[] FileList { get; }

        public event EventHandler<RenamedEventArgs> Renamed;

        public Item(string[] files)
        {
            FileList = files;

            var directory = Path.GetTempPath();
            var fileName = Path.GetRandomFileName() + ".txt";
            TempPath = Path.Combine(directory, fileName);

            File.WriteAllLines(TempPath, FileList);

            Watcher = new FileSystemWatcher()
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.LastAccess,
                Path = directory,
                Filter = fileName,
                EnableRaisingEvents = true,
            };

            Watcher.Changed += OnChanged;
        }
        
        private void OnChanged(object s, FileSystemEventArgs e)
        {
            if (e.FullPath != TempPath)
                return;

            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            try
            {
                var newFiles = File.ReadAllLines(TempPath);
                var filesChanged = 0;

                for (var i = 0; i < FileList.Length; i++)
                {
                    var oldFileName = FileList[i];
                    var newFileName = newFiles[i];

                    if (oldFileName == newFileName) continue;
                    
                    File.Move(oldFileName, newFileName);
                    filesChanged++;
                }

                Renamed?.Invoke(this, new RenamedEventArgs(filesChanged));
            }
            catch (Exception ex)
            {
                using var dialog = new TaskDialog()
                {
                    Caption = "Renamer",
                    Icon = TaskDialogStandardIcon.Error,
                    Text = "An error has occured while renaming files.",
                    DetailsCollapsedLabel = "Show exception details",
                    DetailsExpandedLabel = "Hide exception details",
                    DetailsExpandedText = ex.ToString(),
                    StandardButtons = TaskDialogStandardButtons.Ok
                };
                
                dialog.Show();
            }
            finally
            {
                File.Delete(TempPath);
            }

            Watcher.Dispose();
        }
    }
}