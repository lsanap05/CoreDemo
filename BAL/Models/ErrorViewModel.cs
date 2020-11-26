using System;
using System.Runtime.InteropServices.ComTypes;

namespace BAL.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ErrorMessage { get; set; }

        public string ErrorField { get; set; }
    }
}
