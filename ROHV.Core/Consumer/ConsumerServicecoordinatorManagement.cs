using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;

namespace ROHV.Core.Consumer
{
    public static class ConsumerServiceCoordinatorManagement
    {
        public static ConsumerServiceCoordinatorModel CreateOrUpdate(RayimContext context, BaseConsumerServiceCoordinatorModel inputModel)
        {
            var isNew = inputModel.Id == 0;
            var dbEntity = context.ConsumerServiceCoordinators.FirstOrDefault(x => x.Id == inputModel.Id);
            if (dbEntity == null)
            {
                dbEntity = context.ConsumerServiceCoordinators.Add(CustomMapper.MapEntity<ConsumerServiceCoordinator>(inputModel));


            }
            else
            {
                CustomMapper.MapEntity<ConsumerServiceCoordinator>(inputModel, dbEntity);
            }
            context.SaveChanges();

            if (isNew)
            {
                context.Entry(dbEntity).Reference(c => c.Contact).Load(); ;
            }



            return CustomMapper.MapEntity<ConsumerServiceCoordinatorModel>(dbEntity);
        }

        public static bool Validate(RayimContext context, BaseConsumerServiceCoordinatorModel model)
        {
            var intersectedRecord = context.ConsumerServiceCoordinators.FirstOrDefault(x => x.Id != model.Id && x.ConsumerId == model.ConsumerId &&
              (x.FromDate <= model.FromDate && (x.ToDate == null || model.FromDate <= x.ToDate) ||
              model.ToDate != null && x.FromDate <= model.ToDate && (x.ToDate == null || model.ToDate <= x.ToDate) ||
              x.FromDate >= model.FromDate && (model.ToDate == null || model.ToDate >= x.ToDate && x.ToDate != null) ||
              model.ToDate == null && x.ToDate == null));
            return intersectedRecord == null;
        }

        public static void Delete(RayimContext context, int id)
        {
            var dbEntity = context.ConsumerServiceCoordinators.FirstOrDefault(x => x.Id == id);
            if (dbEntity != null)
            {
                context.ConsumerServiceCoordinators.Remove(dbEntity);
                context.SaveChanges();
            }
        }

        public static Contact GetScheduledContactByDate(RayimContext context, DateTime planOfDate, Int32 consumerId)
        {
            var foundRecord = context.ConsumerServiceCoordinators.FirstOrDefault(x => x.ConsumerId == consumerId && x.FromDate <= planOfDate && planOfDate <= x.ToDate);
            return foundRecord?.Contact;
        }
    }
}
