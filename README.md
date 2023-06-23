# professor-assistant
This is a telegram bot and supporting microservices
# Running locally
In order to start the project locally, currently .NET SDK 6.0, Docker Desktop and ngrok are required.

1. Build docker file and run the container. Container will be exposed to ports 443 (HTTPS) and 80 (HTTP). Mapping ports might look like this: 32774:443, 32775:80.
2. Run command 'ngrok http https://localhost:{port} --host-header=rewrite' where {port} is host port (e.g. 32774).
3. Get the url from output of ngrok and change appsetting.json file config: "BotConfiguration.WebhookUrl" : "{url}/api/bot/update"

# Accounts and configs
You need to create an Azure account and request access to infrastructure and configuration.

# Architecture
https://professor-assistant.atlassian.net/wiki/spaces/PG/pages/196663/Architecture
