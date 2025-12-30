using System.Collections.Generic;

namespace EmployeeService.ResponseModel
{
    public class EmployeeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ManagerId { get; set; }

        public List<EmployeeResponse> Employees { get; set; }
    }
}