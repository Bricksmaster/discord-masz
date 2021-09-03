if [[ ! -f ./.env ]]; then
    echo "Failed to find .env file. Please execute setup.py first."
    exit 5
fi

docker ps -a

echo "Killing old containers"
docker-compose stop
echo "Killed old containers"

docker ps -a

echo "Removing old containers/images/volumes"
docker container rm masz_nginx
docker container rm masz_backend
docker container rm masz_discordbot
docker container rm masz_invitationtracker

docker image rm discord-masz_nginx
docker image rm discord-masz_backend
docker image rm discord-masz_discordbot
docker image rm discord-masz_invitationtracker

echo "Removed old containers/images/volumes"

echo "Starting up..."
docker-compose up --build --force-recreate -d
echo "Started in background"
