using MauiHybridApp.Models;
using MauiHybridApp.Services;
using MauiHybridApp.Models.DataObjects;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiHybridApp.Services.Data
{
    public class ChangeWorkScheduleDataService : IChangeWorkScheduleDataService
    {
        private readonly IGenericRepository _repository;

        public ChangeWorkScheduleDataService(IGenericRepository repository)
        {
            _repository = repository;
        }

        public Task<ChangeWorkScheduleHolder> InitForm(long recordId, DateTime? selectedDate)
        {
            var holder = new ChangeWorkScheduleHolder();
            
            // Mocking Initialization Data
            
            // 1. Mock Shift List
            holder.ShiftList = new ObservableCollection<ShiftDto>
            {
                new ShiftDto { ShiftId = 1, Code = "DS", Description = "Day Shift (8am-5pm)", WorkSchedule = "08:00 - 17:00" },
                new ShiftDto { ShiftId = 2, Code = "NS", Description = "Night Shift (10pm-7am)", WorkSchedule = "22:00 - 07:00", StartTimePreviousDay = 0, EndTimeNextDay = 1 },
                new ShiftDto { ShiftId = 3, Code = "AS", Description = "Afternoon Shift (1pm-10pm)", WorkSchedule = "13:00 - 22:00" },
                new ShiftDto { ShiftId = 99, Code = "OFF", Description = "Rest Day", WorkSchedule = "No Schedule" }
            };

            // 2. Mock Reasons
            holder.ReasonList = new ObservableCollection<ComboBoxObject>
            {
                new ComboBoxObject { Id = 1, Value = "Personal Matter" },
                new ComboBoxObject { Id = 2, Value = "Operation Requirement" },
                new ComboBoxObject { Id = 3, Value = "Change of Shift" }
            };

            if (selectedDate.HasValue)
            {
                holder.WorkDate = selectedDate.Value;
                holder.ChangeWorkScheduleModel.WorkDate = selectedDate.Value;
            }

            return Task.FromResult(holder);
        }

        public Task<ChangeWorkScheduleHolder> SubmitRequest(ChangeWorkScheduleHolder form)
        {
            // Placeholder Mock Submission
            form.Success = true;
            return Task.FromResult(form);
        }

        public Task<ChangeWorkScheduleHolder> GetEmployeeSchedule(ChangeWorkScheduleHolder form, int option)
        {
             // Mock fetching schedule logic
             // In real app, this calls API to get employee's schedule for a specific date
             
             form.OriginalShiftCode = "DS (8am-5pm)";
             form.OriginalSchedule = "08:00 AM - 05:00 PM";
             
             return Task.FromResult(form);
        }

        public Task<ChangeWorkScheduleHolder> WorkflowTransactionRequest(ChangeWorkScheduleHolder form)
        {
             form.Success = true;
            return Task.FromResult(form);
        }
    }
}
