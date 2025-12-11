using System;

namespace EatWork.Mobile.Excemptions
{
    public class ServiceAuthenticationException : Exception
    {
        public string Content { get; set; }

        public ServiceAuthenticationException(string content)
        {
            Content = content;
        }
    }
}