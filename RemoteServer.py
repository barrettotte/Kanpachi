# Connect to remote server with SSH and SFTP

import paramiko, socket, os, time


# Fix Paramiko decoding error
# https://stackoverflow.com/questions/34559158/paramiko-1-16-0-readlines-decode-error
def u_override(s, encoding='utf8'): # cast bytes or unicode to unicode
    if isinstance(s, bytes):
        try:
            return s.decode(encoding)
        except UnicodeDecodeError:
            try:
                return s.decode('windows-1252')
            except UnicodeDecodeError:
                return s.decode('ISO-8859-1', errors='ignore')
    elif isinstance(s, str):
        return s.encode('utf8')
    else:
        raise TypeError("Expected unicode or bytes, got %r" % s)

# Override the module method
paramiko.py3compat.u = u_override


class RemoteServer():

    def __init__(self, ssh_key, host, ssh_port=22, sftp_dir='cache'):
        self.host = host
        self.ssh_port = ssh_port
        self.private_key = paramiko.RSAKey.from_private_key_file(ssh_key) if ssh_key else None
        self.ssh_client = paramiko.SSHClient()
        self.ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        
        self.sftp_client = None
        if not os.path.exists(sftp_dir):
            os.makedirs(sftp_dir)
        self.sftp_dir = os.path.abspath(sftp_dir)


    def __del__(self):
        self.disconnect()


    def is_port_open(self, host, port):
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        try:
            return sock.connect_ex((host, int(port))) == 0
        except socket.timeout:
            return False
        except socket.error:
            return False
        finally:
            sock.close()


    def connect(self, user, pwd):
        try:
            if self.ssh_client:
                if not self.is_port_open(self.host, self.ssh_port):
                    raise Exception("Port '{}' for host '{}' is not open.".format(self.ssh_port, self.host))
                self.user = user
                self.ssh_client.connect(self.host, self.ssh_port, user, pwd)
                
                if not self.keep_alive():
                    raise Exception("SFTP client could not be created.")
                self.sftp_client = paramiko.SFTPClient.from_transport(self.ssh_client.get_transport())
            else:
                raise Exception('SSH Client not initialized.')
        except paramiko.AuthenticationException:
            raise Exception("Authentication failed when connecting to '{}'.".format(self.host))


    def exec_command(self, cmd):
        out, err, status = '','',''
        try:
            channel = self.ssh_client.get_transport().open_session()
            channel.exec_command(cmd)
            time.sleep(0.5)
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
            raise Exception("SSH exception occurred {}\n  While executing command '{}'".format(str(ssh_error), cmd))
        return (''.join(out), ''.join(err), str(status))

    
    def download_file(self, remote_path, local_path=None, timeout=20):
        try:
            if not local_path:
                split = remote_path.split('/')
                out_dir = os.path.abspath(self.sftp_dir + os.sep.join(split[:-1]))
            else:
                split = local_path.split('/')
                out_dir = os.path.abspath(os.sep.join(split[:-1]))
            out_file = split[-1]

            if not os.path.exists(out_dir):
                os.makedirs(out_dir)

            content = self.read_file(remote_path, timeout=timeout)
            print('writing ' + out_dir + os.sep + out_file)
            with open(os.path.abspath(out_dir) + os.sep + out_file, 'w+', encoding='utf8') as f:
                f.write(content)
        except Exception:
            raise Exception("Error occurred downloading file '{}'".format(remote_path))
        return content


    def read_file(self, path, timeout=20):
        try:
            content = ''
            remote_file = self.sftp_client.open(path, 'rb')
            for line in remote_file:
                #content += line.decode('utf8', 'ignore')
                #print(line)
                content += line.decode('utf-8', 'ignore')
        except Exception:
            raise Exception("Error occurred opening file '{}'.".format(path))
        return content

    
    def keep_alive(self):
        transport = self.ssh_client.get_transport()
        if transport is not None:
            try:
                transport.send_ignore()
                return True
            except EOFError: 
                pass
        return False # connection is closed


    def is_connected(self):
        if self.ssh_client.get_transport() is not None:
            return self.ssh_client.get_transport().is_active()
        return False


    def disconnect(self):
        if self.ssh_client:
            self.ssh_client.close()
        if self.sftp_client:
            self.sftp_client.close()
        self.host = None
        self.user = None

