# daedalus

### INSTALL RASPBERRY PI OS ON PI

____
### INSTALL .NET ON RASPBERRY PI
 - Follow instructions in this repo:
 - https://github.com/pjgpetecodes/dotnet5pi
____
### SETUP APPLICATION DIRECTORY
### IN THE 'PI' USER HOME DIRECTORY (it just makes it easier)
 - sudo mkdir daedalus

____
### COPY PROJECT FROM DEV MACHINE TO RASPBERRY PI
 - On "Dev" machine, from "Server" folder
 - dotnet clean
 - dotnet build
 - dotnet publish
 - scp -r .\bin\Debug\net6.0\publish\\* pi@daedalusiot:/home/pi/daedalus
   - Where 'daedalusiot' is the name of the raspberry pi. Can use the ip address as well

____
### SETUP SERVER-- INSTALL THIS STUFF
 - sudo apt install nginx -y
 - sudo apt install ufw -y
 - sudo ufw allow 'Nginx Full'
 - sudo ufw allow 'OpenSSH'
 - sudo ufw --force enable
 - In browser, navigate to the ip address or name of the Pi. This will verify if Nginx is serving up a webpage and allowing access through the firewall

____
### CREATE APPLICATION KESTREL SERVICE
### CREATE THIS FILE, AND THEN ADD THE CONTENT BELOW
 - sudo nano /etc/systemd/system/daedalus.service

<pre>
<code>
[Unit]
Description=daedalus iot process

[Service]
WorkingDirectory=/home/pi/daedalus
ExecStart=/opt/dotnet/dotnet /home/pi/daedalus/daedalus.Server.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=daedalus-iot
User=pi

[Install]
WantedBy=multi-user.target
</code>
</pre>

____
### ENABLE THE NEW KESTREL SERVICE
 - sudo systemctl enable daedalus.service
 - sudo systemctl restart daedalus.service

____
### CREATE NGINX CONFIG FOR NEW APP
### EDIT THE EXISTING DEFAULT NGINX CONFIG WITH CODE BELOW
 - sudo nano /etc/nginx/sites-enabled/default

<pre>
<code>
upstream daedalus_server {
    server localhost:5000;
}
server {
        listen 80 default_server;
        listen [::]:80 default_server;

        server_name _;
        location / {
                proxy_pass         http://daedalus_server;
                proxy_redirect     off;
                proxy_set_header   Host $host;
                proxy_set_header   X-Real-IP $remote_addr;
                proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
                proxy_set_header   X-Forwarded-Proto $scheme;
                proxy_set_header   X-Forwarded-Host $server_name;
                client_max_body_size 32M;
        }
}
</code>
</pre>

____
### CHECK THE APP CONFIG AND RESTART NGINX
 - sudo nginx -t
 - sudo systemctl restart nginx

____
### IN BROWSER, NAVIGATE TO IP ADDRESS OF RASPBERRY PI

____
### HELPFUL TROUBLESHOOTING COMMANDS
<pre>
<code>
sudo systemctl enable daedalus.service
sudo systemctl restart daedalus.service
sudo systemctl status daedalus.service
sudo journalctl -u daedalus.service
sudo systemctl daemon-reload
sudo systemctl status nginx
sudo journalctl -u nginx
</code>
</pre>

____
### REFERENCES
 - https://docs.microsoft.com/en-us/dotnet/iot/

