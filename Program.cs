using System;
using System.IO;
using System.Runtime.InteropServices;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace UseLibgit2sharp
{
    class Program
    {
        // when using the http(s) repository transport, libgit2sharp uses the
        // ManagedHttpSmartSubtransport class, which uses the .net http(s)
        // stack (e.g. HttpClient). to accept a certificate issued by a private
        // ca you must add it to the os/system certificate store.
        // NB ServicePointManager.ServerCertificateValidationCallback no longer
        //    works in .NET 5. instead we could have used
        //    HttpClientHandler.ServerCertificateCustomValidationCallback, but
        //    thats not possible to do with libgit2sharp and at the PR 1618
        //    comments, it seems there is no desire to expose this kind of
        //    customization to the library user.
        //    see https://docs.microsoft.com/en-us/dotnet/framework/network-programming/managing-connections
        //    see https://github.com/libgit2/libgit2sharp/pull/1618
        static void Main(string[] args)
        {
            var repositoryUrl = "http://localhost:3000/jane.doe/test.git";
            var userName = "Jane Doe";
            var userEmail = "jane.doe@example.com";
            var userUsername = "jane.doe";
            var userPassword = "password";

            Console.WriteLine($"libgit2sharp version: {GlobalSettings.Version}");

            // create the temporary directory that will host our local test repository.
            DeleteDirectory("tmp");
            Directory.CreateDirectory("tmp");

            // initialize a test repository.
            var testRepositoryPath = Repository.Init(@"tmp/test");

            // create sample content.
            File.WriteAllText("tmp/test/message.txt", "Hello World");

            // add sample content to the test repository and push it to origin.
            using (var repo = new Repository(testRepositoryPath))
            {
                // stage all the working directory changes.
                Commands.Stage(repo, "*");

                // commit the staged changes.
                var author = new Signature(userName, userEmail, DateTimeOffset.Now);
                var committer = author;
                var commit = repo.Commit("hello world", author, committer);

                // add the origin remote that points to the test remote repository.
                var remote = repo.Network.Remotes.Add("origin", repositoryUrl);

                // push the master branch to the origin remote repository.
                var pushOptions = new PushOptions
                {
                    CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials()
                            {
                                Username = userUsername,
                                Password = userPassword,
                            }
                    ),
                };
                repo.Network.Push(remote, @"refs/heads/master", pushOptions);
            }
        }

        // recursively force the deletion of the given directory.
        // NB on windows, because the git repository files are read-only,
        //    Directory.Delete will fail with UnauthorizedAccessException,
        //    so, before deleting a file, we have to remove its read-only
        //    attribute.
        private static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            // on non-Windows use the regular Directory.Delete because they
            // do not care about the file permissions. to delete a file,
            // only the parent directory permissions matter.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Directory.Delete(path, true);
                return;
            }

            foreach (var directoryPath in Directory.GetDirectories(path))
            {
                DeleteDirectory(directoryPath);
            }

            foreach (var filePath in Directory.GetFiles(path))
            {
                var fileAttributes = File.GetAttributes(filePath);

                if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(filePath, fileAttributes ^ FileAttributes.ReadOnly);
                }

                File.Delete(filePath);
            }

            Directory.Delete(path);
        }
    }
}
