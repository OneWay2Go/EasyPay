namespace EasyPay.Core.Enums;

public enum OrderStatus
{
    PendingPayment,
    InEscrow, // Buyer paid, but seller has not delivered
    Delivered, // Seller marked as done
    Completed, // Buyer confirmed receipt
    Canceled,
    Disputed // Support needs to intervene
}