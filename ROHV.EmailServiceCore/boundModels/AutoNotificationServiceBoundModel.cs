using ROHV.EmailServiceCore.Attributes;

namespace ROHV.EmailServiceCore.boundModels
{
    public class AutoNotificationServiceBoundModel
    {
        [EmailBound(Name = "[InnerEmailBody]")]
        public string InnerEmailBody { set; get; }
    }
}