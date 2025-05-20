using System.Text.RegularExpressions;

namespace dotnet_folder_sync
{
    public static class Extensions
    {
        public static DirectoryInfo[] GetAllDirectories(this string path, string searchPattern = "*") => new DirectoryInfo(path).GetDirectories(searchPattern, SearchOption.AllDirectories);

        public static FileInfo[] GetAllFiles(this string path, string searchPattern = "*") => new DirectoryInfo(path).GetFiles(searchPattern, SearchOption.AllDirectories);

        public static string RemoveLeadingBackslashes(this string input)
        {
            var regex = new Regex(@"^\\+");
            var value = regex.Replace(input, string.Empty);

            return value;
        }
    }
}
