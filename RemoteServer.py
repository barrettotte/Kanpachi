# Connect to remote server with SSH and SFTP

import paramiko


class RemoteServer():

    def __init__(self, ssh_key, host, **kwargs):
        optional = {'username': None, 'password': None, 'ssh_port': 22, 'ssh_timeout': 16}
        optional.update(kwargs)
        self.host = host
        self.username = optional['username']
        self.password = optional['password']
        self.ssh_port = optional['ssh_port']
        self.sftp_client = None

        self.private_key = paramiko.RSAKey.from_private_key_file(ssh_key) if ssh_key else None
        self.ssh_client = paramiko.SSHClient()
        self.ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        self.shell = None

    def __del__(self):
        self.disconnect()


    def connect(self):
        try:
            if self.ssh_client:
                # TODO: ping server
                self.ssh_client.connect(self.host, self.ssh_port, self.username, self.password)
        except paramiko.AuthenticationException:
            print("Authentication failed when connecting to '{}'.".format(self.host))


    def exec_command(self, cmd, timeout=20):
        print("COMMAND: " + cmd)
        buff_size = -1
        try:
            stdin, stdout, stderr = self.ssh_client.exec_command(cmd, get_pty=True)
            for line in iter(stdout.readline, ''):
                print(line, end='')

        except paramiko.SSHException as ssh_error:
            print('ERROR: ' + str(ssh_error))


    def disconnect(self):
        if self.ssh_client:
            self.ssh_client.close()
        if self.sftp_client:
            self.sftp_client.close()
        self.host = None

