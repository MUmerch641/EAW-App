using Acr.UserDialogs;
using EatWork.Mobile.Bootstrap;
using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using EatWork.Mobile.Models.Leave;
using EatWork.Mobile.Utils;
using EAW.API.DataContracts.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Services
{
    public class LeaveRequestDataService : ILeaveRequestDataService
    {
        private readonly IGenericRepository genericRepository_;
        private readonly ICommonDataService commonDataService_;
        private readonly IWorkflowDataService workflowDataService_;
        private readonly IDialogService dialogService_;
        private readonly StringHelper string_;
        private readonly IEmployeeProfileDataService employeeProfileDataService_;

        public LeaveRequestDataService(IGenericRepository genericRepository,
            ICommonDataService commonDataService,
            IWorkflowDataService workflowDataService,
            StringHelper url,
            IDialogService dialogService)
        {
            genericRepository_ = genericRepository;
            commonDataService_ = commonDataService;
            workflowDataService_ = workflowDataService;
            string_ = url;
            dialogService_ = dialogService;
            employeeProfileDataService_ = AppContainer.Resolve<IEmployeeProfileDataService>();
        }

        public async Task<LeaveRequestHolder> InitFormHelpers(long recordId)
        {
            var retValue = new LeaveRequestHolder()
            {
                ApplyTo = new ObservableCollection<ComboBoxObject>(),
                ApplyToSelectedItem = new ComboBoxObject(),
                LeaveType = new ObservableCollection<SelectableListModel>(),
                LeaveTypeSelectedItem = new SelectableListModel(),
                ShowPartialOptions = false,
                LeaveRequestModel = new LeaveRequestModel()
            };

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            try
            {
                #region leave types

                var leaveTypeUrl = new UriBuilder(url)
                {
                    Path = ApiConstants.GetLeaveTypes
                };

                var leaveTypeParam = new R.Requests.LeaveTypeSetupRequest()
                {
                    CompanyId = userInfo.CompanyId
                };

                var requestUrl = string_.CreateUrl<R.Requests.LeaveTypeSetupRequest>(leaveTypeUrl.ToString(), leaveTypeParam);

                var leaveTypes = await genericRepository_.GetAsync<R.Responses.ListResponse<R.Models.LeaveTypeSetup>>(requestUrl.ToString());

                if (leaveTypes.ListData.Count > 0)
                {
                    foreach (var item in leaveTypes.ListData)
                        retValue.LeaveType.Add(new SelectableListModel() { Id = item.LeaveTypeSetupId, DisplayText = item.Code });
                }

                #endregion leave types

                #region apply to

                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.LeaveRequestApplyTo}"
                };

                var applyTo = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                if (applyTo.Count > 0)
                {
                    foreach (var item in applyTo)
                        retValue.ApplyTo.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = item.DisplayText });
                }

                #endregion apply to

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetLeaveRequestDetail}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.LeaveRequestDetailResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        #region get leave type

                        leaveTypeUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetLeaveTypeById}{response.LeaveRequestHeader.LeaveTypeId}"
                        };

                        var leavetype = leaveTypes.ListData.Where(p => p.LeaveTypeSetupId == response.LeaveRequestHeader.LeaveTypeId).FirstOrDefault();

                        if (leavetype != null)
                        {
                            retValue.LeaveTypeSelectedItem = new SelectableListModel()
                            {
                                DisplayText = leavetype.Code,
                                Id = leavetype.LeaveTypeSetupId
                            };
                        }

                        /*
                        var leaveType = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.LeaveTypeSetup>>(leaveTypeUrl.ToString());

                        if (leaveType != null)
                        {
                            retValue.LeaveTypeSelectedItem = new SelectableListModel()
                            {
                                DisplayText = leaveType.Model.Code,
                                Id = leaveType.Model.LeaveTypeSetupId
                            };
                        }
                        */

                        #endregion get leave type

                        retValue.LeaveRequestModel = new LeaveRequestModel()
                        {
                            DateFiled = response.LeaveRequestHeader.CreateDate,
                            InclusiveStartDate = response.LeaveRequestHeader.InclusiveStartDate,
                            InclusiveEndDate = response.LeaveRequestHeader.InclusiveEndDate,
                            LeaveTypeId = response.LeaveRequestHeader.LeaveTypeId,
                            LeaveRequestHeaderId = response.LeaveRequestHeader.LeaveRequestHeaderId,
                            PartialDayApplyTo = response.LeaveRequestHeader.PartialDayApplyTo,
                            PartialDayLeave = response.LeaveRequestHeader.PartialDayLeave,
                            StatusId = response.LeaveRequestHeader.StatusId,
                            Reason = response.LeaveRequestHeader.Reason,
                            RemainingHours = response.LeaveRequestHeader.RemainingHours,
                            ProfileId = response.LeaveRequestHeader.ProfileId,
                        };

                        //PropertyCopier<R.Models.LeaveRequestHeader, LeaveRequestModel>.Copy(response.LeaveRequestHeader, retValue.LeaveRequestModel);

                        var detail = response.LeaveRequestHeader.LeaveRequest.FirstOrDefault();
                        if (detail != null)
                        {
                            retValue.Planned = Convert.ToBoolean(detail.Planned);
                            retValue.LeaveRequestModel.NoOfHours = detail.NoOfHours;
                        }

                        retValue.IsPartialLeave = (short)response.LeaveRequestHeader.PartialDayLeave;
                        retValue.ShowPartialOptions = Convert.ToBoolean(response.LeaveRequestHeader.PartialDayLeave);
                    }

                    //retValue.IsEnabled = (retValue.LeaveRequestModel.StatusId == RequestStatusValue.Draft);
                    retValue.IsEnabled = (retValue.LeaveRequestModel.StatusId == RequestStatusValue.Submitted ||
                                          retValue.LeaveRequestModel.StatusId == RequestStatusValue.Draft);
                    retValue.ShowCancelButton = (retValue.LeaveRequestModel.StatusId == RequestStatusValue.ForApproval);
                }
                else
                {
                    retValue.LeaveRequestModel = new LeaveRequestModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        CompanyId = userInfo.CompanyId
                    };
                    retValue.InclusiveStartDate = (FormSession.IsMySchedule ? Convert.ToDateTime(FormSession.MyScheduleSelectedDate) : DateTime.Now.Date);
                    retValue.InclusiveEndDate = (FormSession.IsMySchedule ? Convert.ToDateTime(FormSession.MyScheduleSelectedDate) : DateTime.Now.Date);
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

        public async Task<LeaveRequestHolder> SubmitRequest(LeaveRequestHolder form)
        {
            var errors = new List<int>();
            form.ErrorLeaveType = false;
            form.ErrorPartialType = false;

            if (form.LeaveTypeSelectedItem.Id == 0)
            {
                form.ErrorLeaveType = true;
                errors.Add(1);
            }

            if (form.ShowPartialOptions && form.ApplyToSelectedItem.Id == 0)
            {
                form.ErrorPartialType = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                /*if (await UserDialogs.Instance.ConfirmAsync(Messages.Submit))*/
                if (await dialogService_.ConfirmDialogAsync(Messages.Submit))
                {
                    try
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);
                            form.LeaveRequestModel.InclusiveStartDate = form.InclusiveStartDate;
                            form.LeaveRequestModel.InclusiveEndDate = form.InclusiveEndDate;
                            form.LeaveRequestModel.LeaveTypeId = form.LeaveTypeSelectedItem.Id;
                            form.LeaveRequestModel.PartialDayApplyTo = (short)form.ApplyToSelectedItem.Id;
                            form.LeaveRequestModel.Planned = (short)(form.Planned ? 1 : 0);

                            var url = await commonDataService_.RetrieveClientUrl();
                            await commonDataService_.HasInternetConnection(url);

                            var builder = new UriBuilder(url)
                            {
                                Path = ApiConstants.SubmitLeaveRequestRequest
                            };

                            var data = new R.Models.LeaveRequest();
                            PropertyCopier<LeaveRequestModel, R.Models.LeaveRequest>.Copy(form.LeaveRequestModel, data);

                            var param = new R.Requests.SubmitLeaveRequest()
                            {
                                Data = data
                            };

                            var response = await genericRepository_.PostAsync<R.Requests.SubmitLeaveRequest, R.Responses.BaseResponse<R.Models.LeaveRequest>>(builder.ToString(), param);

                            if (response.Model.LeaveRequestHeaderId > 0)
                            {
                                if (!string.IsNullOrEmpty(form.FileData.FileName))
                                    await commonDataService_.SaveSingleFileAttachmentAsync(form.FileData, ModuleForms.LeaveRequest, response.Model.LeaveRequestHeaderId.Value);

                                form.Success = true;
                                FormSession.IsSubmitted = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return form;
        }

        public async Task<LeaveApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new LeaveApprovalHolder();
            var userInfo = PreferenceHelper.UserInfo();
            try
            {
                var url = await commonDataService_.RetrieveClientUrl();
                await commonDataService_.HasInternetConnection(url);

                var leavehelperUrl = new UriBuilder(url)
                {
                    Path = string.Format(ApiConstants.GetLeaveRequestHelper, userInfo.ProfileId, userInfo.CompanyId) //profileid, companyid
                };

                var formhelper = await genericRepository_.GetAsync<R.Responses.LeaveRequestInitFormResponse>(leavehelperUrl.ToString());

                /*
                var builder = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetLeaveRequestDetail}{transactionId}"
                };
                */

                var builder2 = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetLeaveRequestRecord}{transactionId}"
                };

                var WFDetail = await workflowDataService_.GetWorkflowDetails(transactionId, transactionTypeId);
                /*var response = await genericRepository_.GetAsync<R.Responses.LeaveRequestDetailResponse>(builder.ToString());*/
                var engine = await genericRepository_.GetAsync<R.Responses.LeaveRequestDetailEngineResponse>(builder2.ToString());

                if (engine.IsSuccess)
                {
                    /*

                    #region get leave type

                    var leaveTypeUrl = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetLeaveTypeById}{response.LeaveRequestHeader.LeaveTypeId}"
                    };

                    var leaveType = await genericRepository_.GetAsync<R.Responses.BaseResponse<R.Models.LeaveTypeSetup>>(leaveTypeUrl.ToString());

                    #endregion get leave type

                    #region get enum value

                    if (response.LeaveRequestHeader.PartialDayApplyTo != 0)
                    {
                        var enumUrl = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetEnums}{EnumValues.LeaveRequestApplyTo}/{response.LeaveRequestHeader.PartialDayApplyTo}"
                        };

                        var enumval = await genericRepository_.GetAsync<R.Models.Enums>(enumUrl.ToString());

                        retValue.ApplyTo = enumval.DisplayText;
                    }

                    #endregion get enum value

                    */

                    var detail = engine.LeaveRequestHeaderModel.LeaveDetail.FirstOrDefault();

                    retValue.LeaveRequestModel = new LeaveRequestModel();

                    PropertyCopier<R.Models.LeaveRequestHeaderModel, LeaveRequestModel>.Copy(engine.LeaveRequestHeaderModel, retValue.LeaveRequestModel);
                    PropertyCopier<R.Models.LeaveRequestHeaderModel, LeaveApprovalHolder>.Copy(engine.LeaveRequestHeaderModel, retValue);

                    retValue.EmployeeName = engine.EmployeeInfo.FullNameMiddleInitialOnly;
                    retValue.EmployeeNo = engine.EmployeeInfo.EmployeeNo;
                    retValue.EmployeePosition = engine.EmployeeInfo.Position;
                    retValue.EmployeeDepartment = engine.EmployeeInfo.Department;
                    retValue.ShowPartialApplyTo = (engine.LeaveRequestHeaderModel.cmbPartialDayApplyTo == 1);
                    retValue.TransactionId = transactionId;
                    retValue.TransactionTypeId = transactionTypeId;
                    retValue.LeaveRequestModel.Reason = engine.LeaveRequestHeaderModel.txtReason;

                    retValue.LeaveRequestModel.ProfileId = engine.EmployeeInfo.ProfileId;
                    retValue.LeaveRequestModel.LeaveTypeId = engine.LeaveRequestHeaderModel.cmbLeaveTypeId;

                    retValue.DateFiled = Convert.ToDateTime(engine.LeaveRequestHeaderModel.dtpDateFiled).ToString(FormHelper.DateFormat);
                    retValue.LeaveType = engine.LeaveRequestHeaderModel.LeaveDetail.FirstOrDefault().LeaveType;
                    retValue.ApplyTo = (engine.LeaveRequestHeaderModel.cmbPartialDayApplyTo == 1 ? formhelper.ChangeApplyToLate : formhelper.ChangeApplyToUndertime);
                    retValue.LeaveType = engine.LeaveRequestHeaderModel.LeaveRequestModel.LeaveType;
                    retValue.LeaveRequestModel.StatusId = engine.LeaveRequestHeaderModel.StatusId;

                    /*retValue.RemainingBalance = string.Format("{0:0.00} hrs", engine.LeaveRequestHeaderModel.txtRemainingHours);*/

                    retValue.InclusiveDate = string.Empty.ConcatDate(Convert.ToDateTime(engine.LeaveRequestHeaderModel.dtpInclusiveStartDate),
                        Convert.ToDateTime(engine.LeaveRequestHeaderModel.dtpInclusiveEndDate));

                    if (formhelper.DisplayInDays)
                    {
                        retValue.TotalRequestedHours = string.Format("{0:0.00} day/s", engine.LeaveRequestHeaderModel.NoOfDays);
                        retValue.RemainingBalance = string.Format("{0:0.00} day/s", engine.LeaveRequestHeaderModel.LeaveRequestModel.txtRemainingDays);
                        retValue.RequestedHoursDisplayType = "Days";
                    }
                    else
                    {
                        retValue.TotalRequestedHours = string.Format("{0:0.00} hrs{1}", engine.LeaveRequestHeaderModel.TotalNoOfHours,
                            (engine.LeaveRequestHeaderModel.LeaveDetail.Count > 1 ? string.Format(" ({0:0.00} hrs per day)", detail.txtNoOfHours) : ""));

                        retValue.RemainingBalance = string.Format("{0:0.00} hrs", engine.LeaveRequestHeaderModel.LeaveRequestModel.txtRemainingHours);

                        retValue.RequestedHoursDisplayType = "Hours";
                    }

                    retValue.IsPartialDay = (engine.LeaveRequestHeaderModel.LeaveRequestModel.optPartialDayLeave_String);

                    retValue.StageId = WFDetail.WorkflowDetail.CurrentStageId;

                    retValue.ShowPartialApplyTo = Convert.ToBoolean(engine.LeaveRequestHeaderModel.optPartialDayLeave);

                    if (engine.LeaveRequestDetailList.Count > 0)
                    {
                        var totalHours = engine.LeaveRequestDetailList.Sum(p => p.txtNoOfHours);

                        foreach (var item in engine.LeaveRequestDetailList)
                        {
                            retValue.LeaveRequestDetailListToDisplay.Add(new LeaveRequestDetailModel()
                            {
                                LeaveDate = item.LeaveDate,
                                DayOfWeek = item.DayOfWeek,
                                txtNoOfHours = item.txtNoOfHours,
                                RequestedHours = (formhelper.DisplayInDays ? item.txtNoOfHours.ToString() : string.Format("{0:0.00}", item.txtNoOfHours)),
                                DisplayType = (formhelper.DisplayInDays ? "day" : "hrs"),
                            });
                        }

                        retValue.LeaveRequestDetailListToDisplay.Add(new LeaveRequestDetailModel()
                        {
                            LeaveDate = string.Empty,
                            DayOfWeek = "TOTAL:",
                            txtNoOfHours = (formhelper.DisplayInDays ? engine.LeaveRequestDetailList.Count : engine.LeaveRequestDetailList.Sum(p => p.txtNoOfHours)),
                            RequestedHours = (formhelper.DisplayInDays ? totalHours.ToString() : string.Format("{0:0.00}", totalHours)),
                            DisplayType = (formhelper.DisplayInDays ? "day/s" : "hrs"),
                            FontAttribute = FontAttributes.Bold,
                        });
                    }

                    retValue.LeaveDocumentModel = new ObservableCollection<LeaveRequestDocumentModel>();
                    foreach (var item in engine.LeaveRequestHeaderModel.LeaveDocumentModel)
                    {
                        var model = new LeaveRequestDocumentModel();
                        PropertyCopier<R.Models.LeaveRequestDocumentModel, LeaveRequestDocumentModel>.Copy(item, model);
                        model.DocumentName = string.Empty ?? "-";
                        retValue.LeaveDocumentModel.Add(model);
                    }

                    foreach (var item in WFDetail.WorkflowDetail.WorkflowActions)
                    {
                        var model = new WorkflowAction();
                        PropertyCopier<R.Models.WorkflowAction, WorkflowAction>.Copy(item, model);
                        retValue.WorkflowActions.Add(model);
                    }

                    if (retValue.LeaveRequestModel.StatusId == RequestStatusValue.Approved && WFDetail.WorkflowDetail.IsFinalApprover)
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

                    retValue.ImageSource = await employeeProfileDataService_.GetProfileImage(engine.EmployeeInfo.ProfileId);
                }
                else
                    throw new Exception(engine.ErrorMessage);

                retValue.IsSuccess = engine.IsSuccess;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;
        }

        public async Task<ObservableCollection<LeaveUsageListHolder>> InitLeaveUsage(long profileId, long leaveTypeId)
        {
            var retValue = new ObservableCollection<LeaveUsageListHolder>();

            try
            {
                var url = await commonDataService_.RetrieveClientUrl();

                await commonDataService_.HasInternetConnection(url);

                var builder = new UriBuilder(url)
                {
                    //Path = $"{ApiConstants.GetLeaveUsage}{profileId}/{leaveTypeId}/{ApiConstants.LeaveUsageConstant}"
                    Path = string.Format(ApiConstants.GetLeaveUsage, profileId, leaveTypeId)
                };

                var response = await genericRepository_.GetAsync<R.Responses.LeaveUsageResponse>(builder.ToString());

                if (response.LeaveUsageList.Count > 0)
                {
                    Parallel.ForEach(response.LeaveUsageList, item =>
                    {
                        retValue.Add(new LeaveUsageListHolder()
                        {
                            LeaveTypeSetup = item.LeaveTypeSetup,
                            DateFiled = item.DateFiled,
                            InclusiveDate = item.InclusiveDate,
                            LeaveRequestDisplay = item.LeaveRequestDisplay,
                            Reason = item.Reason,
                            Status = item.Status,
                            NoOfHours = item.NoOfHours,
                            DisplayInDays = item.DisplayInDays,
                            NoOfHoursPerDay = item.NoOfHoursPerDay
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            return retValue;
        }

        public async Task<LeaveApprovalHolder> WorkflowTransaction(LeaveApprovalHolder form)
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

                        var response = new R.Responses.WFTransactionResponse();

                        if (form.LeaveRequestModel.StatusId == RequestStatusValue.Approved && form.SelectedWorkflowAction.ActionTriggeredId == ActionTypeId.Cancel)
                        {
                            response = await workflowDataService_.CancelWorkFlowByRecordId(form.TransactionId,
                                form.TransactionTypeId,
                                form.SelectedWorkflowAction.ActionTriggeredId,
                                confirm.ResponseText,
                                form.StageId);
                        }
                        else
                        {
                            response = await workflowDataService_.ProcessWorkflowByRecordId(form.TransactionId,
                                form.TransactionTypeId,
                                form.SelectedWorkflowAction.ActionTriggeredId,
                                confirm.ResponseText,
                                form.StageId);
                        }

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(url)
                            {
                                Path = $"{ApiConstants.LeaveApi}/update-wfsource"
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

        public async Task<LeaveRequestHolder> WorkflowTransactionRequest(LeaveRequestHolder form)
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
                        var WFDetail = await workflowDataService_.GetWorkflowDetails(Convert.ToInt64(form.LeaveRequestHeaderModel.LeaveRequestHeaderId), TransactionType.Leave);
                        var response = await workflowDataService_.CancelWorkFlowByRecordId(Convert.ToInt64(form.LeaveRequestHeaderModel.LeaveRequestHeaderId),
                                             TransactionType.Leave,
                                             ActionTypeId.Cancel,
                                             confirm.ResponseText,
                                             WFDetail.WorkflowDetail.CurrentStageId, "", false);

                        if (!string.IsNullOrWhiteSpace(response.ValidationMessage) || !string.IsNullOrWhiteSpace(response.ErrorMessage))
                            throw new Exception((!string.IsNullOrWhiteSpace(response.ValidationMessage) ? response.ValidationMessage : response.ErrorMessage));
                        else
                        {
                            var builder = new UriBuilder(await commonDataService_.RetrieveClientUrl())
                            {
                                Path = $"{ApiConstants.LeaveApi}/cancel-request"
                            };

                            var param = new UpdateWFSourcerequest()
                            {
                                RecordId = form.LeaveRequestHeaderModel.LeaveRequestHeaderId,
                                SourceId = (short)SourceEnum.Mobile,
                            };

                            var updated = await genericRepository_.PostAsync<R.Requests.UpdateWFSourcerequest, bool>(builder.ToString(), param);

                            FormSession.IsSubmitted = updated;
                            form.Success = updated;
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

        public async Task<LeaveRequestHolder> InitLeaveRequestForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new LeaveRequestHolder();

            var userInfo = PreferenceHelper.UserInfo();
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            try
            {
                #region leavetypes

                var leavehelperUrl = new UriBuilder(url)
                {
                    Path = string.Format(ApiConstants.GetLeaveRequestHelper, userInfo.ProfileId, userInfo.CompanyId) //profileid, companyid
                };

                var formhelper = await genericRepository_.GetAsync<R.Responses.LeaveRequestInitFormResponse>(leavehelperUrl.ToString());

                if (formhelper.LeaveTypeList.Count > 0)
                {
                    foreach (var item in formhelper.LeaveTypeList)
                        retValue.LeaveType.Add(new SelectableListModel() { Id = item.DisplayId, DisplayText = item.DisplayField, DisplayData = item.DisplayData });
                }

                if (formhelper.LeaveDocumentDataList.Count > 0)
                {
                    foreach (var item in formhelper.LeaveDocumentDataList)
                        retValue.LeaveTypeDocumentHolder.Add(new SelectableListModel() { Id = item.DisplayId, DisplayText = item.DisplayField, DisplayData = item.DisplayData });
                }

                #endregion leavetypes

                #region apply to

                var enumUrl = new UriBuilder(url)
                {
                    Path = $"{ApiConstants.GetEnums}{EnumValues.LeaveRequestApplyTo}"
                };

                var applyTo = await genericRepository_.GetAsync<List<R.Models.Enums>>(enumUrl.ToString());

                if (applyTo.Count > 0)
                {
                    foreach (var item in applyTo)
                    {
                        var val = (Convert.ToInt64(item.Value) == 1 ? formhelper.ChangeApplyToLate : formhelper.ChangeApplyToUndertime);
                        retValue.ApplyTo.Add(new ComboBoxObject() { Id = Convert.ToInt64(item.Value), Value = val });
                    }
                }

                /*AUTOMATED HALFDAY*/
                var applyToLate = string.IsNullOrWhiteSpace(formhelper.ChangeApplyToLate) ? "1st Half" : formhelper.ChangeApplyToLate;
                var applyToUndertime = string.IsNullOrWhiteSpace(formhelper.ChangeApplyToUndertime) ? "2nd Half" : formhelper.ChangeApplyToUndertime;
                retValue.ApplyToAutomated.Add(new Syncfusion.XForms.Buttons.SfSegmentItem() { Text = applyToLate });
                retValue.ApplyToAutomated.Add(new Syncfusion.XForms.Buttons.SfSegmentItem() { Text = applyToUndertime });

                #endregion apply to

                retValue.DisplayInDays = formhelper.DisplayInDays;

                retValue.AutomateHalfDayLeave = formhelper.AutomateHalfDayLeave;

                if (formhelper.DisplayInDays)
                {
                    retValue.BalanceLabelDisplay = "days";
                    retValue.RequestLabelDisplay = "No. of hours per day  ";
                }

                if (recordId > 0)
                {
                    var builder = new UriBuilder(url)
                    {
                        Path = $"{ApiConstants.GetLeaveRequestRecord}{recordId}"
                    };

                    var response = await genericRepository_.GetAsync<R.Responses.LeaveRequestDetailEngineResponse>(builder.ToString());

                    if (response.IsSuccess)
                    {
                        retValue.LeaveRequestModel = new LeaveRequestModel();

                        retValue.LeaveRequestDetailList = new ObservableCollection<LeaveRequestDetailModel>();
                        foreach (var item in response.LeaveRequestDetailList)
                        {
                            var model = new LeaveRequestDetailModel();
                            PropertyCopier<R.Models.LeaveRequestDetailModel, LeaveRequestDetailModel>.Copy(item, model);
                            model.LeaveDateAndDayOfWeek = string.Format("{0} - {1}", model.LeaveDate, model.DayOfWeek);
                            model.SelectedLeaveType = retValue.LeaveTypeSelectedItem.DisplayText;
                            retValue.LeaveRequestDetailList.Add(model);
                        }

                        retValue.LeaveDocumentModel = new ObservableCollection<LeaveRequestDocumentModel>();
                        foreach (var item in response.LeaveRequestHeaderModel.LeaveDocumentModel)
                        {
                            var model = new LeaveRequestDocumentModel();
                            PropertyCopier<R.Models.LeaveRequestDocumentModel, LeaveRequestDocumentModel>.Copy(item, model);
                            model.ShowCommand = false;
                            retValue.LeaveDocumentModel.Add(model);
                        }

                        foreach (var item in response.LeaveRequestHeaderModel.LeaveDetail)
                        {
                            var entity = new LeaveRequestEngineModel();
                            PropertyCopier<R.Models.LeaveRequestModel, LeaveRequestEngineModel>.Copy(item, entity);
                            retValue.LeaveRequestHeaderModel.LeaveDetail.Add(entity);
                        }

                        retValue.InclusiveStartDate = response.LeaveRequestHeaderModel.dtpInclusiveStartDate;
                        retValue.InclusiveEndDate = response.LeaveRequestHeaderModel.dtpInclusiveEndDate;

                        if (retValue.DisplayInDays)
                            retValue.RemainingBalance = response.LeaveRequestHeaderModel.LeaveRequestModel.txtRemainingDays;
                        else
                            retValue.RemainingBalance = response.LeaveRequestHeaderModel.LeaveRequestModel.txtRemainingHours;

                        retValue.LeaveRequestModel.RemainingHours = response.LeaveRequestHeaderModel.txtRemainingHours;
                        retValue.LeaveRequestModel.PartialDayLeave = response.LeaveRequestHeaderModel.optPartialDayLeave;
                        retValue.LeaveRequestModel.PartialDayApplyTo = response.LeaveRequestHeaderModel.cmbPartialDayApplyTo;
                        retValue.LeaveRequestModel.Reason = response.LeaveRequestHeaderModel.txtReason;
                        retValue.LeaveRequestModel.NoOfHours = response.LeaveRequestHeaderModel.LeaveRequestModel.txtNoOfHours;
                        retValue.LeaveTypeSelectedItem = retValue.LeaveType.Where(p => p.Id == response.LeaveRequestHeaderModel.cmbLeaveTypeId).FirstOrDefault();
                        retValue.ApplyToSelectedItem = retValue.ApplyTo.Where(p => p.Id == response.LeaveRequestHeaderModel.cmbPartialDayApplyTo).FirstOrDefault();
                        retValue.IsPartialLeave = response.LeaveRequestHeaderModel.optPartialDayLeave;

                        retValue.IsGenerated = true;
                        retValue.ShowDocumentList = (retValue.LeaveDocumentModel.Count > 0);

                        retValue.ShowPartialOptions = Convert.ToBoolean(retValue.LeaveRequestModel.PartialDayLeave);

                        retValue.LeaveRequestHeaderModel = new LeaveRequestHeaderModel();
                        PropertyCopier<R.Models.LeaveRequestHeaderModel, LeaveRequestHeaderModel>.Copy(response.LeaveRequestHeaderModel, retValue.LeaveRequestHeaderModel);

                        if (retValue.LeaveRequestDetailList.Count > 0)
                        {
                            var totalHours = retValue.LeaveRequestDetailList.Sum(p => p.txtNoOfHours);

                            retValue.LeaveRequestDetailListToDisplay = new ObservableCollection<LeaveRequestDetailModel>(
                                retValue.LeaveRequestDetailList.Select(x => new LeaveRequestDetailModel()
                                {
                                    LeaveDate = x.LeaveDate,
                                    DayOfWeek = x.DayOfWeek,
                                    txtNoOfHours = x.txtNoOfHours,
                                    RequestedHours = (retValue.DisplayInDays ? x.txtNoOfHours.ToString() : string.Format("{0:0.00}", x.txtNoOfHours)),
                                    DisplayType = (retValue.DisplayInDays ? "day" : "hrs"),
                                })
                            );

                            retValue.LeaveRequestDetailListToDisplay.Add(new LeaveRequestDetailModel()
                            {
                                LeaveDate = string.Empty,
                                DayOfWeek = "TOTAL:",
                                txtNoOfHours = retValue.LeaveRequestDetailList.Sum(p => p.txtNoOfHours),
                                RequestedHours = (retValue.DisplayInDays ? totalHours.ToString() : string.Format("{0:0.00}", totalHours)),
                                DisplayType = (retValue.DisplayInDays ? "day/s" : "hr/s"),
                                FontAttribute = FontAttributes.Bold,
                            });
                        }

                        PropertyCopier<R.Models.LeaveCompanyConfiguration, LeaveCompanyConfiguration>.Copy(response.LeaveRequestHeaderModel.config, retValue.LeaveRequestHeaderModel.config);
                        PropertyCopier<R.Models.LeaveRequestModel, LeaveRequestEngineModel>.Copy(response.LeaveRequestHeaderModel.LeaveRequestModel, retValue.LeaveRequestEngineModel);

                        if (retValue.AutomateHalfDayLeave && Convert.ToBoolean(response.LeaveRequestHeaderModel.optPartialDayLeave))
                        {
                            if (response.LeaveRequestHeaderModel.cmbPartialDayApplyTo == 1)
                                retValue.ApplyToOption = 0;
                            else
                                retValue.ApplyToOption = 1;
                        }

                        retValue.IsEnabled = (response.LeaveRequestHeaderModel.StatusId == RequestStatusValue.Submitted ||
                          response.LeaveRequestHeaderModel.StatusId == RequestStatusValue.Draft);

                        retValue.ShowCancelButton = (response.LeaveRequestHeaderModel.StatusId == RequestStatusValue.ForApproval);
                    }
                }
                else
                {
                    retValue.LeaveRequestModel = new LeaveRequestModel()
                    {
                        ProfileId = userInfo.ProfileId,
                        CompanyId = userInfo.CompanyId,
                    };

                    retValue.InclusiveStartDate = selectedDate ?? DateTime.Now.Date;
                    retValue.InclusiveEndDate = selectedDate ?? DateTime.Now.Date;

                    retValue.IsEnabled = true;

                    //if automated halfday leave
                    if (retValue.AutomateHalfDayLeave)
                        retValue.ApplyToSelectedItem = retValue.ApplyTo.FirstOrDefault();
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

        public async Task<LeaveRequestHolder> GetLeaveBalance(LeaveRequestHolder form)
        {
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            if (form.LeaveTypeSelectedItem.Id > 0)
            {
                using (UserDialogs.Instance.Loading())
                {
                    try
                    {
                        await Task.Delay(500);
                        var builder = new UriBuilder(url)
                        {
                            Path = $"{ApiConstants.GetLeaveRequestBalance}{form.LeaveRequestModel.ProfileId}/{form.LeaveTypeSelectedItem.Id}/{DateTime.Now:yyyy-MM-dd}"
                        };

                        var response = await genericRepository_.GetAsync<R.Responses.LeaveRequestBalanceResponse>(builder.ToString());

                        if (response.IsSuccess)
                        {
                            form.LeaveRequestModel.RemainingHours = Convert.ToDecimal(response.BalanceHours);

                            if (form.DisplayInDays)
                                form.RemainingBalance = Convert.ToDecimal(response.BalanceDays);
                            else
                                form.RemainingBalance = Convert.ToDecimal(response.BalanceHours);
                        }

                        form.LeaveRequestModel.Reason = string.Empty;

                        if (form.LeaveTypeSelectedItem.DisplayData == "true")
                        {
                            form.LeaveRequestModel.Reason = form.LeaveTypeSelectedItem.DisplayText;
                        }

                        foreach (var item in form.LeaveTypeDocumentHolder.Where(p => p.DisplayData == form.LeaveTypeSelectedItem.Id.ToString()))
                        {
                            form.LeaveTypeDocument.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return form;
        }

        public async Task<LeaveRequestHolder> ValidateLeaveRequestGeneration(LeaveRequestHolder form)
        {
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);
            var userInfo = PreferenceHelper.UserInfo();
            var errors = new List<int>();
            form.ErrorLeaveType = false;
            form.ErrorPartialType = false;
            form.LeaveRequestDetailList = new ObservableCollection<LeaveRequestDetailModel>();
            form.HasWarning = false;

            if (form.LeaveTypeSelectedItem.Id == 0)
            {
                form.ErrorLeaveType = true;
                errors.Add(1);
            }

            if (form.ShowPartialOptions && form.ApplyToSelectedItem.Id == 0)
            {
                form.ErrorPartialType = true;
                errors.Add(1);
            }

            if (errors.Count == 0)
            {
                try
                {
                    var request = new UriBuilder(url)
                    {
                        Path = ApiConstants.GetValidateLeaveGeneration
                    };

                    form.InclusiveStartDate = Convert.ToDateTime(form.InclusiveStartDate.ToString("MM/dd/yyyy"));
                    form.InclusiveEndDate = Convert.ToDateTime(form.InclusiveEndDate.ToString("MM/dd/yyyy"));

                    var param = new R.Requests.GenerateLeaveRequest()
                    {
                        AutomateHalfDayLeave = form.AutomateHalfDayLeave,
                        LeaveRequestModel = new R.Models.LeaveRequestModel()
                        {
                            //AvailableHours = form.LeaveRequestModel.RemainingHours.Value,//double check
                            chkPlanned = form.LeaveRequestModel.Planned.Value,
                            cmbLeaveTypeId = form.LeaveTypeSelectedItem.Id,
                            cmbPartialDayApplyTo = (short)form.ApplyToSelectedItem.Id,
                            CompanyId = userInfo.CompanyId,
                            DateFiled = form.LeaveRequestModel.DateFiled.Value,
                            dtpBirthDate = userInfo.BirthDate,
                            dtpDateFiled = form.LeaveRequestModel.DateFiled.Value,
                            dtpInclusiveStartDate = form.InclusiveStartDate,
                            IsImport = false,
                            LeaveRequestHeaderId = form.LeaveRequestModel.LeaveRequestHeaderId.Value,
                            LeaveRequestId = form.LeaveRequestModel.LeaveRequestId,
                            LeaveRequestIds = string.Empty,
                            optPartialDayLeave = form.IsPartialLeave,
                            ProfileId = form.LeaveRequestModel.ProfileId.Value,
                            StatusId = form.LeaveRequestModel.StatusId.Value,
                            txtNoOfHours = form.LeaveRequestModel.NoOfHours.Value,
                            txtReason = form.LeaveRequestModel.Reason,
                            txtRemainingHours = form.LeaveRequestModel.RemainingHours.Value,
                            DisplayInDays = false,
                            dtpInclusiveEndDate = form.InclusiveEndDate,
                        }
                    };

                    //var request = string_.CreateUrl<R.Requests.GenerateLeaveRequest>(requestUrl.ToString(), param);

                    var response = new R.Responses.GeneratLeaveRequestResponse();

                    using (UserDialogs.Instance.Loading("Validating request..."))
                    {
                        await Task.Delay(500);
                        response = await genericRepository_.PostAsync<R.Requests.GenerateLeaveRequest, R.Responses.GeneratLeaveRequestResponse>(request.ToString(), param);
                    }

                    if (response.HasError)
                    {
                        var list = new ObservableCollection<string>(response.ErrorMessages);
                        var page = new EatWork.Mobile.Views.Shared.ErrorPage(list, Messages.ValidationHeaderMessage);
                        await Application.Current.MainPage.Navigation.PushModalAsync(page);
                    }
                    else if (response.HasConflict)
                    {
                        var list = new ObservableCollection<string>(response.ConflictDateList);
                        var page = new EatWork.Mobile.Views.Shared.ErrorPage(list, response.ConflictMessage);
                        await Application.Current.MainPage.Navigation.PushModalAsync(page);
                    }
                    else
                    {
                        form.LeaveRequestDetailList = new ObservableCollection<LeaveRequestDetailModel>();

                        foreach (var item in response.LeaveRequestDetailList)
                        {
                            var model = new LeaveRequestDetailModel();
                            PropertyCopier<R.Models.LeaveRequestDetailModel, LeaveRequestDetailModel>.Copy(item, model);
                            model.LeaveDateAndDayOfWeek = string.Format("{0} - {1}", model.LeaveDate, model.DayOfWeek);
                            model.SelectedLeaveType = form.LeaveTypeSelectedItem.DisplayText;
                            model.DisplayType = (form.DisplayInDays ? "day" : "hrs");
                            model.FontAttribute = FontAttributes.None;
                            model.NoOfDaysDecimal = item.NoOfDaysDecimal;
                            form.LeaveRequestDetailList.Add(model);
                        }

                        form.LeaveRequestHeaderModel = new LeaveRequestHeaderModel();
                        PropertyCopier<R.Models.LeaveRequestHeaderModel, LeaveRequestHeaderModel>.Copy(response.LeaveRequestHeaderModel, form.LeaveRequestHeaderModel);

                        foreach (var item in response.LeaveRequestHeaderModel.WorkScheduleModelList)
                        {
                            var entity = new EmployeeWorkScheduleModel();
                            PropertyCopier<R.Models.EmployeeWorkScheduleModel, EmployeeWorkScheduleModel>.Copy(item, entity);
                            form.LeaveRequestHeaderModel.WorkScheduleModelList.Add(entity);
                        }

                        foreach (var item in response.LeaveRequestHeaderModel.LeaveDetail)
                        {
                            var entity = new LeaveRequestEngineModel();
                            PropertyCopier<R.Models.LeaveRequestModel, LeaveRequestEngineModel>.Copy(item, entity);
                            entity.NoOfDaysDecimal = entity.NoOfDaysDecimal;
                            form.LeaveRequestHeaderModel.LeaveDetail.Add(entity);
                        }

                        /*
                        foreach (var item in response.LeaveRequestHeaderModel.LeaveDocumentModel)
                        {
                            var entity = new LeaveRequestDocumentModel();
                            PropertyCopier<R.Models.LeaveRequestDocumentModel, LeaveRequestDocumentModel>.Copy(item, entity);
                            headermodel.LeaveDocumentModel.Add(entity);
                        }
                        */

                        PropertyCopier<R.Models.LeaveCompanyConfiguration, LeaveCompanyConfiguration>.Copy(response.LeaveRequestHeaderModel.config, form.LeaveRequestHeaderModel.config);
                        //PropertyCopier<R.Models.LeaveRequestModel, LeaveRequestEngineModel>.Copy(response.LeaveRequestHeaderModel.LeaveRequestModel, form.LeaveRequestHeaderModel.LeaveRequestModel);
                        PropertyCopier<R.Models.LeaveRequestModel, LeaveRequestEngineModel>.Copy(response.LeaveRequestModel, form.LeaveRequestEngineModel);

                        /*
                        form.LeaveRequestDetailListToDisplay = new ObservableCollection<LeaveRequestDetailModel>(form.LeaveRequestDetailList)
                        {
                            new LeaveRequestDetailModel()
                            {
                                LeaveDate = string.Empty,
                                DayOfWeek = "TOTAL:",
                                txtNoOfHours = form.LeaveRequestDetailList.Count,
                                DisplayType = "days",
                                FontAttribute = FontAttributes.Bold,
                            }
                        };
                        */
                        form.LeaveRequestDetailListToDisplay = new ObservableCollection<LeaveRequestDetailModel>();
                        if (form.LeaveRequestDetailList.Count > 0)
                        {
                            foreach (var item in form.LeaveRequestDetailList)
                            {
                                form.LeaveRequestDetailListToDisplay.Add(new LeaveRequestDetailModel()
                                {
                                    LeaveDate = item.LeaveDate,
                                    DayOfWeek = item.DayOfWeek,
                                    txtNoOfHours = item.txtNoOfHours,
                                    NoOfDaysDecimal = item.NoOfDaysDecimal,
                                    RequestedHours = (form.DisplayInDays ? item.txtNoOfHours.ToString() : string.Format("{0:0.00}", item.txtNoOfHours)),
                                    DisplayType = (form.DisplayInDays ? "day" : "hrs"),
                                });
                            }

                            form.LeaveRequestDetailListToDisplay.Add(new LeaveRequestDetailModel()
                            {
                                LeaveDate = string.Empty,
                                DayOfWeek = "TOTAL:",
                                txtNoOfHours = form.LeaveRequestDetailList.Count,
                                DisplayType = (form.DisplayInDays ? "day/s" : "hrs"),
                                FontAttribute = FontAttributes.Bold,
                                /*RequestedHours = (form.DisplayInDays ? form.LeaveRequestDetailList.Count.ToString() : string.Format("{0:0.00}", form.LeaveRequestDetailList.Sum(p => p.txtNoOfHours))),*/
                                RequestedHours = (form.DisplayInDays ? form.LeaveRequestHeaderModel.NoOfDays.ToString("0.##") : string.Format("{0:0.00}", form.LeaveRequestHeaderModel.TotalNoOfHours)),
                            });
                        }

                        form.ShowDocumentList = (form.LeaveDocumentModel.Count > 0);

                        if (response.WarningOnly)
                        {
                            form.HasWarning = response.WarningOnly;
                            form.ConflictList = response.ConflictDateList;
                            form.ConflictMessage = response.ConflictMessage;
                            /*
                            form.HasWarning = response.WarningOnly;

                            var list = new ObservableCollection<string>(response.ConflictDateList);
                            var page = new EatWork.Mobile.Views.Shared.WarningPage(list, response.ConflictMessage);
                            await Application.Current.MainPage.Navigation.PushModalAsync(page);
                            */
                        }
                        else
                        {
                            form.IsGenerated = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return form;
        }

        public async Task<LeaveRequestHolder> SubmitRequestEngine(LeaveRequestHolder form)
        {
            var url = await commonDataService_.RetrieveClientUrl();
            await commonDataService_.HasInternetConnection(url);

            var errors = new List<int>();
            form.ErrorLeaveType = false;
            form.ErrorPartialType = false;
            form.Success = false;

            if (form.LeaveTypeSelectedItem.Id == 0)
            {
                form.ErrorLeaveType = true;
                errors.Add(1);
            }

            if (form.ShowPartialOptions && form.ApplyToSelectedItem.Id == 0)
            {
                form.ErrorPartialType = true;
                errors.Add(1);
            }

            if (form.LeaveRequestDetailList.Count == 0)
            {
                errors.Add(1);
                await dialogService_.AlertAsync("No leave details");
            }

            if (errors.Count == 0)
            {
                try
                {
                    var request = new UriBuilder(url)
                    {
                        Path = ApiConstants.SubmitLeaveRequestRequestEngine
                    };

                    var header = new R.Models.LeaveRequestHeaderModel();
                    PropertyCopier<LeaveRequestHeaderModel, R.Models.LeaveRequestHeaderModel>.Copy(form.LeaveRequestHeaderModel, header);

                    var detail = new R.Models.LeaveRequestModel();
                    PropertyCopier<LeaveRequestEngineModel, R.Models.LeaveRequestModel>.Copy(form.LeaveRequestEngineModel, detail);

                    foreach (var item in form.LeaveRequestHeaderModel.WorkScheduleModelList)
                    {
                        var entity = new R.Models.EmployeeWorkScheduleModel();
                        PropertyCopier<EmployeeWorkScheduleModel, R.Models.EmployeeWorkScheduleModel>.Copy(item, entity);
                        header.WorkScheduleModelList.Add(entity);
                    }

                    foreach (var item in form.LeaveRequestHeaderModel.LeaveDetail)
                    {
                        var entity = new R.Models.LeaveRequestModel();
                        PropertyCopier<LeaveRequestEngineModel, R.Models.LeaveRequestModel>.Copy(item, entity);

                        entity.NoOfDaysDecimal = item.NoOfDaysDecimal;
                        entity.NoOfDays = item.NoOfDays;

                        if (form.AutomateHalfDayLeave)
                        {
                            if (form.ApplyToOption == 0)
                                entity.cmbPartialDayApplyTo = 1;
                            else
                                entity.cmbPartialDayApplyTo = 2;
                        }

                        header.LeaveDetail.Add(entity);
                    }

                    if (form.AutomateHalfDayLeave)
                    {
                        if (form.ApplyToOption == 0)
                            header.cmbPartialDayApplyTo = 1;
                        else
                            header.cmbPartialDayApplyTo = 2;
                    }

                    foreach (var item in form.LeaveDocumentModel)
                    {
                        var entity = new R.Models.LeaveRequestDocumentModel();
                        PropertyCopier<LeaveRequestDocumentModel, R.Models.LeaveRequestDocumentModel>.Copy(item, entity);
                        header.LeaveDocumentModel.Add(entity);
                    }

                    //PropertyCopier<LeaveCompanyConfiguration, R.Models.LeaveCompanyConfiguration>.Copy(form.LeaveRequestHeaderModel.config, header.config);
                    //PropertyCopier<LeaveRequestModel, R.Models.LeaveRequestModel>.Copy(form.LeaveRequestHeaderModel.LeaveRequestModel, header.LeaveRequestModel);
                    PropertyCopier<LeaveRequestEngineModel, R.Models.LeaveRequestModel>.Copy(form.LeaveRequestHeaderModel.LeaveRequestModel, header.LeaveRequestModel);

                    var param = new R.Requests.SubmitLeaveEngineRequest()
                    {
                        LeaveRequestHeaderModel = header,
                        LeaveRequestModel = detail
                    };

                    var response = new R.Responses.GeneratLeaveRequestResponse();
                    var isSubmit = form.IsMultipleLeave;

                    if (!form.IsMultipleLeave)
                    {
                        isSubmit = await dialogService_.ConfirmDialogAsync(Messages.Submit);
                    }

                    if (isSubmit)
                    {
                        using (UserDialogs.Instance.Loading())
                        {
                            await Task.Delay(500);
                            response = await genericRepository_.PostAsync<R.Requests.SubmitLeaveEngineRequest, R.Responses.GeneratLeaveRequestResponse>(request.ToString(), param);
                        }

                        if (response.HasError)
                        {
                            var list = new ObservableCollection<string>(response.ErrorMessages);
                            var page = new EatWork.Mobile.Views.Shared.ErrorPage(list);
                            await Application.Current.MainPage.Navigation.PushModalAsync(page);
                        }
                        else
                        {
                            form.Success = true;
                            FormSession.IsSubmitted = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    form.IsContinue = false;
                }
            }

            return form;
        }

        public async Task<LeaveRequestHolder> GenerateAndSubmitRequest(LeaveRequestHolder form)
        {
            var totalDays = (form.InclusiveEndDate - form.InclusiveStartDate).Days;
            form.IsMultipleLeave = false;
            form = await ValidateLeaveRequestGeneration(form);

            if (form.LeaveRequestDetailList.Count > 0 && !form.HasWarning)
            {
                if (totalDays > 0)
                {
                    form.IsMultipleLeave = true;
                    return form;
                }
                else
                {
                    form = await SubmitRequestEngine(form);
                }
            }

            /*
            MessagingCenter.Unsubscribe<WarningPage>(this, "WarningPageClosed");
            MessagingCenter.Subscribe<WarningPage>(this, "WarningPageClosed", async (sender) =>
            {
                if (form.LeaveRequestDetailList.Count > 0)
                {
                    form = await SubmitRequestEngine(form);
                }
            });

            */

            return form;
        }
    }
}