using EmployeeSalary.Exceptions;
using EmployeeSalary.Models;

namespace EmployeeSalary.Services
{
    public class CompanyService : ICompany
    {
        private readonly List<EmployeeInformation> _employees;

        public CompanyService(string companyName)
        {
            _employees = new List<EmployeeInformation>();
            Name = companyName;
        }

        public string Name { get; private set; }

        public Employee[] Employees
        {
            get => _employees
                .Where(e => !e.Contracts.Last().EndDate.HasValue)
                .Select(e => new Employee { Id = e.Id, FullName = e.FullName, HourlySalary = e.Contracts.Last().HourlySalary })
                .ToArray();
        }


        public void AddEmployee(Employee employee, DateTime contractStartDate)
        {
            var existingEmployee = GetEmployeeById(employee.Id);
            if (existingEmployee != null)
            {
                if (existingEmployee.Contracts.Last().StartDate > contractStartDate)
                {
                    throw new CompanyException("Employee have invalid input start date");
                }

                if (!existingEmployee.Contracts.Last().EndDate.HasValue)
                {
                    // If employee is already working here and the salary is the same then adding a new EmployeeContract record will not make difference
                    if (existingEmployee.Contracts.Last().HourlySalary == employee.HourlySalary) return;

                    existingEmployee.Contracts.Last().End(contractStartDate);
                }

                existingEmployee.Contracts.Add(new EmployeeContract(employee.HourlySalary, contractStartDate));
            }
            else
            {
                _employees.Add(new EmployeeInformation(employee.Id, employee.FullName, new EmployeeContract(employee.HourlySalary, contractStartDate)));
            }
        }

        public void RemoveEmployee(int employeeId, DateTime contractEndDate)
        {
            var existingEmployee = GetEmployeeById(employeeId);

            if (existingEmployee == null)
            {
                throw new CompanyException("Employee does not exist");
            }

            if (existingEmployee.Contracts.Last().EndDate.HasValue)
            {
                throw new CompanyException("Employee is already removed");
            }

            if (existingEmployee.Contracts.Last().StartDate >= contractEndDate)
            {
                throw new CompanyException("End date is before employee start date");
            }

            existingEmployee.Contracts.Last().End(contractEndDate);
        }

        public void ReportHours(int employeeId, DateTime dateAndTime, int hours, int minutes)
        {
            var existingEmployee = GetEmployeeById(employeeId);

            if (existingEmployee == null)
            {
                throw new CompanyException("Employee does not exist");
            }

            if (hours < 0)
            {
                throw new CompanyException($"Hours has invalid value {hours}");
            }

            if (minutes < 0 || minutes > 59)
            {
                throw new CompanyException($"Minutes has invalid value {minutes}");
            }

            if ((hours == 8 && minutes != 0) || (hours > 8))
            {
                throw new CompanyException("Overtime is not allowed");
            }

            // If there are more hours in the work log than there are left on the log date, create two work logs, one for the log date and one after it.
            var worklogs = new List<(DateTime DateLogged, TimeSpan Duration)>();

            if (dateAndTime.Hour * 60 + dateAndTime.Minute + hours * 60 + minutes > 1440)
            {
                var firstDay = new TimeSpan(24, 0, 0) - new TimeSpan(dateAndTime.Hour, dateAndTime.Minute, 0);
                worklogs.Add((dateAndTime.Date, firstDay));
                worklogs.Add((dateAndTime.Date.AddDays(1), new TimeSpan(hours, minutes, 0) - firstDay));
            }
            else
            {
                worklogs.Add((dateAndTime.Date, new TimeSpan(hours, minutes, 0)));
            }

            foreach (var worklog in worklogs)
            {
                var contract = existingEmployee.Contracts.LastOrDefault(c => c.StartDate <= dateAndTime && (!c.EndDate.HasValue || c.EndDate >= dateAndTime));

                if (contract == null)
                {
                    throw new CompanyException($"Employee did not have contract at {dateAndTime:dd-MM-yyyy}");
                }

                contract.Worklogs.Add(worklog);
            }
        }

        public EmployeeMonthlyReport[] GetMonthlyReport(DateTime periodStartDate, DateTime periodEndDate)
        {
            if (periodStartDate >= periodEndDate)
            {
                throw new CompanyException("Start date is after end date");
            }

            var reports = new List<EmployeeMonthlyReport>();

            var currentYear = periodStartDate.Year;
            var currentMonth = periodStartDate.Month;
            do
            {
                var currentEndMonth = currentYear == periodEndDate.Year ? periodEndDate.Month : 12;
                do
                {
                    foreach (var employee in _employees)
                    {
                        var report = new EmployeeMonthlyReport
                        {
                            EmployeeId = employee.Id,
                            Year = currentYear,
                            Month = currentMonth,
                            Salary = 0,
                        };

                        foreach (var contract in employee.Contracts)
                        {
                            var worklogs = contract.Worklogs
                                .Where(w => w.DateLogged.Year == currentYear && w.DateLogged.Month == currentMonth);
                            
                            report.Salary += worklogs.Select(w => ((decimal)w.Duration.TotalHours) * contract.HourlySalary).Sum();
                        }
                        
                        if (report.Salary != 0) reports.Add(report);
                    }
                } while (currentMonth++ < currentEndMonth);
                currentMonth = 1;
            } while (currentYear++ < periodEndDate.Year);
            
            return reports.ToArray();
        }

        private EmployeeInformation GetEmployeeById(int id) => _employees.FirstOrDefault(e => e.Id == id);
    }
}
