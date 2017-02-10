//-----------------------------------------------------------------------
// <copyright file="Audit.cs" company="Jareth Development">
// The contents of these files can be freely used on any project without attribution
// </copyright>
//-----------------------------------------------------------------------

namespace CustomAuditing
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using Microsoft.Xrm.Sdk;
    using PluginCacheExample;

    /// <summary>
    /// PluginEntryPoint plug-in.
    /// This is a generic entry point for a plug-in class. Use the Plug-in Registration tool found in the CRM SDK to register this class, import the assembly into CRM, and then create step associations.
    /// A given plug-in can have any number of steps associated with it. 
    /// </summary>    
    public class Audit : PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Audit"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information. 
        /// When using Microsoft Dynamics CRM for Outlook with Offline Access, 
        /// the secure string is not passed to a plug-in that executes while the client is offline.</param>
        public Audit(string unsecure, string secure)
            : base(typeof(Audit))
        {
            // TODO: Implement your custom configuration handling.
        }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new ArgumentNullException("localContext");
            }

            var context = localContext.PluginExecutionContext;

            if (AuditCache.Instance.ShouldAudit(context.PrimaryEntityName, localContext.OrganizationService))
            {
                var entity = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];

                // Setup log
                var log = new Log
                {
                    DateTime = DateTime.Now,
                    Entity = context.PrimaryEntityName,
                    Id = context.PrimaryEntityId,
                    Message = context.MessageName,
                    UserId = context.UserId,
                    UserName = UserCache.Instance.GetUser(context.UserId, localContext.OrganizationService)["domainname"].ToString()
                };

                if (context.MessageName != "Retrieve" && context.MessageName != "RetrieveMultiple")
                {
                    // Get changed properties
                    foreach (var attribute in entity.Attributes)
                    {
                        log.ChangedValues.Add(attribute.Key, this.ConvertValueToString(attribute.Value));
                    }
                }

                this.SendLog(log);
            }
        }

        /// <summary>
        /// Sends a log somewhere
        /// </summary>
        /// <param name="log">The log</param>
        private void SendLog(Log log)
        {
            var serializer = new DataContractJsonSerializer(typeof(Log));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, log);

                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();

                    // Send to web service using WebRequest
                }
            }
        }

        /// <summary>
        /// Converts a CRM attribute to a string (Incomplete)
        /// </summary>
        /// <param name="value">Object to convert</param>
        /// <returns>String value</returns>
        private string ConvertValueToString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            switch (value.GetType().Name)
            {
                case "Boolean":
                    return ((bool)value) ? "Yes" : "No";
                case "DateTime":
                    return ((DateTime)value).ToShortDateString();
                case "Money":
                    return ((Money)value).Value.ToString(CultureInfo.InvariantCulture);

                case "State":
                case "Status":
                case "Picklist":
                case "OptionSetValue":
                    return ((OptionSetValue)value).Value.ToString();

                case "BigInt":
                case "Decimal":
                case "Double":
                case "Integer":
                case "Memo":
                case "String":
                case "Uniqueidentifier":
                    return value.ToString();

                case "Owner":
                case "Customer":
                case "Lookup":
                case "EntityReference":
                    EntityReference reference = (EntityReference)value;
                    return reference.Id.ToString();

                default:
                    throw new InvalidPluginExecutionException("Type Not Found: Type:" + value.GetType().Name + " Value: " + value);
            }
        }
    }
}
