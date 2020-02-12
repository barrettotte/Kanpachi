# Test Python SSH and SFTP, never used it before

import json, sys, os
import utils as utils
from RemoteServer import RemoteServer


def get_credential(config,key):
    if key in config:
       return config[key] 
    return utils.required_input("Enter '{}': ".format(key))


# get config
with open('../config.json', 'r') as f:
    config = json.load(f)
    host = get_credential(config, 'host')
    ssh_port = config['ports']['ssh']

    user = get_credential(config, 'user')
    pwd  = utils.required_pass()


server = RemoteServer(None, host=host, user=user, password=pwd)
server.connect()

#server.exec_command('ls')
#server.exec_command("system 'DSPLIBL'")
#server.exec_command("xyz")

try:
    while True:
        cmd = input('> ')
        if cmd != 'exit':
            server.exec_command(cmd)
            break # TODO: testing
        else:
            print('SSH session ended.')
            break
except Exception as e:
    print(e)
finally:
    server.disconnect()

