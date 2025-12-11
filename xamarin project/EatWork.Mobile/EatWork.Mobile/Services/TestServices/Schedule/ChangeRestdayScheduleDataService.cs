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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services.TestServices
{
    public class ChangeRestdayScheduleDataService : IChangeRestdayScheduleDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;

        public ChangeRestdayScheduleDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
        }

        public async Task<ChangeRestdayHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new ChangeRestdayHolder
            {
                RestDayList = new ObservableCollection<ChangeRestday>(),
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();

            try
            {
                await Task.Delay(500);
                using (UserDialogs.Instance.Loading())
                {
                    if (recordId > 0)
                    {
                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetChangeRestdayRequestDetail}{recordId}"
                        };

                        var response = await genericRepository_.GetAsync<R.Responses.ChangeRestdayRequestDetailResponse>(builder.ToString());

                        if (response.IsSuccess)
                        {
                            var employee = new R.Responses.BaseResponse<R.Models.EmployeeInformation>();

                            if (response.ChangeRestDay.SwapWithProfileId > 0)
                            {
                                builder = new UriBuilder(url)
                                {
                                    Path = $"{ApiConstants.GetEmployeeById}{response.ChangeRestDay.SwapWithProfileId}"
                                };

                                employee = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.EmployeeInformation>>(builder.ToString());
                            }

                            retValue.ChangeRestdayModel = new ChangeRestdayModel();

                            PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayModel>.Copy(response.ChangeRestDay, retValue.ChangeRestdayModel);
                            PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayHolder>.Copy(response.ChangeRestDay, retValue);

                            retValue.SwapWith = (response.ChangeRestDay.SwapWithProfileId > 0 ? employee.Model.EmployeeName : "");
                        }
                        else
                            throw new Exception(response.ErrorMessage);

                        retValue.IsEnabled = (retValue.ChangeRestdayModel.StatusId == RequestStatusValue.Draft);
                        retValue.ShowCancelButton = (retValue.ChangeRestdayModel.StatusId == RequestStatusValue.ForApproval);
                    }
                    else
                    {
                        retValue.ChangeRestdayModel = new ChangeRestdayModel()
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

        public async Task<ChangeRestdayHolder> GetEmployeeSchedule(ChangeRestdayHolder form)
        {
            var retValue = form;

            return retValue;
        }

        public async Task<ChangeRestdayHolder> SubmitRequest(ChangeRestdayHolder form)
        {
            var retValue = form;
            retValue.ErrorReason = false;

            if (string.IsNullOrWhiteSpace(retValue.ChangeRestdayModel.Reason))
                retValue.ErrorReason = true;

            if (form.ChangeRestdayModel.RestDayDate <= Convert.ToDateTime(Constants.NullDate))
                retValue.ErrorOriginalDate = true;

            if (form.ChangeRestdayModel.RequestDate <= Convert.ToDateTime(Constants.NullDate))
                retValue.ErrorRequestedDate = true;

            if (!retValue.ErrorOriginalDate && !retValue.ErrorReason && !retValue.ErrorRequestedDate)
            {
                if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        try
                        {
                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitChangeRestdayRequest
                            };

                            var data = new R.Models.ChangeRestDay();
                            PropertyCopier<ChangeRestdayModel, R.Models.ChangeRestDay>.Copy(form.ChangeRestdayModel, data);

                            var param = new R.Requests.SubmitChangRestdayRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitChangRestdayRequest, R.Responses.BaseResponse<R.Models.ChangeRestDay>>(builder.ToString(), param);

                            if (response.Model.ChangeRestDayId > 0)
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

        public async Task<ChangeRestdayScheduleApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new ChangeRestdayScheduleApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetChangeRestdayRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.ChangeRestdayRequestDetailResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    var employee = new R.Responses.BaseResponse<R.Models.EmployeeInformation>();

                    if (response.ChangeRestDay.SwapWithProfileId > 0)
                    {
                        builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEmployeeById}{response.ChangeRestDay.SwapWithProfileId}"
                        };

                        employee = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.EmployeeInformation>>(builder.ToString());
                    }

                    retValue.ChangeRestdayModel = new ChangeRestdayModel();

                    PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayModel>.Copy(response.ChangeRestDay, retValue.ChangeRestdayModel);
                    PropertyCopier<R.Models.ChangeRestDay, ChangeRestdayScheduleApprovalHolder>.Copy(response.ChangeRestDay, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;
                    retValue.DateFiled = Convert.ToDateTime(response.ChangeRestDay.DateFiled).ToString(FormHelper.DateFormat);
                    retValue.RestdayDate = Convert.ToDateTime(response.ChangeRestDay.RestDayDate).ToString(FormHelper.DateFormat);
                    retValue.RequestedDate = Convert.ToDateTime(response.ChangeRestDay.RequestDate).ToString(FormHelper.DateFormat);
                    retValue.SwapWithEmployeeName = (response.ChangeRestDay.SwapWithProfileId > 0 ? employee.Model.FullNameMiddleInitialOnly : "");

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

        public async Task<ChangeRestdayScheduleApprovalHolder> WorkflowTransaction(ChangeRestdayScheduleApprovalHolder form)
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
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<ChangeRestdayHolder> WorkflowTransactionRequest(ChangeRestdayHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(form.ChangeRestdayModel.ChangeRestDayId, TransactionType.ChangeRestDay);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.ChangeRestdayModel.ChangeRestDayId,
                                             TransactionType.ChangeRestDay,
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