
import socket
from scratchclient import ScratchSession
import threading
import time
import random
import json
import inspect

host='127.0.0.1'
port=7073
# TDD random
## https://scratch.mit.edu/projects/967799973/
project_id=967799973
# Controller with cloud var
## https://scratch.mit.edu/projects/960534356/
project_id=960534356
def send_udp_message(message):
    try:
        # Create a UDP socket
        with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
            # Send the message
            s.sendto(message.encode('utf-8'), (host, port))
        print(f"UDP message sent to {host}:{port}: {message}")
    except Exception as e:
        print(f"Error sending UDP message: {e}")

def read_file(file_path):
    try:
        with open(file_path, 'r') as file:
            file_content = file.read()
            return file_content
    except FileNotFoundError:
        return f"Error: File '{file_path}' not found."
    except Exception as e:
        return f"Error: {e}"


connection=""
def reconnect():
    global connection, session
    #password_path="password.txt"
    password_path="C:\\Users\\elois\\Desktop\\temp\\password.txt"

    print("Read Password.")

    #password= read_file("password.txt")
    password= read_file(password_path)

    name ="EloiStree"

    print("Start connection to scratch")
    session = ScratchSession(name, password)

    connection = session.create_cloud_connection(project_id)
    connection.on("set")

reconnect()


def do_the_stuff():
    # Replace this with the action you want to perform
    try:
        varRandom= random.randint(1, 9999999)
        print(f"Ping {varRandom}")
        connection.set_cloud_variable("ServerPing",varRandom )
    except:
        reconnect()

def action_thread():
    while True:
        do_the_stuff()
        time.sleep(1)



print("Is log test:")
print(session.get_studio(34580905).description)
#print(session.get_project(project_id).get_comments()[0].content)


@connection.on("set")
def on_set(variable):

    try:
        variable.name=variable.name.replace("☁ ", "")
        print(f"CLOUD:{variable.name}:{variable.value}")
        send_udp_message(f"scratchvar:{variable.name}:{variable.value}")
    except:
        reconnect()
        connection.on("set")

    
print("Start ping Thread")
# Create a thread
thread = threading.Thread(target=action_thread)
# Start the thread
thread.start()
thread.join()

# Add any other code here if you want to do something else concurrently

# Wait for the thread to finish (you can also use thread.join() to wait for it)


#print("T2")
#connection.set_cloud_variable("Test", 42)
#print(connection.get_cloud_variable("Test"))

#print("End")


