using EatWork.Mobile.Contants;
using EatWork.Mobile.Models;
using EatWork.Mobile.Views.Shared;
using EatWork.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using EatWork.Mobile.Models.FormHolder.TravelRequest;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Bootstrap;

namespace EatWork.Mobile.ViewModels.TravelRequest
{
    public class TravelRequestApprovalViewModel : BaseViewModel
    {
        #region commands

        public ICommand CloseCommand { get; set; }
        public ICommand TransactionCommand { get; set; }
        public ICommand ViewFileAttachmentsCommand { get; set; }
        public ICommand ViewProfileCommand { get; set; }
        public ICommand ViewTransactionHistoryCommand { get; set; }

        #endregion commands

        private TravelRequestApprovalHolder formHelper_;

        public TravelRequestApprovalHolder FormHelper
        {
            get { return formHelper_; }
            set { formHelper_ = value; RaisePropertyChanged(() => FormHelper); }
        }

        private readonly ITravelRequestDataService service_;

        public TravelRequestApprovalViewModel()
        {
            service_ = AppContainer.Resolve<ITravelRequestDataService>();
        }

        public void Init(INavigation navigation, MyApprovalListModel param)
        {
            NavigationBack = navigation;

            TransactionCommand = new Command(WorkflowTransaction);
            ViewFileAttachmentsCommand = new Command(async () => await ViewFileAttachments());
            CloseCommand = new Command(async () => await NavigationService.PopPageAsync());
            ViewProfileCommand = new Command(async () => await NavigationService.PushPageAsync(new ComingSoonPage("Employee Profile")));
            ViewTransactionHistoryCommand = new Command(async () => await NavigationService.PushModalAsync(new TransactionHistoryPage(FormHelper.TransactionTypeId, FormHelper.TransactionId)));

            InitForm(param);
        }

        private async void InitForm(MyApprovalListModel param)
        {
            try
            {
                if (!IsBusy)
                {
                    IsBusy = true;
                    await Task.Delay(500);

                    FormHelper = await service_.InitApprovalForm(param.TransactionTypeId, param.TransactionId);
                }
            }
            catch (Exception ex)
            {
                ShowPage = false;
                Error(false, ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void WorkflowTransaction(object obj)
        {
            try
            {
                if (obj is Models.DataObjects.WorkflowAction item)
                {
                    FormHelper.SelectedWorkflowAction = item;
                    FormHelper = FormHelper;

                    FormHelper = await service_.WorkflowTransaction(FormHelper); 
                    if (FormHelper.IsSuccess)
                    {
                        Success(true, Messages.ApprovalFormSuccessMessage);
                        await NavigationService.PopToRootAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }

        private async Task ViewFileAttachments()
        {
            try
            {
                if (!IsBusy)
                {
                    var param = new FileAttachmentParams()
                    {
                        ModuleFormId = FormHelper.ModuleFormId,
                        TransactionId = FormHelper.TransactionId
                    };

                    using (Dialogs.Loading())
                    {
                        await Task.Delay(500);
                        await NavigationService.PushModalAsync(new FileAttachmentPage(param));
                    }
                }
            }
            catch (Exception ex)
            {
                Error(false, ex.Message);
            }
        }
    }
}