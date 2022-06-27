# -*- coding: utf-8 -*-
"""
Created on Sat Oct 23 00:05:55 2021

@author: ancev
"""

import argparse

import numpy as np
import sklearn.datasets
import sklearn.linear_model
import sklearn.metrics
import sklearn.model_selection

parser = argparse.ArgumentParser()
# These arguments will be set appropriately by ReCodEx, even if you change them.
parser.add_argument("--batch_size", default=10, type=int, help="Batch size")
parser.add_argument("--data_size", default=100, type=int, help="Data size")
parser.add_argument("--epochs", default=50, type=int, help="Number of SGD iterations over the data")
parser.add_argument("--l2", default=0.0, type=float, help="L2 regularization strength")
parser.add_argument("--learning_rate", default=0.01, type=float, help="Learning rate")
parser.add_argument("--plot", default=True, const=True, nargs="?", type=str, help="Plot the predictions")
parser.add_argument("--recodex", default=False, action="store_true", help="Running in ReCodEx")
parser.add_argument("--seed", default=42, type=int, help="Random seed")
parser.add_argument("--test_size", default=0.5, type=lambda x:int(x) if x.isdigit() else float(x), help="Test set size")
# If you add more arguments, ReCodEx will keep them with your default values.

def main(args: argparse.Namespace) -> tuple[float, float]:
    # Create a random generator with a given seed
    generator = np.random.RandomState(args.seed)

    # Generate an artifical regression dataset
    data, target = sklearn.datasets.make_regression(n_samples=args.data_size, random_state=args.seed)

    # TODO: Append a constant feature with value 1 to the end of every input data
    x = np.concatenate((data,np.ones([args.data_size,1])),axis=1) 
    
    # TODO: Split the dataset into a train set and a test set.
    # Use `sklearn.model_selection.train_test_split` method call, passing
    # arguments `test_size=args.test_size, random_state=args.seed`.
    train_data, test_data, train_target, test_target = sklearn.model_selection.train_test_split(x, target, test_size=args.test_size, random_state=args.seed)
    weights = generator.uniform(size=train_data.shape[1])
    # Generate initial linear regression weights
    # one epoch: pick a mini-batch, calculate gradient of the minib -> update the weights
    # repeat steps for all minib we created

    
    gradients = []
    test_rmses = []

    for epoch in range(args.epochs):
        permutation = generator.permutation(train_data.shape[0])
        train_data_p = train_data[permutation]
        train_target_p = train_target[permutation]
        for n in range(0,train_data.shape[0],args.batch_size):
            X_i = train_data_p[n:n+args.batch_size]
            y_i =  train_target_p[n:n+args.batch_size]

            gradient = np.matmul(((X_i @ weights) - y_i), X_i)
            gradients.append(gradient)
        avg_gra = np.mean(np.array(gradients), axis = 0)
        gradients.clear()
        weights = weights - args.learning_rate * (avg_gra + args.l2 * weights)
        y_com = test_data @ weights
        mse = sklearn.metrics.mean_squared_error(test_target, y_com)
        test_rmses.append(np.sqrt(mse))

    # TODO: Compute into `explicit_rmse` test data RMSE when fitting
    # `sklearn.linear_model.LinearRegression` on train_data (ignoring args.l2).
    model1 = sklearn.linear_model.LinearRegression()
    model1.fit(train_data,train_target)
    y_pre1 = model1.predict(test_data)
    mse = sklearn.metrics.mean_squared_error(test_target, y_pre1)
    explicit_rmse = np.sqrt(mse)



    return test_rmses[-1], explicit_rmse

if __name__ == "__main__":
    args = parser.parse_args([] if "__file__" not in globals() else None)
    sgd_rmse, explicit_rmse = main(args)
    print("Test RMSE: SGD {:.2f}, explicit {:.2f}".format(sgd_rmse, explicit_rmse))
