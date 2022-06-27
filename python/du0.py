# -*- coding: utf-8 -*-
"""
Created on Sat Oct 16 13:25:33 2021

@author: ancev
"""

import argparse

import numpy as np
import sklearn.datasets
import sklearn.metrics
from sklearn import linear_model
import sklearn.model_selection

parser = argparse.ArgumentParser()
# These arguments will be set appropriately by ReCodEx, even if you change them.
parser.add_argument("--recodex", default=False, action="store_true", help="Running in ReCodEx")
parser.add_argument("--seed", default=42, type=int, help="Random seed")
parser.add_argument("--test_size", default=0.1, type=lambda x:int(x) if x.isdigit() else float(x), help="Test set size")
# If you add more arguments, ReCodEx will keep them with your default values.

def main(args):
    # Load Boston housing dataset
    dataset = sklearn.datasets.load_diabetes()

    # The input data are in dataset.data, targets are in dataset.target.

    # TODO: Append a new feature to all input data, with value "1"
    x = np.concatenate((dataset.data,np.ones([442,1])),axis=1) 
    
    # TODO: Split the dataset into a train set and a test set.
    # Use `sklearn.model_selection.train_test_split` method call, passing
    # arguments `test_size=args.test_size, random_state=args.seed`.
    x_train, x_test, y_train, y_test = sklearn.model_selection.train_test_split(x, dataset.target, test_size=args.test_size, random_state=args.seed)

    # TODO: Solve the linear regression using the algorithm from the lecture,
    # explicitly computing the matrix inverse (using `np.linalg.inv`).
    w = np.linalg.inv(np.transpose(x_train) @ x_train) @ np.transpose(x_train) @ y_train

    # Train the model using the training sets
    regr = linear_model.LinearRegression()
    regr.fit(x_train, y_train)

    # Make predictions using the testing set

    # TODO: Predict target values on the test set
    y_pred = regr.predict(x_test)
    
    # TODO: Compute root mean square error on the test set predictions
    mse = sklearn.metrics.mean_squared_error(y_test, y_pred)
    rmse = np.sqrt(mse)

    return rmse

if __name__ == "__main__":
    args = parser.parse_args([] if "__file__" not in globals() else None)
    rmse = main(args)
    print("{:.2f}".format(rmse))