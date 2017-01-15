#coding=utf-8
import httplib
import time
import sys
import os
from keras.models import Sequential, load_model
from keras.layers.core import Dense, Dropout, Activation, Flatten
from keras.layers.advanced_activations import PReLU
from keras.layers.convolutional import Convolution2D, MaxPooling2D
from keras.optimizers import SGD, Adadelta, Adagrad
from keras.utils import np_utils, generic_utils
from six.moves import range
import numpy as np
from keras import backend as K
import time
# K.set_image_dim_ordering('th')

ip = '101.5.129.140'
train_x_abs_max = 247.9712
ensure_delta = 400
finger_data_dim = 63
num_classes = 6

httpClient = None
model = load_model("lx_zy.h5")
sign = ["飞机","电话","你","坏","拳","七"]
last_pre_sign = ""
null_data_notify = "没有手势数据"
last_sign_produce_time = time.time()
last_x = np.zeros((1, 63))
accept_p = 0.96

try:
	while True:  
		httpClient = httplib.HTTPConnection(ip, 8000, timeout = 30)
		httpClient.request('GET', '/')
		response = httpClient.getresponse()
		response_arr = response.read().replace('\r\n', '').split(' ')
		if(len(response_arr) == 1):
			if(last_pre_sign != null_data_notify):
				last_pre_sign = null_data_notify
				last_sign_produce_time = last_sign_produce_time - ensure_delta
				print last_pre_sign
			continue
		inp = map(float, response_arr)
		inp = np.array(inp).reshape(-1,finger_data_dim)
		x_abs_max = max(inp.max(), -inp.min()) + 1 #TODO
		x_abs_max = max(x_abs_max, train_x_abs_max)
		test_x = inp / x_abs_max

		

		# print type(x)
		# print x
		# print x.shape

		proba = model.predict(test_x, batch_size=1, verbose=0)

		# print proba
		if proba.shape[-1] > 1:
			pre_label = proba.argmax(axis=-1)
		else:
			pre_label = (proba > 0.5).astype('int32')
		pre_type = int(pre_label[0])
		pre_sign = sign[pre_type]

		# print pre_sign + ":" + str(proba[0][pre_type])
		# print pre_type
		if(pre_sign != last_pre_sign):
			passed_time = (time.time() - last_sign_produce_time) * 1000.0
			# print "passed_time: " + str(passed_time) + "ms"
			if(passed_time > ensure_delta and proba[0][pre_type] > accept_p):
			# if(passed_time > ensure_delta):
				last_sign_produce_time = time.time()
				last_pre_sign = pre_sign
				print "result: " + pre_sign +  ":" + str(proba[0][pre_type])
				# x = test_x[0].reshape(1,-1)
				# dist = np.linalg.norm(x - last_x)
				# print dist
				# last_x = x
			else:
				continue
		else:
			continue

		oup = str(pre_type);

		httpClient = httplib.HTTPConnection(ip, 8000, timeout = 30)
		httpClient.request('GET', '/' + oup);
		response = httpClient.getresponse().read().replace('\r\n', '')

		# print response
except Exception, e:
	print e
finally:
	if httpClient:
		httpClient.close()
