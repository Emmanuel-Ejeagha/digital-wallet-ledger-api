namespace DigitalWallet.Application.Common.Models
{
    /// <summary>
    /// Represents an idempotency key for deduplication
    /// CRITICAL: Prevents duplicate processing of financial operations
    /// </summary>
    public class IdempotencyKey
    {
        public string Key { get; }
        public string OperationName { get; }
        public string RequestHash { get; }
        public DateTime CreatedAtUtc { get; }
        public DateTime? CompletedAtUtc { get; private set; }
        public string? ResponseBody { get; private set; }
        public int? StatusCode { get; private set; }
        public bool IsCompleted => CompletedAtUtc.HasValue;

        private IdempotencyKey(
            string key, 
            string operationName, 
            string requestHash, 
            DateTime createdAtUtc)
        {
            Key = key;
            OperationName = operationName;
            RequestHash = requestHash;
            CreatedAtUtc = createdAtUtc;
        }

        public static IdempotencyKey Create(
            string key,
            string operationName,
            string requestHash,
            DateTime createdAtUtc)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Idempotency key cannot be empty", nameof(key));
            
            if (string.IsNullOrWhiteSpace(operationName))
                throw new ArgumentException("Operation name cannot be empty", nameof(operationName));

            return new IdempotencyKey(key, operationName, requestHash, createdAtUtc);
        }

        public void Complete(string responseBody, int statusCode)
        {
            if (IsCompleted)
                throw new InvalidOperationException("Idempotency key already completed");

            ResponseBody = responseBody;
            StatusCode = statusCode;
            CompletedAtUtc = DateTime.UtcNow;
        }

        // Hash the request content to detect identical requests
        public static string ComputeRequestHash(string requestBody)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(requestBody);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}