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
    public class LoanRequestDataService : ILoanRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;

        public LoanRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
        }

        public async Task<LoanRequestHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new LoanRequestHolder()
            {
                LoanTypeList = new ObservableCollection<SelectableListModel>(),
                SelectedLoanType = new SelectableListModel(),
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            try
            {
                var loanTypeUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetLoanTypes
                };

                var loanTypes = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.LoanTypeSetup>>(loanTypeUrl.ToString());

                if (loanTypes.ListData.Count > 0)
                {
                    foreach (var item in loanTypes.ListData)
                        retValue.LoanTypeList.Add(new SelectableListModel() { Id = item.LoanTypeSetupId, DisplayText = item.Code });
                }

                retValue.EmployeeName = userInfo.EmployeeName;

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetLoanRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.LoanRequestResponse>(builder.ToString());

                    if (response.LoanRequest.LoanRequestId > 0)
                    {
                        retValue.LoanRequestModel = new LoanRequestModel();
                        PropertyCopier<R.Models.LoanRequest, LoanRequestModel>.Copy(response.LoanRequest, retValue.LoanRequestModel);

                        loanTypeUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetLoanType}{response.LoanRequest.LoanTypeSetupId}"
                        };

                        var loanType = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.LoanTypeSetup>>(loanTypeUrl.ToString());

                        retValue.SelectedLoanType = new SelectableListModel()
                        {
                            Id = response.LoanRequest.LoanTypeSetupId.Value,
                            DisplayText = loanType.Model.Code
                        };
                    }
                    retValue.Aggreed = true;
                    retValue.IsEnabled = (retValue.LoanRequestModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.LoanRequestModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.LoanRequestModel = new LoanRequestModel()
                    {
                        ProfileId = userInfo.ProfileId
                    };

                    retValue.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<LoanRequestHolder> SubmitRequest(LoanRequestHolder form)
        {
            var retValue = form;
            var errors = new List<int>();
            retValue.ErrorLoanType = false;
            retValue.ErrorRequestedAmount = false;

            if (retValue.SelectedLoanType.Id == 0)
            {
                retValue.ErrorLoanType = true;
                errors.Add(1);
            }

            if (retValue.LoanRequestModel.RequestedAmount == 0)
            {
                retValue.ErrorRequestedAmount = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                if (!form.Aggreed)
                    throw new ArgumentException(Messages.LoanRequestAgree);
                else
                {
                    if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))
                    {
                        await Task.Delay(500);
                        using (UserDialogs.Instance.Loading())
                        {
                            retValue.LoanRequestModel.LoanTypeSetupId = form.SelectedLoanType.Id;
                            retValue.LoanRequestModel.LoanAmount = form.LoanRequestModel.RequestedAmount;
                            retValue.LoanRequestModel.ActualLoanAmount = form.LoanRequestModel.RequestedAmount;
                            retValue.LoanRequestModel.TotalAmountDue = form.LoanRequestModel.RequestedAmount;
                            retValue.LoanRequestModel.Balance = form.LoanRequestModel.RequestedAmount;

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitLoanRequest
                            };

                            var data = new R.Models.LoanRequest();
                            PropertyCopier<LoanRequestModel, R.Models.LoanRequest>.Copy(form.LoanRequestModel, data);

                            var param = new R.Requests.SubmitLoanRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitLoanRequest, R.Responses.BaseResponse<R.Models.LoanRequest>>(builder.ToString(), param);

                            if (response.Model.LoanRequestId > 0)
                            {
                                retValue.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                        }
                    }
                }
            }

            return retValue;
        }

        public async Task<LoanRequestApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new LoanRequestApprovalHolder();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetLoanRequestDetail}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                var response = await genericRepository_.GetAsync<R.Responses.LoanRequestResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    retValue.LoanRequestModel = new LoanRequestModel();

                    PropertyCopier<R.Models.LoanRequest, LoanRequestModel>.Copy(response.LoanRequest, retValue.LoanRequestModel);
                    PropertyCopier<R.Models.LoanRequest, LoanRequestApprovalHolder>.Copy(response.LoanRequest, retValue);

                    retValue.EmployeeName = response.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = response.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = response.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = response.EmployeeInfo.Department;

                    #region get loan type

                    var loanTypeUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetLoanType}{response.LoanRequest.LoanTypeSetupId}"
                    };

                    var loanType = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.LoanTypeSetup>>(loanTypeUrl.ToString());

                    retValue.LoanType = loanType.Model.Code;

                    #endregion get loan type

                    retValue.DateFiled = Convert.ToDateTime(response.LoanRequest.CreateDate).ToString(FormHelper.DateFormat);

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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<LoanRequestApprovalHolder> WorkflowTransaction(LoanRequestApprovalHolder form)
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

        public async Task<LoanRequestHolder> WorkflowTransactionRequest(LoanRequestHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(Convert.ToInt64(form.LoanRequestModel.LoanRequestId), TransactionType.Loan);
                        var response = await workflowDataService_.ProcessWorkflowByRecordId(Convert.ToInt64(form.LoanRequestModel.LoanRequestId),
                                             TransactionType.Loan,
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