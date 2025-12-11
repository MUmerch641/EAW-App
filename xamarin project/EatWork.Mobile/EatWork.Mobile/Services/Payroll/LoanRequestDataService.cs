using Acr.UserDialogs;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class LoanRequestDataService : ILoanRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;

        public LoanRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            IDialogService dialogService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            dialogService_ = dialogService;
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
                        retValue.LoanRequestFile = response.LoanRequestFile;

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
                        ProfileId = userInfo.ProfileId,
                        DateRequest = selectedDate ?? DateTime.Now.Date
                    };

                    retValue.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                FormSession.IsMySchedule = false;
            }

            return retValue;
        }

        public async Task<LoanRequestHolder> SubmitRequest(LoanRequestHolder form)
        {
            var errors = new List<int>();
            form.ErrorLoanType = false;
            form.ErrorRequestedAmount = false;

            if (form.SelectedLoanType.Id == 0)
            {
                form.ErrorLoanType = true;
                errors.Add(1);
            }

            if (form.LoanRequestModel.RequestedAmount == 0)
            {
                form.ErrorRequestedAmount = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                if (!form.Aggreed)
                {
                    //throw new ArgumentException(Messages.LoanRequestAgree);
                    await dialogService_.AlertAsync(Messages.LoanRequestAgree);
                }
                else
                {
                    if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                    {
                        await Task.Delay(500);
                        using (UserDialogs.Instance.Loading())
                        {
                            form.LoanRequestModel.LoanTypeSetupId = form.SelectedLoanType.Id;
                            form.LoanRequestModel.LoanAmount = form.LoanRequestModel.RequestedAmount;
                            form.LoanRequestModel.ActualLoanAmount = form.LoanRequestModel.RequestedAmount;
                            form.LoanRequestModel.TotalAmountDue = form.LoanRequestModel.RequestedAmount;
                            form.LoanRequestModel.Balance = form.LoanRequestModel.RequestedAmount;

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitLoanRequest
                            };

                            var data = new R.Models.LoanRequest();
                            PropertyCopier<LoanRequestModel, R.Models.LoanRequest>.Copy(form.LoanRequestModel, data);

                            var file = form.FileAttachments.Count > 0
                                ? new R.Requests.FileAttachmentRequest
                                {
                                    FileName = form.FileAttachments.FirstOrDefault()?.FileName,
                                    FileSize = form.FileAttachments.FirstOrDefault()?.FileSize,
                                    FileType = form.FileAttachments.FirstOrDefault()?.FileType,
                                    FileAttachment = $"data:{form.FileAttachments.FirstOrDefault()?.MimeType};base64,{form.FileAttachments.FirstOrDefault()?.Base64String}"
                                }
                                : new R.Requests.FileAttachmentRequest();

                            var param = new R.Requests.SubmitLoanRequest()
                            {
                                Data = data,
                                Attachment = file,
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitLoanRequest, R.Responses.BaseResponse<R.Models.LoanRequest>>(builder.ToString(), param);

                            if (response.Model.LoanRequestId > 0)
                            {
                                /*
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.LoanRequest, response.Model.LoanRequestId);
                                */
                                /*

                                #region save file attachments

                                if (form.FileAttachments.Count > 0)
                                {
                                    var files = new List<FileAttachmentParamsDto>(
                                            form.FileAttachments.Select(x => new FileAttachmentParamsDto()
                                            {
                                                FileDataArray = x.FileDataArray,
                                                FileName = x.FileName,
                                                FileSize = x.FileSize,
                                                FileTags = "LOAN",
                                                FileType = x.FileType,
                                                MimeType = x.MimeType,
                                                RawFileSize = x.RawFileSize,
                                                ModuleFormId = ModuleForms.LoanRequest,
                                                TransactionId = response.Model.LoanRequestId,
                                            })
                                        );

                                    await commonDataService_.SaveFileAttachmentsAsync(files);
                                }

                                #endregion save file attachments

                                */
                                form.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                        }
                    }
                }
            }

            return form;
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

                    if (retValue.LoanRequestModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
                    {
                        retValue.WorkflowActions.Add(new WorkflowAction()
                        {
                            ActionMessage = Messages.Cancel,
                            ActionTriggeredId = ActionTypeId.Cancel,
                            ActionType = "Cancel",
                            CurrentStageId = WFDetail.WorkflowDetail.CurrentStageId,
                            TransactionId = transactionId,
                            TransactionTypeId = transactionTypeId,
                        });
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
            form.IsSuccess = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.SelectedWorkflowAction.ActionTriggeredId, form.SelectedWorkflowAction.ActionMessage);
                if (confirm.Continue)
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(500);
                        var url = await commonDataService_.RetrieveClientUrl();
                        await commonDataService_.HasInternetConnection(url);

                        var response = await workflowDataService_.ProcessWorkflowByRecordId(form.TransactionId, form.TransactionTypeId, form.SelectedWorkflowAction.ActionTriggeredId, confirm.ResponseText, form.StageId);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.LoanRequestApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.TransactionId);

                            form.IsSuccess = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemUpdated = response.IsSuccess;
                            FormSession.MyApprovalSelectedItemStatus = FormSession.SetDefaultMyApprovalSelectedItemStatus(form.SelectedWorkflowAction.ActionType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return form;
        }

        public async Task<LoanRequestHolder> WorkflowTransactionRequest(LoanRequestHolder form)
        {
            form.Success = false;

            try
            {
                var confirm = await workflowDataService_.ConfirmationMessage(form.ActionTypeId, form.Msg);
                if (confirm.Continue)
                {
                    await Task.Delay(500);
                    using (UserDialogs.Instance.Loading())
                    {
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(Convert.ToInt64(form.LoanRequestModel.LoanRequestId), TransactionType.Loan);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(Convert.ToInt64(form.LoanRequestModel.LoanRequestId),
                                             TransactionType.Loan,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.LoanRequestApi}/update-wfsource"
                            };

                            await workflowDataService_.UpdateWFSourceId(builder.ToString(), form.LoanRequestModel.LoanRequestId);

                            FormSession.IsSubmitted = response.IsSuccess;
                            form.Success = response.IsSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return form;
        }
    }
}