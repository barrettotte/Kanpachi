# Test Python SFTP, never used it before

import json, sys, os, pysftp
import utils as utils
from RemoteServer import RemoteServer


def get_credential(config,key):
    if key in config:
       return config[key] 
    return utils.required_input("Enter '{}': ".format(key))


# get config
with open('config.json', 'r') as f:
    config = json.load(f)
    host = get_credential(config, 'host')
    ssh_port = config['ports']['ssh']

    user = get_credential(config, 'user')
    pwd  = utils.required_pass()

# 'C:\\Users\\otteb\\.ssh'
server = RemoteServer(None, 'DEV400', user=user, password=pwd)
server.connect()

server.exec_command('ls')
server.exec_command("system 'DSPLIBL'")
server.exec_command("xyz")

