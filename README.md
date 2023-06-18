# Blazor App Template

## Project Setup

- Clone the repository
- Rename all occurences of "MyApp" to desired name in:
  - project names
  - folder names
  - find and replace all occurences of "MyApp" keyword in the solution

## Production Environment Setup

### Add Azure B2C authentication
### Create AWS E2C instance
### Install Docker
### Add elastic IP

**IMPORTANT!:** https://repost.aws/knowledge-center/elastic-ip-charges
> An Elastic IP address doesn’t incur charges as long as all the following conditions are true:
> - The Elastic IP address is associated with an EC2 instance.
> - The instance associated with the Elastic IP address is running.
> - The instance has only one Elastic IP address attached to it.
> - The Elastic IP address is associated with an attached network interface. For more information, see Network interface basics.

- Allocate Elastic IP address - it will prevent changing ip of instance on rerun
- Associate Elastic IP address with an E2C instance

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
