using System;
using System.Runtime.Serialization;

namespace DataMigrationFramework.Exceptions
{
    /// <summary>
    /// Represents error threshold exception.
    /// </summary>
    [Serializable]
    public class MaxLimitReachedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLimitReachedException"/> class.
        /// </summary>
        public MaxLimitReachedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLimitReachedException"/> class.
        /// </summary>
        /// <param name="message">
        /// Exception message.
        /// </param>
        /// <param name="currentRecordCount">
        /// Current record count.
        /// </param>
        /// <param name="maxLimit">
        /// Maximum limit value.
        /// </param>
        public MaxLimitReachedException(string message, int currentRecordCount, int maxLimit)
            : base(message)
        {
            this.CurrentRecordCount = currentRecordCount;
            this.MaxLimit = maxLimit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLimitReachedException"/> class.
        /// </summary>
        /// <param name="message">
        /// Exception message.
        /// </param>
        /// <param name="inner">
        /// A <see cref="Exception"/> inner exception.
        /// </param>
        public MaxLimitReachedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLimitReachedException"/> class.
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo"/> instance.
        /// </param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> instance.
        /// </param>
        protected MaxLimitReachedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets message string.
        /// </summary>
        public override string Message => base.Message + Environment.NewLine + $"Current: {this.CurrentRecordCount} MaxLimit: {this.MaxLimit}";

        /// <summary>
        /// Gets error count.
        /// </summary>
        public int CurrentRecordCount { get; }

        /// <summary>
        /// Gets error threshold count.
        /// </summary>
        public int MaxLimit { get; }
    }
}