using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using DataStorage.Mappers;
using Model;

namespace DTO
{
    class Program
    {
        static void Main(string[] args)
        {
            //string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=messagesdb;Integrated Security=True";
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Подключение открыто");
                //string sqlExpression = "select Id, [From], [To], [Text], IsSent from [dbo].[messages]";
                Guid id = Guid.NewGuid();
                Console.WriteLine(id);
                string sqlExpression = $"insert into messages values ('{id}', 'First', 'Second12', 'teretr', 0)";
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                //SqlDataReader reader = command.ExecuteReader();
                int number = command.ExecuteNonQuery();
                //if (reader.HasRows) // если есть данные
                //{
                //    // выводим названия столбцов
                //    Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3), reader.GetName(4));

                //    while (reader.Read()) // построчно считываем данные
                //    {
                //        object id = reader["Id"];
                //        object from = reader.GetValue(1);
                //        object to = reader.GetValue(2);
                //        object text = reader.GetValue(3);
                //        object isSent = reader.GetValue(4);
                //        MessageDTOMapper mapper = new MessageDTOMapper();
                //        MessageDTO msg = mapper.ReadItem(reader);

                //        Console.WriteLine("{0} \t{1} \t{2} \t{3} \t{4}", id, from, to, text, isSent);
                //    }
                //}
            }
            Console.WriteLine("Подключение закрыто...");

            Console.Read();
        }
    }
}
