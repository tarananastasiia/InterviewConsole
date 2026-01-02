using System.Data.SqlClient;
using System.Data;
using System;
using EmployeeService.Model;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;

namespace EmployeeService
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public void UpdateEnableFlag(int id, int enable)
        {
            var query = @"
                    UPDATE Employee
                    SET Enable={enable}
                    WHERE ID=@id";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            GetQueryResult(query, parameters);
        }

        public List<Employee> GetEmployeeById(int id)
        {
            var query = @"
                    WITH EmployeeHierarchy AS
                    (
                        SELECT 
                            ID,
                            Name,
                            ManagerID,
                            Enable,
                            CAST(ID AS VARCHAR(MAX)) AS Path
                        FROM dbo.Employee
                        WHERE ID = @id
                        UNION ALL
                        SELECT 
                            e.ID,
                            e.Name,
                            e.ManagerID,
                            e.Enable,
                            eh.Path + ',' + CAST(e.ID AS VARCHAR(MAX))
                        FROM dbo.Employee e
                        INNER JOIN EmployeeHierarchy eh
                            ON e.ManagerID = eh.ID
                        WHERE CHARINDEX(',' + CAST(e.ID AS VARCHAR) + ',', ',' + eh.Path + ',') = 0
                    )
                    SELECT ID, Name, ManagerID, Enable
                    FROM EmployeeHierarchy";

            var parameters = new SqlParameter[]
            {
                    new SqlParameter("@id", id)
            };

            var employeesDataTable = GetQueryResult(query, parameters);

            var result = employeesDataTable.AsEnumerable().Select(x => new Employee
            {
                Id = x.Field<int>("Id"),
                Name = x.Field<string>("Name"),
                ManagerId = x.Field<int?>("ManagerId")
            }).ToList();

            return result;
        }

        private static DataTable GetQueryResult(string query, SqlParameter[] parameters = null)
        {
            var dt = new DataTable();

            using (var connection = new SqlConnection("Data Source=AN-TARAN-PC\\SQLEXPRESS;Initial Catalog=Test;Integrated Security=SSPI;"))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }
    }
}