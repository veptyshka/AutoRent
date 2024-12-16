using System;
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;

// Database setup
class DbSetup
{
    public static void InitDb()
    {
        using (var connection = new SqliteConnection("Data Source=car_rent.db;Version=3;"))
        {
            connection.Open();

            string createCarsTable = 
            @"CREATE TABLE IF NOT EXISTS Cars (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            Model TEXT NOT NULL,
            HourRate REAL NOT NULL,
            KmRate REAL NOT NULL);";

            string createClientsTable = 
            @"CREATE TABLE IF NOT EXISTS Clients (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            FullName TEXT NOT NULL,
            Email TEXT UNIQUE NOT NULL);";

            string createRentalsTable = 
            @"CREATE TABLE IF NOT EXISTS Rentals (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            ClientID INTEGER,
            CarID INTEGER,
            StartTime TEXT,
            EndTime TEXT,
            KmDriven REAL,
            TotalPay REAL,
            FOREIGN KEY(Client ID) REFERENCES Clients(ID),
            FOREIGN KEY(Car ID) REFERENCES Cars(ID));";

            var command = connection.CreateCommand();
            command.CommandText = createCarsTable + createClientsTable + createRentalsTable;
            command.ExecuteNonQuery();

            Console.WriteLine("DataBase and tables created succesfully");
        }
    }
}

public class Car        // Car class
{
    public int ID { get; set; }
    public string Model { get; set; }
    public double HourRate { get; set; }
    public double KmRate { get; set; }
}

public class Client     // Client class
{
    public int ID { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}

public class Rental     // Rental class
{
    public int ID { get; set; }
    public int ClientID { get; set; }
    public int CarID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double KmDriven { get; set; }
    public double TotalPay { get; set; }
}

// Client register
public static void RegisterClient(string fullName, string email)
{
    using (var connection = new SqliteConnection("Data Source=car_rent.db;Version=3;"))
    {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Clients (FullName, Email) VALUES (@FullName, @Email);";
        command.Parameters.AddWithValue("@FullName", fullName);
        command.Parameters.AddWithValue("@Email", email);
        command.ExecuteNonQuery();

        Console.WriteLine("Client registered succesfully");
    }
}

// Auto register
public static void AddCar(string model, double hourlyRate, double kmRate)
{
    using (var connection = new SqliteConnection("Data Source=car_rent.db;Version=3;"))
    {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Cars (Model, HourRate, KmRate) VALUES (@Model, @HourRate, @KmRate);";
        command.Parameters.AddWithValue("@Model", model);
        command.Parameters.AddWithValue("@HourRate", hourlyRate);
        command.Parameters.AddWithValue("@KmRate", kmRate);
        command.ExecuteNonQuery();

        Console.WriteLine("Car added succesfully");
    }
}

// Car rental
public static void RentRegister(int clientID, int carID, DateTime startTime, DateTime endTime, double kmDriven)
{
    using (var connection = new SqliteConnection("Data Source=car_rent.db;Version=3;"))
    {
        connection.Open();
        var getCarRatesCommand = connection.CreateCommand();
        getCarRatesCommand.CommandText = "SELECT HourRate, KmRate FROM Cars WHERE ID = @CarID;";
        getCarRatesCommand.Parameters.AddWithValue("@CarID", carID);
        var reader = getCarRatesCommand.ExecuteReader();
        if (!reader.Read()) throw new Exception("Car not found");

        double hourlyRate = reader.GetDouble(0);
        double kmRate = reader.GetDouble(1);

        // Calculating payment
        double rentHours = (endTime - startTime).TotalHours;
        double totalPay = (rentHours * hourlyRate) + (kmDriven * kmRate);

        // Saving rental into db
        var insertRentCommand = connection.CreateCommand();
        insertRentCommand.CommandText = 
        @"INSERT INTO Rentals (ClientID, CarID, StartTime, EndTime, KmDriven, TotalPay)
        VALUES (@ClientID, @CarID, @StartTime, @EndTime, @KmDriven, @TotalPay);";
        insertRentCommand.Parameters.AddWithValue("@ClientID", clientID);
        insertRentCommand.Parameters.AddWithValue("@CarID", carID);
        insertRentCommand.Parameters.AddWithValue("@StartTime", startTime);
        insertRentCommand.Parameters.AddWithValue("@EndTime", endTime);
        insertRentCommand.Parameters.AddWithValue("@KmDriven", kmDriven);
        insertRentCommand.Parameters.AddWithValue("@ToTalPay", totalPay);
        insertRentCommand.ExecuteNonQuery();

        Console.WriteLine($"Rental registered, total payment: {totalPay:C}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        DbSetup.InitDb();

        // Adding some cars
        AddCar("Model 1", 10.5, 0.5);
        AddCar("Model 2", 12.0, 0.5);
        AddCar("Model 3", 15.0, 0.8);

        // Registering clients
        RegisterClient("Argus Filch", "argusfilch@gmail.com");
        RegisterClient("Gandalf the Gray", "youshallnotpass@gmail.com");

        // Registering rentals
        RentRegister(
            clientID: 1, carID: 2,
            startTime: DateTime.Now.AddHours(-2),
            endTime: DateTime.Now,
            kmDriven: 123);
        RentRegister(
            clientID: 2, carID: 3,
            startTime: DateTime.Now.AddHours(-24),
            endTime: DateTime.Now,
            kmDriven: 2000);        // He had driven to Shire and back
    }
}