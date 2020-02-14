# Test SSH connection and executing commands

import test_utils as test_utils
from common.remote_server import RemoteServer

host,user,pwd = test_utils.get_creds()

server = RemoteServer(None, host=host)
try:
    server.connect(user, pwd)
    #server.exec_command('ls')
    #server.exec_command("system 'DSPLIBL'")
    #server.exec_command("xyz")
    server.keep_alive()
    server.open_shell()
except Exception as e:
    print('Error occurred ' + str(e))
finally:
    server.disconnect()
