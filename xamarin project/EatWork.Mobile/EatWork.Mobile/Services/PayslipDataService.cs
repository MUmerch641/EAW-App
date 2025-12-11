using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Payslip;
using EatWork.Mobile.Models.Payslip;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Requests;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class PayslipDataService : IPayslipDataService
    {
        public long TotalListItem { get; set; }

        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly StringHelper string_;

        public PayslipDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            string_ = url;
        }

        public async Task<ObservableCollection<MyPayslipListModel>> GetMyPayslipListAsync(ObservableCollection<MyPayslipListModel> list, ListParam args)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var userInfo = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = ApiConstants.MyPayslip
                };

                var param = new MyApprovalRequest
                {
                    ProfileId = userInfo.ProfileId,
                    Page = (args.ListCount == 0 ? 1 : ((args.ListCount + args.Count) / args.Count)),
                    Rows = args.Count,
                    SortOrder = (args.IsAscending ? 0 : 1),
                    Keyword = args.KeyWord,
                    TransactionTypes = args.FilterTypes,
                    Status = args.Status,
                    StartDate = args.StartDate,
                    EndDate = args.EndDate,
                };

                var request = string_.CreateUrl<MyApprovalRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.MyPayslipList>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.TotalListCount != 0)
                {
                    try
                    {
                        foreach (var item in response.ListData)
                        {
                            var data = new Models.Payslip.MyPayslipListModel()
                            {
                                BasicPay = item.BasicPay,
                                IssuedDate = item.IssuedDate.GetValueOrDefault(),
                                NetPay = item.BasicPay,
                                PayrollType = item.PayrollType,
                                PaysheetHeaderId = item.PaySheetHeaderId,
                                ProfileId = item.ProfileId.GetValueOrDefault(),
                                PaysheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                AmountDisplay = string.Format("{1}{0:#,##0.00}{2}", item.NetPay
                                , (item.NetPay < 0 ? "(" : "")
                                , (item.NetPay < 0 ? ")" : "")).Replace("-", ""),
                                IssuedDateDisplay = item.IssuedDate.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat),
                            };

                            list.Add(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }

                TotalListItem = response.TotalListCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return list;
        }

        public async Task<PayslipDetailHolder> GetPayslipDetailAsync(long profileId, long id)
        {
            var holder = new PayslipDetailHolder();
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = string.Format(ApiConstants.MyPayslipDetail, profileId, id)
                };

                var response = await genericRepository_.GetAsync<R.Responses.PayslipDetailResponse>(builder.ToString());

                if (response.PaysheetHeaderId > 0)
                {
                    holder.Model = new PayslipDetailModel();
                    PropertyCopier<R.Responses.PayslipDetailResponse, PayslipDetailModel>.Copy(response, holder.Model);

                    var start = response.PeriodStartDate.GetValueOrDefault();
                    var end = response.PeriodEndDate.GetValueOrDefault();

                    holder.Model.IssuedDateDisplay = holder.Model.IssuedDate.GetValueOrDefault().ToString("MMM. dd, yyyy");

                    if (start.Month == end.Month && start.Year == end.Year)
                    {
                        holder.Model.PeriodDate = string.Format("{0}. {1}-{3}, {2}", start.ToString("MMM")
                            , start.ToString("dd")
                            , start.ToString("yyyy")
                            , end.ToString("dd"));
                    }
                    else
                    {
                        holder.Model.PeriodDate = string.Format("{0} - {1}", start.ToString("MMM. dd, yyyy"), end.ToString("MMM. dd, yyyy"));
                    }

                    holder.Model.GrossPayDisplay = string.Format("{0:#,##0.00}", response.GrossPay);
                    holder.Model.TotalDeductionDisplay = string.Format("{0:#,##0.00}", response.TotalDeduction);
                    holder.Model.NetPayDisplay = string.Format("{0:#,##0.00}", response.NetPay);

                    if (response.Earnings.Count > 0)
                    {
                        foreach (var item in response.Earnings)
                        {
                            holder.Earnings.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = string.Format("{0:#,##0.00}hrs", item.Hours),
                                AmountDisplay = string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", ""),
                            });
                        }
                    }

                    if (response.BenefitsAllowances.Count > 0)
                    {
                        foreach (var item in response.BenefitsAllowances)
                        {
                            holder.Allowances.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = string.Format("{0:#,##0.00}hrs", item.Hours),
                                AmountDisplay = string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", ""),
                            });
                        }
                    }

                    if (response.Deductions.Count > 0)
                    {
                        foreach (var item in response.Deductions)
                        {
                            holder.Deductions.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = string.Format("{0:#,##0.00}hrs", item.Hours),
                                AmountDisplay = string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", ""),
                            });
                        }
                    }

                    if (response.LoanPayments.Count > 0)
                    {
                        foreach (var item in response.LoanPayments)
                        {
                            holder.Loans.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = string.Format("{0:#,##0.00}hrs", item.Hours),
                                AmountDisplay = string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", ""),
                            });
                        }
                    }

                    if (response.YTDS.Count > 0)
                    {
                        foreach (var item in response.YTDS)
                        {
                            holder.YTDS.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = string.Format("{0:#,##0.00}hrs", item.Hours),
                                AmountDisplay = string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", ""),
                            });
                        }
                    }

                    if (response.RunningBalances.Count > 0)
                    {
                        foreach (var item in response.RunningBalances)
                        {
                            holder.RunningBalances.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = string.Format("{0:#,##0.00}hrs", item.Hours),
                                AmountDisplay = string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", ""),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return holder;
        }

        public async Task PrintPayslip(long profileId, long id)
        {
            try
            {
                if (id != 0)
                {
                    var url = await commonDataService_.RetrieveClientUrl();
                    await commonDataService_.HasInternetConnection(url);

                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        var user = PreferenceHelper.UserInfo();

                        var builder = new UriBuilder(url)
                        {
                            Path = string.Format(ApiConstants.PrintSetup, id)
                        };

                        var response = await genericRepository_.GetAsync<R.Responses.PrintPayslipConfigResponse>(builder.ToString());

                        if (string.IsNullOrWhiteSpace(response.Paysliptemplate))
                        {
                            throw new Exception("No payslip template found.");
                        }
                        else if (string.IsNullOrWhiteSpace(response.ReportURL))
                        {
                            throw new Exception("Unable to print file, no Report URL setup found.");
                        }
                        else
                        {
                            builder = new UriBuilder(url)
                            {
                                Path = string.Format(response.ReportURL + ApiConstants.PrintPayslip, id
                                , profileId
                                , response.Paysliptemplate
                                , user.UserSecurityId)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<YTDPayslipDetailHolder> GetPayslipYTDTemplateAsync(long profileId, long id)
        {
            var holder = new YTDPayslipDetailHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = string.Format(ApiConstants.PaslipYTDTemplate, profileId, id)
                };

                var response = await genericRepository_.GetAsync<R.Responses.PayslipYTDOTPaymentDetailResponse>(builder.ToString());

                if (response.PaysheetHeaderId > 0)
                {
                    holder.Model = new PayslipDetailModel();
                    PropertyCopier<R.Responses.PayslipYTDOTPaymentDetailResponse, PayslipDetailModel>.Copy(response, holder.Model);

                    var start = response.PeriodStartDate.GetValueOrDefault();
                    var end = response.PeriodEndDate.GetValueOrDefault();

                    holder.Model.IssuedDateDisplay = holder.Model.IssuedDate.GetValueOrDefault().ToString("MMM. dd, yyyy");

                    if (start.Month == end.Month && start.Year == end.Year)
                    {
                        holder.Model.PeriodDate = string.Format("{0}. {1}-{3}, {2}", start.ToString("MMM")
                            , start.ToString("dd")
                            , start.ToString("yyyy")
                            , end.ToString("dd"));
                    }
                    else
                    {
                        holder.Model.PeriodDate = string.Format("{0} - {1}", start.ToString("MMM. dd, yyyy"), end.ToString("MMM. dd, yyyy"));
                    }

                    holder.Model.GrossPayDisplay = (response.GrossPay > 0 ? string.Format("{0:#,##0.00}", response.GrossPay) : "-");
                    /*holder.Model.TotalDeductionDisplay = (response.TotalDeduction > 0 ? string.Format("{0:#,##0.00}", response.TotalDeduction) : "-");*/
                    holder.Model.NetPayDisplay = (response.NetPay > 0 ? string.Format("{0:#,##0.00}", response.NetPay) : "-");
                    holder.Model.SSSDisplay = (response.SSS > 0 ? string.Format("{0:#,##0.00}", response.SSS) : "-");
                    holder.Model.BasicPayDisplay = (response.BasicPay > 0 ? string.Format("{0:#,##0.00}", response.BasicPay) : "-");
                    holder.Model.BasicRateDisplay = (response.BasicRate > 0 ? string.Format("{0:#,##0.00}", response.BasicRate) : "-");
                    holder.Model.PhilHealthDisplay = (response.PhilHealth > 0 ? string.Format("{0:#,##0.00}", response.PhilHealth) : "-");
                    holder.Model.PAGIBIGDisplay = (response.PAGIBIG > 0 ? string.Format("{0:#,##0.00}", response.PAGIBIG) : "-");
                    holder.Model.WHTDisplay = (response.WHT > 0 ? string.Format("{0:#,##0.00}", response.WHT) : "-");
                    holder.Model.LoanDisplay = (response.Loan > 0 ? string.Format("{0:#,##0.00}", response.Loan) : "-");
                    holder.Model.OtherDeductionDisplay = (response.OtherDeduction > 0 ? string.Format("{0:#,##0.00}", response.OtherDeduction) : "-");

                    #region gross earnings

                    if (response.GrossEarnings.Count > 0)
                    {
                        foreach (var item in response.GrossEarnings.Where(X => X.Description != "Basic Rate"))
                        {
                            holder.GrossEarnings.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                Remarks = item.Remarks
                            });
                        }
                    }

                    #endregion gross earnings

                    #region earnings

                    if (response.Earnings.Count > 0)
                    {
                        foreach (var item in response.Earnings)
                        {
                            holder.Earnings.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                Remarks = item.Remarks
                            });

                            if (item.IsTaxable)
                            {
                                holder.TaxableEarnings.Add(new PaysheetDetailDto()
                                {
                                    Amount = item.Amount,
                                    Description = item.Description,
                                    Hours = item.Hours,
                                    PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                    HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                    AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                    , (item.Amount < 0 ? "(" : "")
                                    , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                    Remarks = item.Remarks
                                });
                            }
                            else
                            {
                                holder.NonTaxableEarnings.Add(new PaysheetDetailDto()
                                {
                                    Amount = item.Amount,
                                    Description = item.Description,
                                    Hours = item.Hours,
                                    PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                    HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                    AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                    , (item.Amount < 0 ? "(" : "")
                                    , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                    Remarks = item.Remarks
                                });
                            }
                        }
                    }

                    #endregion earnings

                    #region overtime

                    if (response.OvertimeDetails.Count > 0)
                    {
                        decimal totalOtHours = 0;
                        decimal totalOTPay = 0;
                        foreach (var item in response.OvertimeDetails)
                        {
                            holder.OvertimeDetails.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                                       , (item.Amount < 0 ? "(" : "")
                                                       , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                Remarks = item.Remarks
                            });

                            totalOtHours += item.Hours;
                            totalOTPay += item.Amount;
                        }

                        holder.TotalOTHours = (totalOtHours > 0 ? string.Format("{0:#,##0.00}", totalOtHours) : "-");
                        holder.TotalOTPay = (totalOTPay > 0 ? string.Format("{0:#,##0.00}", totalOTPay) : "-");
                    }

                    #endregion overtime

                    #region leave

                    if (response.LeaveDetails.Count > 0)
                    {
                        decimal totalEarned = 0;
                        decimal totalUsed = 0;
                        decimal totalBalance = 0;

                        foreach (var item in response.LeaveDetails)
                        {
                            holder.LeaveDetails.Add(new LeaveBalanceDetailDto()
                            {
                                LeaveTypeCode = item.LeaveTypeCode,
                                CurrentBalance = item.CurrentBalance,
                                EarnedHours = item.EarnedHours,
                                UsedHours = item.UsedHours,
                                CurrentBalanceDisplay = (item.CurrentBalance > 0 ? string.Format("{0:#,##0.00}", item.CurrentBalance) : "-"),
                                EarnedHoursDisplay = (item.EarnedHours > 0 ? string.Format("{0:#,##0.00}", item.EarnedHours) : "-"),
                                UsedHoursDisplay = (item.UsedHours > 0 ? string.Format("{0:#,##0.00}", item.UsedHours) : "-"),
                            });

                            totalEarned += item.EarnedHours;
                            totalUsed += item.UsedHours;
                            totalBalance += item.CurrentBalance;
                        }

                        holder.TotalEarnedHoursDisplay = (totalEarned > 0 ? string.Format("{0:#,##0.00}", totalEarned) : "-");
                        holder.TotalUsedHoursDisplay = (totalUsed > 0 ? string.Format("{0:#,##0.00}", totalUsed) : "-");
                        holder.TotalCurrentBalanceDisplay = (totalBalance > 0 ? string.Format("{0:#,##0.00}", totalBalance) : "-");
                    }

                    #endregion leave

                    #region YTDS

                    if (response.YTDDetails.Count > 0)
                    {
                        foreach (var item in response.YTDDetails)
                        {
                            holder.YTDDetails.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                Remarks = item.Remarks
                            });
                        }
                    }

                    #endregion YTDS

                    #region deductions

                    if (response.DeductionDetails.Count > 0)
                    {
                        decimal totalDeduction = 0;

                        foreach (var item in response.DeductionDetails)
                        {
                            holder.DeductionDetails.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                Remarks = item.Remarks
                            });

                            totalDeduction += item.Amount;
                        }

                        holder.TotalDeductionDisplay = (totalDeduction > 0 ? string.Format("{0:#,##0.00}", totalDeduction) : "-");
                    }

                    #endregion deductions

                    #region other deductions

                    if (response.OtherDeductionDetails.Count > 0)
                    {
                        decimal otherDedAmount = 0;

                        foreach (var item in response.OtherDeductionDetails)
                        {
                            holder.OtherDedDetails.Add(new PaysheetDetailDto()
                            {
                                Amount = item.Amount,
                                Description = item.Description,
                                Hours = item.Hours,
                                PaySheetHeaderDetailId = item.PaySheetHeaderDetailId,
                                HoursDisplay = (item.Hours > 0 ? string.Format("{0:#,##0.00}", item.Hours) : "-"),
                                AmountDisplay = (item.Amount > 0 ? string.Format("{1}{0:#,##0.00}{2}", item.Amount
                                , (item.Amount < 0 ? "(" : "")
                                , (item.Amount < 0 ? ")" : "")).Replace("-", "") : "-"),
                                Remarks = item.Remarks
                            });

                            otherDedAmount += item.Amount;
                        }

                        holder.TotalOtherDeductionDisplay = (otherDedAmount > 0 ? string.Format("{0:#,##0.00}", otherDedAmount) : "-");
                    }

                    if (response.DeductionBalances.Count > 0)
                    {
                        foreach (var item in response.DeductionBalances)
                        {
                            holder.DeductionBalances.Add(new DeductionBalanceDetailDto()
                            {
                                Description = item.Description,
                                EarningCode = item.EarningCode,
                                OriginalAmountDisplay = (item.OriginalAmount > 0 ? string.Format("{0:#,##0.00}", item.OriginalAmount) : "-"),
                                PreviousBalanceDisplay = (item.PreviousBalance > 0 ? string.Format("{0:#,##0.00}", item.PreviousBalance) : "-"),
                                PaymentDisplay = (item.Payment > 0 ? string.Format("{0:#,##0.00}", item.Payment) : "-"),
                                RemainingBalanceDisplay = (item.RemainingBalance > 0 ? string.Format("{0:#,##0.00}", item.RemainingBalance) : "-"),
                                Remarks = item.Remarks,
                            });
                        }
                    }

                    #endregion other deductions

                    #region loans

                    if (response.LoanBalances.Count > 0)
                    {
                        foreach (var item in response.LoanBalances)
                        {
                            holder.LoanBalances.Add(new DeductionBalanceDetailDto()
                            {
                                Description = item.Description,
                                EarningCode = item.EarningCode,
                                OriginalAmountDisplay = (item.OriginalAmount > 0 ? string.Format("{0:#,##0.00}", item.OriginalAmount) : "-"),
                                PreviousBalanceDisplay = (item.PreviousBalance > 0 ? string.Format("{0:#,##0.00}", item.PreviousBalance) : "-"),
                                PaymentDisplay = (item.Payment > 0 ? string.Format("{0:#,##0.00}", item.Payment) : "-"),
                                RemainingBalanceDisplay = (item.RemainingBalance > 0 ? string.Format("{0:#,##0.00}", item.RemainingBalance) : "-"),
                                Remarks = item.Remarks,
                            });
                        }
                    }

                    #endregion loans
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }

            return holder;
        }
    }
}