# Test setting up a DB2 for i connection and querying a table

import pyodbc,json
import utils as utils


def get_credential(config,key):
    if key in config:
       return config[key] 
    return utils.required_input("Enter '{}': ".format(key))


with open('config.json', 'r') as f:
    config = json.load(f)
    host = get_credential(config, 'host')
    user = get_credential(config, 'user')
    pwd  = utils.required_pass()


conn = pyodbc.connect(
    driver='{iSeries Access ODBC Driver}',
    system=host,
    uid=user,
    pwd=pwd
)

c1 = conn.cursor()
c1.execute('select * from BOLIB.PERSON')

for row in c1: 
    print(row)

