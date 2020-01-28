import os, datetime

class Logger():

    def __init__(self, file_name='log.txt'):
        self.out_path = os.path.abspath(file_name)
        self.mkdir_ine(self.out_path)


    def mkdir_ine(self, dir_path):
        if not os.path.exists(dir_path): 
            os.makedirs(dir_path)


    def log(self, content, file_path='./log.txt', to_console=True):
        try:
            if to_console: 
                print(content)
            with open(file_path, 'a+') as f:
                f.write("[{}] {}\n".format(self.get_timestamp(), content))
        except Exception as e:
            print("Could not write to log.\n  {}".format(e))


    def get_timestamp(self):
        return str(datetime.datetime.now())

