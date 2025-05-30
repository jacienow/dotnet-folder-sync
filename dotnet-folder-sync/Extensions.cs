﻿using System.Text.RegularExpressions;

namespace dotnet_folder_sync
{
    public static class Extensions
    {
        public static IDictionary<string, DirectoryInfo> GetAllDirectories(this string path, string searchPattern = "*") =>
            new DirectoryInfo(path).GetDirectories(searchPattern, SearchOption.AllDirectories).ToDictionary(key => key.FullName.Replace(path, string.Empty).RemoveLeadingBackslashes());

        public static FileInfo[] GetAllFiles(this string path, string searchPattern = "*") => new DirectoryInfo(path).GetFiles(searchPattern, SearchOption.AllDirectories);

        public static string RemoveLeadingBackslashes(this string input) => new Regex(@"^\\+").Replace(input, string.Empty);

        public static string ReplaceBasePath(this FileMetadata fileMetadata, string source, string target) => fileMetadata.FileInfo.FullName.Replace(source, target);
    }
}
