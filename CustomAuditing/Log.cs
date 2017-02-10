//-----------------------------------------------------------------------
// <copyright file="Log.cs" company="Jareth Development">
// The contents of these files can be freely used on any project without attribution
// </copyright>
//-----------------------------------------------------------------------

namespace CustomAuditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Audit log
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class
        /// </summary>
        public Log()
        {
            this.ChangedValues = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the message name
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the Date time of the log
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the Id of the user who executed the plugin
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the Username of the user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Name of the entity
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        /// Gets or sets the Id of the entity
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the List of values which have changed
        /// </summary>
        public Dictionary<string, string> ChangedValues { get; set; }
    }
}
