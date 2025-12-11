using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class UndertimeRequestDataService : IUndertimeRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;

        public UndertimeRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
        }

        public async Task<UndertimeHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new UndertimeHolder()
            {
                UndertimeReasonSelectedItem = new ComboBoxObject(),
                UndertimeReason = new ObservableCollection<ComboBoxObject>()
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            try
            {
                await Task.Delay(500);
                using (UserDialogs.Instance.Loading())
                {
                    var enumUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{EnumValues.UndertimeType}"
                    };

                    var enums = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                    foreach (var item in enums)
                        retValue.UndertimeReason.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });

                    if (recordId > 0)
                    {
                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetUndertimeRequestDetail}{recordId}"
                        };

                        var response = await genericRepository_.GetAsync<R.Responses.UndertimeRequestDetailResponse>(builder.ToString());

                        if (response.IsSuccess)
                        {
                            retValue.UndertimeModel = new UndertimeModel();

                            PropertyCopier<R.Models.Undertime, UndertimeModel>.Copy(response.Undertime, retValue.UndertimeModel);
                            PropertyCopier<R.Models.Undertime, UndertimeHolder>.Copy(response.Undertime, retValue);

                            retValue.DepartureTime = Convert.ToDateTime(retValue.UndertimeModel.DepartureTime).TimeOfDay;
                            retValue.ArrivalTime = Convert.ToDateTime(retValue.UndertimeModel.ArrivalTime).TimeOfDay;

                            retValue.IsEnabled = (retValue.UndertimeModel.StatusId == RequestStatusValue.Draft);
                            retValue.ShowCancelButton = (retValue.UndertimeModel.StatusId == RequestStatusValue.ForApproval);
                        }
                    }
                    else
                    {
                        retValue.UndertimeModel = new UndertimeModel()
                        {
                            ProfileId = userInfo.ProfileId
                        };

                        retValue.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<UndertimeHolder> SubmitRequest(UndertimeHolder form)
        {
            var retValue = form;

            retValue.ErrorReason = false;
            retValue.ErrorUndertimeDate = false;
            retValue.ErrorUndertimeReason = false;
            retValue.ErrorUTHrs = false;

            if (retValue.UndertimeModel.UTHrs == 0)
                retValue.ErrorUTHrs = true;

            if (retValue.UndertimeReasonSelectedItem.Id == 0)
                retValue.ErrorUndertimeReason = true;

            if (string.IsNullOrWhiteSpace(retValue.UndertimeModel.Reason))
                retValue.ErrorReason = true;

            if (!retValue.ErrorReason && !retValue.ErrorUndertimeDate && !retValue.ErrorUndertimeReason && !retValue.ErrorUTHrs)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            form.UndertimeModel.UndertimeTypeId = (short)form.UndertimeReasonSelectedItem.Id;
                            form.UndertimeModel.DepartureTime = Convert.ToDateTime(form.UndertimeModel.UndertimeDate).Date + form.DepartureTime;
                            form.UndertimeModel.ArrivalTime = Convert.ToDateTime(form.UndertimeModel.UndertimeDate).Date + form.ArrivalTime;

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitUndertimeRequest
                            };

                            var data = new R.Models.Undertime();
                            PropertyCopier<UndertimeModel, R.Models.Undertime>.Copy(form.UndertimeModel, data);

                            var param = new R.Requests.SubmitUndertimeRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitUndertimeRequest, R.Responses.BaseResponse<R.Models.Undertime>>(builder.ToString(), param);

                            if (response.Model.UndertimeId > 0)
                            {
                                retValue.Success = true;

                                FormSession.IsSubmitted = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            return retValue;
        }

        public async Task<UndertimeApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new UndertimeApprovalHolder();
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetUndertimeRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.UndertimeRequestDetailResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    var enumUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{EnumValues.UndertimeType}/{response.Undertime.UndertimeTypeId}"
                    };

                    var enumVal = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());

                    retValue.UndertimeModel = new UndertimeModel();

                    PropertyCopier<R.Models.Undertime, UndertimeModel>.Copy(response.Undertime, retValue.UndertimeModel);
                    PropertyCopier<R.Models.Undertime, UndertimeApprovalHolder>.Copy(response.Undertime, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.EmployeeName;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.Undertime.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.UndertimeDate = Convert.ToDateTime(response.Undertime.UndertimeDate).ToString(FormHelper.DateFormat);
                    retValue.UndertimeHours = string.Format("{0:0.00} hrs", response.Undertime.UTHrs);
                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;
                    retValue.TimeRange = string.Empty.ConcatDateTime((DateTime)response.Undertime.UndertimeDate,
                        (DateTime)response.Undertime.DepartureTime,
                        (DateTime)response.Undertime.ArrivalTime);

                    retValue.Reason = enumVal.DisplayText;
                    retValue.Remarks = response.Undertime.Reason;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }
                }
                else
                    throw new Exception(response.ErrorMessage);

                retValue.IsSuccess = response.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<UndertimeApprovalHolder> WorkflowTransaction(UndertimeApprovalHolder form)
        {
            var retValue = form;
            retValue.IsSuccess = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.SelectedWorkflowAction.ActionTriggeredId, form.SelectedWorkflowAction.ActionMessage);
                if (confirm.Continue)
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);

                        await commonDataService_.HasInternetConnection(await commonDataService_.RetrieveClientUrl());

                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.TransactionId, form.TransactionTypeId, form.SelectedWorkflowAction.ActionTriggeredId, confirm.ResponseText, form.StageId);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            retValue.IsSuccess = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemUpdated = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemStatus = FormSession.SetDefaultMyApprovalSelectedItemStatus(form.SelectedWorkflowAction.ActionType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return retValue;
        }

        public async Task<UndertimeHolder> WorkflowTransactionRequest(UndertimeHolder form)
        {
            var retValue = form;
            retValue.Success = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.ActionTypeId, form.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.UndertimeModel.UndertimeId, TransactionType.Undertime);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.UndertimeModel.UndertimeId,
                                             TransactionType.Undertime,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            FormSession.IsSubmitted = response.IsSuccess;
                            retValue.Success = response.IsSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }
    }
}