# Connect to remote server with SSH and SFTP

import paramiko, socket, os, time
from common.logger import Logger

class RemoteServer():

    def __init__(self, ssh_key, host, **kwargs):
        opt_args = {
            'ssh_port': 22,
            'buffer_size': 8192,
            'encoding': 'utf-8',
            'banner_timeout': 1000,
            'sftp_dir': 'cache',
            'log_name': 'remote_server.log'
        }
        opt_args.update(kwargs)
        self.ssh_port = opt_args['ssh_port']
        self.buffer_size = opt_args['buffer_size']
        self.encoding = opt_args['encoding']
        self.banner_timeout = opt_args['banner_timeout']
        self.sftp_dir = opt_args['sftp_dir']

        self.host = host
        self.private_key = paramiko.RSAKey.from_private_key_file(ssh_key) if ssh_key else None
        self.ssh_client = paramiko.SSHClient()
        self.ssh_client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        
        if not os.path.exists(self.sftp_dir):
            self.logger.log("Path '{}' does not exist. Creating directory...".format(self.sftp_dir))
            os.makedirs(self.sftp_dir)
        self.sftp_dir = os.path.abspath(self.sftp_dir)
        self.sftp_client = None

        self.logger = Logger()


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


    def connect(self, user, pwd, timeout=20):
        try:
            if self.ssh_client:
                if not self.is_port_open(self.host, self.ssh_port):
                    self.logged_raise("Port '{}' for host '{}' is not open.".format(self.ssh_port, self.host))
                self.user = user
                self.ssh_client.connect(self.host, self.ssh_port, user, pwd, 
                    timeout=timeout, banner_timeout=self.banner_timeout)

                if not self.keep_alive():
                    self.logger.logged_raise('SFTP client could not be created.')
                
                transport = self.ssh_client.get_transport()
                transport.banner_timeout = self.banner_timeout
                self.sftp_client = paramiko.SFTPClient.from_transport(transport)

                self.logger.log("Successfully connected to '{}' as user '{}'".format(self.host, self.user))
            else:
                self.logger.logged_raise('SSH Client not initialized.')
        except paramiko.AuthenticationException:
            self.logger.logged_raise("Authentication failed when connecting to '{}'.".format(self.host))
        # Probably will need additional catches...


    def open_shell(self, timeout=20):
        cmd = ''
        shell = self.ssh_client.invoke_shell()
        prompt = '{}@{} ~> '.format(self.user, self.host)
        try:            
            while cmd != 'exit':
                cmd = input(prompt)
                out, err, status = self.exec_command(cmd, timeout)
                self.logger.log(prompt + cmd, to_console=False)
                if len(out) > 0:
                    print(out)
                elif len(err) > 0 and status != 0: 
                    print(err)
        except Exception as e:
            self.logged_raise("Unexpected error occurred in interactive shell.", e)
        finally:
            shell.close()
            self.logger.log('Interactive shell to {} closed.'.format(self.host))


    def exec_command(self, cmd, timeout=20):
        out, err, status = '', '', ''
        channel = self.ssh_client.get_transport().open_session()
        try:
            channel.settimeout(timeout)
            channel.exec_command(cmd)
            time.sleep(0.5)
            while True:
                if channel.recv_ready():
                    out += channel.recv(self.buffer_size).decode(self.encoding)
                if channel.recv_stderr_ready():
                    err += channel.recv_stderr(self.buffer_size).decode(self.encoding)
                if channel.exit_status_ready():
                    status = channel.recv_exit_status()
                    break
        except paramiko.SSHException as ssh_error:
            self.logger.logged_raise("SSH exception occurred {}\n  While executing command '{}'"
                .format(str(ssh_error), cmd))
        finally:
            channel.close()
        return (''.join(out), ''.join(err), str(status))

    
    def download_file(self, remote_path, local_path=None, fallback_codec='windows-1252'):
        try:
            local_path = self.sftp_dir + os.sep + remote_path if not local_path else local_path
            split_path = local_path.replace('/', os.sep).replace('\\', os.sep).split(os.sep)
            out_dir = os.path.abspath(os.sep.join(split_path[:-1]))
            
            if not os.path.exists(out_dir):
                os.makedirs(out_dir)
            content = self.read_file(remote_path, fallback_codec=fallback_codec)
            with open(os.path.abspath(out_dir) + os.sep + split_path[-1], 'w+', encoding=self.encoding) as f:
                f.write(content)
        except Exception as e:
            self.logger.logged_raise("Error occurred downloading file '{}'".format(remote_path), e)
        return content


    def read_file(self, path, fallback_codec='windows-1252'):
        content = []
        remote_file = self.sftp_client.open(path, 'rb')
        try:
            for line in remote_file:
                try:
                    content.append(line.decode(self.encoding).rstrip('\n\r'))
                except UnicodeDecodeError:
                    content.append(line.decode(fallback_codec).rstrip('\n\r'))
        except Exception as e:
            self.logger.logged_raise("Error occurred opening file '{}'.".format(path), 'ERROR', e)
        finally:
            remote_file.close()
        return '\n'.join(content)

    
    def keep_alive(self):
        transport = self.ssh_client.get_transport()
        # TODO: Try multiple times 1-3 ?
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
        if hasattr(self, 'logger') and self.logger:
            del self.logger
        if hasattr(self, 'sftp_client') and self.sftp_client:
            try:
                self.sftp_client.close()
            except TypeError:
                pass # TODO: write your own logger, this is beyond stupid
        if hasattr(self, 'ssh_client') and self.ssh_client:
            self.ssh_client.close()
        self.host, self.user = None, None



    # -------------------------------TO IMPLEMENT--------------------------------------
    #   chdir(path)                        change directory
    #   getcwd()                           get current working directory
    #   get_channel()                      get ssh channel
    #   listdir(path)                      list containing directory list
    #   mkdir(path, mode)                  make directory
    #   get(remotepath, localpath)         copy file to local
    #   put(localpath, remotepath)         copy file to remote
    #   remove(path)                       remove file
    #   rename(oldpath, newpath)           rename file/folder
    #   rmdir(path)                        remove folder
    #   stat(path)                         list statistics of file
    # --------------------------------------------------------------------------------


    # If speed issues downloading file contents:
    #   Consider using http://docs.paramiko.org/en/stable/api/sftp.html#paramiko.sftp_file.SFTPFile.prefetch
    #     in a thread or something.

