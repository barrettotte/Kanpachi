import os, pyodbc, paramiko
from logger import Logger
import utils as utils

class Ibmi():


    def __init__(self, out_path, logger):
        self.logger = Logger(out_path + os.sep + 'log.txt') if not logger else logger
        self.user = None
        self.sftp = None
        self.db2 = None
        self.host = None


    def __del__(self):
        self.disconnect()
        self.logger.log("All IBMi connection(s) for '{}' closed.".format(self.host))


    def __str__(self):
         # TODO: add more info to dump
        return "IBMi:\n  {}\n  {}".format('host: '+ self.host, 'user: ' + self.user)


    def connect(self, host, user, password, timeout=2500):
        self.connect_sftp(host, user, password, timeout)
        self.connect_db2(host, user, password, timeout)


    def connect_sftp(self, host, user, password, timeout=2500):
        #if not self.ftp_client:
        #    self.ftp_client = ftplib.FTP()
        if not self.sftp:
            self.sftp = pysftp.Connection(host, username=user, password=password)
        try:
            #self.ftp_client.connect(host, timeout=timeout)
            self.logger.log("SFTP: Connection to '{}' established.".format(host))
        except Exception as e:
            self.raise_exception("SFTP: Connection failed; Cannot connect to host '{}'.".format(host), False, e)


    def connect_db2(self, host, user, password, timeout=2500):
        try:
            # TODO: connect to DB2
            #self.logger.log("DB2: Connection to '{}' established.".format(host))
            pass
        except Exception as e:
            self.raise_exception("DB2: Connection failed; Cannot connect to host '{}'.".format(host), False, e)


    def disconnect(self):
        self.user = None
        self.host = None
        if self.sftp: self.disconnect_sftp()
        if self.db2:  self.disconnect_db2()


    def disconnect_sftp(self):
        #self.ftp_client.quit()
        #self.ftp_client = None
        self.sftp.close()
        self.sftp = None
        self.logger.log("SFTP: Connection to '{}' closed.".format(self.host))


    def disconnect_db2(self):
        # TODO: disconnect db2
        self.db2 = None
        self.logger.log("DB2: Connection to '{}' closed.".format(self.host))


    # def login(self, creds):
    #     try:
    #         self.ftp_client.login(creds['user'], creds['password'])
    #         resp = self.keep_alive()
    #         if resp:
    #             self.logger.log("SFTP: Logged in successfully as '{}' to '{}'.".format(creds['user'], self.host))
    #             self.user = creds['user']
    #             return resp
    #     except ftplib.all_errors as e:
    #         self.raise_exception("SFTP: Log in unsuccessful as '{}' to '{}'.".format(creds['user'], self.host), False, e)


    def keep_alive(self):
        try:
            return self.exec_sftp(['NOOP'], False)
        except Exception as e:
            self.raise_exception('SFTP', 'Failed to keep connection alive', True, e)
        #except ftplib.all_errors as e:
        #    self.ftp_exception_handler('SFTP: Failed to keep connection alive.', e)


    def exec_sftp(self, cmds, log_it=True):
        try:
            for cmd in cmds:
                if log_it:
                    self.logger.log("SFTP: Executing command '{}'.".format(cmd))
                #self.ftp_client.sendcmd(cmd)
        #except ftplib.all_errors as e:
        #    self.ftp_exception_handler("FTP: Failed to execute command '{}'.".format(cmd), e)
        except Exception as e:
            self.raise_exception('SFTP: Unexpected exception occurred.', True, e)


    #def ftp_exception_handler(self, msg, e=None):
    #    msg = self.FTP_E_DICT[str(e).split(None, 1)[0]] if e else msg
    #    self.logger.log("FTP Exception: {} ; {}".format(msg, e))


    def raise_exception(self, prefix='ERROR', msg='Unexpected exception occurred', is_fatal=False, e=None):
        self.logger.log("{} ; {}".format(msg, e))
        if is_fatal:
            self.disconnect()
            exit(str(e))

