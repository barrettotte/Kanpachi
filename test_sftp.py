# Testing SFTP

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

try:
    if server.keep_alive():
        print(server.read_file('/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle'))
        server.download_file('/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle')
        server.download_file('/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle', 'cache/hello.rpgle')
        print(server.read_file('/QSYS.LIB/BOLIB.LIB/QRPGLESRC.FILE/FIZZBUZZ.MBR'))
finally:
    server.disconnect()
