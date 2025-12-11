using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MauiHybridApp.Models.Workflow
{
    public class MyRequestListListResponse
    {
        [JsonProperty("listData")]
        public List<MyRequestItem> ListData { get; set; } = new();

        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }

    public class MyRequestItem
    {
        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }

        [JsonProperty("dateFiled")]
        public DateTime? DateFiled { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("transactionId")]
        public long TransactionId { get; set; }
    }
}
