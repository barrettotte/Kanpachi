# Testing SFTP

import test_utils
from RemoteServer import RemoteServer

host,user,pwd = test_utils.get_creds()

server = RemoteServer(None, host=host)
try:
    server.connect(user, pwd)

    if server.keep_alive():
        server.read_file('/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle')
        server.download_file('/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle')
        server.download_file('/home/OTTEB/Hello-IBMi/QRPGLESRC/hellogit.rpgle', 'cache/hello.rpgle')
        
        # NOTES: 
        #   - IBMi source member returns as EBCDIC (IBM039) encoding and as one blob of string
        #   - In the implementation, check for '/QSYS.LIB' and separate into lines based on member type / record length
        server.read_file('/QSYS.LIB/BOLIB.LIB/QRPGLESRC.FILE/FIZZBUZZ.MBR', fallback_codec='IBM039')
        server.download_file('/QSYS.LIB/BOLIB.LIB/QRPGLESRC.FILE/FIZZBUZZ.MBR', fallback_codec='IBM039')
except Exception as e:
    print('Error testing SFTP\n  ' + str(e))
finally:
    server.disconnect()
