//-----------------------------------------------------------------------
// <copyright file="AuditCache.cs" company="Jareth Development">
// The contents of these files can be freely used on any project without attribution
// </copyright>
//-----------------------------------------------------------------------
namespace PluginCacheExample
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Caches user information
    /// </summary>
    public class AuditCache
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static AuditCache instance;

        /// <summary>
        /// Users cache - Using concurrent as multiple instances of plugin could be running
        /// </summary>
        private IEnumerable<string> entitiesToAudit;

        /// <summary>
        /// Prevents a default instance of the <see cref="AuditCache"/> class from being created.
        /// </summary>
        private AuditCache()
        {
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static AuditCache Instance
        {
            get
            {
                // If not initialised, set instance
                if (instance == null)
                {
                    instance = new AuditCache();
                }

                return instance;
            }
        }

        /// <summary>
        /// Checks if an entity should be audited
        /// </summary>
        /// <param name="entityName">Logical name of the entity</param>
        /// <param name="service">Organization service</param>
        /// <returns>True if should audit</returns>
        public bool ShouldAudit(string entityName, IOrganizationService service)
        {
            if (this.entitiesToAudit == null)
            {
                var query = new QueryExpression("jd_auditentities");
                this.entitiesToAudit = service.RetrieveMultiple(query).Entities.Select(x => (string)x["jd_name"]);
            }

            return this.entitiesToAudit.Contains(entityName);
        }
    }
}
