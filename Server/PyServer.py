# -*- coding: utf-8 -*-

import sys
import asyncio
import logging
from websocket_server import WebsocketServer

total = len(sys.argv)
cmdargs = str(sys.argv)
params = {}

def onMessage(message):
    print(message)

def insert():
    print("function insert")

def backup():
    print("function backup")

#当新的客户端连接时会提示

def new_client(client, server):
    print("A new client has joined us: %s, now we have %s clients" % (client['id'], len(server.clients)))
    print(server.clients)
    server.send_message_to_all("Hey all, a new client has joined us")
 
 
# 当旧的客户端离开
def client_left(client, server):
    print("Client %s left, now we have %s clients" % (client['id'], len(server.clients)))
    print(server.clients)
    server.send_message(client, "completed")
 
 
# 接收客户端的信息。
def message_received(client, server, message):
    print("Client(%d) said: %s" % (client['id'], message))
    onMessage(message)
    server.send_message(client, "received")

def launchWebSocketServer():
    server = WebsocketServer(8131)
    server.set_fn_new_client(new_client)
    server.set_fn_client_left(client_left)
    server.set_fn_message_received(message_received)
    server.run_forever()

if __name__ == '__main__':
    launchWebSocketServer()

    print("Hello world!")
