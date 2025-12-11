using System.Collections.Generic;

namespace EatWork.Mobile.Models
{
    public class BadRequestModel
    {
        public BadRequestModel()
        {
            Title = string.Empty;
            Errors = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, List<string>> Errors { get; set; }
        public string Title { get; set; }
    }
}