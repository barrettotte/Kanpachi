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
    while True:
        cmd = input('> ')
        if cmd != 'exit':
            out, err, status = server.exec_command(cmd)
            if len(out) > 0: print(out)
            if len(err) > 0: print(err)
            print('status: ' + status)
        else:
            print('SSH session ended.')
            break
except KeyboardInterrupt:
    pass 
except Exception as e:
    print('Error occurred during SSH session. ({})'.format(str(e)))
finally:
    server.disconnect()

