# climatepi

Use a Raspberry Pi, with a BME280 sensor to measure temperature, pressure, and humidity. Application reads the sensor data, saves it in a Sqlite DB, and hosts a small website on the Pi to view the data.

### STEPS FOR DEPLOYMENT TO PI
____

### INSTALL RASPBERRY PI OS ON PI

____
### INSTALL .NET ON RASPBERRY PI
 - Follow instructions in this repo:
 - https://github.com/dahln/dotnet-installer
____
### SETUP APPLICATION DIRECTORY
### IN THE 'PI' USER HOME DIRECTORY (it just makes it easier)
```
sudo mkdir climatepi
```
____
### COPY PROJECT FROM DEV MACHINE TO RASPBERRY PI
On "Dev" machine, from "Server" folder
```
dotnet clean
dotnet build
dotnet publish
scp -r .\bin\Debug\net6.0\publish\\* pi@climatepi:/home/pi/climatepi
```
 - Where 'climatepiiot' is the name of the raspberry pi. Can use the ip address as well

____
### SETUP SERVER-- INSTALL THIS STUFF
```
sudo apt install nginx -y
sudo apt install ufw -y
sudo ufw allow 'Nginx Full'
sudo ufw allow 'OpenSSH'
sudo ufw --force enable
```
In browser, navigate to the ip address or name of the Pi. This will verify if Nginx is serving up a webpage and allowing access through the firewall

____
### CREATE APPLICATION KESTREL SERVICE
### CREATE THIS FILE, AND THEN ADD THE CONTENT BELOW
```
sudo nano /etc/systemd/system/climatepi.service
```

```
[Unit]
Description=climatepi iot process

[Service]
WorkingDirectory=/home/pi/climatepi
ExecStart=/opt/dotnet/dotnet /home/pi/climatepi/climatepi.Server.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=climatepi-iot
User=pi

[Install]
WantedBy=multi-user.target
```


____
### ENABLE THE NEW KESTREL SERVICE
```
sudo systemctl enable climatepi.service
```
```
sudo systemctl restart climatepi.service
```
____
### CREATE NGINX CONFIG FOR NEW APP
### EDIT THE EXISTING DEFAULT NGINX CONFIG WITH CODE BELOW
```
sudo nano /etc/nginx/sites-enabled/default
```

```
upstream climatepi_server {
    server localhost:5000;
}
server {
        listen 80 default_server;
        listen [::]:80 default_server;

        server_name _;
        location / {
                proxy_pass         http://climatepi_server;
                proxy_redirect     off;
                proxy_set_header   Host $host;
                proxy_set_header   X-Real-IP $remote_addr;
                proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
                proxy_set_header   X-Forwarded-Proto $scheme;
                proxy_set_header   X-Forwarded-Host $server_name;
                client_max_body_size 32M;
        }
}
```


____
### CHECK THE APP CONFIG AND RESTART NGINX
```
sudo nginx -t
sudo systemctl restart nginx
```

____
### IN BROWSER, NAVIGATE TO IP ADDRESS OF RASPBERRY PI

____
### HELPFUL TROUBLESHOOTING COMMANDS

```
sudo systemctl enable climatepi.service
sudo systemctl restart climatepi.service
sudo systemctl status climatepi.service
sudo journalctl -u climatepi.service
sudo systemctl daemon-reload
sudo systemctl status nginx
sudo journalctl -u nginx

vcgencmd measure_temp
```


____
### REFERENCES
 - https://docs.microsoft.com/en-us/dotnet/iot/

