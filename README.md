# HomeAssistantCheck
Checking Home Assistant internal and external connections 

I am using home assistant for my home automation. I use the RPI 3 with SD card. This setup fails sometimes. I can not reach the home assitant web page. When home assistant can not be reached I wonder what is happening. So I made a small program to make sense of what is happening. It checks the local and external connection to home assitant abd check if the ports of the internal and external URL are reachable.
You have to change the external URL and port. You can also change the local address *homeassistant.local" and port. The program settings will be stored in the registry, so the program will remember your settings.
This program helps me to see what connection problems there are. Is the external Ip reachable? is the external port open. Has the system started yet (if not, the local port is not open and the external port is not reachable). I hope you also find this program usefull.

This is what the program does:
1)  ping 8.8.8.8 to check if there is an internet connection
2)  check if you can get the local ip for "homeassistant.local"
3)  ping the local ip address
4)  check if the local port is open
5)  check if the program can get the ip number for the external DNS name
6)  check if the program can ping the external ip address
7)  check if the external port from that IP address is open


<img width="373" alt="Schermafdruk 2024-08-19 01 39 51" src="https://github.com/user-attachments/assets/0fc45ad7-bb59-4a0e-852a-407ccedacd86">
