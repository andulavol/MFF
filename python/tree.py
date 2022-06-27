import os

f = os.scandir()
print(f)
list = []
for entry in f:
    if entry.is_dir():
        print(entry.name)
    if entry.is_file() or entry.is_dir:
        print(entry.name)
        list.append(entry.name)
print(list)

    