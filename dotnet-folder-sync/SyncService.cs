﻿using dotnet_folder_sync.Interfaces;
using Microsoft.Extensions.Logging;

namespace dotnet_folder_sync
{
    public class SyncService : ISyncService
    {
        private readonly ILogger<SyncService> _logger;

        public SyncService(ILogger<SyncService> logger)
        {
            _logger = logger;
        }

        public async Task Sync(string source, string target, TimeSpan interval)
        {
            ValidateIfDirExists(source);
            ValidateIfDirExists(target);

            while (true)
            {
                _logger.LogInformation("Sync start.");
                SyncDirectories(source, target);
                SyncFiles(source, target);

                _logger.LogInformation("Sync finished.");
                await Task.Delay(interval);
            }
        }

        private static void ValidateIfDirExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory ${path} not found.");
            }
        }

        private void SyncFiles(string source, string target)
        {
            var sourceFolderFiles = source.GetAllFiles().ToList();
            var targetFolderFiles = target.GetAllFiles().ToList();

            var sourceDict = sourceFolderFiles.Select(file => new FileMetadata(file, source)).ToDictionary(key => key.RelativePath, value => value);
            var targetDict = targetFolderFiles.Select(file => new FileMetadata(file, target)).ToDictionary(key => key.RelativePath, value => value);

            var toCopyFromSourceToTarget = sourceDict.Keys.Except(targetDict.Keys).Select(x => sourceDict[x]);
            foreach (var file in toCopyFromSourceToTarget)
            {
                var targetPath = file.ReplaceBasePath(source, target);

                File.Copy(file.FileInfo.FullName, targetPath, false);
                _logger.LogInformation($"CREATE: [{targetPath}]");
            }

            var toReplaceInTarget = sourceDict.Keys.Intersect(targetDict.Keys).Select(x => sourceDict[x]);
            foreach (var file in toReplaceInTarget)
            {
                var targetPath = file.ReplaceBasePath(source, target);

                if (file.Hash == targetDict[file.RelativePath].Hash)
                {
                    _logger.LogInformation($"SKIP COPY - OVERWRITE [Hash match]: [{targetPath}]");
                    continue;
                }

                File.Copy(file.FileInfo.FullName, targetPath, true);
                _logger.LogInformation($"COPY - OVERWRITE: [{targetPath}]");
            }

            var toDeleteFromTarget = targetDict.Keys.Except(sourceDict.Keys).Select(x => targetDict[x]);
            foreach (var file in toDeleteFromTarget)
            {
                var targetPath = file.ReplaceBasePath(source, target);

                File.Delete(file.FileInfo.FullName);
                _logger.LogInformation($"DELETE: [{targetPath}]");
            }
        }

        private void SyncDirectories(string source, string target)
        {
            var sourceDirs = source.GetAllDirectories();
            var targetDirs = target.GetAllDirectories();
            var dirsToCopyFromSourceToTarget = sourceDirs.Keys.Except(targetDirs.Keys);
            var dirsToRemoveFromTarget = targetDirs.Keys.Except(sourceDirs.Keys);

            foreach (var dir in dirsToCopyFromSourceToTarget)
            {
                var path = Path.Combine(target, dir);
                Directory.CreateDirectory(path);

                _logger.LogInformation($"CREATE: [{path}]");
            }

            foreach (var dir in dirsToRemoveFromTarget)
            {
                var path = Path.Combine(target, dir);
                if (!Directory.Exists(path))
                {
                    _logger.LogInformation($"SKIP DELETE [No longer exists]: [{path}]");
                    continue;
                    
                }
                Directory.Delete(path, true);
                _logger.LogInformation($"DELETE [Recursive]: [{path}]");
            }
        }
    }
}
