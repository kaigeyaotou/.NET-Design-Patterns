namespace Lunz.ProductCenter.Core.Models.OrderManagement
{
    /// <summary>
    /// ������Ŀ
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        public decimal Total { get; set; }
    }
}