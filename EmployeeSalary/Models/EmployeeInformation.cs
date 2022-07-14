namespace EmployeeSalary.Models
{
    internal class EmployeeInformation
    {
        public EmployeeInformation(int id, string fullName, EmployeeContract firstContract)
        {
            Id = id;
            FullName = fullName;
            Contracts = new List<EmployeeContract> { firstContract };
        }

        public int Id { get; private set; }
        public string FullName { get; private set; }
        public List<EmployeeContract> Contracts { get; private set; }
    }
}
