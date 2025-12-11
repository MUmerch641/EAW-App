using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MauiHybridApp.Models.DataObjects;

namespace MauiHybridApp.Models
{
    public class ChangeWorkScheduleHolder : ObservableObject
    {
        public ChangeWorkScheduleHolder()
        {
            OriginalShiftCode = string.Empty;
            SwapWith = string.Empty;
            Success = false;
            EnableStartTimePreviousDay = true;
            EnableEndTimeNextDay = true;
            EnableCustomSched = false;
            WorkDate = DateTime.Now.Date;
            
            ShiftList = new ObservableCollection<ShiftDto>();
            ShiftSelectedItem = new ShiftDto();
            ReasonList = new ObservableCollection<ComboBoxObject>();
            ReasonSelectedItem = new ComboBoxObject();
            ChangeWorkScheduleModel = new ChangeWorkScheduleModel();
            FileAttachments = new ObservableCollection<FileUploadResponse>();
        }

        // RequestHolder simulation
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


        private ObservableCollection<ShiftDto> _shiftList;
        public ObservableCollection<ShiftDto> ShiftList
        {
            get => _shiftList;
            set => SetProperty(ref _shiftList, value);
        }

        private ShiftDto _shiftSelectedItem;
        public ShiftDto ShiftSelectedItem
        {
            get => _shiftSelectedItem;
            set => SetProperty(ref _shiftSelectedItem, value);
        }

        private ObservableCollection<ComboBoxObject> _reasonList;
        public ObservableCollection<ComboBoxObject> ReasonList
        {
            get => _reasonList;
            set => SetProperty(ref _reasonList, value);
        }

        private ComboBoxObject _reasonSelectedItem;
        public ComboBoxObject ReasonSelectedItem
        {
            get => _reasonSelectedItem;
            set => SetProperty(ref _reasonSelectedItem, value);
        }

        private DateTime _workDate;
        public DateTime WorkDate
        {
            get => _workDate;
            set => SetProperty(ref _workDate, value);
        }

        private string _originalShiftCode;
        public string OriginalShiftCode
        {
            get => _originalShiftCode;
            set => SetProperty(ref _originalShiftCode, value);
        }

        private bool _enableCustomSched;
        public bool EnableCustomSched
        {
            get => _enableCustomSched;
            set => SetProperty(ref _enableCustomSched, value);
        }

        private bool _enableStartTimePreviousDay;
        public bool EnableStartTimePreviousDay
        {
            get => _enableStartTimePreviousDay;
            set => SetProperty(ref _enableStartTimePreviousDay, value);
        }

        private bool _enableEndTimeNextDay;
        public bool EnableEndTimeNextDay
        {
            get => _enableEndTimeNextDay;
            set => SetProperty(ref _enableEndTimeNextDay, value);
        }

        private string _swapWith;
        public string SwapWith
        {
            get => _swapWith;
            set => SetProperty(ref _swapWith, value);
        }
        
        // TimeSpans for UI binding if needed
        private TimeSpan? _scheduleStartTime;
        public TimeSpan? ScheduleStartTime
        {
            get => _scheduleStartTime;
            set => SetProperty(ref _scheduleStartTime, value);
        }

        private TimeSpan? _scheduleEndTime;
        public TimeSpan? ScheduleEndTime
        {
            get => _scheduleEndTime;
            set => SetProperty(ref _scheduleEndTime, value);
        }

        private TimeSpan? _lunchStartTime;
        public TimeSpan? LunchStartTime
        {
            get => _lunchStartTime;
            set => SetProperty(ref _lunchStartTime, value);
        }

        private TimeSpan? _lunchEndTime;
        public TimeSpan? LunchEndTime
        {
            get => _lunchEndTime;
            set => SetProperty(ref _lunchEndTime, value);
        }
        
        // Strings for display
        private string _originalSchedule = string.Empty;
        public string OriginalSchedule
        {
            get => _originalSchedule;
            set => SetProperty(ref _originalSchedule, value);
        }

        private ChangeWorkScheduleModel _changeWorkScheduleModel;
        public ChangeWorkScheduleModel ChangeWorkScheduleModel
        {
            get => _changeWorkScheduleModel;
            set => SetProperty(ref _changeWorkScheduleModel, value);
        }
        
        #region Validators
        // ... (Omitting full validators logic typically handled in VM/UI validation, keeping minimal for object structure)
        #endregion
    }
}
