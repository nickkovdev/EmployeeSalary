namespace EmployeeSalary.Models
{
    internal class EmployeeContract
    {
        public EmployeeContract(decimal hourlySalary, DateTime startDate)
        {
            StartDate = startDate;
            HourlySalary = hourlySalary;
            Worklogs = new List<(DateTime DateLogged, TimeSpan Duration)>();
        }
        
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public List<(DateTime DateLogged, TimeSpan Duration)> Worklogs { get; private set; }
        public decimal HourlySalary { get; private set; }

        public void End(DateTime endDate)
        {
            EndDate = endDate;
        }
    }
}
