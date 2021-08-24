# About

[![Build](https://github.com/rgl/UseLibgit2sharp/actions/workflows/build.yml/badge.svg)](https://github.com/rgl/UseLibgit2sharp/actions/workflows/build.yml)

This example application uses the [libgit2sharp library](https://github.com/libgit2/libgit2sharp) to create a local git repository, a commit, and a push to a remote repository.

## Usage

Start a temporary git server to host our test remote repository:

```bash
# start gitea in background.
# see https://docs.gitea.io/en-us/config-cheat-sheet/
# see https://github.com/go-gitea/gitea/blob/v1.15.0/docker/root/etc/s6/gitea/setup
docker run \
    --detach \
    --name gitea \
    -v /etc/timezone:/etc/timezone:ro \
    -v /etc/localtime:/etc/localtime:ro \
    -e SECRET_KEY=abracadabra \
    -p 3000:3000 \
    gitea/gitea:1.15.0
# create user in gitea.
docker exec gitea gitea admin user create \
    --admin \
    --email jane.doe@example.com \
    --username jane.doe \
    --password password
# create remote repository in gitea.
curl \
    -s \
    -X POST \
    -H 'Accept: application/json' \
    -H 'Authorization: Basic amFuZS5kb2U6cGFzc3dvcmQ=' \
    -H 'Content-Type: application/json' \
    -d '{"name": "test"}' \
    http://localhost:3000/api/v1/user/repos
```

Execute the example application:

```bash
dotnet run
```

List the local repository log:

```bash
git --git-dir tmp/test/.git log --oneline
```

Observe the remote repository:

http://localhost:3000/jane.doe/test

Destroy the temporary git server:

```bash
docker rm -f gitea
```

## Reference

* [LibGit2Sharp Hitchhiker's Guide to Git](https://github.com/libgit2/libgit2sharp/wiki/LibGit2Sharp-Hitchhiker%27s-Guide-to-Git)
