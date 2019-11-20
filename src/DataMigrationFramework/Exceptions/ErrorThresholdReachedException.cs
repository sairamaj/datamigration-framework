using System;
using System.Runtime.Serialization;

namespace DataMigrationFramework.Exceptions
{
    /// <summary>
    /// Represents error threshold exception.
    /// </summary>
    [Serializable]
    public class ErrorThresholdReachedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorThresholdReachedException"/> class.
        /// </summary>
        public ErrorThresholdReachedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorThresholdReachedException"/> class.
        /// </summary>
        /// <param name="message">
        /// Exception message.
        /// </param>
        /// <param name="errorCount">
        /// Error count.
        /// </param>
        /// <param name="errorThreshold">
        /// Error threshold.
        /// </param>
        public ErrorThresholdReachedException(string message,int errorCount, int errorThreshold)
            : base(message)
        {
            this.ErrorCount = errorCount;
            this.ErrorThreshold = errorThreshold;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorThresholdReachedException"/> class.
        /// </summary>
        /// <param name="message">
        /// Exception message.
        /// </param>
        /// <param name="inner">
        /// A <see cref="Exception"/> inner exception.
        /// </param>
        public ErrorThresholdReachedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorThresholdReachedException"/> class.
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo"/> instance.
        /// </param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> instance.
        /// </param>
        protected ErrorThresholdReachedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets message string.
        /// </summary>
        public override string Message => base.Message + Environment.NewLine + $"Errors: {this.ErrorCount} Threshold: {this.ErrorThreshold}";

        /// <summary>
        /// Gets error count.
        /// </summary>
        public int ErrorCount { get; }

        /// <summary>
        /// Gets error threshold count.
        /// </summary>
        public int ErrorThreshold { get; }
    }
}