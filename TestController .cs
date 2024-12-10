using Microsoft.AspNetCore.Mvc;
using DAL;
using DAL.Customer;
using System;
using System.Data;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography.X509Certificates;
using BLL;

namespace WEBAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase    
    {
        [Route("QuestionOne")]
        [HttpPost]
        public IActionResult QuestionOne(List<int> ParameterInput)
        {
            try
            {
                int ExcludeIndex = 1;
                List<int> defaultInput = [2, 4, 6, 5, 3, 1, 7, 9, 10, 8];
                List<int> input = ParameterInput.Count <= ExcludeIndex ? defaultInput : ParameterInput;
                List<int> result = new List<int>();

                foreach (int i in input)
                {
                    if (i % 2 != 0)
                    {
                        result.Add(i);
                    }                   
                }

                result.Sort((a, b) => b.CompareTo(a));
                result.RemoveAt(ExcludeIndex);

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [Route("QuestionTwo")]
        [HttpPost]
        public IActionResult QuestionTwo(string ParamInput = "")
        {
            try
            {
                string input = ParamInput.Length < 0 ? ParamInput : "aaabbcccaaaac";
                int count = 1;
                string result = "";

                for (int i = 1; i < input.Length; i++)
                {
                    if (input[i] == input[i - 1])
                    {
                        count++;
                    }
                    else
                    {
                        result = $"{result} {input[i - 1]} = {count}\n";

                        count = 1;
                    }
                }
                result = $"{result} {input[input.Length - 1]} = {count}";

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("QuestionThree")]
        [HttpPost]
        public IActionResult QuestionThree(string ParamInput = "")
        {
            try
            {
                string input = ParamInput.Length < 0 ? ParamInput : "aaabbcccaaaac";
                int count = 1;
                string stringResult;

                ExecuteNonQuery(StringQuery.CreateSPCustomerTransactionRank);
                DataTable dt = ExecuteTable(StringQuery.ExecuteCreateSPCustomerTransactionRank);

                var result = dt.AsEnumerable().Select(row => new
                {
                    Customer_ID = row["Customer_ID"],
                    TotalPenjualan = row["TotalPenjualan"]
                });

                stringResult = "Customer_ID\tTotal Penjualan\n";
                foreach (var row in result)
                {
                    stringResult = $"{stringResult}{row.Customer_ID}\t\t{row.TotalPenjualan}\n";
                }

                return Ok(stringResult);
            }
            catch (Exception)
            {
                throw;
            }
        }

        static DataTable ExecuteTable (string query)
        {
            string connectionString = Config.connectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    DataTable dataTable = new DataTable();

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, conn))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    conn.Close();
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        static void ExecuteNonQuery(string query)
        {
            string connectionString = Config.connectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public class StringQuery
        {
            public static string CreateSPCustomerTransactionRank { get; } = $"CREATE OR ALTER PROCEDURE usp_CustomerTransactionRank\r\nAS\r\nBEGIN\r\n\tDROP TABLE IF EXISTS #Transactions\r\n\tCREATE TABLE #Transactions (\r\n\t\tID INT,\r\n\t\tCustomer_ID VARCHAR(MAX),\r\n\t\tTransaction_Date DATE\r\n\t);\r\n\r\n\tINSERT INTO #Transactions (ID, Customer_ID, Transaction_Date)\r\n\tVALUES\t(1, '21', '2019-07-30'),\r\n\t(2, '15', '2019-07-21'),\r\n\t(3, '16', '2019-07-18'),\r\n\t(4, '20', '2019-07-22'),\r\n\t(5, '15', '2019-07-15'),\r\n\t(6, '20', '2019-07-12'),\r\n\t(7, '15', '2019-07-21'),\r\n\t(8, '20', '2019-07-12');\r\n\r\n\tSELECT Customer_ID, COUNT(Customer_ID) AS TotalPenjualan FROM #Transactions GROUP BY Customer_ID\r\n\tORDER BY COUNT(Customer_ID) DESC, Customer_ID\r\nEND";

            public static string ExecuteCreateSPCustomerTransactionRank { get; } = $"EXEC usp_CustomerTransactionRank";
        }

        public class Config
        {
            public static string connectionString { get; } = "Server=NDS-LPT-0512\\SQL2019;Database=main;User Id=sa;Password=nawadata;";
        }
    }
}
