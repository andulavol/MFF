import sys
def prime(number):  
        prime_n = []
        for i in range(2, number + 1):
            while (number % i) == 0:
                new_num = number // i 
                number = new_num
                prime_n.append(i)
        prime_numbers_str = '\n'.join(map(str, prime_n))
        print(prime_numbers_str)

def read_number():
    if len(sys.argv) !=  2:
        return -1
    try:
        num_int = int(sys.argv[1])
    except ValueError:
        return - 1

    if num_int < 1:
        return -1
    else:
        return num_int

def main(): 
    number = read_number();
    if number == -1:
        print('-')
    else:
        prime(number)

if __name__ == '__main__':
    main()
