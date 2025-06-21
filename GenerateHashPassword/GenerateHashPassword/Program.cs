using System;
using BCrypt.Net;

class Program
{
    static void Main(string[] args)
    {
        string password = "John@123";
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine(hashedPassword);
        Console.ReadLine();
    }
}
