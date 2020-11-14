using System;

#nullable disable

namespace Ats.Model.Models
{
    public partial class ExceptionLog
    {
        public int PkId { get; set; }
        public string Message { get; set; }
        public string SpecialMessage { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public DateTime ExceptionDateTime { get; set; }
    }
}
