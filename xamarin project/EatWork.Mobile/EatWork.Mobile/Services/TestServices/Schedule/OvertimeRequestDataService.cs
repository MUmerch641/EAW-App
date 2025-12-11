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
    public class OvertimeRequestDataService : IOvertimeRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;

        public OvertimeRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
        }

        public async Task<OvertimeRequestHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new OvertimeRequestHolder()
            {
                OvertimeReasonSelectedItem = new ComboBoxObject(),
                OvertimeReason = new ObservableCollection<ComboBoxObject>() { new ComboBoxObject() { Id = 0, Value = "" } },
                PreshiftOption = new ObservableCollection<string>() { "No", "Yes" }
            };
            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            try
            {
                await Task.Delay(100);
                using (UserDialogs.Instance.Loading())
                {
                    var enumUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetEnums}{EnumValues.OvertimeType}"
                    };

                    var enums = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                    foreach (var item in enums)
                        retValue.OvertimeReason.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });

                    if (recordId > 0)
                    {
                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetOvertimeRequestDetail}{recordId}"
                        };

                        var response = await genericRepository_.GetAsync<R.Responses.OvertimeRequestDetailResponse>(builder.ToString());

                        if (response.IsSuccess)
                        {
                            retValue.OvertimeModel = new OvertimeModel();

                            PropertyCopier<R.Models.Overtime, OvertimeModel>.Copy(response.Overtime, retValue.OvertimeModel);
                            PropertyCopier<R.Models.Overtime, OvertimeRequestHolder>.Copy(response.Overtime, retValue);

                            retValue.StartTime = Convert.ToDateTime(retValue.OvertimeModel.StartTime).TimeOfDay;
                            retValue.EndTime = Convert.ToDateTime(retValue.OvertimeModel.EndTime).TimeOfDay;

                            retValue.IsEnabled = (retValue.OvertimeModel.StatusId == RequestStatusValue.Draft);
                            retValue.ShowCancelButton = (retValue.OvertimeModel.StatusId == RequestStatusValue.ForApproval);
                            retValue.IsPreshift = (short)retValue.OvertimeModel.PreShiftOT;
                        }
                    }
                    else
                    {
                        retValue.OvertimeModel = new OvertimeModel()
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

        public async Task<OvertimeRequestHolder> SubmitRequest(OvertimeRequestHolder form)
        {
            var retValue = form;
            retValue.ErrorOTHours = false;
            retValue.ErrorReason = false;

            if (retValue.OvertimeModel.OROTHrs == 0)
                retValue.ErrorOTHours = true;

            if (string.IsNullOrWhiteSpace(retValue.OvertimeModel.Reason))
                retValue.ErrorReason = true;

            if (!retValue.ErrorOTHours && !retValue.ErrorReason)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            form.OvertimeModel.StartTime = Convert.ToDateTime(form.OvertimeModel.OvertimeDate).Date + form.StartTime;
                            form.OvertimeModel.EndTime = Convert.ToDateTime(form.OvertimeModel.OvertimeDate).Date + form.EndTime;
                            form.OvertimeModel.PreShiftOT = form.IsPreshift;

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitOvertimeRequest
                            };

                            var data = new R.Models.Overtime();
                            PropertyCopier<OvertimeModel, R.Models.Overtime>.Copy(form.OvertimeModel, data);

                            var param = new R.Requests.SubmitOvertimeRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitOvertimeRequest, R.Responses.BaseResponse<R.Models.Overtime>>(builder.ToString(), param);

                            if (response.Model.OvertimeId > 0)
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

        public async Task<OvertimeApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new OvertimeApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetOvertimeRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.OvertimeRequestDetailResponse>(builder.ToString());
                if (response.IsSuccess)
                {
                    retValue.OvertimeModel = new OvertimeModel();

                    PropertyCopier<R.Models.Overtime, OvertimeModel>.Copy(response.Overtime, retValue.OvertimeModel);
                    PropertyCopier<R.Models.Overtime, OvertimeApprovalHolder>.Copy(response.Overtime, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.EmployeeName;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.Overtime.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.OvertimeDate = Convert.ToDateTime(response.Overtime.OvertimeDate).ToString(FormHelper.DateFormat);
                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    retValue.TimeRange = string.Empty.ConcatDateTime((DateTime)response.Overtime.OvertimeDate,
                        (DateTime)response.Overtime.StartTime,
                        (DateTime)response.Overtime.EndTime);

                    retValue.OvertimeHours = string.Format("{0:0.00} hrs", response.Overtime.OROTHrs + response.Overtime.NSOTHrs);
                    retValue.IsPreshift = (response.Overtime.PreShiftOT == 1 ? "Yes" : "No");
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

        public async Task<OvertimeApprovalHolder> WorkflowTransaction(OvertimeApprovalHolder form)
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

                        var formData = JsonConvert.SerializeObject(new
                        {
                            txtApprovedOROTHrs = form.OvertimeModel.ApprovedOROTHrs.GetValueOrDefault(0),
                            dtpOffsettingExpirationDate = Constants.NullDate.ToString(Constants.DateFormatMMDDYYYY),
                        });

                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.TransactionId,
                            form.TransactionTypeId,
                            form.SelectedWorkflowAction.ActionTriggeredId,
                            confirm.ResponseText,
                            form.StageId,
                            formData);

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

        public async Task<OvertimeRequestHolder> WorkflowTransactionRequest(OvertimeRequestHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.OvertimeModel.OvertimeId, TransactionType.Overtime);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.OvertimeModel.OvertimeId,
                                             TransactionType.Overtime,
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

        public async Task<OvertimeRequestHolder> PreOTValidation(OvertimeRequestHolder form)
        {
            return form;
        }
    }
}