using ROHV.Core.Consumer;
using ROHV.Core.Database;
using ROHV.Core.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ROHV.Core.Models
{
    public interface IPDFhabReviewReportModel
    {
        string ReportPath { get; set; }

        ServiceTypeIdEnum ServiceTypeId { get; set; }

        string TitleDocument { get; set; }

        string Name { get; set; }

        Contact CoordinatorContact { get; set; }

        string CoordinatorLabel { get; set; }

        bool ShowReviewedBy { get; set; }

        IEnumerable<object> CreateDataSet(ConsumerHabReview document);

        Contact GetCoordinatorContact(ConsumerHabReview document);
        void CalculateTemplatePath(DateTime? templateDate, RayimContext context);
    }
}