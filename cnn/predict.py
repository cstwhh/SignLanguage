from keras.models import Sequential, load_model
from keras.layers.core import Dense, Dropout, Activation, Flatten
from keras.layers.advanced_activations import PReLU
from keras.layers.convolutional import Convolution2D, MaxPooling2D
from keras.optimizers import SGD, Adadelta, Adagrad
from keras.utils import np_utils, generic_utils
from six.moves import range
import numpy as np
from keras import backend as K
K.set_image_dim_ordering('th')

test_x = np.loadtxt("data0_test.txt", delimiter = ",").reshape(-1,63)
test_x /= 196.9604
print test_x

model = load_model("model.h5")
proba = model.predict(test_x, batch_size=1, verbose=1)
print proba
if proba.shape[-1] > 1:
	print proba.argmax(axis=-1)
else:
	print (proba > 0.5).astype('int32')