using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.Expense;
using EatWork.Mobile.Models.FormHolder.Expenses;
using EatWork.Mobile.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class ExpenseDataService : IExpenseDataService
    {
        public long TotalListItem { get; set; }

        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;
        private readonly StringHelper string_;

        public ExpenseDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            StringHelper url,
            IDialogService dialogService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            string_ = url;
            dialogService_ = dialogService;
        }

        public async Task<ObservableCollection<MyExpensesListDto>> GetLisyAsync(ObservableCollection<MyExpensesListDto> list, ListParam args)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var user = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.MyExpenses}/list"
                };

                var param = new R.Requests.MyApprovalRequest
                {
                    ProfileId = user.ProfileId,
                    Page = (args.ListCount == 0 ? 1 : ((args.ListCount + args.Count) / args.Count)),
                    Rows = args.Count,
                    SortOrder = (args.IsAscending ? 0 : 1),
                    Keyword = args.KeyWord,
                    TransactionTypes = args.FilterTypes,
                    Status = args.Status,
                    StartDate = args.StartDate,
                    EndDate = args.EndDate,
                };

                var request = string_.CreateUrl<R.Requests.MyApprovalRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.MyExpensesList>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.TotalListCount != 0)
                {
                    try
                    {
                        foreach (var item in response.ListData)
                        {
                            var data = new MyExpensesListDto()
                            {
                                Amount = item.Amount,
                                ExpenseDate = item.ExpenseDate,
                                ExpenseType = item.ExpenseType,
                                Icon = item.Icon,
                                ProfileId = item.ProfileId,
                                VendorName = item.VendorName,
                                ExpenseDateDisplay = item.ExpenseDate.GetValueOrDefault().ToString(Constants.ListDefaultDateFormat),
                                AmountDisplay = string.Format("{0:#,##0.00}", item.Amount),
                                FileAttachment = item.FileAttachment,
                                ExpenseReportDetailId = item.ExpenseReportDetailId,
                                IconColor = item.IconColor,
                                TotalCount = item.TotalCount,
                                ExpenseSetupId = item.ExpenseSetupId,
                                FileName = item.FileName,
                                FileType = item.FileType,
                                FileUpload = item.FileUpload,
                                Notes = item.Notes,
                                ORNo = item.ORNo,
                                VendorId = item.VendorId,
                                IsChecked = false,
                                HasAttachment = !string.IsNullOrWhiteSpace(item.FileAttachment)
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

        public async Task<AppExpenseReportDetail> GetRecordAsync(long id)
        {
            var retValue = new AppExpenseReportDetail();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.MyExpenses}/{id}/detail"
                };

                var response = await genericRepository_.GetAsync<R.Responses.MyExpenseDetailResponse>(builder.ToString());

                if (response.Model != null)
                {
                    PropertyCopier<R.Models.AppExpenseReportDetail, AppExpenseReportDetail>.Copy(response.Model, retValue);

                    retValue.ExpenseDateDisplay = retValue.ExpenseDate.GetValueOrDefault().ToString("ddd, MMM. dd, yyyy");
                    retValue.AmountDisplay = string.Format("{0:#,##0.00}", retValue.Amount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<NewExpenseHolder> InitExpenseForm()
        {
            var retValue = new NewExpenseHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var expense = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.MyExpenses}/expense-setup-list"
                };

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.ExpenseSetupList>>(expense.ToString());

                if (response.ListData.Count > 0)
                {
                    foreach (var item in response.ListData)
                    {
                        retValue.ExpenseTypes.Add(new ExpenseSetupModel()
                        {
                            AccountCode = item.AccountCode,
                            AccountTitle = item.AccountTitle,
                            ColorValue = item.ColorValue,
                            ExpenseSetupId = item.ExpenseSetupId,
                            ExpenseType = item.ExpenseType,
                            Icon = item.Icon,
                            IconEquivalent = item.IconEquivalent,
                        });
                    }
                }

                var vendor = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.Vendor}/vendor-list"
                };

                var vendorResp = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.Vendor>>(vendor.ToString());

                if (vendorResp.ListData.Count > 0)
                {
                    foreach (var item in vendorResp.ListData)
                    {
                        retValue.Vendors.Add(new VendorModel()
                        {
                            Address = item.Address,
                            Name = item.Name,
                            TINNo = item.TINNo,
                            VendorId = item.VendorId,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<NewExpenseHolder> SubmitRecord(NewExpenseHolder holder)
        {
            if (holder.ExecuteSubmit())
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        try
                        {
                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.MyExpenses
                            };

                            if (!string.IsNullOrWhiteSpace(holder.FileUploadResponse.FileName))
                            {
                                holder.Model.FileName = holder.FileUploadResponse.FileName;
                                holder.Model.FileType = holder.FileUploadResponse.FileType;
                                holder.Model.Attachment = string.Format("data:{0};base64,{1}", holder.FileUploadResponse.MimeType, Convert.ToBase64String(holder.FileUploadResponse.FileDataArray));
                            }

                            var newvendor = new R.Models.Vendor();
                            if (!string.IsNullOrWhiteSpace(holder.NewVendor.Name))
                            {
                                newvendor = new R.Models.Vendor()
                                {
                                    Address = holder.NewVendor.Address,
                                    Name = holder.NewVendor.Name,
                                    TINNo = holder.NewVendor.TINNo,
                                };
                            }

                            var param = new R.Requests.SubmitAppExpenseReportDetailRequest()
                            {
                                Data = holder.Model,
                                Vendor = newvendor
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitAppExpenseReportDetailRequest, R.Responses.BaseResponse<R.Models.AppExpenseReportDetail>>(builder.ToString(), param);

                            if (response.Model.AppExpenseReportDetailId > 0)
                            {
                                holder.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                        }
                        catch (HttpRequestExceptionEx ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            return holder;
        }

        public async Task<int> DeleteAsync(ObservableCollection<MyExpensesListDto> list)
        {
            var retVal = 0;

            try
            {
                if (await dialogService_.ConfirmDialogAsync(Messages.DeleteExpenses))
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        foreach (var item in list)
                        {
                            var builder = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.MyExpenses}/{item.ExpenseReportDetailId}"
                            };

                            await genericRepository_.DeleteAsync(builder.ToString());

                            retVal++;
                        }

                        /*
                        var items = new List<long>();

                        foreach (var item in list)
                        {
                            items.Add(item.ExpenseReportDetailId);
                        }

                        var param = new R.Requests.DeleteExpensesRequest
                        {
                            Ids = items
                        };

                        var request = string_.CreateUrl<R.Requests.DeleteExpensesRequest>(builder.ToString(), param);

                        await genericRepository_.DeleteAsync(request);
                        */
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retVal;
        }

        public async Task<ObservableCollection<SelectableListModel>> GetExpenseTypes()
        {
            var types = new ObservableCollection<SelectableListModel>();
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var expense = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.MyExpenses}/expense-setup-list"
                };

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.ExpenseSetupList>>(expense.ToString());

                if (response.ListData.Count > 0)
                {
                    foreach (var item in response.ListData)
                    {
                        types.Add(new SelectableListModel()
                        {
                            DisplayText = item.AccountTitle,
                            IsChecked = false,
                            Id = item.ExpenseSetupId,
                            Icon = item.Icon,
                            DisplayData = item.AccountCode,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw ex;
            }

            return types;
        }
    }
}