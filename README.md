<h1 align="center">SparkCheck</h1>
<h3 align="center">Swipe-Less Dating App Written in Blazor Server with EF Core</h3>

---

<p align="center">
<img alt="SparkCheck" src="https://sparkcheck.date/images/logo.png" width="500px"/>
<br>
<br>
<a href="https://sparkcheck.date">
<img alt="sparkcheck.date" src="https://img.shields.io/website?url=http%3A%2F%2Fsparkcheck.date%2Fimages%2Flogo.png&up_message=SparkCheck.date&down_message=down">
</a>
<a href="https://hub.docker.com/r/judebeans/sparkcheck">
<img alt="Docker Pulls" src="https://img.shields.io/docker/pulls/judebeans/sparkcheck">
</a>
<a href="https://github.com/SparkCheckDevHub/SparkCheck/releases">
<img alt="Latest Release" src="https://img.shields.io/github/v/release/SparkCheckDevHub/SparkCheck">
</a>
<a href="https://github.com/SparkCheckDevHub/SparkCheck/blob/main/LICENSE">
<img alt="AGLPv3 License" src="https://img.shields.io/github/license/SparkCheckDevHub/SparkCheck">
</a>

---

SparkCheck is a live matchmaking dating platform, built with Blazor Server and EF Core. Unlike traditional swipe apps, SparkCheck instantly pairs users for live chat sessions, making connections faster, more interactive, and meaningful. 

Key Features:
- <strong>Filtered matchmaking queue</strong> - Find the best match based on age, location, gender, and preferences
- <strong>Live interactive sessions</strong> - Engage with AI-generated conversation prompts in real-time
- <strong>Date Designer</strong> - Curated date ideas based on mutual interests and nearby locations
- <strong>Ease of use and setup</strong> - Only requiring phone number login, interests selection, and profile completion to begin.

Tech Stack
- <strong>Blazor Server</strong> (.NET 8)
- <strong>Entity Framework Core</strong> for database management.
- <strong>MudBlazor</strong> for UI components
- <strong>Python microservices</strong> for matchmaking and phone verification
- <strong>SQLAlchemy</strong> for python database connectivity
- <strong>AsteriskPBX</strong> for initiating verification calls to the user
- <strong>Dockerized</strong> for easy deployment
- <strong>Nginx</strong> as a reverse proxy

Original Contributing Members (alphabetical order):
- <strong>Brandon</strong> (<a href="https://github.com/burtonb0210"><img src="https://github.com/burtonb0210.png" width="24" height="24" style="border-radius:50%; vertical-align:middle;"> burtonb0210</a>)
- <strong>James</strong> (<a href="https://github.com/judebeans"><img src="https://github.com/judebeans.png" width="24" height="24" style="border-radius:50%; vertical-align:middle;">judebeans</a>)
- <strong>Seth/Homer</strong> (<a href="https://github.com/sbkb1"><img src="https://github.com/sbkb1.png" width="24" height="24" style="border-radius:50%; vertical-align:middle;">sbkb1</a>)
- <strong>Seth</strong> (<a href="https://github.com/Seth7051"><img src="https://github.com/Seth7051.png" width="24" height="24" style="border-radius:50%; vertical-align:middle;">Seth7051</a>)


<strong>Ready to find a match?</strong>
<br>
Check out our live demo at <a href="https://sparkcheck.date">SparkCheck.date</a>. NOTE: We reset the data on the server weekly.

<strong>Want to run your own instance of SparkCheck?</strong>
<br>
Visit the <a href="#server-installation">server installation</a> section to learn how deploy your own instance.

<strong>Want to contribute?</strong>
<br>
We welcome code contributions for our project :) Please familiarize yourself with our codebase, read and agree to the <a href="https://github.com/SparkCheckDevHub/SparkCheck/blob/main/CONTRIBUTING.md">contributor code of conduct</a>, and submit a pull request!

---

## Server Installation
To install SparkCheck on your own server, you'll need a machine capable of running Docker and Docker-Compose and any additional requirements.

### Dependencies
(Optional) To install all of the necessary Docker and related dependencies before deployment, run the following script provided in the repository

```sh
chmod +x ./scripts/install.sh
sudo bash ./scripts/install.sh
```

The necessary dependencies include:
- Docker
- Docker Compose
- Git
- Certbot
- Nginx
- Socat (optional, for testing)

### Cloning the Repository
Next, make sure you clone the repository onto your machine and enter it as the working directory.

```sh
git clone https://github.com/SparkCheckDevHub/SparkCheck.git
cd SparkCheck
```

### Environment Variables
SparkCheck uses a variety of environment variables to properly deploy an instance. The grid below shows each environment variable and its purpose.

Variable        | Purpose/Description
----------------|-------
SA_PASSWORD     | This is the password used by Microsoft SQL Server. Please set a password following the <a href="https://learn.microsoft.com/en-us/sql/relational-databases/security/strong-passwords?view=sql-server-ver17">strong passwords guideline</a>.
SC_USE_ASTERISK | True\|False depending on if your server will be using phone verification
SC_SIP_HOST     | The hostname of the VOIP point of presence.
SC_SIP_PORT     | The port used by the VOIP point of presence
SC_SIP_NUMBER   | The DID used to make calls.
SC_SIP_USERNAME | The username for VOIP authentication.
SC_SIP_PASSWORD | The password for VOIP authentication.
SC_OPENAI_KEY   | An API key provided by OpenAI for generative features in match chats.

To deploy the most basic of configurations on a Linux machine, create a .env file in the repository root, and write our basic configuration to the file. NOTE: Please use a secure database password.

```sh
touch .env
nano .env
# Then write the basic configuration to the file, and run the following command:
source .env
```

Basic Configuration:
```
SA_PASSWORD=pl34s3-ch4ng3-th1s-s3cur31y
SC_USE_ASTERISK=False
```

On Windows, run the following commands in PowerShell and then start a new session for basic configuration:
```powershell
setx SA_PASSWORD "pl34s3-ch4ng3-th1s-s3cur31y"
setx SC_USE_ASTERISK "False"
```

### Startup
Once your environment variables are setup, finish by building the docker container and running it. Your instance will be accessible at `127.0.0.1:5980`.

```sh
docker-compose build
docker-compose up -d
```

Alternatively, use our quick-start script

```sh
sudo bash ./start.sh
```

To update the containers from the repo, use the following script
```sh
sudo bash ./scripts/update.sh
```

### Additional Setup
Optionally, you can configure your instance with Nginx and CertBot. Modify the files in `/nginx` to match your domain configuration. Then, run...

```sh
sudo bash ./nginx/setup-nginx.sh
```

This will guide you through Certbot setup when invoked.

---

### Screenshots
<img src="https://kodanux.com/sparkcheck/S1.png" width="200px">
<img src="https://kodanux.com/sparkcheck/S2.png" width="200px">
<img src="https://kodanux.com/sparkcheck/S3.png" width="200px">

&copy; 2025 Team Cortanix 💛