using System;
using System.IO;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace UseLibgit2sharp
{
    class Program
    {
        static void Main(string[] args)
        {
            // create the temporary directory that will host our local test repository.
            if (Directory.Exists("tmp"))
            {
                Directory.Delete("tmp", true);
            }
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
                var author = new Signature("Jane Doe", "jane.doe@example.com", DateTimeOffset.Now);
                var committer = author;
                var commit = repo.Commit("hello world", author, committer);

                // add the origin remote that points to the test remote repository.
                var remote = repo.Network.Remotes.Add("origin", "http://localhost:3000/jane.doe/test.git");

                // push the master branch to the origin remote repository.
                var pushOptions = new PushOptions
                {
                    CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials()
                            {
                                Username = "jane.doe",
                                Password = "password",
                            }
                    ),
                };
                repo.Network.Push(remote, @"refs/heads/master", pushOptions);
            }
        }
    }
}
