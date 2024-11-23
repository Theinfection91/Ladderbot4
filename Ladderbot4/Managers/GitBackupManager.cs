using LibGit2Sharp;
using System;
using System.Diagnostics;
using System.IO;

namespace Ladderbot4.Managers
{
    public class GitBackupManager
    {
        private string _repoPath;
        private readonly string _remoteUrl = Token.gitRemoteUrlPath;
        private readonly string _token = Token.gitPatToken;

        public GitBackupManager()
        {
            Console.WriteLine("GitBackupManager init");
            SetRepoFilePath();
            InitializeRepository();
        }

        private void SetRepoFilePath()
        {
            // Set the file path relative to the base directory of the executable
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct a path in a "BackupRepo" folder within the base directory
            _repoPath = Path.Combine(appBaseDirectory, "BackupRepo");

            // Ensure the directory exists
            if (!Directory.Exists(_repoPath))
            {
                Directory.CreateDirectory(_repoPath); // Create the directory if it doesn't exist
                Console.WriteLine($"Directory created: {_repoPath}");
            }
            else
            {
                Console.WriteLine($"Directory already exists: {_repoPath}");
            }
        }

        public void InitializeRepository()
        {
            if (!Repository.IsValid(_repoPath))
            {
                Console.WriteLine("Cloning repository...");
                try
                {
                    var options = new CloneOptions
                    {
                        FetchOptions =
                        {
                            CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                            {
                                Username = "Ixnay",
                                Password = _token
                            }
                        }
                    };
                    Repository.Clone(_remoteUrl, _repoPath, options);
                    Console.WriteLine("Repository cloned successfully.");
                }
                catch (LibGit2SharpException ex)
                {
                    Console.WriteLine($"Error cloning repository: {ex.Message}");
                    throw; // Re-throw for visibility if debugging
                }
            }
            else
            {
                Console.WriteLine("Repository already initialized.");
            }
        }


        public void BackupFiles(string[] files)
        {
            using (var repo = new Repository(_repoPath))
            {
                foreach (var file in files)
                {
                    string fullPath = Path.Combine(_repoPath, file);
                    if (File.Exists(fullPath))
                    {
                        LibGit2Sharp.Commands.Stage(repo, fullPath);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: {file} does not exist and will be skipped.");
                    }
                }

                if (repo.RetrieveStatus().IsDirty)
                {
                    Signature author = new Signature("Ladderbot4", "ladderbot4@bot.com", DateTimeOffset.Now);
                    Commit commit = repo.Commit("Backup: Update data files", author, author);

                    var options = new PushOptions
                    {
                        CredentialsProvider = (_, _, _) => new UsernamePasswordCredentials
                        {
                            Username = "Ixnay", // Can be anything
                            Password = _token
                        }
                    };

                    repo.Network.Push(repo.Branches["main"], options);
                    Console.WriteLine("Backup completed successfully.");
                }
                else
                {
                    Console.WriteLine("No changes detected; nothing to backup.");
                }
            }
        }
    }
}
