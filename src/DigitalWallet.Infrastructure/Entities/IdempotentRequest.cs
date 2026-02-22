namespace DigitalWallet.Infrastructure.Entities
{
    /// <summary>
    /// Represents a processed or in-progress idempotent request.
    /// Used by IdempotencyService to ensure exactly-once execution.
    /// </summary>
    public class IdempotentRequest
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// Unique idempotency key (scoped per user).
        /// </summary>
        public string Key { get; set; } = string.Empty;
        
        /// <summary>
        /// Hash of the request payload to detect key reuse with different data.
        /// </summary>
        public string RequestHash { get; set; } = string.Empty;
        
        /// <summary>
        /// JSON‑serialized response, once the request is processed.
        /// </summary>
        public string Response { get; set; } = string.Empty;
        
        /// <summary>
        /// Indicates whether processing has completed successfully.
        /// If false, the request is still being processed (locked).
        /// </summary>
        public bool IsProcessed { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        
        /// <summary>
        /// Concurrency token to prevent race conditions (row version).
        /// </summary>
        public byte[] ConcurrencyToken { get; set; } = Array.Empty<byte>();
    }
}