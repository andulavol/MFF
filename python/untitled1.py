# -*- coding: utf-8 -*-
"""
Created on Thu Feb 17 21:13:08 2022

@author: ancev
"""

"""
Reduced matrices actually represent the users and items individually. 
The m rows in the first matrix represent the m users, and the 
p columns tell you about the features or characteristics of 
the users. The same goes for the item matrix with n items and 
p characteristics. The two columns in the user matrix and the 
two rows in the item matrix are called latent factors and are 
an indication of hidden characteristics about the users or 
the items.
Note: Overfitting happens when the model trains to fit the 
training data so well that it doesn’t perform well with new 
data.
One of the popular algorithms to factorize a matrix is the 
singular value decomposition (SVD) algorithm. SVD came into 
the limelight when matrix factorization was seen performing 
well in the Netflix prize competition. Other algorithms 
include PCA and its variations, NMF, and so on. Autoencoders 
can also be used for dimensionality reduction in case you 
want to use Neural Networks.
Note: Surprise is a Python SciKit that comes with various 
recommender algorithms and similarity metrics to make it easy 
to build and analyze recommenders.

Defaultdict is a container like dictionaries present in the 
module collections. Defaultdict is a sub-class of the 
dictionary class that returns a dictionary-like object. 
The functionality of both dictionaries and defaultdict are 
almost same except for the fact that defaultdict never raises 
a KeyError. It provides a default value for the key that does 
not exists.
"""
from surprise import SVD
from surprise import Dataset
from surprise import Reader
from surprise.model_selection import train_test_split
from collections import defaultdict

"""TODO: why did you choose module surprise, why svd()?, """

def get_top_n(predictions, n=10):
    """Return the top-N recommendation for each user from a set of predictions.
    Args:
        predictions(list of Prediction objects): The list of predictions, as
            returned by the test method of an algorithm.
        n(int): The number of recommendation to output for each user. Default
            is 10.
    Returns:
    A dict where keys are user (raw) ids and values are lists of tuples:
        [(raw item id, rating estimation), ...] of size n.
    """

    # First map the predictions to each user.
    top_n_books = defaultdict(list)
    for user_id, book_id, true_r, est, _ in predictions:
        top_n_books[user_id].append((book_id, est))

    # Then sort the predictions for each user and retrieve the k highest ones.
    for user_id, user_ratings in top_n_books.items():
        user_ratings.sort(key=lambda x: x[1], reverse=True)
        top_n_books[user_id] = user_ratings[:n]

    return top_n_books

reader = Reader(line_format=u'user item rating', sep=';', rating_scale=(0, 10), skip_lines=1)
data = Dataset.load_from_file("BX-Book-Ratings.csv", reader=reader)

trainset, testset = train_test_split(data, test_size=.2)
algo = SVD()
algo.fit(trainset)

# Than predict ratings for all pairs (u, i) that are NOT in the training set.
predictions = algo.test(testset)
top_n = get_top_n(predictions, n=10)
