# -*- coding: utf-8 -*-
"""
Created on Thu Feb 17 21:13:08 2022

@author: ancev
"""

from sklearn.decomposition import TruncatedSVD
import pandas as pd
import numpy as np

#Read csv
df_ratings = pd.read_csv('BX-Book-Ratings.csv', sep = ';', skiprows = 0, on_bad_lines='skip', encoding = 'unicode_escape')
df_books = pd.read_csv('BX-Books.csv', sep = ';', skiprows = 0, on_bad_lines='skip', encoding = 'unicode_escape')

book_title_input = "Angela's Ashes: A Memoir"
book_ISBN = df_books[df_books['Book-Title'] == book_title_input][['ISBN']]
book_ISBN = book_ISBN.iloc[0]['ISBN']

min_book_ratings = 25
filter_books = df_ratings['ISBN'].value_counts() > min_book_ratings
filter_books = filter_books[filter_books].index.tolist()

min_user_ratings = 10
filter_users = df_ratings['User-ID'].value_counts() > min_user_ratings
filter_users = filter_users[filter_users].index.tolist()

df_filter_ratings = df_ratings[(df_ratings['ISBN'].isin(filter_books)) & (df_ratings['User-ID'].isin(filter_users))]
print(df_ratings.shape)
print(df_filter_ratings.shape)

#creating user-item table
rating_crosstab = df_filter_ratings.pivot_table(values='Book-Rating', index='User-ID', columns='ISBN', fill_value=0) 

#KNN - didnt use knn because of memory problems

X = rating_crosstab.T

#perform dimensionality reduction
SVD = TruncatedSVD(n_components=12, random_state=5)
resultant_matrix = SVD.fit_transform(X)
print(resultant_matrix)
#correlation coefficients
corr_mat = np.corrcoef(resultant_matrix)
print(corr_mat)
col_idx = rating_crosstab.columns.get_loc(book_ISBN)
corr_specific = corr_mat[col_idx]
df_recomm = pd.DataFrame({'corr_specific':corr_specific, 'Books-ISBN': rating_crosstab.columns})\
.sort_values('corr_specific', ascending=False)\
.head(10)
for i in range(1,10):
    book_ISBN = df_recomm.iloc[i]['Books-ISBN']
    book_titles= df_books[df_books['ISBN'] == book_ISBN][['Book-Title']]
    if book_titles.empty != True:
        print(book_titles.iloc[0]['Book-Title'])