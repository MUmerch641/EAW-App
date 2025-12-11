using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Expenses;
using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class ExpenseReportDataService : IExpenseReportDataService
    {
        public long TotalListItem { get; set; }
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly StringHelper string_;

        public ExpenseReportDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            StringHelper url,
            IDialogService dialogService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            string_ = url;
            dialogService_ = dialogService;
            workflowDataService_ = AppContainer.Resolve<IWorkflowDataService>();
        }

        public async Task<ExpenseReportDetailHolder> InitForm(ObservableCollection<MyExpensesListDto> details = null)
        {
            var retValue = new ExpenseReportDetailHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.ExpenseReport}/generate-number"
                };

                var number = await genericRepository_.GetAsync<string>(builder.ToString());

                if (!string.IsNullOrWhiteSpace(number))
                {
                    retValue.ReportNo = number;
                }

                if (details != null && details.Count > 0)
                {
                    foreach (var item in details)
                    {
                        retValue.Details.Add(new Models.ExpenseReportDetailModel()
                        {
                            Amount = item.Amount,
                            ExpenseDate = item.ExpenseDate,
                            ExpenseReportId = 0,
                            ExpenseSetupId = item.ExpenseSetupId,
                            ExpenseType = item.ExpenseType,
                            IconColor = item.IconColor,
                            IconEquivalent = item.Icon,
                            Notes = item.Notes,
                            ORNo = item.ORNo,
                            VendorId = item.VendorId,
                            VendorName = item.VendorName,
                            FileName = item.FileName,
                            FileType = item.FileType,
                            FileUpload = item.FileUpload,
                            Attachment = item.FileAttachment,
                            ExpenseDateDisplay = item.ExpenseDate.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat),
                            AmountDisplay = item.AmountDisplay,
                        });

                        retValue.AppExpenseDetailIds.Add(item.ExpenseReportDetailId);
                        retValue.TotalAmount += item.Amount;
                    }

                    retValue.TotalAmountDisplay = string.Format("{0:#,##0.00}", retValue.TotalAmount);
                    retValue.ForSubmission = true;
                    retValue.IsEnabled = true;
                    retValue.ShowCancelButton = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<ExpenseReportDetailHolder> RetrieveRecord(long id)
        {
            var retValue = new ExpenseReportDetailHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.ExpenseReport}/{id}"
                };

                var response = await genericRepository_.GetAsync<R.Responses.ExpenseReportResponse>(builder.ToString());

                if (response.ExpenseReport != null)
                {
                    foreach (var item in response.ExpenseReport.ExpenseReportDetail)
                    {
                        retValue.Details.Add(new Models.ExpenseReportDetailModel()
                        {
                            Amount = item.Amount,
                            ExpenseDate = item.ExpenseDate,
                            ExpenseReportId = 0,
                            ExpenseSetupId = item.ExpenseSetupId,
                            ExpenseType = item.ExpenseType,
                            IconColor = item.IconColor,
                            IconEquivalent = item.IconEquivalent,
                            Notes = item.Notes,
                            ORNo = item.ORNo,
                            VendorId = item.VendorId,
                            VendorName = item.VendorName,
                            FileName = item.FileName,
                            FileType = item.FileType,
                            FileUpload = item.FileUpload,
                            Attachment = item.Attachment,
                            ExpenseDateDisplay = item.ExpenseDate.GetValueOrDefault().ToString("ddd, MMM. dd, yyyy"),
                            AmountDisplay = string.Format("{0:#,##0.00}", item.Amount),
                            HasAttachment = (!string.IsNullOrWhiteSpace(item.Attachment))
                        });

                        retValue.TotalAmount += item.Amount.GetValueOrDefault(0);
                    }
                }

                retValue.TotalAmountDisplay = string.Format("{0:#,##0.00}", retValue.TotalAmount);
                retValue.ForSubmission = false;
                retValue.RecordId = response.ExpenseReport.ExpenseReportId;
                retValue.ReportNo = response.ExpenseReport.ReportNo;
                retValue.Date = response.ExpenseReport.CreateDate.GetValueOrDefault();
                /*retValue.ShowCancelButton = (response.ExpenseReport.StatusId == RequestStatusValue.ForApproval);*/
                /*retValue.ForSubmission = (response.ExpenseReport.StatusId == RequestStatusValue.ForApproval);*/
                retValue.IsEnabled = false;
                retValue.ShowCancelButton = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<ExpenseReportDetailHolder> SubmitRecord(ExpenseReportDetailHolder holder)
        {
            var user = PreferenceHelper.UserInfo();
            holder.Success = false;
            try
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        var builder = new UriBuilder(url)
                        {
                            Path = ApiConstants.ExpenseReport
                        };

                        var details = new List<R.Models.ExpenseReportDetail>(
                                holder.Details.Select(p => new R.Models.ExpenseReportDetail
                                {
                                    Amount = p.Amount,
                                    Attachment = p.Attachment,
                                    ExpenseDate = p.ExpenseDate,
                                    ExpenseSetupId = p.ExpenseSetupId,
                                    FileName = p.FileName,
                                    FileType = p.FileType,
                                    FileUpload = p.FileUpload,
                                    Notes = p.Notes,
                                    ORNo = p.ORNo,
                                    VendorId = p.VendorId,
                                    SourceId = (int)SourceEnum.Mobile,
                                }));

                        var header = new R.Models.ExpenseReport()
                        {
                            AgreeDate = holder.Date,
                            Amount = holder.TotalAmount,
                            AmountIssued = 0,
                            Balance = (holder.TotalAmount * -1),
                            AmountReimbursment = 0,
                            ProfileId = user.ProfileId,
                            ReportDate = holder.Date.Date,
                            ReportNo = holder.ReportNo,
                            SalaryDeduction = false,
                            StatusId = RequestStatusValue.Submitted,
                            SourceId = (int)SourceEnum.Mobile,
                            ExpenseReportDetail = details,
                        };

                        var param = new R.Requests.SubmitExpenseReportRequest()
                        {
                            Data = header,
                            AppExpenseReportDetailIds = holder.AppExpenseDetailIds,
                        };

                        var response = await genericRepository_.PostAsync<R.Requests.SubmitExpenseReportRequest, R.Responses.BaseResponse<R.Models.ExpenseReport>>(builder.ToString(), param);

                        if (response.Model.ExpenseReportId > 0)
                        {
                            holder.Success = true;
                            FormSession.IsSubmitted = true;
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

        public async Task<ObservableCollection<MyExpenseReportsList>> RetrieveMyExpenses(ObservableCollection<MyExpenseReportsList> list, ListParam args)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var user = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.ExpenseReport}/my-expense-report"
                };

                var param = new R.Requests.MyExpensesReportRequest
                {
                    ProfileId = user.ProfileId,
                    Page = (args.ListCount == 0 ? 1 : ((args.ListCount + args.Count) / args.Count)),
                    Rows = args.Count,
                    SortOrder = (args.IsAscending ? 0 : 1),
                    Keyword = args.KeyWord,
                    Status = args.Status,
                    StartDate = args.StartDate,
                    EndDate = args.EndDate,
                };

                var request = string_.CreateUrl<R.Requests.MyExpensesReportRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.MyExpenseReportsList>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.TotalListCount != 0)
                {
                    foreach (var item in response.ListData)
                    {
                        var data = new MyExpenseReportsList()
                        {
                            Amount = item.Amount,
                            ExpenseDate = item.ExpenseDate,
                            ExpenseReportId = item.ExpenseReportId,
                            ProfileId = item.ProfileId,
                            ReportNo = item.ReportNo,
                            Status = item.Status,
                            StatusId = item.StatusId,
                            TotalCount = item.TotalCount,
                            AmountDisplay = string.Format("{0:#,##0.00}", item.Amount),
                            ExpenseDateDisplay = item.ExpenseDate.GetValueOrDefault().ToString("ddd, MMM. dd, yyyy"),
                        };

                        list.Add(data);
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

        public async Task<ExpenseReportDetailHolder> WorkflowTransactionRequest(ExpenseReportDetailHolder holder)
        {
            holder.Success = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(holder.ActionTypeId, holder.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(holder.Header.ExpenseReportId, TransactionType.ExpenseReport);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(holder.Header.ExpenseReportId,
                                             TransactionType.ExpenseReport,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.ExpeseReportApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), holder.Header.ExpenseReportId);

                            FormSession.IsSubmitted = response.IsSuccess;
                            holder.Success = response.IsSuccess;
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
    }
}