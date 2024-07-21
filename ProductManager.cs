using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class ProductManager
{
    private string connectionString = "Server=localhost;Database=cafe_shop;User=root;Password=youngboy19";

    public List<Product> GetAllProducts()
    {
        var products = new List<Product>();

        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "SELECT * FROM Products";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            Id = reader.GetInt32("Id"),
                            ProductID = reader.GetString("ProductID"),
                            ProductName = reader.GetString("ProductName"),
                            Type = reader.GetString("Type"),
                            Stock = reader.GetInt32("Stock"),
                            Price = reader.GetDecimal("Price"),
                            Status = reader.GetString("Status"),
                            Image = reader.GetString("Image"),
                            DateInsert = reader.GetDateTime("DateInsert"),
                            DateUpdate = reader.IsDBNull(reader.GetOrdinal("DateUpdate")) ? (DateTime?)null : reader.GetDateTime("DateUpdate")
                        };
                        products.Add(product);
                    }
                }
            }
        }

        return products;
    }

    public void AddProduct(Product product)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "INSERT INTO Products (ProductID, ProductName, Type, Stock, Price, Status, Image, DateInsert) VALUES (@ProductID, @ProductName, @Type, @Stock, @Price, @Status, @Image, @DateInsert)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProductID", product.ProductID);
                command.Parameters.AddWithValue("@ProductName", product.ProductName);
                command.Parameters.AddWithValue("@Type", product.Type);
                command.Parameters.AddWithValue("@Stock", product.Stock);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Status", product.Status);
                command.Parameters.AddWithValue("@Image", product.Image);
                command.Parameters.AddWithValue("@DateInsert", product.DateInsert);
                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdateProduct(Product product)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "UPDATE Products SET ProductName = @ProductName, Type = @Type, Stock = @Stock, Price = @Price, Status = @Status, Image = @Image, DateUpdate = @DateUpdate WHERE Id = @Id";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", product.Id);
                command.Parameters.AddWithValue("@ProductName", product.ProductName);
                command.Parameters.AddWithValue("@Type", product.Type);
                command.Parameters.AddWithValue("@Stock", product.Stock);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Status", product.Status);
                command.Parameters.AddWithValue("@Image", product.Image);
                command.Parameters.AddWithValue("@DateUpdate", product.DateUpdate);
                command.ExecuteNonQuery();
            }
        }
    }


    public int GetNextProductID()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var query = "SELECT COUNT(*) FROM Products";
            using (var command = new MySqlCommand(query, connection))
            {
                return Convert.ToInt32(command.ExecuteScalar()) + 1;
            }
        }
    }
}
