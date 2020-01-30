import ftplib, os
from logger import Logger
import utils as utils

class Ibmi():

    FTP_E_DICT = {
        '530': 'Connection failed: bad credentials'
    }


    def __init__(self, out_path, logger):
        self.logger = Logger(out_path + os.sep + 'log.txt') if not logger else logger
        self.is_logged_in = False
        self.ftp_client = None
        self.host = None
        self.user = None


    def __del__(self):
        self.disconnect()
        self.logger.log('All IBMi connection(s) closed.')


    def __str__(self): # TODO: add more info to dump
        return "IBMi - {}".format('logged in' if self.is_logged_in else 'logged out')


    def connect(self, host, timeout=2500):
        if not self.ftp_client:
            self.ftp_client = ftplib.FTP()
        try:
            self.ftp_client.connect(host, timeout=timeout)
            self.logger.log("FTP connection to '{}' established.".format(host))
        except Exception as e:
            self.raise_exception("Connection failed: cannot connect to host '{}'.".format(host), False, e)


    def disconnect(self):
        self.is_logged_in = False
        self.host = None
        self.user = None
        if self.ftp_client:
            self.ftp_client.quit()
            self.ftp_client = None
            self.logger.log('FTP connection closed.')


    def login(self, creds):
        try:
            self.ftp_client.login(creds['user'], creds['password'])
            resp = self.keep_alive()
            if resp:
                self.logger.log("Logged in successfully as '{}'.".format(creds['user']))
                self.is_logged_in = True
                return resp
        except ftplib.all_errors as e:
            self.raise_exception("Log in unsuccessful as '{}'.".format(creds['user']), False, e)


    def keep_alive(self):
        try:
            return self.exec_ftp(['NOOP'], False)
        except ftplib.all_errors as e:
            self.ftp_exception_handler('Failed to keep connection alive.', e)


    def exec_ftp(self, cmds, log_it=True):
        try:
            for cmd in cmds:
                if log_it:
                    self.logger.log("Executing FTP command '{}'.".format(cmd))
                self.ftp_client.sendcmd(cmd)
        except ftplib.all_errors as e:
            self.ftp_exception_handler("Failed to execute command '{}'.".format(cmd), e)
        except Exception as e2:
            self.raise_exception('Unexpected exception occurred.', True, e2)


    def ftp_exception_handler(self, msg, e=None):
        if e:
            msg = self.FTP_E_DICT[str(e).split(None, 1)[0]]
        self.logger.log("FTP Exception: {} ; {}".format(msg, e))


    def raise_exception(self, msg='Unexpected exception occurred', is_fatal=False, e=None):
        self.logger.log("{} ; {}".format(msg, e))
        if is_fatal:
            self.disconnect()
            exit(str(e))

