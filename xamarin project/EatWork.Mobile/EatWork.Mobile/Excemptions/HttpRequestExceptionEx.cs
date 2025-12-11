using EatWork.Mobile.Models;
using System;
using System.Net;
using System.Net.Http;

namespace EatWork.Mobile.Excemptions
{
    public class HttpRequestExceptionEx : HttpRequestException
    {
        public HttpStatusCode HttpCode { get; }
        public BadRequestModel Model { get; }

        public HttpRequestExceptionEx(HttpStatusCode code) : this(code, null, null)
        {
        }

        public HttpRequestExceptionEx(HttpStatusCode code, string message) : this(code, message, null)
        {
        }

        public HttpRequestExceptionEx(HttpStatusCode code, string message, Exception inner) : base(message, inner)
        {
            HttpCode = code;
        }

        //==ADDED JMBG 01.18.2021
        public HttpRequestExceptionEx(HttpStatusCode code, string message, BadRequestModel data, Exception inner = null) : this(code, message, inner)
        {
            //if (data.Errors.Count == 0)
            //{
            //    var errors = new System.Collections.Generic.List<string>
            //    {
            //        message
            //    };

            //    data.Errors.Add("", errors);
            //}

            Model = data;
        }
    }
}