using EatWork.Mobile.Contants;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Models;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Models.FormHolder.Approvals;
using EatWork.Mobile.Models.FormHolder.Request;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    public class DocumentRequestDataService : IDocumentRequestDataService
    {
        public async Task<ObservableCollection<ComboBoxObject>> RetrieveDocumentRequestList(int listcount, int count, ObservableCollection<ComboBoxObject> list)
        {
            var retValue = list;

            await Task.Run(() =>
            {
                var temp = new ObservableCollection<ComboBoxObject>
                {
                    new ComboBoxObject { Id = 1,  Value="AWOL"},
                    new ComboBoxObject { Id = 2,  Value="Certificate of Employment" },
                    new ComboBoxObject { Id = 3,  Value="Certificate of Employment and Compensation" },
                    new ComboBoxObject { Id = 4,  Value="Certificate of Employment and Compensation for TAP" },
                    new ComboBoxObject { Id = 5,  Value="Certificate of Employment for RJG"},
                    new ComboBoxObject { Id = 6,  Value="Certificate of Car Loan Deduction"  },
                    new ComboBoxObject { Id = 7,  Value="AWOL" },
                    new ComboBoxObject { Id = 8,  Value="Certificate of Employment"},
                    new ComboBoxObject { Id = 9,  Value="Certificate of Employment and Compensation" },
                    new ComboBoxObject { Id = 10, Value="Certificate of Employment and Compensation for TAP"},
                    new ComboBoxObject { Id = 11, Value="Certificate of Employment for RJG" },
                    new ComboBoxObject { Id = 12, Value="Certificate of Car Loan Deduction" },
                };

                count = (temp.Count <= count ? temp.Count : count);

                if (temp.Count > 0)
                {
                    try
                    {
                        for (int i = listcount; i < listcount + count; i++)
                        {
                            if (temp.ElementAtOrDefault(i) != null)
                            {
                                var model = new ComboBoxObject()
                                {
                                    Id = temp[i].Id,
                                    Value = temp[i].Value
                                };

                                retValue.Add(model);
                            }
                            else
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        //throw new Exception(ex.Message);
                    }
                }
            });

            return retValue;
        }

        public async Task<DocumentRequestHolder> SubmitRequest(DocumentRequestHolder form)
        {
            var retvalue = form;
            retvalue.ErrorDetails = false;
            retvalue.ErrorDocumentType = false;
            retvalue.ErrorReason = false;

            if (retvalue.DocumentType.Id == 0)
                retvalue.ErrorDocumentType = true;

            if (string.IsNullOrWhiteSpace(retvalue.DocumentRequestModel.Reason))
                retvalue.ErrorReason = true;

            if (string.IsNullOrWhiteSpace(retvalue.DocumentRequestModel.Details))
                retvalue.ErrorDetails = true;

            if (!retvalue.ErrorDocumentType && !retvalue.ErrorDetails && !retvalue.ErrorReason)
                retvalue.Success = true;

            return retvalue;
        }

        public async Task<DocumentRequestApprovalHolder> InitApprovalForm(long transactionTypeId, long transactionId)
        {
            var retValue = new DocumentRequestApprovalHolder()
            {
                EmployeeName = "Him, Johnny A.",
                EmployeeNo = "654645822",
                EmployeePosition = "HR Associate",
                EmployeeDepartment = "Development Group",
                TransactionId = transactionId,
                TransactionTypeId = transactionTypeId,
                DocumentRequestModel = new DocumentRequestModel()
                {
                    Reason = "Personal remarks",
                    Remark = "Remarks for Document Request. Just text only.",
                    DateRequested = Convert.ToDateTime("09/25/2019"),
                    Details = "Details for Document Request.",
                    DateStart = Convert.ToDateTime("01/01/2019"),
                    DateEnd = Convert.ToDateTime("09/30/2019"),
                }
            };

            retValue.DateFiled = Convert.ToDateTime("09/25/2019").ToString(FormHelper.DateFormat);
            retValue.DateRequested = Convert.ToDateTime("09/25/2019").ToString(FormHelper.DateFormat);
            retValue.DocumentType = "NDA - AIS";
            retValue.PeriodCovered = string.Format("{0} - {1}", Convert.ToDateTime(retValue.DocumentRequestModel.DateStart).ToString(FormHelper.DateFormat)
                                                              , Convert.ToDateTime(retValue.DocumentRequestModel.DateEnd).ToString(FormHelper.DateFormat));

            return retValue;
        }

        public async Task<DocumentRequestApprovalHolder> WorkflowTransaction(DocumentRequestApprovalHolder form)
        {
            var retValue = form;

            //if (actionType == 2 || actionType == 3)
            //{
            //    if ((await UserDialogs.Instance.PromptAsync(WFTransactionHelper.WFMessage(actionType), "", "Yes", "No")).Ok)
            //        retValue.IsSuccess = true;
            //}
            //else
            //{
            //    if (await UserDialogs.Instance.ConfirmAsync(WFTransactionHelper.WFMessage(actionType), "", "Yes", "No"))
            //        retValue.IsSuccess = true;
            //}

            return retValue;
        }

        public async Task<DocumentRequestHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var retValue = new DocumentRequestHolder();

            return retValue;
        }

        public async Task<DocumentRequestHolder> WorkflowTransactionRequest(DocumentRequestHolder form)
        {
            var retValue = form;

            return retValue;
        }
    }
}