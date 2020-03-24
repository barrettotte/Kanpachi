# Common utils used in tests

import json, sys, os, getpass

def get_creds():
    with open(os.path.abspath('config.json'), 'r') as f:
        config = json.load(f)
        host = config['host'] if 'host' in config else input("Enter host: ")
        user = config['user'] if 'user' in config else input("Enter user: ")
        pwd  = getpass.getpass('Enter password: ')
    return (host,user,pwd)
