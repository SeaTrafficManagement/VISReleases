using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using STM.Common.DataAccess.Entities;
using STM.Common.DataAccess;
using STM.SSC.Internal;
using STM.Common.Services.Internal.Interfaces;
using STM.Common;
using Newtonsoft.Json;

namespace STM.Common.Services.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityService : InternalServiceBase<Identity, StmDbContext>, IIdentityService
    {
        private ISccPrivateService _sscService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="sscService"></param>
        public IdentityService(StmDbContext dbContext,
            ISccPrivateService sscService) : base(dbContext)
        {
            _sscService = sscService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Identity GetCallerIdentity()
        {
            // Get id for caller
            var identity = Get(x => x.UID == InstanceContext.CallerOrgId).FirstOrDefault();
            if (identity == null)
            {
                var fiResponse = _sscService.FindIdentities();

                if (fiResponse.StatusCode == 200
                    && fiResponse != null && fiResponse.Organizations != null)
                {
                    var orgs = fiResponse.Organizations;
                    var org = orgs.FirstOrDefault(x => x.Mrn == InstanceContext.CallerOrgId);
                    if (org == null)
                    {
                        throw new Exception("Unable to lookup identity in ID registry");
                    }

                    identity = new Identity
                    {
                        UID = org.Mrn,
                        Name = org.Name
                    };
                    Insert(identity);
                }
                else
                {
                    throw new Exception("Unable to lookup identity in ID registry");
                }
            }

            return identity;
        }
    }
}
