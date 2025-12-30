using EmployeeService.Model;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using EmployeeService.ResponseModel;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService()
        {
            _employeeRepository = new EmployeeRepository();
        }
        public EmployeeResponse GetEmployeeById(int id)
        {
            var employees = _employeeRepository.GetEmployeeById(id);
            var mainEmploee = employees.Find(x => x.Id == id);
            var result = MapResponse(mainEmploee);
            FillSubEmployees(result, employees.ToLookup(x=> x.ManagerId));

            return result;
        }

        public void EnableEmployee(int id, int enable)
        {
            _employeeRepository.UpdateEnableFlag(id, enable);
        }

        private static void FillSubEmployees(EmployeeResponse employee, ILookup<int?, Employee> employees, List<int> visited = null)
        {
            if (visited == null)
                visited = new List<int>();

            visited.Add(employee.Id);

            var newLayer = employees[employee.Id].Where(x => !visited.Contains(x.Id));
            employee.Employees = newLayer.Select(x => MapResponse(x)).ToList();
            employee.Employees.ForEach(x => FillSubEmployees(x, employees, visited));
        }

        private static EmployeeResponse MapResponse(Employee employee)
        {
            var response = new EmployeeResponse()
            {
                Id = employee.Id,
                Name = employee.Name,
                ManagerId = employee.ManagerId
            };
            return response;
        }
    }


}