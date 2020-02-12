# Connect to remote server with SSH and SFTP

import paramiko, select


class RemoteServer():

    def __init__(self, ssh_key, host, ssh_port=22):
        self.host = host
        self.ssh_port = ssh_port
        self.private_key = paramiko.RSAKey.from_private_key_file(ssh_key) if ssh_key else None
        self.ssh_client = paramiko.SSHClient()
        self.ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        self.sftp_client = None # TODO:
        

    def __del__(self):
        self.disconnect()


    def connect(self, user, pwd):
        try:
            if self.ssh_client:
                # TODO: ping server
                self.user = user
                self.ssh_client.connect(self.host, self.ssh_port, user, pwd)
                
        except paramiko.AuthenticationException:
            print("Authentication failed when connecting to '{}'.".format(self.host))


    def exec_command(self, cmd, timeout=20):
        out, err, status = '','',''
        try:
            channel = self.ssh_client.get_transport().open_session()
            channel.exec_command(cmd)
            while True:
                if channel.recv_ready():
                    out += channel.recv(4096).decode('utf-8')
                if channel.recv_stderr_ready():
                    err += channel.recv_stderr(4096).decode('utf-8')
                if channel.exit_status_ready():
                    status = channel.recv_exit_status()
                    break
            channel.close()
        except paramiko.SSHException as ssh_error:
            print('ERROR: ' + str(ssh_error))
        return (''.join(out), ''.join(err), str(status))

    
    def keep_alive(self):
        try:
            transport = self.ssh_client.get_transport()
            transport.send_ignore()
        except EOFError:
            return False # connection closed
        return True


    def disconnect(self):
        if self.ssh_client:
            self.ssh_client.close()
        if self.sftp_client:
            self.sftp_client.close()
        self.host = None
        self.user = None

