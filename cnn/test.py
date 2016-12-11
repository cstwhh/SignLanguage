import numpy as np
from keras.utils import np_utils # utilities for one-hot encoding of ground truth values
from random import sample
import random

# para
task_is_train = True

test_len = 2
num_classes = 5

batch_size = 128 # in each iteration, we consider 128 training examples at once
num_epochs = 20 # we iterate twenty times over the entire training set
hidden_size = 512 # there will be 512 neurons in both hidden layers
saved_model_name = "model.h5"
finger_data_dim = 6



# read data file and slice train and test set
data = np.loadtxt("test.txt", delimiter = ",")
x = data[:, :-1]
x_abs_max = max(x.max(), -x.min()) + 1 #TODO
x /= x_abs_max

print data
print "*" * 30

train_len = len(data) - test_len
test_set = np.empty((test_len, finger_data_dim + 1))
train_set = np.empty((train_len, finger_data_dim + 1))

p = test_len * 1.0 / len(data)

test_size = 0
train_size = 0

for v in data:
	if (test_size < test_len) and (random.random() <= p):
		test_set[test_size] = v
		test_size = test_size + 1
	else:
		if(train_size < train_len):
			train_set[train_size] = v
			train_size = train_size + 1
		else:
			test_set[test_size] = v
			test_size = test_size + 1

test_x = test_set[:, :-1]
test_y = test_set[:, -1:].reshape(-1, ).astype("int32")
test_y = np_utils.to_categorical(test_y, num_classes) # One-hot encode the labels


train_x = train_set[:, :-1]
train_y = train_set[:, -1:].reshape(-1, ).astype("int32")
train_y = np_utils.to_categorical(train_y, num_classes) # One-hot encode the labels


print train_x
print train_y
print "=" * 10	
print test_x
print test_y