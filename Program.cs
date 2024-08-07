﻿using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Esf;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Dapper;

class Program
{
    static void Main(string[] args)
    {
        var userManager = new UserManager();
        

        while (true)
        {
            // Title
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
            
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    
                    .AddChoices("Log in", "Register", "Exit")
                    
                    );
                    


            switch (choice)
            {
                case "Log in":
                    {
                        var username = AnsiConsole.Ask<string>("[green]User name[/]:");
                        var password = AnsiConsole.Prompt(
                            new TextPrompt<string>("[green]Password[/]:").Secret());

                        var user = userManager.LoginUser(username, password);

                        if (user != null && user.Role == "Admin")
                        {
                            AnsiConsole.MarkupLine("[green]Logged in successfully![/]");
                            MainMenu(user);
                            
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Login failed or no access rights![/]");
                            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
                            AnsiConsole.Clear();
                        }

                        break;
                    }

                case "Register":
                    {
                        var userManagerInstance = new UserManager();
                        while (true)
                        {
                            var username = AnsiConsole.Ask<string>("[green]User name[/]:");
                            if (userManagerInstance.UsernameExists(username))
                            {
                                AnsiConsole.MarkupLine("[red]Username already exists, please re-enter.[/]");
                                continue;
                            }
                            var password = AnsiConsole.Prompt(
                                new TextPrompt<string>("[green]Password[/]:").Secret());
                            var confirmPassword = AnsiConsole.Prompt(
                                new TextPrompt<string>("Re-enter [green]Password[/]:").Secret());
                        
                            if (password != confirmPassword)
                            {
                                AnsiConsole.MarkupLine("[red]The re-entered password does not match![/]");
                                AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
                                AnsiConsole.Clear();
                                break;
                            }
                            var role = "Admin";
                            var status = "Active";
                            var registrationDate = DateTime.Now;

                            var user = new User
                            {
                                Username = username,
                                Password = password,
                                Role = role,
                                Status = status,
                                RegistrationDate = registrationDate

                            };

                            try
                            {
                            userManager.RegisterUser(user);
                            AnsiConsole.MarkupLine("[green]Sign up success![/]");
                            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to continue").AddChoices("Continue"));
                            AnsiConsole.Clear();
                            break;
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
                                AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
                                AnsiConsole.Clear();
                                break;
                            }
                        }
                        break;
                    }

                case "Exit":
                    return;
            }
        }
    }


    static void MainMenu(User loggedInUser)
    {
        while (true)
        {
            AnsiConsole.Clear();
            
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
            AnsiConsole.MarkupLine($"[bold blue]Hello Admin, {loggedInUser.Username}![/]");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    
                    .AddChoices("Dashboard", "Cashier", "Products", "Orders", "Customers", "Logout"));

            switch (choice)
            {
                case "Dashboard":
                    DashboardMenu();
                    break;

                case "Cashier":
                    CashierMenu(loggedInUser);
                    break;

                case "Products":
                    ProductsMenu();
                    break;

                case "Orders":
                    OrderMenu();
                    break; 

                case "Customers":
                    CustomerMenu();
                    break;

                case "Logout":
                    AnsiConsole.Clear();
                    return;
            }
        }
    }

    static void CashierMenu(User loggedInUser)
    {
        var userManager = new UserManager();

        while (true)
        {
            AnsiConsole.Clear();
            // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
            AnsiConsole.MarkupLine($"[bold blue]Manager - {loggedInUser.Username}[/]");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    
                    .AddChoices("Cashier List", "Add Cashier", "Edit Cashier", "Delete Cashier", "Return"));

            switch (choice)
            {
                case "Cashier List":
                    DisplayCashiers(userManager);
                    break;

                case "Add Cashier":
                    AddCashier(userManager);
                    break;

                case "Edit Cashier":
                    EditCashier(userManager);
                    break;

                case "Delete Cashier":
                    DeleteCashier(userManager);
                    break;

                case "Return":
                    return;
            }
        }
    }

    static void DisplayCashiers(UserManager userManager)
    {
        var users = userManager.GetAllUsers();
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Username");
        table.AddColumn("Password");
        table.AddColumn("Role");
        table.AddColumn("Status");
        table.AddColumn("Image");
        table.AddColumn("RegistrationDate");

        foreach (var user in users)
        {
            table.AddRow(user.Id.ToString(), user.Username,user.Password, user.Role, user.Status,user.Image, user.RegistrationDate.ToString());
        }

        AnsiConsole.Render(table);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
    }

    static void AddCashier(UserManager userManager)
    {
        DisplayCashiers(userManager);
        var username = AnsiConsole.Ask<string>(" [green]User name[/]:");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>(" [green]Password[/]:").Secret());

        var role = "Cashier";
        var status = "Active";
        var image = AnsiConsole.Ask<string>(" [green]Image (URL)[/]:");
        var registrationDate = DateTime.Now;

        var user = new User
        {
            Username = username,
            Password = password,
            Role = role,
            Status = status,
            Image = image,
            RegistrationDate = registrationDate
        };

        userManager.RegisterUser(user);
        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[green]Added Cashier successfully![/]");
        DisplayCashiers(userManager);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
    }

    static void EditCashier(UserManager userManager)
    {
        DisplayCashiers(userManager);
        var cashierId = AnsiConsole.Ask<int>("Enter [green]Enter the ID of the Cashier [/] to edit:");
        var cashier = userManager.GetUserById(cashierId);

        if (cashier == null || cashier.Role != "Cashier")
        {
            AnsiConsole.MarkupLine("[red]The Cashier does not exist or is not a Cashier![/]");
            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
            return;
        }

        var username = AnsiConsole.Ask<string>($" [green]User name[/] ({cashier.Username}):", cashier.Username);
        var password = AnsiConsole.Ask<string>($" [green]Password[/] ({cashier.Password}):", cashier.Password);
        var status = AnsiConsole.Ask<string>($" [green]Active status[/] ({cashier.Status}):", cashier.Status);
        var image = AnsiConsole.Ask<string>($" [green]Image (URL)[/] ({cashier.Image}):", cashier.Image);

        cashier.Username = username;
        cashier.Password = password;
        cashier.Status = status;
        cashier.Image = image;

        userManager.UpdateUser(cashier);
        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[green]Successfully edited Cashier information![/]");
        DisplayCashiers(userManager);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
    }

    static void DeleteCashier(UserManager userManager)
    {
        DisplayCashiers(userManager);
        var cashierId = AnsiConsole.Ask<int>(" [green]Cashier ID[/] need to delete:");
        var cashier = userManager.GetUserById(cashierId);

        if (cashier == null)
        {
            AnsiConsole.MarkupLine("[red]Cashier does not exist![/]");
            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
            return;
        }

        userManager.DeleteUser(cashierId);
        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[green]Successfully deleted Cashier![/]");
        DisplayCashiers(userManager);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
    }





    static void ProductsMenu()
    {
        var productManager = new ProductManager();

        while (true)
        {
            AnsiConsole.Clear();
            // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
            DisplayProducts(productManager);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    
                    .AddChoices("Add Product", "Edit Product", "Return"));

            switch (choice)
            {
                case "Add Product":
                    AddProduct(productManager);
                    break;

                case "Edit Product":
                    EditProduct(productManager);
                    break;

                case "Return":
                    return;
            }
        }
    }


    static void DisplayProducts(ProductManager productManager)
    {
        var products = productManager.GetAllProducts();
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("ProductID");
        table.AddColumn("ProductName");
        table.AddColumn("Type");
        table.AddColumn("Stock");
        table.AddColumn("Price");
        table.AddColumn("Status");
        table.AddColumn("Image");
        table.AddColumn("DateInsert");
        table.AddColumn("DateUpdate");

        // Đặt độ dài tối đa cho cột Image
        int maxLength = 29;  

        foreach (var product in products)
        {
            table.AddRow(
                product.Id.ToString(), 
                product.ProductID, 
                product.ProductName, 
                product.Type, 
                product.Stock.ToString(), 
                product.Price.ToString(), 
                product.Status, 
                Truncate(product.Image, maxLength), 
                product.DateInsert.ToString(), 
                product.DateUpdate?.ToString()
            );
        }

        AnsiConsole.Render(table);
    }

    static string Truncate(string text, int maxLength)
    {
        if (text.Length > maxLength)
        {
            return text.Substring(0, maxLength) + "...";
        }
        return text;
    }


    static void AddProduct(ProductManager productManager)
    {
        var productID = $"PROD-{productManager.GetNextProductID()}";
        var productName = AnsiConsole.Ask<string>(" [green]Product name[/]:");
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(" [green]Type[/]:")
                .AddChoices("Meal", "Drink"));
        
        int stock;
        while (true)
        {
            try
            {
                stock = AnsiConsole.Ask<int>(" [green]Quantity:[/]");
                if (stock < 0)
                {
                    AnsiConsole.MarkupLine("[red]Error, please re-enter.[/]");
                }
                else
                {
                    break;
                }
            }
            catch (FormatException)
            {
                AnsiConsole.MarkupLine("Error.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
            }
        }
        
        decimal price;
        while (true)
        {
            try
            {
                price = AnsiConsole.Ask<decimal>(" [green]Price:[/]");
                if (price < 0)
                {
                    AnsiConsole.MarkupLine("[red]Error, please re-enter.[/]");
                }
                else
                {
                    break;
                }
            }
            catch (FormatException)
            {
                AnsiConsole.MarkupLine("Error.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
            }

        }
        
        var status = stock > 0 ? "Available" : "Unavailable";
        var image = AnsiConsole.Ask<string>(" [green]Image (URL)[/]:");
        var dateInsert = DateTime.Now;
        var dateUpdate = DateTime.Now;

        var product = new Product
        {
            ProductID = productID,
            ProductName = productName,
            Type = type,
            Stock = stock,
            Price = price,
            Status = status,
            Image = image,
            DateInsert = dateInsert,
            DateUpdate = dateUpdate
        };

        productManager.AddProduct(product);
        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[green]Successfully![/]");
        DisplayProducts(productManager);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
    }

    static void EditProduct(ProductManager productManager)
    {
        var productId = AnsiConsole.Ask<int>(" [green]Product ID[/] need to edit:");
        var product = productManager.GetAllProducts().Find(p => p.Id == productId);

        if (product == null)
        {
            AnsiConsole.MarkupLine("[red]Product does not exist![/]");
            return;
        }

        var productName = AnsiConsole.Ask<string>($" [green]Product name[/] ({product.ProductName}):", product.ProductName);
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($" [green]Type[/] ({product.Type}):")
                .AddChoices("Meal", "Drink")
                .UseConverter(item => item == product.Type ? $"{item} (Current)" : item));
        
        int stock;
        while (true)
        {
            try
            {
                stock = AnsiConsole.Ask<int>($" [green]Quantity[/] ({product.Stock}):", product.Stock);
                if (stock < 0)
                {
                    AnsiConsole.MarkupLine("[red]Error, please re-enter.[/]");
                }
                else
                {
                    break;
                }
            }
            catch (FormatException)
            {
                AnsiConsole.MarkupLine("[red]Error.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
            }
        }
        
        decimal price;
        while (true)
        {
            try
            {
                price = AnsiConsole.Ask<decimal>($" [green]Price[/] ({product.Price:C}):", product.Price);
                if (price < 0)
                {
                    AnsiConsole.MarkupLine("[red]Error, pleasee re-enter.[/]");
                }
                else
                {
                    break;
                }
            }
            catch (FormatException)
            {
                AnsiConsole.MarkupLine("[red]Error.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
            }
        }
        
        var status = stock > 0 ? "Available" : "Unavailable";
        var image = AnsiConsole.Ask<string>($" [green]Image (URL)[/] ({product.Image}):", product.Image);
        var dateUpdate = DateTime.Now;

        product.ProductName = productName;
        product.Type = type;
        product.Stock = stock;
        product.Price = price;
        product.Status = status;
        product.Image = image;
        product.DateUpdate = dateUpdate;

        productManager.UpdateProduct(product);
        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[green]Successfully![/]");
        DisplayProducts(productManager);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
    }

    




    static Order currentOrder = null;
    static decimal currentTips = 0;
    static decimal currentTotalAmount = 0;
    
    static void OrderMenu()
    {
        
        List<Product> products = GetProducts();

        bool showProducts = true;

    
        while(true)
        {
            AnsiConsole.Clear();
            
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));

            if (showProducts)
            {
                DisplayProduct(products); 
            }

		    var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    
                    .AddChoices("Create Order", "Invoice", "Return"));
            
            switch (choice)
            {
                case "Create Order":
                    AnsiConsole.Clear();
                    
                    AnsiConsole.Write(
                    new FigletText("Cafe Shop")
                        .Centered()
                        .Color(Color.Green));
                    
                    showProducts = true; 

                    currentOrder = CreateOrder(products);
                    currentTips = 0;
                    currentTotalAmount = 0;
                    break;
                
                case "Invoice":
                    AnsiConsole.Clear();
                    AnsiConsole.Write(
                    new FigletText("Cafe Shop")
                        .Centered()
                        .Color(Color.Green));

                    if (currentOrder != null)
                    {
                        showProducts = false; 
                        AnsiConsole.MarkupLine("\n[green]18 Tam Trinh, Hoang Mai, Hanoi[/]");
                        HandlePayment(currentOrder);
                        AnsiConsole.MarkupLine($"[green]Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}[/]");

                        AnsiConsole.MarkupLine("\n[green]Press Enter to return Order Menu.[/]");
                        Console.ReadLine();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]No order available to display the receipt.[/]");
                        AnsiConsole.MarkupLine("\n[green]Press Enter to continue[/]");
                        Console.ReadLine();
                    }
                    break;
                
                case "Return":
                    return;
            }
        }

    }


    static List<Product> GetProducts()
    {
        
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        List<Product> products = new List<Product>();

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Products", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32("ID"),
                        ProductID = reader.GetString("ProductID"),
                        ProductName = reader.GetString("ProductName"),
                        Type = reader.GetString("Type"),
                        Stock = reader.GetInt32("Stock"),
                        Price = reader.GetDecimal("Price"),
                        Status = reader.GetString("Status")
                    });
                }
            }
        }
        return products;
    }

    static void DisplayProduct(List<Product> products)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("ProductID");
        table.AddColumn("ProductName");
        table.AddColumn("Type");
        table.AddColumn("Stock");
        table.AddColumn("Price");
        table.AddColumn("Status");

        foreach (var product in products)
        {
            table.AddRow(product.Id.ToString(), product.ProductID, product.ProductName, product.Type, product.Stock.ToString(), product.Price.ToString("F2"), product.Status);
        }

        AnsiConsole.Write(table);
    }


    static int CreateNewCustomer()
    {
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("INSERT INTO Customers (TotalPrice, Amount, Tips, PaymentTime) VALUES (0, 0, 0, NOW())", connection);
            command.ExecuteNonQuery();
            return Convert.ToInt32(new MySqlCommand("SELECT LAST_INSERT_ID()", connection).ExecuteScalar());
        }
    }   
    
    static Order CreateOrder(List<Product> products)
    {
        var order = new Order();
        order.Items = new List<OrderItem>();

        
        int newCustomerID = CreateNewCustomer();
        order.CustomerID = newCustomerID;
        
        
        while (true)
        {
            AnsiConsole.Clear();
            
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
            
            DisplayProduct(products);
            
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Order Menu")
                    .AddChoices("Add Item", "Remove Item", "Clear Order", "Finish Order"));

            switch (action)
            {
                case "Add Item":
                    DisplayOrder(order);
                    AddItemToOrder(order, products);
                    break;
                case "Remove Item":
                    DisplayOrder(order);
                    RemoveItemFromOrder(order, products);
                    break;
                case "Clear Order":
                    DisplayOrder(order);
                    ClearOrder(order, products);
                    break;
                case "Finish Order":
                    HandlePayment(order);
                    SaveOrder(order, currentTips, currentTotalAmount);
                    AnsiConsole.MarkupLine("\n[green]Press Enter to return to the main menu.[/]");
                    Console.ReadLine(); 
                    return order;
            }
        }
    }

    static void AddItemToOrder(Order order, List<Product> products)
    {
        AnsiConsole.Clear();
    
        AnsiConsole.Write(
            new FigletText("Cafe Shop")
                .Centered()
                .Color(Color.Green));
            
        DisplayProduct(products);
        DisplayOrder(order);
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Product Type")
                .AddChoices(products.Select(p => p.Type).Distinct()));

        var productID = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Product ID")
                .AddChoices(products.Where(p => p.Type == type).Select(p => p.ProductID)));

        var product = products.First(p => p.ProductID == productID);

        var quantity = AnsiConsole.Prompt(
            new TextPrompt<int>($"Enter Quantity (max {product.Stock}):")
                .Validate(q =>
                {
                    if (q <= 0)
                    {
                        return ValidationResult.Error("[red]Quantity must be greater than 0![/]");
                    }
                    if (q > product.Stock)
                    {
                        return ValidationResult.Error($"[red]Quantity cannot exceed stock ({product.Stock})![/]");
                    }
                    return ValidationResult.Success();
                }));
        

        order.Items.Add(new OrderItem
        {
            ProductID = product.ProductID,
            ProductName = product.ProductName,
            Type = product.Type,
            Quantity = quantity,
            Price = product.Price
        });
        product.Stock -= quantity;
        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[green]Product added to order successfully![/]");
        DisplayProduct(products);
        DisplayOrder(order);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to continue").AddChoices("Continue"));
    }

    static void RemoveItemFromOrder(Order order, List<Product> products)
    {
        AnsiConsole.Clear();
        // tiêu đề
        AnsiConsole.Write(
            new FigletText("Cafe Shop")
                .Centered()
                .Color(Color.Green));
            
        DisplayProduct(products);
        DisplayOrder(order);
        
        // Kiểm tra nếu danh sách sản phẩm trong đơn hàng rỗng
        if (order.Items == null || !order.Items.Any())
        {
            AnsiConsole.MarkupLine("[red]The order is empty. No items to remove.[/]");
            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
            return;
        }

        var productIDs = order.Items.Select(i => i.ProductID).ToList();

        // Kiểm tra nếu không có sản phẩm nào để chọn
        if (!productIDs.Any())
        {
            AnsiConsole.MarkupLine("[red]No products available to remove.[/]");
            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
            return;
        }
        var productID = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Product ID to Remove:")
                .AddChoices(order.Items.Select(i => i.ProductID)));

        var item = order.Items.First(i => i.ProductID == productID);

        // Kiểm tra nếu sản phẩm được chọn không có trong đơn hàng
        if (item == null)
        {
            AnsiConsole.MarkupLine("[red]Selected Product ID does not exist in the order.[/]");
            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
            return;
        }
        order.Items.Remove(item);

        // Restore the stock when the item is removed from the order
        var product = products.First(p => p.ProductID == item.ProductID);

        // Kiểm tra nếu sản phẩm không tồn tại trong danh sách sản phẩm
        if (product == null)
        {
            AnsiConsole.MarkupLine("[red]Product not found in the product list.[/]");
            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to return").AddChoices("Return"));
            return;
        }
        product.Stock += item.Quantity;

        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[green]Product removed to order successfully![/]");
        DisplayProduct(products);
        DisplayOrder(order);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to continue").AddChoices("Continue"));
    }

    static void ClearOrder(Order order, List<Product> products)
    {
        AnsiConsole.Clear();
        // tiêu đề
        AnsiConsole.Write(
            new FigletText("Cafe Shop")
                .Centered()
                .Color(Color.Green));
            
        DisplayProduct(products);
        DisplayOrder(order);
        foreach (var item in order.Items)
        {
            var product = products.First(p => p.ProductID == item.ProductID);
            product.Stock += item.Quantity;
        }

        order.Items.Clear();

        AnsiConsole.Clear();
        // tiêu đề
            AnsiConsole.Write(
                new FigletText("Cafe Shop")
                    .Centered()
                    .Color(Color.Green));
        AnsiConsole.MarkupLine("[yellow]Order cleared and stocks restored.[/]");
        DisplayProduct(products);
        DisplayOrder(order);
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Press [green]Enter[/] to continue").AddChoices("Continue"));
    }

    static void DisplayOrder(Order order)
    {
        var table = new Table();
        table.AddColumn("CustomerID");
        table.AddColumn("ProductID");
        table.AddColumn("ProductName");
        table.AddColumn("Type");
        table.AddColumn("Quantity");
        table.AddColumn("Price");

        foreach (var item in order.Items)
        {
            table.AddRow(order.CustomerID.ToString(), item.ProductID, item.ProductName, item.Type, item.Quantity.ToString(), item.Price.ToString("F2"));
        }

        AnsiConsole.Write(table);
    }

    static void HandlePayment(Order order)
    {
        

        DisplayReceipt(order, currentTips, currentTotalAmount);

        decimal totalPrice = order.Items.Sum(i => i.Quantity * i.Price);
        currentTips = totalPrice * 0.05m; 
        currentTotalAmount = totalPrice + currentTips;

        AnsiConsole.MarkupLine($"Total Amount: [green]{currentTotalAmount:F2}[/]");
        AnsiConsole.MarkupLine($"Tips (5% of total): [green]{currentTips:F2}[/]");

        // Define the directory where invoices will be saved
        string directoryPath = @"C:\Users\ADMIN\CafeShop\Invoices\Invoice_001.txt";

        // Save the receipt to a file
        SaveInvoiceToFile(order, currentTips, currentTotalAmount, directoryPath);
        // Save the receipt to the specified file
        try
        {
            SaveInvoiceToFile(order, currentTips, currentTotalAmount, directoryPath);
            AnsiConsole.MarkupLine($"[green]Invoice has been saved to {directoryPath}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
        }
    
       
    
    }

    static void SaveInvoiceToFile(Order order, decimal tips, decimal totalAmount, string directoryPath)
    {
        // Tạo thư mục nếu chưa tồn tại
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Tạo tên file độc nhất dựa trên thời gian hiện tại và ID đơn hàng
        string fileName = $"Invoice_{order.CustomerID}.txt";
        string filePath = Path.Combine(directoryPath, fileName);

        using (var writer = new StreamWriter(filePath))
        {
            string header = "Cafe Shop";
            writer.WriteLine(new string('=', 50));
            writer.WriteLine(header.PadLeft(50 / 2 + header.Length / 2).PadRight(50));
            writer.WriteLine(new string('=', 50));
            writer.WriteLine();

            writer.WriteLine("18 Tam Trinh, Hoang Mai, Hanoi");
            writer.WriteLine($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine();
            writer.WriteLine("Items:");

            
            const int colWidth1 = 12; // Width for Customer ID
            const int colWidth2 = 12; // Width for Product ID
            const int colWidth3 = 10; // Width for Quantity
            const int colWidth4 = 15; // Width for Price
            const int colWidth5 = 15; // Width for Total

    
            writer.WriteLine("+" + new string('-', colWidth1) + "+" + new string('-', colWidth2) + "+" + new string('-', colWidth3) + "+" + new string('-', colWidth4) + "+" + new string('-', colWidth5) + "+");
            writer.WriteLine($"| {"Customer ID".PadRight(colWidth1)} | {"Product ID".PadRight(colWidth2)} | {"Quantity".PadRight(colWidth3)} | {"Price".PadRight(colWidth4)} | {"Total".PadRight(colWidth5)} ");
            writer.WriteLine("+" + new string('-', colWidth1) + "+" + new string('-', colWidth2) + "+" + new string('-', colWidth3) + "+" + new string('-', colWidth4) + "+" + new string('-', colWidth5) + "+");


            // Dữ liệu bảng
            foreach (var item in order.Items)
            {
                // Chuyển CustomerID từ int sang string
                string customerId = order.CustomerID.ToString().PadRight(colWidth1);
                string productId = item.ProductID.PadRight(colWidth2);
                string quantity = item.Quantity.ToString().PadRight(colWidth3);
                string price = item.Price.ToString("C").PadRight(colWidth4);
                string total = (item.Quantity * item.Price).ToString("C").PadRight(colWidth5);

                writer.WriteLine($"| {customerId} | {productId} | {quantity} | {price} | {total} ");
            }

            writer.WriteLine("+" + new string('-', colWidth1) + "+" + new string('-', colWidth2) + "+" + new string('-', colWidth3) + "+" + new string('-', colWidth4) + "+" + new string('-', colWidth5) + "+");
        
            writer.WriteLine();
            writer.WriteLine($"Total Amount: {totalAmount:F2}");
            writer.WriteLine($"Tips (5% of total): {tips:F2}");
        }
    }


    static void SaveOrder(Order order, decimal tips, decimal totalAmount)
    {
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        

                        var orderCommand = new MySqlCommand("INSERT INTO Orders (CustomerID, Tips, TotalAmount, OrderDate) VALUES (@CustomerID, @Tips, @TotalAmount, @OrderDate)", connection, transaction);
                        orderCommand.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                        orderCommand.Parameters.AddWithValue("@Tips", tips);
                        orderCommand.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        orderCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        orderCommand.ExecuteNonQuery();

                        var orderID = Convert.ToInt32(new MySqlCommand("SELECT LAST_INSERT_ID()", connection, transaction).ExecuteScalar());

                        // Get the last inserted order ID
                        long orderId = orderCommand.LastInsertedId;
                        foreach (var item in order.Items)
                        {
                            var itemCommand = new MySqlCommand("INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price) VALUES (@OrderID, @ProductID, @Quantity, @Price)", connection, transaction);
                            itemCommand.Parameters.AddWithValue("@OrderID", orderId);
                            itemCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                            itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            itemCommand.Parameters.AddWithValue("@Price", item.Price);
                            AnsiConsole.MarkupLine("[yellow]Executing SQL:[/] {0}", itemCommand.CommandText);
                            itemCommand.ExecuteNonQuery();

                            // Update the product stock in the database
                            var updateStockCommand = new MySqlCommand("UPDATE Products SET Stock = Stock - @Quantity WHERE ProductID = @ProductID", connection, transaction);
                            updateStockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            updateStockCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                            updateStockCommand.ExecuteNonQuery();
                        }

                        decimal totalPrice = order.Items.Sum(i => i.Quantity * i.Price);
                        var customerCommand = new MySqlCommand("UPDATE Customers SET TotalPrice = @TotalPrice, Amount = @TotalAmount, Tips = @Tips, PaymentTime = @OrderDate WHERE CustomerID = @CustomerID", connection, transaction);
                        customerCommand.Parameters.AddWithValue("@TotalPrice", totalPrice);
                        customerCommand.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        customerCommand.Parameters.AddWithValue("@Tips", tips);
                        customerCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        customerCommand.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                        customerCommand.ExecuteNonQuery();

                        transaction.Commit();
                        AnsiConsole.MarkupLine("[green]Order saved successfully.[/]");
                    }
                    catch (MySqlException ex)
                    {
                        transaction.Rollback();
                        AnsiConsole.MarkupLine("[red]SQL Error during transaction: {0}[/]", ex.Message);
                        AnsiConsole.MarkupLine("[red]SQL Error details: {0}[/]", ex.ToString());
                        throw;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        AnsiConsole.MarkupLine("[red]Error during transaction: {0}[/]", ex.Message);
                        AnsiConsole.MarkupLine("[red]Error details: {0}[/]", ex.ToString());
                        throw;
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            AnsiConsole.MarkupLine("[red]SQL Error saving order: {0}[/]", ex.Message);
            AnsiConsole.MarkupLine("[red]SQL Error details: {0}[/]", ex.ToString());
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Error saving order: {0}[/]", ex.Message);
            AnsiConsole.MarkupLine("[red]Error details: {0}[/]", ex.ToString());
        }
    }


    static void DisplayReceipt(Order order, decimal tips, decimal totalAmount)
    {
        DateTime orderDate;
        if (order == null || order.Items == null)
        {
            AnsiConsole.MarkupLine("[red]Invalid order data.[/]");
            return;
        }
        var receipt = new Table();
        receipt.AddColumn("CustomerID");
        receipt.AddColumn("ProductID");
        receipt.AddColumn("ProductName");
        receipt.AddColumn("Type");
        receipt.AddColumn("Quantity");
        receipt.AddColumn("Price");

        foreach (var item in order.Items)
        {
            receipt.AddRow(order.CustomerID.ToString(), item.ProductID, item.ProductName, item.Type, item.Quantity.ToString(), item.Price.ToString("F2"));
        }


        AnsiConsole.Write(receipt);
    }



    static void CustomerMenu()
    {
        List<Customer> customers = GetCustomers();
        while (true)
        {
            DisplayCustomers(customers);
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Press [green]Enter[/] back to Main menu.")
                .AddChoices("back"));
            
            if (choice == "back")
                return;            
            
        }
    }
    

    static List<Customer> GetCustomers()
    {
        // Kết nối và lấy dữ liệu khách hàng từ MySQL
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        List<Customer> customers = new List<Customer>();

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Customers", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        CustomerID = reader.GetInt32("CustomerID"),
                        TotalPrice = reader.GetDecimal("TotalPrice"),
                        Amount = reader.GetDecimal("Amount"),
                        Tips = reader.GetDecimal("Tips"),
                        PaymentTime = reader.GetDateTime("PaymentTime")
                    });
                }
            }
        }
        return customers;
    }

    static void DisplayCustomers(List<Customer> customers)
    {
        var table = new Table();
        table.AddColumn("CustomerID");
        table.AddColumn("TotalPrice");
        table.AddColumn("Amount");
        table.AddColumn("Tips");
        table.AddColumn("PaymentTime");

        foreach (var customer in customers)
        {
            table.AddRow(customer.CustomerID.ToString(), customer.TotalPrice.ToString("F2"), customer.Amount.ToString("F2"), customer.Tips.ToString("F2"), customer.PaymentTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        AnsiConsole.Write(table);
    }




    static void DashboardMenu()
    {
        int totalCashiers = GetTotalCashier();
        int totalCustomers = GetTotalCustomers();
        decimal todaysIncome = GetTodaysIncome();
        decimal totalIncome = GetTotalIncome();

        var table = new Table();
        table.AddColumn("Total of Cashiers");
        table.AddColumn("Total of Customers");
        table.AddColumn("Today's Income");
        table.AddColumn("Total Income");

        table.AddRow(totalCashiers.ToString(), totalCustomers.ToString(), todaysIncome.ToString("F2"), totalIncome.ToString("F2"));
        AnsiConsole.Write(table);

        // Lấy dữ liệu doanh số 7 ngày gần nhất và hiển thị sơ đồ cột
        var dailySales = GetDailySales();
        DisplaySalesBarChart(dailySales);

        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Press [green]Enter[/] back to Main Menu.")
                .AddChoices("back"));
    }

    static int GetTotalCashier()
    {
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Role = 'Cashier'", connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }

    static int GetTotalCustomers()
    {
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT COUNT(*) FROM Customers", connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }

    static decimal GetTodaysIncome()
    {
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT SUM(TotalAmount) FROM Orders WHERE DATE(OrderDate) = CURDATE()", connection);
            return Convert.ToDecimal(command.ExecuteScalar());
        }
    }

    static decimal GetTotalIncome()
    {
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT SUM(TotalAmount) FROM Orders", connection);
            return Convert.ToDecimal(command.ExecuteScalar());
        }
    }


    static List<DailySales> GetDailySales()
    {
        string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = @"
                SELECT DATE(OrderDate) AS SaleDate, SUM(TotalAmount) AS TotalSales
                FROM Orders
                GROUP BY DATE(OrderDate)
                ORDER BY SaleDate DESC
                LIMIT 7";
            return connection.Query<DailySales>(query).ToList();
        }
    }

    static void DisplaySalesBarChart(List<DailySales> sales)
    {
        var chart = new BarChart()
            .Width(60)
            .Label("[bold underline]Today's Income (Last 7 Days)[/]")
            .CenterLabel();

        // Sắp xếp lại theo thứ tự tăng dần của ngày
        sales = sales.OrderBy(s => s.SaleDate).ToList();

        foreach (var sale in sales)
        {
            chart.AddItem(sale.SaleDate.ToString("yyyy-MM-dd"), (double)sale.TotalSales, Color.Green);
        }

        AnsiConsole.Write(chart);
    }

}

