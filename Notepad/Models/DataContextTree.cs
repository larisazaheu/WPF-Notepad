using System.Collections.ObjectModel;
using System.IO;

namespace Notepad.Models
{
    public class DataContextTree
    {
        public ObservableCollection<FileDirectoryModel> Partitions { get; }
        public DataContextTree()
        {
            Partitions = new ObservableCollection<FileDirectoryModel>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                Partitions.Add(new FileDirectoryModel(drive.Name));
            }
        }
    }
}