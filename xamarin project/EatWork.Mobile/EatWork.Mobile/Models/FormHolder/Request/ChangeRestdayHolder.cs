using EatWork.Mobile.Models.DataObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EatWork.Mobile.Models.FormHolder.Request
{
    public class ChangeRestdayHolder : RequestHolder
    {
        public ChangeRestdayHolder()
        {
            ErrorReason = false;
            ErrorSwapWith = false;
            SwapWith = string.Empty;
            RestDayDateStart = DateTime.Now;
            RestDayDateEnd = DateTime.Now;
            RestDayList = new ObservableCollection<ChangeRestday>();
            ChangeRestDayDetailList = new List<ChangeRestDayDetailList>();
        }

        private string swapWith_;

        public string SwapWith
        {
            get { return swapWith_; }
            set { swapWith_ = value; RaisePropertyChanged(() =>SwapWith); }
        }

        private DateTime restDayDateStart_;

        public DateTime RestDayDateStart
        {
            get { return restDayDateStart_; }
            set { restDayDateStart_ = value; RaisePropertyChanged(() => RestDayDateStart); }
        }

        private DateTime restDayDateEnd_;

        public DateTime RestDayDateEnd
        {
            get { return restDayDateEnd_; }
            set { restDayDateEnd_ = value; RaisePropertyChanged(() => RestDayDateEnd); }
        }

        private ObservableCollection<ChangeRestday> restdayList_;

        public ObservableCollection<ChangeRestday> RestDayList
        {
            get { return restdayList_; }
            set { restdayList_ = value; RaisePropertyChanged(() => RestDayList); }
        }

        private ChangeRestdayModel model_;

        public ChangeRestdayModel ChangeRestdayModel
        {
            get { return model_; }
            set { model_ = value; RaisePropertyChanged(() => ChangeRestdayModel); }
        }

        private List<ChangeRestDayDetailList> changeRestDayDetailList_;

        public List<ChangeRestDayDetailList> ChangeRestDayDetailList
        {
            get { return changeRestDayDetailList_; }
            set { changeRestDayDetailList_ = value; RaisePropertyChanged(() => ChangeRestDayDetailList); }
        }

        #region validators

        private bool errorReason_;

        public bool ErrorReason
        {
            get { return errorReason_; }
            set { errorReason_ = value; RaisePropertyChanged(() => ErrorReason); }
        }

        private bool errorSwapWith_;

        public bool ErrorSwapWith
        {
            get { return errorSwapWith_; }
            set { errorSwapWith_ = value; RaisePropertyChanged(() => ErrorSwapWith); }
        }

        private bool errorOriginalDate_;

        public bool ErrorOriginalDate
        {
            get { return errorOriginalDate_; }
            set { errorOriginalDate_ = value; RaisePropertyChanged(() => ErrorOriginalDate); }
        }

        private bool errorRequestedDate_;

        public bool ErrorRequestedDate
        {
            get { return errorRequestedDate_; }
            set { errorRequestedDate_ = value; RaisePropertyChanged(() => ErrorRequestedDate); }
        }

        private string errorOriginalDateMessage_;

        public string ErrorOriginalDateMessage
        {
            get { return errorOriginalDateMessage_; }
            set { errorOriginalDateMessage_ = value; RaisePropertyChanged(() => ErrorOriginalDateMessage); }
        }

        private string errorRequestedDateMessage_;

        public string ErrorRequestedDateMessage
        {
            get { return errorRequestedDateMessage_; }
            set { errorRequestedDateMessage_ = value; RaisePropertyChanged(() => ErrorRequestedDateMessage); }
        }

        #endregion validators
    }
}