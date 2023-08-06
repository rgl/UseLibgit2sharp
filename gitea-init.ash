#!/bin/ash
# see https://docs.gitea.io/en-us/config-cheat-sheet/
# see https://github.com/go-gitea/gitea/releases
# see https://github.com/go-gitea/gitea/blob/v1.17.2/docker/root/etc/s6/gitea/setup

# wait for gitea to be up.
while [ -z "$(curl -s http://localhost:3000/api/v1/version | grep version)" ]; do sleep 3; done

# create the admin user.
if [ -z "$(su-exec "$USER" gitea admin user list --admin | grep "$ADMIN_USERNAME")" ]; then
    su-exec "$USER" gitea admin user create \
        --admin \
        --username "$ADMIN_USERNAME" \
        --password "$ADMIN_PASSWORD" \
        --email "$ADMIN_EMAIL"
fi

# create the test repository.
if [ -z "$(
    curl \
        -s \
        -S \
        -u "$ADMIN_USERNAME:$ADMIN_PASSWORD" \
        -X GET \
        -H 'Accept: application/json' \
        "http://localhost:3000/api/v1/repos/$ADMIN_USERNAME/test" | grep test)" ]; then
    curl \
        -s \
        -S \
        -u "$ADMIN_USERNAME:$ADMIN_PASSWORD" \
        -X POST \
        -H 'Accept: application/json' \
        -H 'Content-Type: application/json' \
        -d '{"name": "test"}' \
        http://localhost:3000/api/v1/user/repos
fi
