# Blazor App Template

## Project Setup

- Clone the repository
- Rename all occurences of "MyApp" to desired name in:
  - project names
  - folder names
  - find and replace all occurences of "MyApp" keyword in the solution:
    - Namespaces
    - Database connection string environment variable name
    - Database names - prord and dev

## Development Environment Setup

- Install & Setup Docker

&nbsp;&nbsp;&nbsp;&nbsp; It is adviced to keep repositories in <disk>:\git folder. If a project is dependent on another one located in different repository and is actively developed together it complicates Docker builds, since Dockerfile can operate only on directories higher than it is located in. The solution here is to have a slightly different Dockerfile which would be located in <disk>:\git, so the context may include any desired repository. The downside is that if you have many repositories, the context becomes big and builds takes more time, thus it is the best to create sibling .dockerignore file which ignores all unrelevant folders/projects. To make it work for any project and in automated way [here](/dev/build.py) is a python script which analyses the Dockerfile, generates .dockerignore dynamically and temporarily modify any required .csproj projects to include project references, so the context is small. It should be copied into the git folder along with the template Dockerfile-app-<...> which can be edited as desired.

Build command:
```
# Format:
py build.py <Dockerfile-name> <image-name>

# Example:
py build.py Dockerfile-app-bs user/app-bs:0.0.1
```

### Build Docker Image With Local Project References
&nbsp;&nbsp;&nbsp;&nbsp; Deploying nuget packages results in delays, so it quickly test or build app in Docker 

## Production Environment Setup

Docker run command:
```
# Format:
 docker run -e "<MyAppServerClientSecret>=xxx" -e "<MyAppDatabaseConn>=mongodb+srv://<mongoLogin>:<mongoPassword>@cluster0-rgv8x.mongodb.net/test?retryWrites=false&w=majority" -p 7073:80 -d <dockerLogin>/<MyApp>-bs:0.1.6
```
###

- Create mongo cloud database account
- In security - network access Add allowed ips for dev and prod environments

### Create and Configure Azure B2C Authentication

- Remeber to Redirect Urls add both urls for development and production, ex.
  - https://localhost:7073/signin-oidc
  - https://mydomain.com/signin-oidc
 
- Add Roles claim - https://www.clintmcmahon.com/add-role-claims-to-an-azure-b2c-user-flow-access-token/

*Links*
- [How to add role claims to an Azure B2C user flow access token](https://www.clintmcmahon.com/add-role-claims-to-an-azure-b2c-user-flow-access-token/)

### Create and Configure AWS Account

- Open AWS Console and go to - IAM
- Go to users, click add user
  - Provide User Name, ex. AppUser, ..next
  - Leave Permissions options as Add user to group, and click Create group
  - Provide User Group Name, ex. AppUserGroup, ..create
  - Select created group, ..next, ..create
- Go to the user users -> Groups, open the group, go to Permissions, Add Permission, Create inline policy
  - Service - S3
  - Actions - List
  - Resources - All resources, ..review
  - Provide name, ex. ListS3Policy
- Go to the user again -> Security Credentials -> Create access key
  - Access key best practices... - Other, ..next
  - Provide Access Key Name, ex. KinergizeUserAccessKey, ..next
  - Download .csv file and store safely, ..done  

- Install AWS CLI - https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html
- Check AWS CLI version
```
aws --version
```

- Configure profile and provide keys
```
aws configure --profile <name>
```
aws toolkit for Visual Studio
*Links*
- [Configuring AWS Credentials for .NET Applications](https://www.youtube.com/watch?v=oY0-1mj4oCo)

### Create AWS E2C instance

Pricing - ex. for t3.micro
- running instance 750h free per month
- outbound data transfer - 100GB free per month (global), then first 10 TB / Month	$0.09 per GB and so on..

#### Links
[E2C Pricing](https://aws.amazon.com/ec2/pricing/on-demand/)

### Install Docker
### Add elastic IP

- Allocate Elastic IP address - it will prevent changing ip of instance on rerun
- Associate Elastic IP address with an E2C instance

**IMPORTANT!:** https://repost.aws/knowledge-center/elastic-ip-charges
> An Elastic IP address doesn’t incur charges as long as all the following conditions are true:
> - The Elastic IP address is associated with an EC2 instance.
> - The instance associated with the Elastic IP address is running.
> - The instance has only one Elastic IP address attached to it.
> - The Elastic IP address is associated with an attached network interface. For more information, see Network interface basics.

### Point domains to the IP
- Copy Public IPv4 address of the instance
- In your domain provider, point desired domains to the copied IP update A records with the IP
  
### Configure NGinx and add SSL certificate
- Add inbound rule in AWS E2C instance for port HTTPS 443
- Install and start nginx
```
sudo amazon-linux-extras install nginx1
sudo service nginx status
sudo service nginx start
```

- Install dependencies
```
sudo yum update
sudo yum install augeas-libs
```

- Set up a Python virtual environment
```
sudo python3 -m venv /opt/certbot/
sudo /opt/certbot/bin/pip install --upgrade pip
sudo /opt/certbot/bin/pip install urllib3==1.26 // downgrade urllib if conflicts with openssl
```

- Install Certbot  
```
sudo /opt/certbot/bin/pip install certbot certbot-nginx
```

- Create link to Certbot so that you can run the certbot command directly  
```
sudo ln -s /opt/certbot/bin/certbot /usr/bin/certbot
```

- Acquire certificates (Answers: YES, NO)  
```
sudo certbot --nginx -d kinergize.com -d kinergize.me -d kinergize.pl   // Change domains to yours!
```

- Navigate to Nginx config file and edit  
```
cd /etc/nginx
ls -ltr // list files and directories in a long format
sudo vim nginx.conf
```

- Add HTTPS server - Uncomment code block or edit/copy, so it look like this:
```
server {
  listen 443 ssl http2;
  listen [::]:443 ssl http2;
  
  # Replace domains with your own
  server_name kinergize.com www.kinergize.com;
  root /usr/share/nginx/html;
  
  # Replace cert paths with your own
  ssl_certificate "/etc/letsencrypt/live/kinergize.com/fullchain.pem";
  ssl_certificate_key "/etc/letsencrypt/live/kinergize.com/privkey.pem";
  ssl_session_cache shared:SSL:1m;
  ssl_session_timeout 10m;
  ssl_protocols TLSv1 TLSv1.1 TLSv1.2;
  ssl_ciphers HIGH:!aNULL:!MD5;
  ssl_prefer_server_ciphers on;
  
  # Load configuration files for the default server block.
  include /etc/nginx/default.d/*.conf;
  
  error_page 404 /404.html;
    location = /40x.html {
  }
  
  error_page 500 502 503 504 /50x.html;
    location = /50x.html {
  }
}
```

- Restart Nginx and check if it runs on HTTPS now
```
sudo service nginx restart
sudo service nginx status
openssl s_client -connect kinergize.com:443 -servername kinergize.com -showcerts // Validate SSL connection (certificates right?)
```

- Run certificate auto renewal cron job:
```
echo "0 0,12 * * * root /opt/certbot/bin/python -c 'import random; import time; time.sleep(random.random() * 3600)' && certbot renew -q" | sudo tee -a /etc/crontab > /dev/null
sudo certbot renew --dry-run
```

- Check running cron jobs (Ctrl + X to exit)  
```
sudo nano /etc/crontab
```

- Redirect HTTP to HTTPS - add this to 80 port server in nginx.conf
```
location / {
  return 301 https://$host$request_uri;
}
```

- Forward https requests into your app port:

*Here at port 3000:*
```
location  / {
  add_header 'Access-Control-Allow-Origin' '*'; 
  proxy_set_header X-Forwarded-Host $host; 
  proxy_set_header X-Forwarded-Server $host;
  proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
  proxy_pass http://localhost:3000;
}
```

- Redirect to single domain as server_name use any domains to be redirected from (same for http - port 80):
```
server {
    listen       443 ssl http2;
    listen       [::]:443 ssl http2;
    server_name  kinergize.com www.kinergize.com kinergize.pl www.kinergize.pl;
    root         /usr/share/nginx/html;
    
    include /etc/nginx/default.d/*.conf;
    
    ssl_certificate "/etc/letsencrypt/live/kinergize.com/fullchain.pem";
    ssl_certificate_key "/etc/letsencrypt/live/kinergize.com/privkey.pem";
    
    return 301 https://kinergize.me$request_uri;
}
```

- TO DO: setup github actions?

## Links
- https://nikhilpurwant.com/post/tech-lets-encrypt-on-ec2/
- https://codewithmukesh.com/blog/securing-dotnet-webapi-with-amazon-cognito/
- https://www.youtube.com/watch?v=cdZlakbZ8Cg

# Setup Amazon SES for Email Marketing

Pricing:
- 3,000 outbound and inbound messages free per month, then $0.10/1000 emails + $0.12 for each GB of attachments you send + E2C outbound charges
