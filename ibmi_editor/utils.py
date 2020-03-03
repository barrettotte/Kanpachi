import json, os, getpass, datetime


def get_pretty_json(jsonRaw, sort=False):
    return json.dumps(jsonRaw, indent=4, sort_keys=sort)


def read_file_json(file_path):
    try:
        with open(file_path, 'r') as f: 
            return json.load(f)
    except Exception as e:
        print("File at {} could not be read.\n  {}".format(file_path, e))
    

def write_file_json(file_path, buffer):
    try:
        with open(file_path, 'w+') as f: 
            f.write(get_pretty_json(buffer))
    except Exception as e: 
        print("File at {} could not be written.\n  {}".format(file_path, e))


def required_pass(prompt='Enter password: '):
    try:
        return getpass.getpass(prompt)
    except KeyboardInterrupt:
        exit('Keyboard interrupt occurred. Cannot continue.')


def required_input(prompt):
    try:
        return input(prompt)
    except KeyboardInterrupt:
        exit("Keyboard interrupt occurred. Cannot continue.")




       