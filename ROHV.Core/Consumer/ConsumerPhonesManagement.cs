using ROHV.Core.Database;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using ROHV.Core.Services;

namespace ROHV.Core.Consumer
{
    public class ConsumerPhonesManagement : BaseModel
    {
        public ConsumerPhonesManagement(RayimContext context) : base(context) { }
        public async Task<List<ConsumerPhone>> GetPhones(Int32 consumerId)
        {
            var phones = await _context.ConsumerPhones.Where(x => x.ConsumerId == consumerId).OrderBy(x => x.PhoneTypeId).ThenBy(x => x.Phone).ToListAsync();
            return phones;
        }

        public async Task Save(List<ConsumerPhone> phones, Int32 consumerId)
        {
            var old = _context.ConsumerPhones.Where(x => x.ConsumerId == consumerId);          
            _context.ConsumerPhones.RemoveRange(old);
            await _context.SaveChangesAsync();
            if (phones.Count > 0)
            {
                foreach (var phone in phones)
                {
                    phone.ConsumerId = consumerId;
                    phone.ConsumerPhoneId = 0;
                    _context.ConsumerPhones.Add(phone);
                }
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteAll(Int32 consumerId)
        {
            var models = _context.ConsumerPhones.Where(x => x.ConsumerId == consumerId);
            if (models.Count() > 0)
            {
                _context.ConsumerPhones.RemoveRange(models);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Delete(Int32 consumerPhoneId)
        {
            var model = _context.ConsumerPhones.FirstOrDefault(x => x.ConsumerPhoneId == consumerPhoneId);
            if (model != null)
            {
                _context.ConsumerPhones.Remove(model);
                await _context.SaveChangesAsync();
            }
        }
        public int Save(ConsumerPhone newConsumerPhone)
        {
            int result = 0;
            
            ConsumerPhone contextConsumerPhone = _context.ConsumerPhones.FirstOrDefault(x => x.ConsumerPhoneId == newConsumerPhone.ConsumerPhoneId);
            if (contextConsumerPhone != null)
            {
                contextConsumerPhone.ConsumerId = newConsumerPhone.ConsumerId;
                contextConsumerPhone.Extension = newConsumerPhone.Extension;
                contextConsumerPhone.Phone = newConsumerPhone.Phone;
                contextConsumerPhone.PhoneTypeId = newConsumerPhone.PhoneTypeId;
                contextConsumerPhone.Note = newConsumerPhone.Note;
                result = newConsumerPhone.ConsumerPhoneId;
                _context.SaveChanges();
            }
            else
            {
                ConsumerPhone resultPhone = _context.ConsumerPhones.Add(newConsumerPhone);
                _context.SaveChanges();
                result = resultPhone.ConsumerPhoneId;
            }
            
            return result;
        }
    }
}
