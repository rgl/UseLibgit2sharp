# About

[![Build](https://github.com/rgl/UseLibgit2sharp/actions/workflows/build.yml/badge.svg)](https://github.com/rgl/UseLibgit2sharp/actions/workflows/build.yml)

This example application uses the [libgit2sharp library](https://github.com/libgit2/libgit2sharp) to create a local git repository, a commit, and a push to a remote repository.

## Usage (Ubuntu 22.04)

Install [docker](https://github.com/moby/moby) and [docker compose](https://github.com/docker/compose).

Build and start the local gitea git server:

```bash
docker compose down --remove-orphans --volumes
docker compose up --build
```

In another shell, build and execute the example application:

```bash
dotnet run # or: docker compose run --build test
```

List the local repository log:

```bash
git --git-dir tmp/test/.git log --oneline
```

Observe the remote repository:

http://localhost:3000/jane.doe/test

Destroy everything:

```bash
docker compose down --remove-orphans --volumes
```

## Reference

* [LibGit2Sharp Hitchhiker's Guide to Git](https://github.com/libgit2/libgit2sharp/wiki/LibGit2Sharp-Hitchhiker%27s-Guide-to-Git)
