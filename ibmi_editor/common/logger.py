# Basic wrapper over logging module

import os, datetime, logging

class Logger():

    def __init__(self, file_name='temp.log', lvl='INFO'):
        self.out_path = os.path.abspath(file_name)
        self.mkdir_ine(os.path.dirname(self.out_path))
        #logging.basicConfig(filename=self.out_path, format='%(asctime)s%(levelname)s:%(message)s', level=lvl)

    def mkdir_ine(self, dir_path):
        if not os.path.exists(dir_path): 
            os.makedirs(dir_path)

    def log(self, msg, level='INFO', to_console=True):
        try:
            if to_console:
                print(msg)
            #logging.log(level=logging._nameToLevel[level.upper()], msg=msg)
            # TODO: write your own logger since the "standard" one keeps failing 
        except Exception as e:
            print("Could not write to log.\n  {}".format(e))


    def logged_raise(self, msg, e=None):
        s = "{}.\n  {}".format(msg, str(e))
        self.log(s, level='ERROR')
        raise Exception(s) # TODO: stacktrace ?

