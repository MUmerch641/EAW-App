using MauiHybridApp.Models;
using MauiHybridApp.Models.DataObjects;
using MauiHybridApp.Services.Data;
using MauiHybridApp.Utils;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class PayrollDataService : IPayrollDataService
    {
        private readonly IGenericRepository _repository;

        public PayrollDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        // 1. GET LIST
        public async Task<List<object>> GetPayslipsAsync()
        {
            try
            {
                var profileIdStr = await SecureStorage.GetAsync("profile_id");
                long.TryParse(profileIdStr, out long pid);
                
                var request = new MyApprovalRequest
                {
                    ProfileId = pid,
                    Page = 1,
                    Rows = 50,
                    SortOrder = 1
                };

                var queryString = $"?ProfileId={request.ProfileId}&Page={request.Page}&Rows={request.Rows}&SortOrder={request.SortOrder}";
                var url = $"{ApiEndpoints.GetPayslipList}{queryString}";

                var response = await _repository.GetAsync<PayslipListResponse>(url);

                if (response != null && response.ListData != null)
                {
                    return response.ListData.Cast<object>().ToList();
                }
                
                return new List<object>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payslip List Error: {ex.Message}");
                return new List<object>();
            }
        }

        // 2. GET DETAIL
        public async Task<object> GetPayslipDetailAsync(long id)
        {
            // Simulate API Delay
            await Task.Delay(500);

            var detail = new PayslipDetailModel
            {
                PayrollType = "Semi-Monthly Payroll",
                ReferenceNumber = $"PAY-{DateTime.Now.Year}-{id:D4}",
                IssuedDate = DateTime.Now,
                PeriodStartDate = DateTime.Now.AddDays(-15),
                PeriodEndDate = DateTime.Now,
                GrossPay = 25000.00m,
                TotalDeduction = 2500.00m,
                NetPay = 22500.00m,
                BasicPay = 20000.00m,
                
                Earnings = new List<PaysheetDetailDto>
                {
                    new PaysheetDetailDto { Description = "Basic Pay", Amount = 20000.00m, Hours = 88, Remarks = "Regular Hours" },
                    new PaysheetDetailDto { Description = "Overtime", Amount = 1500.00m, Hours = 5, Remarks = "Approved OT" }
                },
                
                Allowances = new List<PaysheetDetailDto>
                {
                    new PaysheetDetailDto { Description = "Rice Allowance", Amount = 2000.00m },
                    new PaysheetDetailDto { Description = "Laundry Allowance", Amount = 500.00m },
                    new PaysheetDetailDto { Description = "Transport Allowance", Amount = 1000.00m }
                },

                Deductions = new List<PaysheetDetailDto>
                {
                    new PaysheetDetailDto { Description = "SSS Premium", Amount = 581.30m },
                    new PaysheetDetailDto { Description = "PhilHealth", Amount = 400.00m },
                    new PaysheetDetailDto { Description = "Pag-IBIG", Amount = 100.00m },
                    new PaysheetDetailDto { Description = "Withholding Tax", Amount = 1418.70m }
                },

                LoanPayments = new List<PaysheetDetailDto>
                {
                     new PaysheetDetailDto { Description = "SSS Salary Loan", Amount = 500.00m, Remarks = "2 of 24" }
                },

                OvertimeDetails = new List<PaysheetDetailDto>
                {
                    new PaysheetDetailDto { Description = "Regular Overtime", Hours = 3, Amount = 900.00m, Remarks = "Oct 15 - Project Deadline" },
                    new PaysheetDetailDto { Description = "Rest Day Overtime", Hours = 2, Amount = 600.00m, Remarks = "Oct 20 - System Maintenance" }
                },

                YTDs = new List<PaysheetDetailDto>
                {
                    new PaysheetDetailDto { Description = "Gross Income", Amount = 150000.00m },
                    new PaysheetDetailDto { Description = "Taxable Income", Amount = 140000.00m },
                    new PaysheetDetailDto { Description = "Withholding Tax", Amount = 12000.00m },
                    new PaysheetDetailDto { Description = "Net Pay", Amount = 135000.00m }
                }
            };

            return detail;
        }
    }
}
