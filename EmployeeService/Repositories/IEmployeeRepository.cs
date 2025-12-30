using EmployeeService.Model;
using System.Collections.Generic;

namespace EmployeeService
{
    public interface IEmployeeRepository
    {
        void UpdateEnableFlag(int id, int enable);
        List<Employee> GetEmployeeById(int id);
    }
}
