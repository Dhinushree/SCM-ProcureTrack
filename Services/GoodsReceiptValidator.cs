
namespace DotNetAssignmentApp.Services
{
    public class GoodsReceiptValidator
    {
        public bool IsReceiptQuantityValid(
            decimal received,
            decimal accepted,
            decimal rejected)
        {
            if (received < 0 || accepted < 0 || rejected < 0)
                return false;

            return accepted + rejected == received;
        }

        public bool IsAcceptedQuantityWithinOrder(
            decimal orderedQuantity,
            decimal previouslyAccepted,
            decimal newlyAccepted)
        {
            if (orderedQuantity < 0 ||
                previouslyAccepted < 0 ||
                newlyAccepted < 0)
                return false;

            return previouslyAccepted + newlyAccepted
                   <= orderedQuantity;
        }
    }
}
