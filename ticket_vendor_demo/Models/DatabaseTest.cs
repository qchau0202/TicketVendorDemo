using System;
using System.Data.SqlClient;
using System.Configuration;

namespace ticket_vendor_demo.Models
{
    public class DatabaseTest
    {
        public static string TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    connection.Open();
                    return "Kết nối database thành công!";
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi kết nối: {ex.Message}";
            }
        }
    }
}