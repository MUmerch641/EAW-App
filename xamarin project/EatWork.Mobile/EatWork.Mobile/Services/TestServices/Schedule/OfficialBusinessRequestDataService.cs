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
    public class OfficialBusinessRequestDataService : IOfficialBusinessRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;

        public OfficialBusinessRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
        }

        public async Task<OfficialBusinessHolder> InitForm(long recordId, int obTypeId, DateTime? selectedDate)
        {
            var retValue = new OfficialBusinessHolder()
            {
                OBApplyToSelectedItem = new ComboBoxObject(),
                OBReasonSelectedItem = new ComboBoxObject(),
                OBReason = new ObservableCollection<ComboBoxObject>(),
                OBApplyTo = new ObservableCollection<ComboBoxObject>()
            };
            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            retValue.StartCheckboxEnabled = true;
            retValue.EndCheckboxEnabled = true;

            try
            {
                await Task.Delay(500);
                using (UserDialogs.Instance.Loading())
                {
                    var enumType = (obTypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessType : EnumValues.TimeOffType);
                    var applyAgainst = (obTypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessApplyAgainst : EnumValues.TimeOffApplyAgainst);

                    var enumTypeUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{enumType}"
                    };

                    var applyAgainstUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{applyAgainst}"
                    };

                    var reason = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumTypeUrl.ToString());
                    var applyTo = await genericRepository_.GetAsync<List<R.Models.Enums>>(applyAgainstUrl.ToString());

                    foreach (var item in reason)
                    {
                        if (!string.IsNullOrWhiteSpace(item.DisplayText))
                            retValue.OBReason.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });
                    }

                    foreach (var item in applyTo)
                    {
                        if (!string.IsNullOrWhiteSpace(item.DisplayText))
                            retValue.OBApplyTo.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });
                    }

                    if (recordId > 0)
                    {
                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetOfficialBusinessRequestDetail}{recordId}"
                        };

                        var response = await genericRepository_.GetAsync<R.Responses.OfficialBusinessRequestDetailResponse>(builder.ToString());
                        if (response.IsSuccess)
                        {
                            enumType = (response.OfficialBusiness.TypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessType : EnumValues.TimeOffType);
                            applyAgainst = (response.OfficialBusiness.TypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessApplyAgainst : EnumValues.TimeOffApplyAgainst);

                            enumTypeUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetEnums}{enumType}/{response.OfficialBusiness.OBTypeId}"
                            };

                            applyAgainstUrl = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.GetEnums}{applyAgainst}/{response.OfficialBusiness.ApplyTo}"
                            };

                            var reasonVal = await genericRepository_.GetAsync<R.Models.Enums>(enumTypeUrl.ToString());
                            var applyToVal = await genericRepository_.GetAsync<R.Models.Enums>(applyAgainstUrl.ToString());

                            retValue.OfficialBusinessModel = new OfficialBusinessModel();

                            PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessModel>.Copy(response.OfficialBusiness, retValue.OfficialBusinessModel);
                            PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessHolder>.Copy(response.OfficialBusiness, retValue);

                            retValue.OBStartDate = response.OfficialBusiness.OfficialBusinessDate;
                            retValue.OBEndDate = response.OfficialBusiness.OfficialBusinessDate;
                            retValue.StartTime = Convert.ToDateTime(response.OfficialBusiness.StartTime).TimeOfDay;
                            retValue.EndTime = Convert.ToDateTime(response.OfficialBusiness.EndTime).TimeOfDay;
                        }
                        else
                            throw new Exception(response.ErrorMessage);

                        retValue.IsEnabled = (retValue.OfficialBusinessModel.StatusId == RequestStatusValue.Draft);
                        retValue.ShowCancelButton = (retValue.OfficialBusinessModel.StatusId == RequestStatusValue.ForApproval);
                    }
                    else
                    {
                        retValue.OfficialBusinessModel = new OfficialBusinessModel()
                        {
                            ProfileId = userInfo.ProfileId,
                            TypeId = Convert.ToByte(obTypeId)
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

        public async Task<OfficialBusinessHolder> SubmitRequest(OfficialBusinessHolder form)
        {
            var retValue = form;
            retValue.ErrorOBReason = false;
            retValue.ErrorOBStartDate = false;
            retValue.ErrorRemarks = false;
            retValue.ErrorNoOfHours = false;
            retValue.ErrorOBApplyTo = false;

            if (retValue.OfficialBusinessModel.NoOfHours == 0)
                retValue.ErrorNoOfHours = true;

            if (retValue.OBReasonSelectedItem.Id == 0)
                retValue.ErrorOBReason = true;

            if (string.IsNullOrWhiteSpace(retValue.OfficialBusinessModel.Remarks))
                retValue.ErrorRemarks = true;

            if (retValue.OBApplyToSelectedItem.Id == 0)
                retValue.ErrorOBApplyTo = true;

            if (!retValue.ErrorOBReason && !retValue.ErrorOBStartDate && !retValue.ErrorRemarks && !retValue.ErrorNoOfHours && !retValue.ErrorOBApplyTo)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            retValue.OfficialBusinessModel.OfficialBusinessDate = form.OBStartDate;
                            retValue.OfficialBusinessModel.StartTime = form.OBStartDate + form.StartTime;
                            retValue.OfficialBusinessModel.EndTime = form.OBStartDate + form.EndTime;
                            retValue.OfficialBusinessModel.OBTypeId = form.OBReasonSelectedItem.Id;
                            retValue.OfficialBusinessModel.ApplyTo = Convert.ToByte(form.OBApplyToSelectedItem.Id);

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitOfficialBusinessRequest
                            };

                            var data = new R.Models.OfficialBusiness();
                            PropertyCopier<OfficialBusinessModel, R.Models.OfficialBusiness>.Copy(form.OfficialBusinessModel, data);

                            var param = new R.Requests.SubmitOfficialBusinessRequest()
                            {
                                Data = data,
                                StartDate = (DateTime)form.OBStartDate,
                                EndDate = (DateTime)form.OBEndDate
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitOfficialBusinessRequest, R.Responses.BaseResponse<R.Models.OfficialBusiness>>(builder.ToString(), param);

                            if (response.Model.OfficialBusinessId > 0)
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

        public async Task<OfficialBusinessApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new OfficialBusinessApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetOfficialBusinessRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.OfficialBusinessRequestDetailResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    var enumType = (response.OfficialBusiness.TypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessType : EnumValues.TimeOffType);
                    var applyAgainst = (response.OfficialBusiness.TypeId == Constants.OfficialBusiness ? EnumValues.OfficialBusinessApplyAgainst : EnumValues.TimeOffApplyAgainst);

                    var enumTypeUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{enumType}/{response.OfficialBusiness.OBTypeId}"
                    };

                    var applyAgainstUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{applyAgainst}/{response.OfficialBusiness.ApplyTo}"
                    };

                    var reason = await genericRepository_.GetAsync<R.Models.Enums>(enumTypeUrl.ToString());
                    var applyTo = await genericRepository_.GetAsync<R.Models.Enums>(applyAgainstUrl.ToString());

                    retValue.OfficialBusinessModel = new OfficialBusinessModel();

                    PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessModel>.Copy(response.OfficialBusiness, retValue.OfficialBusinessModel);
                    PropertyCopier<R.Models.OfficialBusiness, OfficialBusinessApprovalHolder>.Copy(response.OfficialBusiness, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.EmployeeName;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.OfficialBusiness.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.OBDate = Convert.ToDateTime(response.OfficialBusiness.OfficialBusinessDate).ToString(FormHelper.DateFormat);
                    retValue.OBTime = string.Empty.ConcatDateTime((DateTime)response.OfficialBusiness.OfficialBusinessDate,
                                       (DateTime)response.OfficialBusiness.StartTime,
                                       (DateTime)response.OfficialBusiness.EndTime);

                    retValue.OBHours = string.Format("{0:0.00} hrs", response.OfficialBusiness.OBHours);
                    retValue.Reason = reason.DisplayText;
                    retValue.ApplyAgainst = applyTo.DisplayText;

                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

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

        public async Task<OfficialBusinessApprovalHolder> WorkflowTransaction(OfficialBusinessApprovalHolder form)
        {
            var retValue = form;
            retValue.IsSuccess = false;

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

            return retValue;
        }

        public async Task<OfficialBusinessHolder> WorkflowTransactionRequest(OfficialBusinessHolder form)
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
                        var transactionType = (form.OfficialBusinessModel.TypeId == Constants.OfficialBusiness ? TransactionType.OfficialBusiness : TransactionType.TimeOff);

                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.OfficialBusinessModel.OfficialBusinessId, transactionType);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.OfficialBusinessModel.OfficialBusinessId,
                                             transactionType,
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

        public async Task<OfficialBusinessHolder> SaveRecord(OfficialBusinessHolder form, R.Requests.SubmitOfficialBusinessRequest request)
        {
            throw new NotImplementedException();
        }
    }
}