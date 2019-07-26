using ROHV.Core.Database;
using System.Data.Entity;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace ROHV.Core.Consumer
{
    public class ConsumerNotesManagement : BaseModel
    {
        public ConsumerNotesManagement(RayimContext context) : base(context) { }

        public async Task<List<ConsumerNote>> GetNotes(Int32 consumerId)
        {
            var result = await _context.ConsumerNotes.Where(x => x.ConsumerId == consumerId).OrderByDescending(x => x.DateCreated).ToListAsync();
            return result;
        }
        public async Task<Int32> Save(ConsumerNote dbModel)
        {
            if (dbModel.ConsumerNoteId == 0)
            {
                _context.ConsumerNotes.Add(dbModel);
            }
            else
            {
                var model = await _context.ConsumerNotes.SingleOrDefaultAsync(x => x.ConsumerNoteId == dbModel.ConsumerNoteId);
                if (model != null)
                {
                    model.ContactId = dbModel.ContactId;
                    model.Date = dbModel.Date;
                    model.TypeId = dbModel.TypeId;
                    model.TypeFromId = dbModel.TypeFromId;
                    model.Notes = dbModel.Notes;
                    model.AditionalInformation = model.AditionalInformation;
                    model.AddedById = dbModel.AddedById;
                    model.UpdatedById = dbModel.UpdatedById;
                    model.DateCreated = dbModel.DateCreated;
                    model.DateUpdated = dbModel.DateUpdated;
                }
            }
            await _context.SaveChangesAsync();
            return dbModel.ConsumerNoteId;
        }

        public async Task<Boolean> Delete(Int32 id)
        {
            var model = await _context.ConsumerNotes.SingleOrDefaultAsync(x => x.ConsumerNoteId == id);
            if (model != null)
            {
                _context.ConsumerNotes.Remove(model);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }
        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerNotes.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                _context.ConsumerNotes.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ConsumerNoteType>> GetTypes()
        {
            var result = await _context.ConsumerNoteTypes.Where(x => x.IsActive).ToListAsync();
            return result;
        }
        public async Task<List<ConsumerNoteFromType>> GetFromTypes()
        {
            var result = await _context.ConsumerNoteFromTypes.Where(x => x.IsActive).ToListAsync();
            return result;
        }
        public async Task<ConsumerNote> GetNote(Int32 noteId)
        {
            var result = await _context.ConsumerNotes.FirstOrDefaultAsync(x => x.ConsumerNoteId == noteId);
            return result;
        }
    }
}
