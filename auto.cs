using System;
using Microsoft.Data.Sqlite;

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
        }
    }
}