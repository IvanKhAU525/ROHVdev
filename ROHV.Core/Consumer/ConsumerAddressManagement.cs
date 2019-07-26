using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITCraftFrame;
using ROHV.Core.Database;
using ROHV.Core.Models;
using ROHV.Core.Models.Addresses;

namespace ROHV.Core.Consumer
{
    public static class ConsumerAddressManagement
    {
        public static bool Validate(RayimContext context, BaseConsumerAddressModel model)
        {
            var intersectedRecord = context.ConsumerAddresses.FirstOrDefault(x => x.Id != model.Id && x.ConsumerId == model.ConsumerId &&
           (x.FromDate <= model.FromDate &&  (x.ToDate == null || model.FromDate <= x.ToDate) ||
           model.ToDate != null && x.FromDate <= model.ToDate && (x.ToDate == null || model.ToDate <= x.ToDate) ||
           x.FromDate >= model.FromDate &&  (model.ToDate == null || x.ToDate != null && model.ToDate >= x.ToDate ) ||
           model.ToDate == null && x.ToDate == null));
            return intersectedRecord == null;
        }

        public static ConsumerAddressModel CreateOrUpdate(RayimContext context, BaseConsumerAddressModel inputModel)
        {
            var isNew = inputModel.Id == 0;
            var dbEntity = context.ConsumerAddresses.FirstOrDefault(x => x.Id == inputModel.Id);
            if (dbEntity == null)
            {
                dbEntity = context.ConsumerAddresses.Add(CustomMapper.MapEntity<ConsumerAddress>(inputModel));
            }
            else
            {
                CustomMapper.MapEntity<ConsumerAddress>(inputModel, dbEntity);
            }
            context.SaveChanges();


            return CustomMapper.MapEntity<ConsumerAddressModel>(dbEntity);
        }

        public static void Delete(RayimContext context, int id)
        {
            var dbEntity = context.ConsumerAddresses.FirstOrDefault(x => x.Id == id);
            if (dbEntity != null)
            {
                context.ConsumerAddresses.Remove(dbEntity);
                context.SaveChanges();
            }

        }

        public static ReportAddressModel GetScheduledAddressByDate(RayimContext context, DateTime actualDate, Int32 consumerId)
        {
            var foundRecord = context.ConsumerAddresses.FirstOrDefault(x => x.ConsumerId == consumerId && x.FromDate <= actualDate && (actualDate <= x.ToDate || x.ToDate == null));
            return foundRecord != null ? CustomMapper.MapEntity<ReportAddressModel>(foundRecord) : null;
        }
    }
}
