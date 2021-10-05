# daedalus

https://docs.microsoft.com/en-us/dotnet/iot/

### On Pi, in Home "pi" user "Home" directory:
mkdir daedalus

### From dev machine, in project directory.
dotnet publish

scp -r .\bin\Debug\net5.0\publish\\* pi@daedalusio:/home/pi/daedalus

### Run on Pi:
dotnet daedalus.iot.dll &

The '&' makes it run in the background

#### OR 
Create a background service:

Create file and add content:

sudo nano /etc/systemd/system/daedalus.service

<pre>
<code>
[Unit]
Description=daedalus iot process

[Service]
WorkingDirectory=/home/pi/daedalus
ExecStart=/opt/dotnet/dotnet /home/pi/daedalus/daedalus.iot.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=daedalus-iot
User=pi

[Install]
WantedBy=multi-user.target

</pre>
</code>

<pre>
<code>
sudo systemctl enable daedalus.service
sudo systemctl restart daedalus.service
sudo systemctl status daedalus.service
</code>
</pre>

#### Re-Arch Ideas
1. Switching the server side to use Azure Functions and HTTP triggers.
2. If the Web App is converted to Azure Functions, the Blazor app could be converted into a static web app, that uses an Azure function to query and display data.




