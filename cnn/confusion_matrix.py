from keras.models import Sequential, load_model
from keras.layers.core import Dense, Dropout, Activation, Flatten
from keras.layers.advanced_activations import PReLU
from keras.layers.convolutional import Convolution2D, MaxPooling2D
from keras.optimizers import SGD, Adadelta, Adagrad
from keras.utils import np_utils, generic_utils
from six.moves import range
import numpy as np


def get_type(test_y):
	test_y_len = len(test_y)
	for i in range(test_y_len):
		hot = test_y[i]
		if hot == 1.0:
			return i


def get_confusion_matrix(model, test_x, test_y):
	num_classes = len(test_y[0])
	finger_data_dim = len(test_x[0])
	test_len = len(test_y)
	confusion_matrix = np.zeros((num_classes, num_classes), dtype=np.int32)
	for i in range(test_len):
		proba = model.predict(test_x[i].reshape(-1,finger_data_dim), batch_size=1, verbose=0)
		if proba.shape[-1] > 1:
			pre_label = proba.argmax(axis=-1)
		else:
			pre_label = (proba > 0.5).astype('int32')
		pre_type = int(pre_label[0])
		act_type = get_type(test_y[i])

		confusion_matrix[act_type][pre_type] += 1

		# if(pre_type != act_type):
		# 	print "[" + str(i) + "] act_type is " + str(act_type) + "; pre_type is " + str(pre_type)

	print confusion_matrix