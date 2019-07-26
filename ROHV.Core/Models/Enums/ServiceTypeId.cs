using System.ComponentModel;

namespace ROHV.Core.Enums
{
    public enum ServiceTypeIdEnum
    {
        CommunityHabilitation = 1,
        GroupDayHabilitation = 2,
        Respite = 3,
        Broker = 4, 
        SelfHired = 5,
        SupplementalGroupDayHabilitation = 6,
        ContractedCommunityHabilitation = 7,

    }

    public enum DocumentPrintTypeEnum
    {
        [Description("Community Habilitation Documentation Record - Individual Summary")]
        ComHabDocumentationRecord =1,

        [Description("Community Habilitation Monthly Progress Summary")]
        ComHubMonthlyProgressSummary =2,

        [Description("Group Day Habilitation Documentation Record - Individual Summary")]
        GroupDayHabDocumentationRecord =4,

        [Description("Group Day Habilitation Monthly Progress Summary")]
        GroupDayMonthlyProgresSummary =5,

        [Description("Respite Documentation Record - Individual Summary")]
        RespiteDocumentationRecord =7,

        [Description("Broker Documentation Record - Individual Summary")]
        BrokerDocumentationRecord =8,

        [Description("Self Hired Community Habilitation Documentation Record - Individual Summary")]
        SelfHiredComHubDocumentationRecord = 9,

        [Description("Self Hired Community Habilitation Monthly Progress Summary")]
        SelfHiredComHubMonthlyProgressSummary =10,

        [Description("Self Hired Respite Documentation Record - Individual Summary")]
        SelfHiredRespiteDocumentationRecord =11,

        [Description("Supplemental Group Day Habilitation Documentation Record - Individual Summary")]
        SupplementalGroupDayHabDocumentationRecord = 12,

        [Description("Supplemental Group Day Habilitation Monthly Progress Summary")]
        SupplementalGroupDayMonthlyProgresSummary = 13,

        [Description("Contracted Community Habilitation Documentation Record - Individual Summary")]
        ContractedComHabDocumentationRecord = 14,

        [Description("Contracted Community Habilitation Monthly Progress Summary")]
        ContractedComHubMonthlyProgressSummary = 15,
    }
}