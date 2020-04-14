#!/usr/bin/env python
#  -*- coding:utf-8 -*-
 
import urllib2
import json
 
def http_post(url,data_json):
    jdata = json.dumps(data_json)
    req = urllib2.Request(url, jdata)
    response = urllib2.urlopen(req)
    return response.read()
 
url = 'http://127.0.0.1:8080'
data_json = {'orderId': '10000001','type':'compile','logFile':'xxxxxxx'}
print("request:" + str(data_json))
resp = http_post(url,data_json)
print("response:" + str(resp))