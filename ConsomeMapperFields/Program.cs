using DeveloperTi.Mapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace ConsomeMapperFields
{
    class Program
    {
        static void Main(string[] args)
        {
            var userMapper = new MapperFields<User>();
            
            //var userMapper = new MapperFields<Carro>();
            //var dataTable = CreateDataListCars();

            //var dataTable = CreateDataTable();
            var dataTable = CreateDataListUsers();

            var resultUser = userMapper.MapAll(dataTable);
            foreach (var user in resultUser)
            {
                Console.WriteLine("Id:{0} - Name:{1}", user.Id, user.Name);
            }
            Console.ReadKey();
        }

        private static List<User> CreateDataListUsers()
        {
            return new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "João" },
                new User { Id = Guid.NewGuid(), Name = "Pedro" }
            };
        }

        private static List<Carro> CreateDataListCars()
        {
            return new List<Carro>
            {
                new Carro { Id = Guid.NewGuid(), Name = "Palio" },
                new Carro { Id = Guid.NewGuid(), Name = "Corsa" }
            };
        }

        private static DataTable CreateDataTable()
        {
            DataTable datatable = new DataTable("User");

            datatable.Columns.AddRange
            (
                new[]
                {
            new DataColumn {ColumnName = "id_user", DataType = typeof(Guid)},
            new DataColumn {ColumnName = "first_name", DataType = typeof(string)}
                }
            );

            datatable.Rows.Add(Guid.NewGuid(), "Paul");
            datatable.Rows.Add(Guid.NewGuid(), "Max");

            return datatable;
        }
    }

    public class User
    {
        [MapFieldName("id", "id_user", "user_ID")]
        public Guid Id { get; set; }

        [MapFieldName("first_name", "name_user", "name")]
        public string Name { get; set; }
    }

    public class Carro
    {
        [MapFieldName("id", "id_user", "user_ID")]
        public Guid Id { get; set; }

        [MapFieldName("first_name", "name_user", "name")]
        public string Name { get; set; }
    }
}
