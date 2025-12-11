using EatWork.Mobile.Contants;
using EatWork.Mobile.Models.DataObjects;
using EatWork.Mobile.Utils;
using EatWork.Mobile.Validations;
using System;
using System.Collections.ObjectModel;
using R = EAW.API.DataContracts;

namespace EatWork.Mobile.Models.FormHolder.CashAdvance
{
    public class CashAdvanceRequestHolder : RequestHolder
    {
        public CashAdvanceRequestHolder()
        {
            RequestNo = string.Empty;
            DateRequested = DateTime.UtcNow.Date;
            DateNeeded = DateTime.UtcNow.Date;
            Amount = new ValidatableObject<decimal>();
            Reason = new ValidatableObject<string>();
            ChargeCode = new ValidatableObject<string>();
            Model = new R.Models.CashAdvance();
            OtherChargeCode = new ValidatableObject<string>();
            ChargeCodeSource = new ObservableCollection<ComboBoxObject>();
            ShowOtherChargeCode = false;
            SelectedChargeCode = new ComboBoxObject();
            DateNeededValidation = new ValidatableObject<string>();
        }

        private string requestNo_;

        public string RequestNo
        {
            get { return requestNo_; }
            set { requestNo_ = value; RaisePropertyChanged(() => RequestNo); }
        }

        private DateTime dateRequested_;

        public DateTime DateRequested
        {
            get { return dateRequested_; }
            set { dateRequested_ = value; RaisePropertyChanged(() => DateRequested); }
        }

        private DateTime dateNeeded_;

        public DateTime DateNeeded
        {
            get { return dateNeeded_; }
            set { dateNeeded_ = value; RaisePropertyChanged(() => DateNeeded); }
        }

        private ValidatableObject<decimal> amount_;

        public ValidatableObject<decimal> Amount
        {
            get { return amount_; }
            set { amount_ = value; RaisePropertyChanged(() => Amount); }
        }

        private ValidatableObject<string> reason_;

        public ValidatableObject<string> Reason
        {
            get { return reason_; }
            set { reason_ = value; RaisePropertyChanged(() => Reason); }
        }

        private ObservableCollection<ComboBoxObject> chargeCodeSource_;

        public ObservableCollection<ComboBoxObject> ChargeCodeSource
        {
            get { return chargeCodeSource_; }
            set { chargeCodeSource_ = value; RaisePropertyChanged(() => ChargeCodeSource); }
        }

        private ComboBoxObject selectedChargeCode_;

        public ComboBoxObject SelectedChargeCode
        {
            get { return selectedChargeCode_; }
            set { selectedChargeCode_ = value; RaisePropertyChanged(() => SelectedChargeCode); }
        }

        private ValidatableObject<string> chargeCode_;

        public ValidatableObject<string> ChargeCode
        {
            get { return chargeCode_; }
            set { chargeCode_ = value; RaisePropertyChanged(() => ChargeCode); }
        }

        private ValidatableObject<string> otherChargeCode_;

        public ValidatableObject<string> OtherChargeCode
        {
            get { return otherChargeCode_; }
            set { otherChargeCode_ = value; RaisePropertyChanged(() => OtherChargeCode); }
        }

        private ValidatableObject<string> dateNeededValidation_;

        public ValidatableObject<string> DateNeededValidation
        {
            get { return dateNeededValidation_; }
            set { dateNeededValidation_ = value; RaisePropertyChanged(() => DateNeededValidation); }
        }

        private bool showOtherChargeCode_;

        public bool ShowOtherChargeCode
        {
            get { return showOtherChargeCode_; }
            set { showOtherChargeCode_ = value; RaisePropertyChanged(() => ShowOtherChargeCode); }
        }

        private R.Models.CashAdvance model_;

        public R.Models.CashAdvance Model
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => ChargeCode); }
        }

        public bool ExecuteSubmit()
        {
            var user = PreferenceHelper.UserInfo();

            Model = new R.Models.CashAdvance()
            {
                Amount = Amount.Value,
                ChargeCode = ChargeCode.Value,
                DateNeeded = DateNeeded,
                Reason = Reason.Value,
                RequestedDate = DateRequested,
                ProfileId = user.ProfileId,
                RequestNo = RequestNo,
                StatusId = RequestStatusValue.Submitted,
                CostCenterId = SelectedChargeCode.Id,
                SourceId = (short)SourceEnum.Mobile,
            };

            return IsValid();
        }

        public bool IsValid()
        {
            Amount.Validations.Clear();
            Amount.Validations.Add(new NumberRule<decimal>
            {
                ValidationMessage = ""
            });

            /*Amount.Validate();*/

            Reason.Validations.Clear();
            Reason.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });

            /*
            ChargeCode.Validations.Clear();
            ChargeCode.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = ""
            });
            ChargeCode.Validate();
            */

            DateNeededValidation.Validations.Clear();

            Amount.Validate();
            Reason.Validate();
            DateNeededValidation.Validate();

            if (DateNeeded < DateRequested)
            {
                DateNeededValidation.Errors.Add(Messages.CashAdvanceDateNeededValidation);
            }

            return Amount.IsValid &&
                   Reason.IsValid &&
                   DateNeededValidation.IsValid;
        }
    }
}