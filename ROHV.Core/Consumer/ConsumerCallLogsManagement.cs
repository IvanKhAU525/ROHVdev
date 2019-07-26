using ROHV.Core.Database;
using System.Data.Entity;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace ROHV.Core.Consumer
{
    public class ConsumerCallLogsManagement : BaseModel
    {
        public ConsumerCallLogsManagement(RayimContext context) : base(context) { }

        public async Task<List<ConsumerContactCall>> GetCallLogs(Int32 consumerId)
        {
            var result = await _context.ConsumerContactCalls.Where(x => x.ConsumerId == consumerId).OrderByDescending(x => x.DateCreated).ToListAsync();
            return result;
        }
        public async Task<Int32> Save(ConsumerContactCall dbModel)
        {
            if (dbModel.ConsumerContactCallId == 0)
            {
                _context.ConsumerContactCalls.Add(dbModel);
            }
            else
            {
                var model = await _context.ConsumerContactCalls.SingleOrDefaultAsync(x => x.ConsumerContactCallId == dbModel.ConsumerContactCallId);
                if (model != null)
                {
                    model.ContactId = dbModel.ContactId;
                    model.CalledOn = dbModel.CalledOn;
                    model.Notes = dbModel.Notes;
                    model.AddedById = dbModel.AddedById;
                    model.UpdatedById = dbModel.UpdatedById;
                    model.DateCreated = dbModel.DateCreated;
                    model.DateUpdated = dbModel.DateUpdated;
                }
            }
            await _context.SaveChangesAsync();
            return dbModel.ConsumerContactCallId;
        }
        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerContactCalls.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                _context.ConsumerContactCalls.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Boolean> Delete(Int32 id)
        {
            var model = await _context.ConsumerContactCalls.SingleOrDefaultAsync(x => x.ConsumerContactCallId == id);
            if (model != null)
            {
                _context.ConsumerContactCalls.Remove(model);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }

    }
}
