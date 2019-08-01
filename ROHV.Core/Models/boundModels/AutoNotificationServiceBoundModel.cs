using  ROHV.Core.Attributes;
namespace ROHV.Core.Models
{
    public class AutoNotificationServiceBoundModel
    {
        [EmailBound(Name = "[InnerEmailBody]")]
        public string InnerEmailBody { set; get; }
    }
}