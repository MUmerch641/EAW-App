using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiHybridApp.Models.DataObjects;

namespace MauiHybridApp.Models
{
    public class ChangeRestdayHolder : ObservableObject
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
            ChangeRestdayModel = new ChangeRestdayModel();
            FileAttachments = new ObservableCollection<FileUploadResponse>();
        }
        
        // Base RequestHolder properties (simulated)
        private bool _success;
        public bool Success
        {
            get => _success;
            set => SetProperty(ref _success, value);
        }

        private string _msg = string.Empty;
        public string Msg
        {
            get => _msg;
            set => SetProperty(ref _msg, value);
        }
        
        public long ActionTypeId { get; set; }
        public FileUploadResponse? SelectedFile { get; set; }
        public ObservableCollection<FileUploadResponse> FileAttachments { get; set; }
        public bool IsEnabled { get; set; } = true;


        private string _swapWith = string.Empty;
        public string SwapWith
        {
            get => _swapWith;
            set => SetProperty(ref _swapWith, value);
        }

        private DateTime _restDayDateStart;
        public DateTime RestDayDateStart
        {
            get => _restDayDateStart;
            set => SetProperty(ref _restDayDateStart, value);
        }

        private DateTime _restDayDateEnd;
        public DateTime RestDayDateEnd
        {
            get => _restDayDateEnd;
            set => SetProperty(ref _restDayDateEnd, value);
        }

        private ObservableCollection<ChangeRestday> _restdayList;
        public ObservableCollection<ChangeRestday> RestDayList
        {
            get => _restdayList;
            set => SetProperty(ref _restdayList, value);
        }

        private ChangeRestdayModel _changeRestdayModel;
        public ChangeRestdayModel ChangeRestdayModel
        {
            get => _changeRestdayModel;
            set => SetProperty(ref _changeRestdayModel, value);
        }

        private List<ChangeRestDayDetailList> _changeRestDayDetailList;
        public List<ChangeRestDayDetailList> ChangeRestDayDetailList
        {
            get => _changeRestDayDetailList;
            set => SetProperty(ref _changeRestDayDetailList, value);
        }

        #region validators

        private bool _errorReason;
        public bool ErrorReason
        {
            get => _errorReason;
            set => SetProperty(ref _errorReason, value);
        }

        private bool _errorSwapWith;
        public bool ErrorSwapWith
        {
            get => _errorSwapWith;
            set => SetProperty(ref _errorSwapWith, value);
        }

        private bool _errorOriginalDate;
        public bool ErrorOriginalDate
        {
            get => _errorOriginalDate;
            set => SetProperty(ref _errorOriginalDate, value);
        }

        private bool _errorRequestedDate;
        public bool ErrorRequestedDate
        {
            get => _errorRequestedDate;
            set => SetProperty(ref _errorRequestedDate, value);
        }

        private string _errorOriginalDateMessage = string.Empty;
        public string ErrorOriginalDateMessage
        {
            get => _errorOriginalDateMessage;
            set => SetProperty(ref _errorOriginalDateMessage, value);
        }

        private string _errorRequestedDateMessage = string.Empty;
        public string ErrorRequestedDateMessage
        {
            get => _errorRequestedDateMessage;
            set => SetProperty(ref _errorRequestedDateMessage, value);
        }

        #endregion validators
    }
}
