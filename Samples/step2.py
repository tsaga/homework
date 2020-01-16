

import os
import tensorflow as tf # 导入TF 库
from tensorflow import keras # 导入TF 子库

from tensorflow_core.python.keras import layers, optimizers, datasets # 导入TF 子库

from tensorflow_core.python.keras.optimizers import SGD



# 加载手写数字图片数据集
# 为了方便业界统一测试和评估算法， (Lecun, Bottou, Bengio, & Haffner, 1998)发布了手写数字图
# 片数据集，命名为MNIST，它包含了0~9 共10 种数字的手写图片，每种数字一共有7000 张图片，采集自
# 不同书写风格的真实手写图片，一共70000 张图片。其中60000张图片作为训练集𝔻train(Training Set)，
# 用来训练模型，剩下10000 张图片作为测试集𝔻test(Test Set)，用来预测或者测试，训练集和测试集共
# 同组成了整个MNIST 数据集。

# 加载数据集 - Keras子库datasets自动下载、管理和加载MNIST 数据集
# (x, y)是训练集，(x_val, y_val)是测试集
(x, y), (x_val, y_val) = datasets.mnist.load_data() 
# load_data()函数返回两个元组(tuple)对象，第一个是训练集，第二个是测试集，每个tuple的第一个元素
# 是多个训练图片数据X，第二个元素是训练图片对应的类别数字Y。其中训练集X的大小为(60000,28,28)，
# 代表了60000 个样本，每个样本由28 行、28 列构成，由于是灰度图片，故没有RGB 通道；训练集Y的大小
# 为(60000, )，代表了这60000 个样本的标签数字，每个样本标签用一个0~9 的数字表示。测试集X 的大小
# 为(10000,28,28)，代表了10000 张测试图片，Y 的大小为(10000, )。

# x 是多个训练图片数据，大小为(60000,28,28)，即6000个28x28的灰度图 
# 转换为张量，将0~255灰度值缩放到-1~1
x = 2*tf.convert_to_tensor(x, dtype=tf.float32)/255.-1 

# y 是训练样本的标签数字，用一个0~9 的数字表示
y = tf.convert_to_tensor(y, dtype=tf.int32) # 转换为张量
# 手写数字有0~9共10种分类，建立depth=10的one-hot编码
y = tf.one_hot(y, depth=10) # one-hot 编码


print(x.shape, y.shape)


train_dataset = tf.data.Dataset.from_tensor_slices((x, y)) # 构建数据集对象
train_dataset = train_dataset.batch(512) # 批量训练


layers.Dense(256, activation='relu'),
model = keras.Sequential([ # 3 个非线性层的嵌套模型
    layers.Dense(256, activation='relu'),
    layers.Dense(128, activation='relu'),
    layers.Dense(10)])

sgd = SGD(lr=0.01, decay=1e-6, momentum=0.9, nesterov=True)
model.compile(loss='categorical_crossentropy',
              optimizer=sgd,
              metrics=['accuracy'])

model.fit(x_train, y_train,
          epochs=20,
          batch_size=128)
score = model.evaluate(x_test, y_test, batch_size=128)    


# 自动求导
with tf.GradientTape() as tape: # 构建梯度记录环境
    # 打平，[b, 28, 28] => [b, 784]
    x = tf.reshape(x, (-1, 28*28))
    # Step1. 得到模型输出output
    # [b, 784] => [b, 10]
    out = model(x)
    loss = out - y
    # Step3. 计算参数的梯度 w1, w2, w3, b1, b2, b3
    grads = tape.gradient(loss, x)
    # w' = w - lr * grad，更新网络参数
    optimizers.apply_gradients(zip(grads, x))