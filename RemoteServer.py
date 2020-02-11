import socket, subprocess, sys
import paramiko

class RemoteServer():


    def __init__(self, ssh_key, host, **kwargs):
        optional = {'username': None, 'password': None, 'ssh_port': 22, 'ssh_timeout': 16}
        optional.update(kwargs)
        self.host = host
        self.username = optional['username']
        self.password = optional['password']
        self.ssh_port = optional['ssh_port']
        self.ssh_client = None
        self.sftp_client = None

    def __del__(self):
        self.disconnect()

    def connect(self):
        pass

    def exec_cmd(self, cmd, timeout=20):
        pass

    def disconnect(self):
        if self.ssh_client:
            self.ssh_client.close()
        if self.sftp_client:
            self.sftp_client.close()
