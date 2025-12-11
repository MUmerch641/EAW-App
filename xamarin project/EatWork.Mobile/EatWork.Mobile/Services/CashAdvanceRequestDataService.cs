using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Excemptions;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.Accountability;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.CashAdvance;
using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class CashAdvanceRequestDataService : ICashAdvanceRequestDataService
    {
        private readonly IDialogService dialogService_;
        private readonly ICommonDataService commonDataService_;
        private readonly IGenericRepository genericRepository_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly StringHelper string_;

        public CashAdvanceRequestDataService()
        {
            dialogService_ = AppContainer.Resolve<IDialogService>();
            commonDataService_ = AppContainer.Resolve<ICommonDataService>();
            genericRepository_ = AppContainer.Resolve<IGenericRepository>();
            workflowDataService_ = AppContainer.Resolve<IWorkflowDataService>();
            string_ = AppContainer.Resolve<StringHelper>();
        }

        public long TotalListItem { get; set; }

        public async Task<ObservableCollection<CashAdvanceRequestList>> RetrieveList(ObservableCollection<CashAdvanceRequestList> list, ListParam args)
        {
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var user = PreferenceHelper.UserInfo();

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.CashAdvance}/list"
                };

                var param = new R.Requests.CashAdvanceListRequest
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

                var request = string_.CreateUrl<R.Requests.CashAdvanceListRequest>(builder.ToString(), param);

                var response = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.CashAdvanceList>>(request);
                args.Count = (response.ListData.Count <= args.Count ? response.ListData.Count : args.Count);

                if (response.TotalListCount != 0)
                {
                    foreach (var item in response.ListData)
                    {
                        var data = new CashAdvanceRequestList()
                        {
                            Amount = item.Amount,
                            CashAdvanceId = item.CashAdvanceId,
                            DateIssued = item.DateIssued,
                            DateRequested = item.DateRequested,
                            ProfileId = item.ProfileId,
                            ReferenceNumber = item.ReferenceNumber,
                            Status = item.Status,
                            StatusId = item.StatusId,
                            AmountDisplay = string.Format("{0:#,##0.00}", item.Amount),
                            DateIssuedDisplay = (item.DateIssued == Constants.NullDate ? "--" : item.DateIssued.ToString(Constants.ListDefaultDateFormat)),
                            DateRequestedDisplay = item.DateRequested.ToString(Constants.ListDefaultDateFormat)
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

        public async Task<CashAdvanceRequestHolder> InitForm(long recordId)
        {
            var holder = new CashAdvanceRequestHolder();

            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            #region get cost center

            var COSTCENTER_URL = new UriBuilder(url)
            {
                Path = $"{ApiConstants.CostCenter}/list"
            };

            var COSTCENTER_RESPONSE = await genericRepository_.GetAsync<List<R.Models.CostCenter>>(COSTCENTER_URL.ToString());

            if (COSTCENTER_RESPONSE.Count > 0)
            {
                holder.ChargeCodeSource = new ObservableCollection<ComboBoxObject>(
                    COSTCENTER_RESPONSE.Select(item => new ComboBoxObject
                    {
                        Id = Convert.ToInt64(item.CostCenterId),
                        Value = item.Code,
                    })
                );

                holder.ChargeCodeSource.Add(new ComboBoxObject() { Id = -1, Value = Constants.OptionOthers });
            }

            #endregion get cost center

            if (recordId > 0)
            {
                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.CashAdvance}/{recordId}"
                };

                var response = await genericRepository_.GetAsync<R.Responses.CashAdvanceDetailResponse>(builder.ToString());

                if (response.Model != null)
                {
                    holder.Model = response.Model;

                    holder.RequestNo = response.Model.RequestNo;
                    holder.DateRequested = response.Model.RequestedDate.GetValueOrDefault();
                    holder.DateNeeded = response.Model.DateNeeded.GetValueOrDefault();
                    holder.Amount.Value = response.Model.Amount.GetValueOrDefault();
                    holder.Reason.Value = response.Model.Reason;
                    holder.ChargeCode.Value = response.Model.ChargeCode;

                    if (holder.ChargeCodeSource.Count > 0)
                        holder.SelectedChargeCode = holder.ChargeCodeSource.FirstOrDefault(x => x.Id == response.Model.CostCenterId);

                    holder.IsEnabled = (holder.Model.StatusId == RequestStatusValue.Draft);
                    holder.ShowCancelButton = (holder.Model.StatusId == RequestStatusValue.ForApproval);
                }
            }
            else
            {
                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.CashAdvance}/generate-number"
                };

                var number = await genericRepository_.GetAsync<string>(builder.ToString());

                if (!string.IsNullOrWhiteSpace(number))
                {
                    holder.RequestNo = number;
                }

                holder.IsEnabled = true;
            }

            return holder;
        }

        public async Task<CashAdvanceRequestHolder> SubmitAsync(CashAdvanceRequestHolder holder)
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
                                Path = ApiConstants.CashAdvance
                            };

                            var param = new R.Requests.Accountability.SubmitCashAdvanceRequest()
                            {
                                Data = holder.Model
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.Accountability.SubmitCashAdvanceRequest, R.Responses.BaseResponse<R.Models.CashAdvance>>(builder.ToString(), param);

                            if (response.Model.CashAdvanceId > 0)
                            {
                                #region save file attachments

                                if (holder.FileAttachments.Count > 0)
                                {
                                    var files = new List<FileAttachmentParamsDto>(
                                            holder.FileAttachments.Select(x => new FileAttachmentParamsDto()
                                            {
                                                FileDataArray = x.FileDataArray,
                                                FileName = x.FileName,
                                                FileSize = x.FileSize,
                                                FileTags = "CASH ADVANCE",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = ModuleForms.CashAdvanceRequest,
                                                TransactionId = response.Model.CashAdvanceId,
                                            })
                                        );

                                    await commonDataService_.SaveFileAttachmentsAsync(files);
                                }

                                #endregion save file attachments

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
                            throw ex;
                        }
                    }
                }
            }

            return holder;
        }

        public async Task<CashAdvanceRequestHolder> CancelRequestAsync(CashAdvanceRequestHolder holder)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(holder.Model.CashAdvanceId, TransactionType.CashAdvance);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(holder.Model.CashAdvanceId,
                                             TransactionType.CashAdvance,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
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