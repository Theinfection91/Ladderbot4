using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.IO;

namespace Ladderbot4.Managers
{
    public class GitBackupManager
    {
        private readonly string _repoPath;
        private readonly string _remoteUrl = Token.gitRemoteUrlPath;
        private readonly string _token = Token.gitPatToken;
        private readonly string _databasesFolderPath;

        public GitBackupManager()
        {
            // Set BackupRepo file path and init repo if necessary
            _repoPath = SetRepoFilePath();
            InitializeRepository();

            // Set the Databases folder
            _databasesFolderPath = SetDatabasesFolders();
        }

        private string SetRepoFilePath()
        {
            // Set the file path relative to the base directory of the executable
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct a path in a "BackupRepo" folder within the base directory
            string repoPath = Path.Combine(appBaseDirectory, "BackupRepo");

            // Ensure the directory exists
            if (!Directory.Exists(repoPath))
            {
                Directory.CreateDirectory(repoPath); // Create the directory if it doesn't exist
                Console.WriteLine($"{DateTime.Now} - Directory created: {_repoPath}");
            }

            return repoPath;
        }

        private void InitializeRepository()
        {
            if (!Repository.IsValid(_repoPath))
            {
                Console.WriteLine($"{DateTime.Now} - GitBackupManager - No local repo found. Cloning repository...");
                try
                {
                    var options = new CloneOptions
                    {
                        FetchOptions =
                        {
                            CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                            {
                                Username = "Ladderbot4",
                                Password = _token
                            }
                        }
                    };
                    Repository.Clone(_remoteUrl, _repoPath, options);
                    Console.WriteLine($"{DateTime.Now} - GitBackupManager - Repository cloned successfully.");
                }
                catch (LibGit2SharpException ex)
                {
                    Console.WriteLine($"{DateTime.Now} - GitBackupManager - Error cloning repository: {ex.Message}");
                }
            }
        }

        private string SetDatabasesFolders()
        {
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            return Path.Combine(appBaseDirectory, "Databases");
        }

        public void CopyJsonFilesToBackupRepo()
        {
            try
            {
                if (Directory.Exists(_databasesFolderPath))
                {
                    var jsonFiles = Directory.GetFiles(_databasesFolderPath, "*.json", SearchOption.TopDirectoryOnly);

                    foreach (var jsonFile in jsonFiles)
                    {
                        string fileName = Path.GetFileName(jsonFile);
                        string destinationPath = Path.Combine(_repoPath, fileName);

                        try
                        {
                            // Copy the file even if it's in use
                            using (FileStream sourceStream = new FileStream(jsonFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                            {
                                sourceStream.CopyTo(destinationStream);
                            }
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine($"{DateTime.Now} - GitBackupManager - Error copying file {jsonFile}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} - GitBackupManager - Error during backup process: {ex.Message}");
            }
        }

        public void BackupFiles()
        {
            using (var repo = new Repository(_repoPath))
            {
                // Get all files in the repository directory
                var files = Directory.GetFiles(_repoPath, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    // Skip Git metadata files
                    if (file.Contains(".git")) continue;

                    // Stage the file
                    LibGit2Sharp.Commands.Stage(repo, file);
                }

                // Check if there are any changes to commit
                if (repo.RetrieveStatus().IsDirty)
                {
                    // Create a commit with the current timestamp
                    Signature author = new Signature("Ladderbot4", "ladderbot4@bot.com", DateTimeOffset.Now);
                    Commit commit = repo.Commit($"Backup: Update data files ({DateTime.Now})", author, author);

                    // Push changes to the remote repository
                    var options = new PushOptions
                    {
                        CredentialsProvider = (_, _, _) => new UsernamePasswordCredentials
                        {
                            Username = "Ladderbot4",
                            Password = _token
                        }
                    };

                    try
                    {
                        repo.Network.Push(repo.Branches["main"], options);
                        Console.WriteLine($"{DateTime.Now} - GitBackupManager - Backup pushed successfully.");
                    }
                    catch (LibGit2SharpException ex)
                    {
                        Console.WriteLine($"Error during push: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now} - GitBackupManager - No changes detected; nothing to backup.");
                }
            }
        }

        public void CopyAndBackupFilesToGit()
        {
            CopyJsonFilesToBackupRepo();
            BackupFiles();
        }

        public void ForceBackupFiles()
        {
            using (var repo = new Repository(_repoPath))
            {
                // Get all files in the repository directory
                var files = Directory.GetFiles(_repoPath, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    // Skip Git metadata files
                    if (file.Contains(".git")) continue;

                    // Stage the file
                    LibGit2Sharp.Commands.Stage(repo, file);
                }

                // Create a commit with the current timestamp
                Signature author = new Signature("Ladderbot4", "ladderbot4@bot.com", DateTimeOffset.Now);
                try
                {
                    Commit commit = repo.Commit(
                        $"Backup: Commit at {DateTime.Now}",
                        author,
                        author,
                        new CommitOptions { AllowEmptyCommit = true } // Force empty commits
                    );

                    // Push changes to the remote repository
                    var options = new PushOptions
                    {
                        CredentialsProvider = (_, _, _) => new UsernamePasswordCredentials
                        {
                            Username = "Ladderbot4",
                            Password = _token
                        }
                    };
                    repo.Network.Push(repo.Branches["main"], options);
                    Console.WriteLine($"{DateTime.Now} GitBackupManager - Automated Backup pushed successfully.");
                }
                catch (LibGit2Sharp.EmptyCommitException)
                {
                    Console.WriteLine($"{DateTime.Now} - GitBackupManager - No changes; empty commit was skipped.");
                }
                catch (LibGit2SharpException ex)
                {
                    Console.WriteLine($"{DateTime.Now} - GitBackupManager - Error during push: {ex.Message}");
                }
            }
        }
    }
}
