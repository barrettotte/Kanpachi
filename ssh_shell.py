# Test Python SSH module, never used it before

import json, sys, os, getpass
import utils as utils
from RemoteServer import RemoteServer

def get_property(config,key):
    if key in config: 
        return config[key] 
    return input("Enter '{}': ".format(key))

with open('config.json', 'r') as f:
    config = json.load(f)
    host = get_property(config, 'host')
    user = get_property(config, 'user')
    pwd  = getpass.getpass('Enter password: ')

server = RemoteServer(None, host=host)
server.connect(user, pwd)
#server.exec_command('ls')
#server.exec_command("system 'DSPLIBL'")
#server.exec_command("xyz")
server.keep_alive()

try:
    server.open_shell()
except Exception as e:
    print('Error occurred ' + str(e))
finally:
    server.disconnect()
