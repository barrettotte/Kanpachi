import json,os
from ibmi import Ibmi
from logger import Logger
import utils as utils


def get_credential(config,key):
    if key in config:
       return config[key] 
    return utils.required_input("Enter '{}': ".format(key))


def main():
    with open('config.json', 'r') as f:
        config = json.load(f)

    host = get_credential(config, 'host')
    user = get_credential(config, 'user')
    pwd  = utils.required_pass()

    logger = Logger() # optional inject logger

    box = Ibmi('out', logger)
    box.connect(host, user, pwd)
    #box.login({'user': user, 'password': pwd})

    box.keep_alive()

    box.disconnect()


if __name__ == "__main__": main()