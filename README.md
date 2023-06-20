# Blazor App Template

## Project Setup

- Clone the repository
- Rename all occurences of "MyApp" to desired name in:
  - project names
  - folder names
  - find and replace all occurences of "MyApp" keyword in the solution

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

### Configure Amazon SES

&nbsp;&nbsp;&nbsp;&nbsp; *From E2C -> $0.12/1GB sent always; 62,000/month free, then $0.10/1000 emails*

- Open AWS Console and navigate to - Amazon Simple Email Service
- Navigate to - Verified Identities
- Click - Create identity
- Set Identity type as - Domain
- Provide your - Domain
- Select - Use a custom MAIL FROM domain
- Add - MAIL FROM domain - ex. ses
- In - Advanced DKIM settings - set Identity type as - Easy DKIM, RSA_1024_BIT, Create
- Copy all CNAME DNS records - name and value (name without domain suffix, ex. 562jsdfsdfssdfpsdfsdfnj3csdf._domainkey) and add it in your domain provider dns settings
- Copy MX similar way - but trailing value number (ex. 10) exclude from value and paste to DNS priority
- Copy TXT similar way - value as is
- Wait 24h-72h till verified records

### Add AWS Cognito authentication

&nbsp;&nbsp;&nbsp;&nbsp; *50,000 free monthly active users (MAUs). Per month. Active user - any activity related to signin, get token, etc. Then $0.0055 / month / user till 900k, and then less..*

- Open AWS Console and navigate to AWS Cognito
- In 1st Configure sign-in experience:
  - In Provider types leave selected as - Cognito user pool
  - In Cognito user pool sign-in options select - User name, Email
  - Select - Allow users to sign in with a preferred user name
- In 2nd Configure security requirements:
  - Multi-factor authentication - No MFA
  - User account recovery - leave as is - Email
- In 3rd Configure sign-up experience:
  - Leave all as email
  - In Additional required attributes select desired:
    - For instance - name
- In 4th Configure message delivery:
  - Leave Email provider as - Amazon SES - From E2C -> $0.12/1GB sent always; 62,000/month free, then $0.10/1000 emails
### Create AWS E2C instance
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
  
### Add SSL certificate
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

### Reverse proxy nginx into app port

Here at port 3000:
```
location  / {
  add_header 'Access-Control-Allow-Origin' '*'; 
  proxy_set_header X-Forwarded-Host $host; 
  proxy_set_header X-Forwarded-Server $host;
  proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
  proxy_pass http://localhost:3000;
}
```

- TO DO: setup github actions?

## Links
- https://nikhilpurwant.com/post/tech-lets-encrypt-on-ec2/
