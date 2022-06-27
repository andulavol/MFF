# -*- coding: utf-8 -*-
"""
Created on Thu Feb 17 19:41:33 2022

@author: ancev
"""

# Recommender systems 
# Simple Recommenders - basic systems that recommend 
# the top items based on a certain metric or score

import pandas as pd
metadata = pd.read_csv('movies_metadata.csv', low_memory=False)
metadata.head(3)

# Calculate mean of vote average column
C = metadata['vote_average'].mean()

# Calculate the minimum number of votes required to be in the chart, m, 
# received by a movie in the 90th percentile and filter metadata

m = metadata['vote_count'].quantile(0.90)
q_movies = metadata.copy().loc[metadata['vote_count'] >= m]

# Function that computes the weighted rating of each movie
def weighted_rating(x, m=m, C=C):
    v = x['vote_count']
    R = x['vote_average']
    # Calculation based on the IMDB formula
    return (v/(v+m) * R) + (m/(m+v) * C)

# Define a new feature 'score' and calculate its value with `weighted_rating()`
q_movies['score'] = q_movies.apply(weighted_rating, axis=1)
# why axis = 1, find in documentation

#Sort movies based on score calculated above
q_movies = q_movies.sort_values('score', ascending=False)


# Merging two tables

credits = pd.read_csv('credits.csv')
keywords = pd.read_csv('keywords.csv')

# Remove rows with bad IDs.
metadata = metadata.drop([19730, 29503, 35587])

# Convert IDs to int. Required for merging
keywords['id'] = keywords['id'].astype('int')
credits['id'] = credits['id'].astype('int')
metadata['id'] = metadata['id'].astype('int')

# Merge keywords and credits into your main metadata dataframe
metadata = metadata.merge(credits, on='id')
metadata = metadata.merge(keywords, on='id')

# Parse the stringified features into their corresponding python objects
from ast import literal_eval

features = ['cast', 'crew', 'keywords', 'genres']
for feature in features:
    metadata[feature] = metadata[feature].apply(literal_eval)
    
# Import CountVectorizer and create the count matrix
# One key difference is that you use the CountVectorizer() instead of TF-IDF. 
# This is because you do not want to down-weight the actor/director's presence 
# if he or she has acted or directed in relatively more movies. It doesn't make
# much intuitive sense to down-weight them in this context.
from sklearn.feature_extraction.text import CountVectorizer

count = CountVectorizer(stop_words='english')
count_matrix = count.fit_transform(metadata['soup'])

# Compute the Cosine Similarity matrix based on the count_matrix
from sklearn.metrics.pairwise import cosine_similarity

cosine_sim2 = cosine_similarity(count_matrix, count_matrix)
# Reset index of your main DataFrame and construct reverse mapping as before
metadata = metadata.reset_index()
indices = pd.Series(metadata.index, index=metadata['title'])

# Function that takes in movie title as input and outputs most similar movies
def get_recommendations(title, cosine_sim=cosine_sim):
    # Get the index of the movie that matches the title
    idx = indices[title]

    # Get the pairwsie similarity scores of all movies with that movie
    sim_scores = list(enumerate(cosine_sim[idx]))

    # Sort the movies based on the similarity scores
    sim_scores = sorted(sim_scores, key=lambda x: x[1], reverse=True)

    # Get the scores of the 10 most similar movies
    sim_scores = sim_scores[1:11]

    # Get the movie indices
    movie_indices = [i[0] for i in sim_scores]

    # Return the top 10 most similar movies
    return metadata['title'].iloc[movie_indices]


combined_book_data = pd.merge(df_ratings, df_books, on='ISBN', how='inner')
combined_book_data.drop(['Book-Author', 'Year-Of-Publication', 'Publisher', 'Image-URL-S', 'Image-URL-M', 'Image-URL-L'], axis=1, inplace=True)
df_sorted_isbn = combined_book_data.groupby('ISBN')['Book-Rating'].count().reset_index().sort_values('Book-Rating', ascending=False)

#reader = Reader(line_format=u'user item rating', sep=';', rating_scale=(7, 10), skip_lines=1)
#data_ratings = Dataset.load_from_df(df_filter_ratings[['User-ID', 'ISBN', 'Book-Rating']], reader)
#trainset = data_ratings.build_full_trainset()
