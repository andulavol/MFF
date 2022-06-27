using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Linq;

namespace NezarkaBookstore
{
    //
    // Model
    //

    class ModelStore
    {
        public List<Book> books = new List<Book>();
        public List<Customer> customers = new List<Customer>();

        public IList<Book> GetBooks()
        {
            return books;
        }

        public Book GetBook(int id)
        {
            return books.Find(b => b.Id == id);
        }

        public Customer GetCustomer(int id)
        {
            return customers.Find(c => c.Id == id);
        }

        public static ModelStore LoadFrom()
        {
            var store = new ModelStore();

            try
            {
                if (Console.ReadLine() != "DATA-BEGIN")
                {
                    return null;
                }
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line == null)
                    {
                        return null;
                    }
                    else if (line == "DATA-END")
                    {
                        break;
                    }

                    string[] tokens = line.Split(';');
                    switch (tokens[0])
                    {
                        case "BOOK":
                            store.books.Add(new Book
                            {
                                Id = int.Parse(tokens[1]),
                                Title = tokens[2],
                                Author = tokens[3],
                                Price = decimal.Parse(tokens[4])
                            });
                            break;
                        case "CUSTOMER":
                            store.customers.Add(new Customer
                            {
                                Id = int.Parse(tokens[1]),
                                FirstName = tokens[2],
                                LastName = tokens[3]
                            });
                            break;
                        case "CART-ITEM":
                            var customer = store.GetCustomer(int.Parse(tokens[1]));
                            if (customer == null)
                            {
                                return null;
                            }
                            customer.ShoppingCart.Items.Add(new ShoppingCartItem
                            {
                                BookId = int.Parse(tokens[2]),
                                Count = int.Parse(tokens[3])
                            });
                            break;
                        default:
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is IndexOutOfRangeException)
                {
                    return null;
                }
                throw;
            }

            return store;
        }
    }

    class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
    }

    class Customer
    {
        private ShoppingCart shoppingCart;

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ShoppingCart ShoppingCart
        {
            get
            {
                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCart();
                }
                return shoppingCart;
            }
            set
            {
                shoppingCart = value;
            }
        }
    }

    class ShoppingCartItem
    {
        public int BookId { get; set; }
        public int Count { get; set; }
    }

    class ShoppingCart
    {
        public int CustomerId { get; set; }
        public List<ShoppingCartItem> Items = new List<ShoppingCartItem>();

        public ShoppingCartItem GetItem(int id)
        {
            return Items.Find(b => b.BookId == id);
        }
    }

    class CommandReader
    {
        public static void ReadAndCall(ModelStore store)
        {
            Customer act_Customer;
            Book act_Book;
            ShoppingCart act_Shopcart;
            int id;
            try
            {
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    else if (line == "")
                    {
                        break;
                    }
                    else if (line != "====")
                    {
                        string[] tokens = line.Split(' ');
                        if (tokens[0] != "GET" || !int.TryParse(tokens[1], out id))
                        {
                            Printer.ErrorHtml();
                            continue;
                        }

                        string output1 = line.Split(".net/").Last();
                        string[] output = line.Split('/');
                        act_Customer = store.GetCustomer(int.Parse(tokens[1]));
                        IList<Book> act_books = store.GetBooks();
                        if (act_Customer != null & tokens[2] == "http://www.nezarka.net/" + output1)
                        {
                            if (output.Length == 4)
                            {
                                switch (output1)
                                {
                                    case "Books":
                                        Printer.PrintAllBooks(act_books, act_Customer, store);
                                        break;
                                    case "ShoppingCart":
                                        Printer.PrintShoppingCart(act_Customer, store);
                                        break;
                                    default:
                                        Printer.ErrorHtml();
                                        break;

                                }
                            }
                            else if (output.Length == 6)
                            {
                                try
                                {

                                    act_Book = store.GetBook(int.Parse(output[5]));

                                    if (act_Customer == null || act_Book == null)
                                    {
                                        Printer.ErrorHtml();
                                    }
                                    else
                                    {
                                        if (output[3] == "Books")
                                        {
                                            if (output[4] == "Detail")
                                            {
                                                Printer.PrintBookDetail(act_Book, act_Customer, store);
                                            }
                                        }
                                        else if (output[3] == "ShoppingCart")
                                        {
                                            switch (output[4])
                                            {

                                                case "Add":
                                                    act_Shopcart = act_Customer.ShoppingCart;
                                                    ShoppingCartItem act_Item = act_Shopcart.GetItem(act_Book.Id);
                                                    if (act_Item == null)
                                                    {
                                                        act_Shopcart.Items.Add(new ShoppingCartItem
                                                        {
                                                            BookId = act_Book.Id,
                                                            Count = 1
                                                        });
                                                    }
                                                    else
                                                    {
                                                        act_Item.Count += 1;
                                                    }
                                                    Printer.PrintShoppingCart(act_Customer, store);
                                                    break;

                                                case "Remove":
                                                    act_Shopcart = act_Customer.ShoppingCart;
                                                    ShoppingCartItem act_ItemDel = act_Shopcart.GetItem(act_Book.Id);
                                                    if (act_ItemDel != null)
                                                    {
                                                        if (act_ItemDel.Count == 1)
                                                        {
                                                            act_Shopcart.Items.Remove(act_ItemDel);
                                                        }
                                                        else
                                                        {
                                                            act_ItemDel.Count -= 1;
                                                        }
                                                        Printer.PrintShoppingCart(act_Customer, store);
                                                    }
                                                    else
                                                    {
                                                        Printer.ErrorHtml();
                                                    }

                                                    break;
                                                default:
                                                    Printer.ErrorHtml();
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Printer.ErrorHtml();
                                        }
                                    }
                                }
                                catch
                                {
                                    Printer.ErrorHtml();
                                }
                            }
                            else
                            {
                                Printer.ErrorHtml();
                            }
                        }
                        else
                        {
                            Printer.ErrorHtml();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is IndexOutOfRangeException)
                {
                    return;
                }
                throw;
            }
        }
    }
    class Printer
    {
        public static void Header(Customer cust, ModelStore store)
        {
            Console.WriteLine("<!DOCTYPE html>");
            Console.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
            Console.WriteLine("<head>");
            Console.WriteLine("    <meta charset=\"utf-8\" />");
            Console.WriteLine("    <title>Nezarka.net: Online Shopping for Books</title>");
            Console.WriteLine("</head>");
            Console.WriteLine("<body>");
            Console.WriteLine("    <style type=\"text/css\">");
            Console.WriteLine("        table, th, td {");
            Console.WriteLine("            border: 1px solid black;");
            Console.WriteLine("            border-collapse: collapse;");
            Console.WriteLine("        }");
            Console.WriteLine("        table {");
            Console.WriteLine("            margin-bottom: 10px;");
            Console.WriteLine("        }");
            Console.WriteLine("        pre {");
            Console.WriteLine("            line-height: 70%;");
            Console.WriteLine("        }");
            Console.WriteLine("    </style>");
            Console.WriteLine("    <h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
            Console.WriteLine("    " + cust.FirstName + ", here is your menu:");
            Console.WriteLine("    <table>");
            Console.WriteLine("        <tr>");
            Console.WriteLine("            <td><a href=\"/Books\">Books</a></td>");
            Console.WriteLine("            <td><a href=\"/ShoppingCart\">Cart (" + cust.ShoppingCart.Items.Count + ")</a></td>");
            Console.WriteLine("        </tr>");
            Console.WriteLine("    </table>");
        }
        public static void ErrorHtml()
        {
            Console.WriteLine("<!DOCTYPE html>");
            Console.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
            Console.WriteLine("<head>");
            Console.WriteLine("    <meta charset=\"utf-8\" />");
            Console.WriteLine("    <title>Nezarka.net: Online Shopping for Books</title>");
            Console.WriteLine("</head>");
            Console.WriteLine("<body>");
            Console.WriteLine("<p>Invalid request.</p>");
            Console.WriteLine("</body>");
            Console.WriteLine("</html>");
            Console.WriteLine("====");
        }
        
        public static void PrintAllBooks(IList<Book> act_books, Customer cust, ModelStore store)
        {
            Header(cust, store);
            Console.WriteLine("    Our books for you:");
            int column = 0;
            if (act_books == null)
            {
                Console.WriteLine("    <table>");
                Console.WriteLine("    </table>");
            }
            else
            {
                Console.WriteLine("    <table>");
                Console.WriteLine("        <tr>");
                for (int i = 1; i <= act_books.Count; i++  )
                {
                    if (column == 3)
                    {
                        Console.WriteLine("        </tr>");
                        Console.WriteLine("        <tr>");
                        column = 0;
                    }
                    Console.WriteLine("            <td style=\"padding: 10px;\">");
                    Console.WriteLine("                <a href=\"/Books/Detail/" + act_books[i - 1].Id.ToString() + "\">" + act_books[i-1].Title + "</a><br />");
                    Console.WriteLine("                Author: " + act_books[i-1].Author + "<br />");
                    Console.WriteLine("                Price: " + act_books[i-1].Price + " EUR &lt;<a href=\"/ShoppingCart/Add/" + act_books[i - 1].Id.ToString() + "\">Buy</a>&gt;");
                    Console.WriteLine("            </td>");
                    column += 1;
                }
                Console.WriteLine("        </tr>");
                Console.WriteLine("    </table>");
            }
            Console.WriteLine("</body>");
            Console.WriteLine("</html>");
            Console.WriteLine("====");
        }
        public static void PrintBookDetail(Book book, Customer cust, ModelStore store)
        {
            Header(cust, store);
            Console.WriteLine("    Book details");
            Console.WriteLine("    <h2>" + book.Title + "</h2>");
            Console.WriteLine("    <p style=\"margin-left:20p0x\">");
            Console.WriteLine("    Author: " + book.Author + "<br />");
            Console.WriteLine("    Price: " + book.Price + " EUR<br />");
            Console.WriteLine("    </p>");
            Console.WriteLine("    <h3>&lt;<a href=\"/ShoppingCart/Add/" + book.Id.ToString() + "\">Buy this book</a>&gt;</h3>");
            Console.WriteLine("</body>");
            Console.WriteLine("</html>");
            Console.WriteLine("====");
        }
        
        public static void PrintShoppingCart(Customer cust, ModelStore store)
        {
            Header(cust, store);
            int totalprice = 0;
            if (cust.ShoppingCart.Items.Count == 0)
            {
                Console.WriteLine("    Your shopping cart is EMPTY.");
                Console.WriteLine("</body>");
                Console.WriteLine("</html>");
            }
            else
            {
                Console.WriteLine("    Your shopping cart:");
                Console.WriteLine("    <table>");
                Console.WriteLine("        <tr>");
                Console.WriteLine("            <th>Title</th>");
                Console.WriteLine("            <th>Count</th>");
                Console.WriteLine("            <th>Price</th>");
                Console.WriteLine("            <th>Actions</th>");
                Console.WriteLine("        </tr>");

                foreach (ShoppingCartItem item in cust.ShoppingCart.Items)
                {
                    Book book = store.GetBook(item.BookId);
                    int act_price = item.Count * Convert.ToInt32(book.Price);
                    totalprice += act_price;
                    Console.WriteLine("        <tr>");
                    Console.WriteLine("            <td><a href=\"/Books/Detail/" + book.Id.ToString() + "\">" + book.Title + "</a></td>");
                    Console.WriteLine("            <td>" + item.Count.ToString() + "</td>");
                    if (item.Count == 1)
                    {
                        Console.WriteLine("            <td>" + book.Price.ToString() + " EUR</td>");
                    }
                    else
                    {
                        Console.WriteLine("            <td>" + item.Count.ToString() + " * " + book.Price.ToString() + " = " + act_price.ToString() + " EUR</td>");
                    }
                    Console.WriteLine("            <td>&lt;<a href=\"/ShoppingCart/Remove/" + book.Id.ToString() + "\">Remove</a>&gt;</td>");

                    Console.WriteLine("        </tr>");

                }
                Console.WriteLine("    </table>");
                Console.WriteLine("    Total price of all items: " + totalprice.ToString() + " EUR");
                Console.WriteLine("</body>");
                Console.WriteLine("</html>");
            }
            Console.WriteLine("====");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var store = new ModelStore();
            store = ModelStore.LoadFrom();

            if (store == null)
            {
                Console.WriteLine("Data error");
                return;
            }
            CommandReader.ReadAndCall(store);
        }
    }
}
