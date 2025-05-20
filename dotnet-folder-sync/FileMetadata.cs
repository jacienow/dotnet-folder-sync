using System.IO.Hashing;

namespace dotnet_folder_sync
{
    public class FileMetadata
    {
        public FileMetadata(FileInfo fileInfo, string path)
        {
            FileInfo = fileInfo;
            Hash = BitConverter.ToString(XxHash64.Hash(File.ReadAllBytes(FileInfo.FullName))).Replace("-", string.Empty);
            RelativePath = FileInfo.FullName.Replace(path, string.Empty).RemoveLeadingBackslashes();
        }

        public FileInfo FileInfo { get; init; }

        public string RelativePath {  get; init; }

        public string Hash { get; init; }
    }
}
