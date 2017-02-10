//-----------------------------------------------------------------------
// <copyright file="UserCache.cs" company="Jareth Development">
// The contents of these files can be freely used on any project without attribution
// </copyright>
//-----------------------------------------------------------------------
namespace PluginCacheExample
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Caches user information
    /// </summary>
    public class UserCache
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static UserCache instance;

        /// <summary>
        /// Users cache - Using concurrent as multiple instances of plugin could be running
        /// </summary>
        private ConcurrentDictionary<Guid, Entity> users;

        /// <summary>
        /// Prevents a default instance of the <see cref="UserCache"/> class from being created.
        /// </summary>
        private UserCache()
        {
            this.users = new ConcurrentDictionary<Guid, Entity>();
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static UserCache Instance
        {
            get
            {
                // If not initialised, set instance
                if (instance == null)
                {
                    instance = new UserCache();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets a user from the cache or CRM
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="service">Organization service</param>
        /// <returns>User entity</returns>
        public Entity GetUser(Guid id, IOrganizationService service)
        {
            // Check for user in cache
            if (this.users.ContainsKey(id))
            {
                return this.users[id];
            }

            // Get user from CRM
            var user = service.Retrieve("systemuser", id, new ColumnSet("domainname"));

            // Add to dictionary
            this.users.TryAdd(id, user);

            return user;
        }
    }
}
