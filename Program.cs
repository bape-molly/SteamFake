using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Esf;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var userManager = new UserManager();
        

        while (true)
        {
            
            
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Chọn [green]tác vụ[/]:")
                    .AddChoices("Đăng nhập", "Đăng ký", "Thoát"));

            switch (choice)
            {
                case "Đăng nhập":
                    {
                        var username = AnsiConsole.Ask<string>("Nhập [green]tên tài khoản[/]:");
                        var password = AnsiConsole.Prompt(
                            new TextPrompt<string>("Nhập [green]mật khẩu[/]:").Secret());

                        var user = userManager.LoginUser(username, password);

                        if (user != null && user.Role == "Admin")
                        {
                            AnsiConsole.MarkupLine("[green]Đăng nhập thành công![/]");
                            MainMenu(user);
                            // Thêm logic khi đăng nhập thành công, ví dụ: chuyển đến menu chính của hệ thống.
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Đăng nhập thất bại hoặc không có quyền truy cập![/]");
                        }

                        break;
                    }

                case "Đăng ký":
                    {
                        var username = AnsiConsole.Ask<string>("Nhập [green]tên tài khoản[/]:");
                        var password = AnsiConsole.Prompt(
                            new TextPrompt<string>("Nhập [green]mật khẩu[/]:").Secret());
                        var confirmPassword = AnsiConsole.Prompt(
                            new TextPrompt<string>("Nhập lại [green]mật khẩu[/]:").Secret());
                        
                        if (password != confirmPassword)
                        {
                            AnsiConsole.MarkupLine("[red]Mật khẩu nhập lại không khớp![/]");
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

                        userManager.RegisterUser(user);
                        AnsiConsole.MarkupLine("[green]Đăng ký thành công![/]");

                        break;
                    }

                case "Thoát":
                    return;
            }
        }
    }


    static void MainMenu(User loggedInUser)
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold blue]Xin chào Admin, {loggedInUser.Username}![/]");
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Chọn [green]chức năng[/]:")
                    .AddChoices("Dashboard", "Cashier", "Products", "Orders", "Customers", "Logout"));

            switch (choice)
            {
                case "Dashboard":
                    AnsiConsole.MarkupLine("[yellow]Bảng điều khiển hiện chưa có chức năng![/]");
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
            AnsiConsole.MarkupLine($"[bold blue]Quản lý thu ngân - {loggedInUser.Username}[/]");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Chọn [green]chức năng[/]:")
                    .AddChoices("Hiển thị danh sách thu ngân", "Thêm thu ngân", "Sửa thông tin thu ngân", "Xóa thu ngân", "Quay lại"));

            switch (choice)
            {
                case "Hiển thị danh sách thu ngân":
                    DisplayCashiers(userManager);
                    break;

                case "Thêm thu ngân":
                    AddCashier(userManager);
                    break;

                case "Sửa thông tin thu ngân":
                    EditCashier(userManager);
                    break;

                case "Xóa thu ngân":
                    DeleteCashier(userManager);
                    break;

                case "Quay lại":
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
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Nhấn [green]Enter[/] để quay lại").AddChoices("Quay lại"));
    }

    static void AddCashier(UserManager userManager)
    {
        var username = AnsiConsole.Ask<string>("Nhập [green]tên tài khoản[/]:");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Nhập [green]mật khẩu[/]:").Secret());

        var role = "Cashier";
        var status = "Active";
        var image = AnsiConsole.Ask<string>("Nhập [green]hình ảnh (URL)[/]:");
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
        AnsiConsole.MarkupLine("[green]Thêm thu ngân thành công![/]");
    }

    static void EditCashier(UserManager userManager)
    {
        var cashierId = AnsiConsole.Ask<int>("Nhập [green]ID thu ngân[/] cần sửa:");
        var cashier = userManager.GetUserById(cashierId);

        if (cashier == null || cashier.Role != "Cashier")
        {
            AnsiConsole.MarkupLine("[red]Thu ngân không tồn tại hoặc không phải thu ngân![/]");
            return;
        }

        var username = AnsiConsole.Ask<string>($"Nhập [green]tên tài khoản[/] ({cashier.Username}):", cashier.Username);
        var password = AnsiConsole.Ask<string>($"Nhập [green]mật khẩu[/] ({cashier.Password}):", cashier.Password);
        var status = AnsiConsole.Ask<string>($"Nhập [green]trạng thái[/] ({cashier.Status}):", cashier.Status);
        var image = AnsiConsole.Ask<string>($"Nhập [green]hình ảnh (URL)[/] ({cashier.Image}):", cashier.Image);

        cashier.Username = username;
        cashier.Password = password;
        cashier.Status = status;
        cashier.Image = image;

        userManager.UpdateUser(cashier);
        AnsiConsole.MarkupLine("[green]Sửa thông tin thu ngân thành công![/]");
    }

    static void DeleteCashier(UserManager userManager)
    {
        var cashierId = AnsiConsole.Ask<int>("Nhập [green]ID thu ngân[/] cần xóa:");
        var cashier = userManager.GetUserById(cashierId);

        if (cashier == null)
        {
            AnsiConsole.MarkupLine("[red]Thu ngân không tồn tại![/]");
            return;
        }

        userManager.DeleteUser(cashierId);
        AnsiConsole.MarkupLine("[green]Xóa thu ngân thành công![/]");
    }





    static void ProductsMenu()
    {
        var productManager = new ProductManager();

        while (true)
        {
            AnsiConsole.Clear();
            DisplayProducts(productManager);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Chọn [green]chức năng[/]:")
                    .AddChoices("Thêm sản phẩm", "Sửa thông tin sản phẩm", "Xóa sản phẩm", "Quay lại"));

            switch (choice)
            {
                case "Thêm sản phẩm":
                    AddProduct(productManager);
                    break;

                case "Sửa thông tin sản phẩm":
                    EditProduct(productManager);
                    break;

                case "Xóa sản phẩm":
                    DeleteProduct(productManager);
                    break;

                case "Quay lại":
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
        table.AddColumn("DateInsert");
        table.AddColumn("DateUpdate");

        foreach (var product in products)
        {
            table.AddRow(product.Id.ToString(), product.ProductID, product.ProductName, product.Type, product.Stock.ToString(), product.Price.ToString(), product.Status, product.DateInsert.ToString(), product.DateUpdate?.ToString());
        }

        AnsiConsole.Render(table);
    }


    static void AddProduct(ProductManager productManager)
    {
        var productID = $"PROD-{productManager.GetNextProductID()}";
        var productName = AnsiConsole.Ask<string>("Nhập [green]tên sản phẩm[/]:");
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Chọn [green]loại sản phẩm[/]:")
                .AddChoices("Meal", "Drink"));
        var stock = AnsiConsole.Ask<int>("Nhập [green]số lượng[/]:");
        var price = AnsiConsole.Ask<decimal>("Nhập [green]giá[/]:");
        var status = "Available";
        var image = AnsiConsole.Ask<string>("Nhập [green]hình ảnh (URL)[/]:");
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
        AnsiConsole.MarkupLine("[green]Thêm sản phẩm thành công![/]");
        DisplayProducts(productManager);
    }

    static void EditProduct(ProductManager productManager)
    {
        var productId = AnsiConsole.Ask<int>("Nhập [green]ID sản phẩm[/] cần sửa:");
        var product = productManager.GetAllProducts().Find(p => p.Id == productId);

        if (product == null)
        {
            AnsiConsole.MarkupLine("[red]Sản phẩm không tồn tại![/]");
            return;
        }

        var productName = AnsiConsole.Ask<string>($"Nhập [green]tên sản phẩm[/] ({product.ProductName}):", product.ProductName);
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Chọn [green]loại sản phẩm[/] ({product.Type}):")
                .AddChoices("Meal", "Drink")
                .UseConverter(item => item == product.Type ? $"{item} (Current)" : item));
        var stock = AnsiConsole.Ask<int>($"Nhập [green]số lượng[/] ({product.Stock}):", product.Stock);
        var price = AnsiConsole.Ask<decimal>($"Nhập [green]giá[/] ({product.Price:C}):", product.Price);
        var status = "Available";
        var image = AnsiConsole.Ask<string>($"Nhập [green]hình ảnh (URL)[/] ({product.Image}):", product.Image);
        var dateUpdate = DateTime.Now;

        product.ProductName = productName;
        product.Type = type;
        product.Stock = stock;
        product.Price = price;
        product.Status = status;
        product.Image = image;
        product.DateUpdate = dateUpdate;

        productManager.UpdateProduct(product);
        AnsiConsole.MarkupLine("[green]Sửa thông tin sản phẩm thành công![/]");
        DisplayProducts(productManager);
    }

    static void DeleteProduct(ProductManager productManager)
    {
        var productId = AnsiConsole.Ask<int>("Nhập [green]ID sản phẩm[/] cần xóa:");
        var product = productManager.GetAllProducts().Find(p => p.Id == productId);

        if (product == null)
        {
            AnsiConsole.MarkupLine("[red]Sản phẩm không tồn tại![/]");
            return;
        }

        productManager.DeleteProduct(productId);
        AnsiConsole.MarkupLine("[green]Xóa sản phẩm thành công![/]");
        DisplayProducts(productManager);
    }




    static Order currentOrder;
    static decimal currentTips;
    static decimal currentTotalAmount;
    
    static void OrderMenu()
    {
        
        List<Product> products = GetProducts();
        while(true)
        {
            AnsiConsole.Clear();
		    DisplayProduct(products);
		    var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Chọn [green]chức năng[/]:")
                    .AddChoices("Tạo đơn hàng", "Xuất hoá đơn", "Quay lại"));
            
            switch (choice)
            {
                case "Tạo đơn hàng":
                    CreateOrder(products);
                    break;
                
                case "Xuất hoá đơn":
                    DisplayReceipt(currentOrder, currentTips, currentTotalAmount);
                    break;
                
                case "Quay lại":
                    return;
            }
        }

    }


    static List<Product> GetProducts()
    {
        // Kết nối và lấy dữ liệu sản phẩm từ MySQL
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
    
    static Order CreateOrder(List<Product> products)
    {
        var order = new Order();
        order.Items = new List<OrderItem>();

        while (true)
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Order Menu")
                    .AddChoices("Add Item", "Remove Item", "Clear Order", "Finish Order"));

            switch (action)
            {
                case "Add Item":
                    AddItemToOrder(order, products);
                    break;
                case "Remove Item":
                    RemoveItemFromOrder(order, products);
                    break;
                case "Clear Order":
                    ClearOrder(order, products);
                    break;
                case "Finish Order":
                    SaveOrder(order, currentTips, currentTotalAmount);
                    HandlePayment(order);
                    return order;
            }
        }
    }

    static void AddItemToOrder(Order order, List<Product> products)
    {
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
        AnsiConsole.MarkupLine("[green]Product added to order successfully![/]");
        DisplayOrder(order);
    }

    static void RemoveItemFromOrder(Order order, List<Product> products)
    {
        var productID = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Product ID to Remove:")
                .AddChoices(order.Items.Select(i => i.ProductID)));

        var item = order.Items.First(i => i.ProductID == productID);
        order.Items.Remove(item);

        // Restore the stock when the item is removed from the order
        var product = products.First(p => p.ProductID == item.ProductID);
        product.Stock += item.Quantity;
        AnsiConsole.MarkupLine("[green]Product removed to order successfully![/]");
        DisplayOrder(order);
    }

    static void ClearOrder(Order order, List<Product> products)
    {
        foreach (var item in order.Items)
        {
            var product = products.First(p => p.ProductID == item.ProductID);
            product.Stock += item.Quantity;
        }

        order.Items.Clear();
        AnsiConsole.MarkupLine("[yellow]Order cleared and stocks restored.[/]");
        DisplayOrder(order);
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
        decimal totalPrice = order.Items.Sum(i => i.Quantity * i.Price);
        currentTips = totalPrice * 0.05m; // tính tips là 5% tổng giá trị sản phẩm
        currentTotalAmount = totalPrice + currentTips;

        AnsiConsole.MarkupLine($"Total Amount: [green]{currentTotalAmount:F2}[/]");
        AnsiConsole.MarkupLine($"Tips (5% of total): [green]{currentTips:F2}[/]");

        DisplayReceipt(order, currentTips, currentTotalAmount);
        
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

                        // Get the last inserted order ID
                        long orderId = orderCommand.LastInsertedId;
                        foreach (var item in order.Items)
                        {
                            var itemCommand = new MySqlCommand("INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price) VALUES (@OrderID, @ProductID, @Quantity, @Price)", connection, transaction);
                            itemCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                            itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            itemCommand.Parameters.AddWithValue("@Price", item.Price);
                            itemCommand.ExecuteNonQuery();

                            // Update the product stock in the database
                            var updateStockCommand = new MySqlCommand("UPDATE Products SET Stock = Stock - @Quantity WHERE ProductID = @ProductID", connection, transaction);
                            updateStockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                            updateStockCommand.Parameters.AddWithValue("@ProductID", item.ProductID);
                            updateStockCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        AnsiConsole.MarkupLine("[red]Error during transaction: {0}[/]", ex.Message);
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Error saving order: {0}[/]", ex.Message);
        }
    }

    static void DisplayReceipt(Order order, decimal tips, decimal totalAmount)
    {
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

        receipt.AddRow("", "", "", "", "Tips", tips.ToString("F2"));
        receipt.AddRow("", "", "", "", "Total Amount", totalAmount.ToString("F2"));

        AnsiConsole.Write(receipt);
    }



    static void CustomerMenu()
    {
        List<Customer> customers = GetCustomers();
        DisplayCustomers(customers);
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

}

