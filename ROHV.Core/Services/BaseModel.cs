using ROHV.Core.Database;

namespace ROHV.Core.Services
{
    public class BaseModel
    {
        protected RayimContext _context;
        public BaseModel(RayimContext context)
        {
            this._context = context;
        }
    }
}
