using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;

namespace ROHV.Core.Consumer
{
    public static class ConsumerMedicaidNumberManagement
    {
        public static ConsumerMedicaidNumberModel CreateOrUpdate(RayimContext context, ConsumerMedicaidNumberModel model)
        {
            var dbEntity = context.ConsumerMedicaidNumbers.FirstOrDefault(x => x.Id == model.Id);

            if (dbEntity == null)
            {
                var map = CustomMapper.MapEntity<ConsumerMedicaidNumber>(model);
                dbEntity = context.ConsumerMedicaidNumbers.Add(map);
            }
            else
            {
                CustomMapper.MapEntity<ConsumerMedicaidNumber>(model, dbEntity);
            }
            context.SaveChanges();

            return CustomMapper.MapEntity<ConsumerMedicaidNumberModel>(dbEntity);
        }

        public static List<string> Validate(RayimContext context, ConsumerMedicaidNumberModel model)
        {
            var errors = new List<string>();

            if (!ValidateDateRange(context, model))
            {
                errors.Add("Not valid date range for Medicaid number");
            }

            if (!ValidateMedicaidNumber(context, model))
            {
                errors.Add("This Medicaid number already exists");
            }

            return errors;
        }

        private static bool ValidateDateRange(RayimContext context, ConsumerMedicaidNumberModel model)
        {
            var intersectedRecord = context.ConsumerMedicaidNumbers
                .FirstOrDefault(x => x.Id != model.Id && x.ConsumerId == model.ConsumerId &&
                               (x.FromDate <= model.FromDate && (x.ToDate == null || model.FromDate <= x.ToDate) ||
                                model.ToDate != null && x.FromDate <= model.ToDate && (x.ToDate == null || model.ToDate <= x.ToDate) ||
                                x.FromDate >= model.FromDate && (model.ToDate == null || model.ToDate >= x.ToDate && x.ToDate != null) ||
                                model.ToDate == null && x.ToDate == null));
            return intersectedRecord == null;
        }

        private static bool ValidateMedicaidNumber(RayimContext context, ConsumerMedicaidNumberModel model)
        {
            var intersectedRecord = context.ConsumerMedicaidNumbers.FirstOrDefault(x => x.MedicaidNo == model.MedicaidNo && x.Id != model.Id);

            return intersectedRecord == null;
        }

        public static void Delete(RayimContext context, int id)
        {
            var entity = context.ConsumerMedicaidNumbers.FirstOrDefault(x => x.Id == id);

            if (entity != null)
            {
                context.ConsumerMedicaidNumbers.Remove(entity);
                context.SaveChanges();
            }
        }

        public static ConsumerMedicaidNumber GetMedicaidNumberByDate(RayimContext context, DateTime? processingDate, Int32 consumerId)
        {
            return context.ConsumerMedicaidNumbers.FirstOrDefault(x =>
                x.ConsumerId == consumerId && x.FromDate <= processingDate && processingDate <= x.ToDate);
        }
    }
}
