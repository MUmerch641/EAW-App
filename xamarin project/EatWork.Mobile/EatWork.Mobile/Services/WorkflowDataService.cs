using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Requests;
using EAW.API.DataContracts.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class WorkflowDataService : IWorkflowDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IDialogService dialogService_;
        private readonly StringHelper string_;

        public WorkflowDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IDialogService dialogService,
            StringHelper url)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            dialogService_ = dialogService;
            string_ = url;
        }

        public async Task<WFResponseMessage> ConfirmationMessage(long actionType, string msg)
        {
            var retValue = new WFResponseMessage();

            if (actionType == 2 || actionType == 3)
            {
                /*var alert = await UserDialogs.Instance.PromptAsync(msg, "", "Yes", "No");*/
                var alert = await dialogService_.InputDialogAsync(msg);
                retValue.Continue = alert.Confirmed;
                retValue.ResponseText = alert.ResponseText;
            }
            else
            {
                /*retValue.Continue = (await UserDialogs.Instance.ConfirmAsync(msg, "", "Yes", "No"));*/
                retValue.Continue = (await dialogService_.ConfirmDialogAsync(msg));
            }

            return retValue;
        }

        public async Task<GetWorkflowDetailsResponse> GetWorkflowDetails(long transactionId, long TransactionTypeId)
        {
            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
            {
                Path = ApiConstants.GetWorkflowDetails
            };

            var userInfo = PreferenceHelper.UserInfo();

            var param = new GetWorkflowDetailsRequest()
            {
                TransactionId = transactionId,
                TransactionTypeId = TransactionTypeId,
                ApproverId = userInfo.ProfileId,
                UserTypeId = userInfo.UserTypeId
            };

            var url = string_.CreateUrl<GetWorkflowDetailsRequest>(builder.ToString(), param);

            return await genericRepository_.GetAsync<GetWorkflowDetailsResponse>(url);
        }

        public async Task<WFTransactionResponse> ProcessWorkflowByRecordId(long TransactionId, long TransactionTypeId, long actionType, string remarks = "", long stageId = 0, string formData = "")
        {
            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
            {
                Path = ApiConstants.ProcessWFTransaction
            };

            var userInfo = PreferenceHelper.UserInfo();

            var param = new WFTransactionRequest()
            {
                TransactionTypeId = TransactionTypeId,
                TransactionId = TransactionId,
                StageId = stageId,
                ActionTriggeredId = actionType,
                ApproverId = userInfo.ProfileId,
                FormData = formData,
                Remarks = remarks,
                UserAccessId = userInfo.UserSecurityId
            };

            return await genericRepository_.PostAsync<WFTransactionRequest, WFTransactionResponse>(builder.ToString(), param);
        }

        public async Task<List<TransactionHistory>> GetTransactionHistory(long TransactionTypeId, long transactionId)
        {
            var retValue = new List<TransactionHistory>();

            try
            {
                retValue.Add(new TransactionHistory()
                {
                    RequestersName = "",
                    ApproversName = "",
                    TransactionType = "",
                    StageDescription = "",
                    NextApprovers = "",
                    ActionTypeId = 0,
                    ActionPastTense = "",
                    MessageTemplate = "Start",
                    Remarks = "",
                    DetailedMessage = "",
                    HistoryTypeId = HistoryType.CUSTOM_FOR_DISPLAY,
                    LogDate = Constants.NullDate,
                    CreatedBy = ""
                });

                var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                {
                    Path = string.Format(ApiConstants.GetTranasactionHistory, TransactionTypeId, transactionId)
                };

                var response = await genericRepository_.GetAsync<TransactionHistoryResponse>(builder.ToString());

                if (response.IsSuccess)
                {
                    if (response.TransactionHistoryDetails.Count > 0)
                    {
                        foreach (var item in response.TransactionHistoryDetails)
                        {
                            var model = new TransactionHistory();
                            PropertyCopier<R.Models.TransactionHistoryDetails, Models.TransactionHistory>.Copy(item, model);
                            //replace keywords
                            model.MessageTemplate = string_.StringBinder<TransactionHistory>(model.MessageTemplate, model, "{{", "}}");

                            if (!string.IsNullOrWhiteSpace(model.Remarks))
                            {
                                model.MessageTemplate = string.Format("{0}<br/><b>Remarks:</b><br/>{1}", model.MessageTemplate, model.Remarks);
                            }

                            retValue.Add(model);
                        }
                    }
                }
                else
                    throw new Exception(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return retValue;
        }

        public async Task<WFTransactionResponse> CancelWorkFlowByRecordId(long TransactionId, long TransactionTypeId, long actionType, string remarks = "", long stageId = 0, string formData = "", bool hasApproverId = true)
        {
            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
            {
                Path = ApiConstants.WorkFlowCancelRequest
            };

            var userInfo = PreferenceHelper.UserInfo();

            var param = new WFTransactionRequest()
            {
                TransactionTypeId = TransactionTypeId,
                TransactionId = TransactionId,
                StageId = stageId,
                ActionTriggeredId = actionType,
                ApproverId = (hasApproverId ? userInfo.ProfileId : 0),
                FormData = formData,
                Remarks = remarks,
                UserAccessId = userInfo.UserSecurityId
            };

            return await genericRepository_.PostAsync<WFTransactionRequest, WFTransactionResponse>(builder.ToString(), param);
        }

        public async Task<bool> UpdateWFSourceId(string url, long transactionId)
        {
            try
            {
                var param = new UpdateWFSourcerequest()
                {
                    RecordId = transactionId,
                    SourceId = (short)SourceEnum.Mobile,
                };

                return await genericRepository_.PostAsync<UpdateWFSourcerequest, bool>(url, param);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                throw;
            }
        }
    }
}