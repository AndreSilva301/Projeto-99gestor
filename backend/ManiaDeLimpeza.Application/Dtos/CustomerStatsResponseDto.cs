namespace ManiaDeLimpeza.Application.Dtos
{
    /// <summary>
    /// DTO for customer statistics
    /// </summary>
    public class CustomerStatsResponseDto
    {
        /// <summary>
        /// Total number of customers (excluding deleted)
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Number of active customers
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// Number of inactive (deleted) customers
        /// </summary>
        public int Inactive { get; set; }

        /// <summary>
        /// Number of customers created in the current month
        /// </summary>
        public int NewThisMonth { get; set; }
    }
}
