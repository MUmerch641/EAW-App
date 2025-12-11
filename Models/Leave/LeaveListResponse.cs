using System.Collections.Generic;
using Newtonsoft.Json;

namespace MauiHybridApp.Models.Leave
{
    public class LeaveListResponse
    {
        [JsonProperty("leaveRequestDetailList")]
        public List<LeaveRequestModel> LeaveRequestDetailList { get; set; } = new();

        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
