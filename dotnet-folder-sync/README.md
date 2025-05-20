## How to use

`dotnet-folder-sync.exe --source "source_folder" --target "target_folder" --logs-dir "logs_folder" --interval 1`


## ToDo:
- [x] Synchronization must be one-way: after the synchronization content of the replica folder should be modified to exactly match content of the source folder;
- [x] Synchronization should be performed periodically;
- [x] File creation/copying/removal operations should be logged to a file and to the console output;
- [x] Folder paths, synchronization interval and log file path should be provided using the command line arguments;
- [x] It is undesirable to use third-party libraries that implement folder synchronization;
- [x] It is allowed (and recommended) to use external libraries implementing other well-known algorithms. For example, there is no point in implementing yet another function that calculates MD5 if you need it for the task – it is perfectly acceptable to use a third-party (or built-in) library.

## Assumptions
* Since requirements did not specify how files should be compared - a mix of path and hash algorithm is used - since there was no req to use cryptographic algorithm, i decided to use non-crypto one - xxHash, because it's fast and for basic check if file contents are same, it should be enough.
* System.CommandLine is used in order to not bother with parsing args
* For same reason, Serilog is used - to not bother with manual handling of logging.
* For running in intervals, a simple approach with Task.Delay is used - in real-world scenario, i'd probably go with some cronjob or task scheduler in windows, to not have to implement that logic.

## Potential improvements
* Implement some sort of caching mechanism for hashes - which could speed up sync process