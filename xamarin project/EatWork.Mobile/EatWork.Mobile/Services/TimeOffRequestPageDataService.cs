using EatWork.Mobile.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EatWork.Mobile.Services.TestServices
{
    class TimeOffRequestPageDataService : ITimeOffRequestPageDataService
    {
            public async Task SubmitRequest()
            {
                await Task.Delay(1000);
            }
        }
    }