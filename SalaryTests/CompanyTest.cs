using EmployeeSalary.Exceptions;
using EmployeeSalary.Models;
using EmployeeSalary.Services;
using Xunit;

namespace SalaryTests
{

    public class CompanyTest
    {
        #region Add Employee

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Should_Add_Employees(int idToDelete)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now);

            createdEmployee = new Employee
            {
                Id = 2,
                FullName = "Test worker 2",
                HourlySalary = 25m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now);

            Assert.Equal(2, companyService.Employees.Length);

            companyService.RemoveEmployee(idToDelete, DateTime.Now);

            Assert.Single(companyService.Employees);
            Assert.Equal(idToDelete == 1 ? 2 : 1, companyService.Employees.Single().Id);
        }

        [Fact]
        public void Same_Employees_With_Same_Id_And_Same_Salary_Added()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now);

            createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now.AddMonths(1));

            Assert.Single(companyService.Employees);
        }

        [Fact]
        public void Existing_Employee_Have_Invalid_Contract_Start_Date()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now.AddMonths(1));

            createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };

            Assert.Throws<CompanyException>(() => companyService.AddEmployee(createdEmployee, DateTime.Now));
        }

        [Theory]
        [InlineData(20.5)]
        [InlineData(30.5)]
        public void Existing_Employee_Have_Updated_Contract(decimal updatedContarctHourlySalary)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now);

            createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = updatedContarctHourlySalary
            };

            companyService.AddEmployee(createdEmployee, DateTime.Now.AddDays(1));

            Assert.Equal(updatedContarctHourlySalary, companyService.Employees.FirstOrDefault(e => e.Id.Equals(1)).HourlySalary);
        }

        #endregion

        #region Remove Employee
        [Fact]
        public void Trying_To_Remove_Employee_That_Does_Not_Exist()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            Assert.Throws<CompanyException>(() => companyService.RemoveEmployee(1, DateTime.Now));
        }

        [Fact]
        public void Trying_To_Remove_Employee_That_Already_Is_Removed()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 15.5m
            };

            companyService.AddEmployee(createdEmployee, DateTime.Now);
            companyService.RemoveEmployee(1, DateTime.Now.AddMonths(1));

            Assert.Throws<CompanyException>(() => companyService.RemoveEmployee(1, DateTime.Now.AddMonths(1)));
        }

        [Fact]
        public void Trying_To_Remove_Employee_With_Start_Date_Greater_Than_Provided_End_Date()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 15.5m
            };

            companyService.AddEmployee(createdEmployee, DateTime.Now);

            Assert.Throws<CompanyException>(() => companyService.RemoveEmployee(1, DateTime.Now.AddMonths(-1)));
        }

        [Fact]
        public void Remove_Employee()
        {
            DateTime contractEndDate = DateTime.Now.AddMonths(1);
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 15.5m
            };

            companyService.AddEmployee(createdEmployee, DateTime.Now);
            companyService.RemoveEmployee(1, contractEndDate);

            Assert.Empty(companyService.Employees);
        }
        #endregion

        #region Report Hours and Monthly Report
        [Fact]
        public void Report_Hours_For_Employee_That_Does_Not_Exist()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            Assert.Throws<CompanyException>(() => companyService.ReportHours(1, DateTime.Now, 1, 30));
        }

        [Fact]
        public void Report_Hours_With_Invalid_Hour()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now);

            Assert.Throws<CompanyException>(() => companyService.ReportHours(1, DateTime.Now, -1, 0));
        }

        [Fact]
        public void Report_Hours_With_Invalid_Minutes()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now);

            Assert.Throws<CompanyException>(() => companyService.ReportHours(1, DateTime.Now, 1, 60));
            Assert.Throws<CompanyException>(() => companyService.ReportHours(1, DateTime.Now, 1, -20));
        }

        [Fact]
        public void Report_Hours_With_Overtime()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20.1m
            };
            companyService.AddEmployee(createdEmployee, DateTime.Now);

            Assert.Throws<CompanyException>(() => companyService.ReportHours(1, DateTime.Now, 8, 30));
            Assert.Throws<CompanyException>(() => companyService.ReportHours(1, DateTime.Now, 10, 30));
        }

        [Theory]
        [InlineData("2022/7/31 23:30:0")]
        [InlineData("2022/8/31 18:00:0")]
        public void Report_Hours_With_More_Hours_In_The_Work_Log_Than_There_Is_Left_On_Log_Date(string date)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20m
            };
            var dateTime = DateTime.Parse(date);
            companyService.AddEmployee(createdEmployee, DateTime.Now);
            companyService.ReportHours(1, dateTime, 8, 00);
            var report = companyService.GetMonthlyReport(dateTime.AddYears(-1), dateTime.AddYears(1));
            Assert.Equal(2, report.Length);
        }

        [Theory]
        [InlineData("2022/7/31 23:30:0")]
        public void Report_Hours_Should_Return_Exception(string date)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20m
            };
            var dateTime = DateTime.Parse(date);
            companyService.AddEmployee(createdEmployee, dateTime.AddYears(1));
            Assert.Throws<CompanyException>(() => companyService.ReportHours(1, dateTime, 8, 00));
        }

        [Theory]
        [InlineData("2022/7/31 23:30:0")]
        public void Should_Return_Exception_Get_Monthly_Report(string date)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);
            var dateTime = DateTime.Parse(date);

            Assert.Throws<CompanyException>(() => companyService.GetMonthlyReport(dateTime.AddMonths(3), dateTime.AddMonths(-3)));
        }

        [Fact]
        public void Should_Calculate_Correct_Salary_With_Updated_Contract_For_Same_Employee()
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);
            var dateTime = DateTime.Parse("2022/7/1 12:00:0");

            decimal expectedSalary = 0;
            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20m
            };
            companyService.AddEmployee(createdEmployee, dateTime);

            for (int i = 0; i < 5; i++)
            {
                companyService.ReportHours(1, dateTime.AddDays(i), 8, 00);
                expectedSalary += 8 * 20m;
            }

            //Updating employee contract, increasing salary to 40
            createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 40m
            };
            companyService.AddEmployee(createdEmployee, dateTime);

            for (int i = 0; i < 5; i++)
            {
                companyService.ReportHours(1, dateTime.AddMonths(1).AddDays(i), 8, 00);
                expectedSalary += 8 * 40m;
            }

            var report = companyService.GetMonthlyReport(dateTime, dateTime.AddMonths(1));
            Assert.Equal(expectedSalary, report.Select(s => s.Salary).Sum());
        }

        [Theory]
        [InlineData(20, 30)]
        [InlineData(40.5, 20.5)]
        [InlineData(20.5, 15.5)]
        public void Report_Should_Contain_Salary_For_Each_Added_Emloyee(decimal firstEmployeeSalary, decimal secondEmployeeSalary)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);
            var dateTime = DateTime.Parse("2022/7/1 12:00:0");

            decimal expectedSalaryForFirstEmployee = 0;
            decimal expectedSalaryForSecondEmployee = 0;
            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = firstEmployeeSalary
            };
            companyService.AddEmployee(createdEmployee, dateTime);

            for (int i = 0; i < 5; i++)
            {
                companyService.ReportHours(createdEmployee.Id, dateTime.AddDays(i), 8, 00);
                expectedSalaryForFirstEmployee += 8 * firstEmployeeSalary;
            }

            createdEmployee = new Employee
            {
                Id = 2,
                FullName = "Test worker 2",
                HourlySalary = secondEmployeeSalary
            };
            companyService.AddEmployee(createdEmployee, dateTime);

            for (int i = 0; i < 5; i++)
            {
                companyService.ReportHours(createdEmployee.Id, dateTime.AddMonths(1).AddDays(i), 8, 00);
                expectedSalaryForSecondEmployee += 8 * secondEmployeeSalary;
            }

            var report = companyService.GetMonthlyReport(dateTime, dateTime.AddMonths(1));
            Assert.Equal(expectedSalaryForFirstEmployee, report.Where(e => e.EmployeeId.Equals(1)).Select(s => s.Salary).Sum());
            Assert.Equal(expectedSalaryForSecondEmployee, report.Where(e => e.EmployeeId.Equals(2)).Select(s => s.Salary).Sum());
        }

        [Theory]
        [InlineData("2022/1/1 12:00:0", "2023/1/1 12:00:0")]
        [InlineData("2021/1/1 12:00:0", "2024/1/1 12:00:0")]
        public void Report_Should_Contain_Correct_Months_Count_Based_On_Date_Range(string startDate, string endDate)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            var createdEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = 20m
            };
            companyService.AddEmployee(createdEmployee, start);
            var datesDifference = end.Date - start.Date;

            for (int i = 0; i < datesDifference.Days; i++)
            {
                if ((start.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (start.AddDays(i).DayOfWeek == DayOfWeek.Sunday)) continue;

                companyService.ReportHours(createdEmployee.Id, start.AddDays(i), 8, 00);
            }

            var reports = companyService.GetMonthlyReport(start, end);
            Assert.Equal(GetTotalMonths(start, end), reports.Select(r => r.Month).Count());
        }

        [Theory]
        [InlineData("2022/1/1 12:00:0", "2024/1/1 12:00:0")]
        [InlineData("2021/1/1 12:00:0", "2024/1/1 12:00:0")]
        public void FullFunctionalityTest(string startDate, string endDate)
        {
            var companyName = "Test Company";
            var companyService = new CompanyService(companyName);

            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            // Adding first employee to the company
            decimal firstEmployeeSalary = 20m;
            decimal firstEmployeeExpectedSalary = 0m;
            var firstEmloyeeStartDate = DateTime.Parse("2022/07/1 10:00:0");
            var firstEmployeeContractEnd = DateTime.Parse("2022/10/1 12:00:0");
            var firstExpectedWorkDurationInMonths = GetTotalMonths(firstEmloyeeStartDate, firstEmployeeContractEnd);
            
            var firstEmployee = new Employee
            {
                Id = 1,
                FullName = "Test worker 1",
                HourlySalary = firstEmployeeSalary
            };
            companyService.AddEmployee(firstEmployee, firstEmloyeeStartDate);

            // First employee starting working with his initial salary
            var firstEmployeeContractDuration = firstEmployeeContractEnd.Date - firstEmloyeeStartDate.Date;
            for (int i = 0; i < firstEmployeeContractDuration.Days; i++)
            {
                if ((firstEmloyeeStartDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (firstEmloyeeStartDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday)) continue;

                companyService.ReportHours(firstEmployee.Id, firstEmloyeeStartDate.AddDays(i), 8, 00);
                firstEmployeeExpectedSalary += 8 * firstEmployeeSalary;
            }

            // We decided to raise salary for first employee and update his contract start and end date
            firstEmployeeSalary = 40m;
            firstEmployee.HourlySalary = firstEmployeeSalary;
            firstEmloyeeStartDate = DateTime.Parse("2022/10/15 12:00:0");
            firstEmployeeContractEnd = DateTime.Parse("2023/6/1 12:00:0");
            firstEmployeeContractDuration = firstEmployeeContractEnd.Date - firstEmloyeeStartDate.Date;
            firstExpectedWorkDurationInMonths += GetTotalMonths(firstEmloyeeStartDate, firstEmployeeContractEnd);

            companyService.AddEmployee(firstEmployee, firstEmloyeeStartDate);

            // First employee starting working with his new salary raise
            for (int i = 0; i < firstEmployeeContractDuration.Days; i++)
            {
                if ((firstEmloyeeStartDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (firstEmloyeeStartDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday)) continue;

                companyService.ReportHours(firstEmployee.Id, firstEmloyeeStartDate.AddDays(i), 8, 00);
                firstEmployeeExpectedSalary += 8 * firstEmployeeSalary;
            }

            // Adding second employee 
            decimal secondEmployeeSalary = 60m;
            decimal secondEmployeeExpectedSalary = 0m;
            var secondEmloyeeStartDate = DateTime.Parse("2022/05/1 10:00:0");
            var secondEmployeeContractEnd = DateTime.Parse("2023/05/1 12:00:0");
            
            var secondEmployee = new Employee
            {
                Id = 2,
                FullName = "Test worker 2",
                HourlySalary = secondEmployeeSalary
            };
            companyService.AddEmployee(secondEmployee, secondEmloyeeStartDate);

            // Second employee starting working 
            var secondEmployeeContractDuration = secondEmployeeContractEnd.Date - secondEmloyeeStartDate.Date;
            for (int i = 0; i < secondEmployeeContractDuration.Days; i++)
            {
                if ((secondEmloyeeStartDate.AddDays(i).DayOfWeek == DayOfWeek.Saturday) || (secondEmloyeeStartDate.AddDays(i).DayOfWeek == DayOfWeek.Sunday)) continue;

                companyService.ReportHours(secondEmployee.Id, secondEmloyeeStartDate.AddDays(i), 8, 00);
                secondEmployeeExpectedSalary += 8 * secondEmployeeSalary;
            }

            // We decided to fire second employee, so we need end his contract
            companyService.RemoveEmployee(secondEmployee.Id, secondEmployeeContractEnd);
            
            var reports = companyService.GetMonthlyReport(start, end);
            Assert.Equal(firstEmployeeExpectedSalary, reports.Where(e => e.EmployeeId.Equals(firstEmployee.Id)).Select(s => s.Salary).Sum());
            Assert.Equal(firstExpectedWorkDurationInMonths, reports.Where(r => r.EmployeeId.Equals(firstEmployee.Id)).Select(s => s.Month).Count());
            Assert.Equal(secondEmployeeExpectedSalary, reports.Where(e => e.EmployeeId.Equals(secondEmployee.Id)).Select(s => s.Salary).Sum());
        }


        #endregion

        private static int GetTotalMonths(DateOnly firstDate, DateOnly lastDate)
        {
            int yearsApart = lastDate.Year - firstDate.Year;
            int monthsApart = lastDate.Month - firstDate.Month;
            return (yearsApart * 12) + monthsApart;
        }

        private static int GetTotalMonths(DateTime firstDate, DateTime lastDate)
        {
            return GetTotalMonths(DateOnly.FromDateTime(firstDate), DateOnly.FromDateTime(lastDate));
        }

    }
}