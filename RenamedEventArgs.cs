using System;

namespace Renamer
{
    public class RenamedEventArgs : EventArgs
    {
        public RenamedEventArgs(int filesChanged) => TotalFilesChanged = filesChanged;

        public int TotalFilesChanged { get; }
    }
}
