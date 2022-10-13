# About

[![Build](https://github.com/rgl/UseLibgit2sharp/actions/workflows/build.yml/badge.svg)](https://github.com/rgl/UseLibgit2sharp/actions/workflows/build.yml)

This example application uses the [libgit2sharp library](https://github.com/libgit2/libgit2sharp) to create a local git repository, a commit, and a push to a remote repository.

## Usage

Start a temporary git server to host our test remote repository:

```bash
# start gitea in background.
# see https://docs.gitea.io/en-us/config-cheat-sheet/
# see https://github.com/go-gitea/gitea/releases
# see https://github.com/go-gitea/gitea/blob/v1.17.2/docker/root/etc/s6/gitea/setup
docker run \
    --detach \
    --name gitea \
    -v /etc/timezone:/etc/timezone:ro \
    -v /etc/localtime:/etc/localtime:ro \
    -e SECRET_KEY=abracadabra \
    -p 3000:3000 \
    gitea/gitea:1.17.2

# set the user credentials.
GITEA_USERNAME='jane.doe'
GITEA_PASSWORD='password'
GITEA_EMAIL="$GITEA_USERNAME@example.com"

# create user in gitea.
docker exec --user git gitea gitea admin user create \
    --admin \
    --email "$GITEA_EMAIL" \
    --username "$GITEA_USERNAME" \
    --password "$GITEA_PASSWORD"
# create remote repository in gitea.
curl \
    -s \
    -u "$GITEA_USERNAME:$GITEA_PASSWORD" \
    -X POST \
    -H 'Accept: application/json' \
    -H 'Content-Type: application/json' \
    -d '{"name": "test"}' \
    http://localhost:3000/api/v1/user/repos \
    | jq
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
