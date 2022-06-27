import pandas as pd
import numpy as np

#Read csv
df_ratings = pd.read_csv('BX-Book-Ratings.csv', sep = ';', skiprows = 0, on_bad_lines='skip', encoding = 'unicode_escape')
df_books = pd.read_csv('BX-Books.csv', sep = ';', skiprows = 0, on_bad_lines='skip', encoding = 'unicode_escape')

distr_book_ratings = df_ratings['Book-Rating'].value_counts().sort_index(ascending=False)
print(distr_book_ratings.head())

df_sorted_isbn = df_ratings.groupby('ISBN')['Book-Rating'].count().reset_index().sort_values('Book-Rating', ascending=False)
print(df_sorted_isbn.head())
