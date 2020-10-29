using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrderServiceTest
{
    [TestClass]
    public class OrderServicePaymentTest
    {
        //If the payment is for a physical product, generate a packing slip for shipping.
        [TestMethod]
        public void PhysicalProduct_PackingSlipCreated()
        {
            Assert.Inconclusive();
        }
        //If the payment is for a book, create a duplicate packing slip for the royalty department.
        [TestMethod]
        public void Book_RoyaltyPackingSlipCreated()
        {
            Assert.Inconclusive();
        }
        //If the payment is for a membership, activate that membership.
        [TestMethod]
        public void MembershipNew_MembershipActivated()
        {
            Assert.Inconclusive();
        }
        //If the payment is an upgrade to a membership, apply the upgrade.
        [TestMethod]
        public void MembershipUpgrade_UpgradeApplied()
        {
            Assert.Inconclusive();
        }
        //If the payment is for a membership or upgrade, e-mail the owner and inform them of the activation/upgrade.
        [TestMethod]
        public void MembershipPayment_EmailOwner()
        {
            Assert.Inconclusive();
        }
        public void MembershipUpgrade_EmailOwner()
        {
            Assert.Inconclusive();
        }
        //If the payment is for the video “Learning to Ski,” add a free “First Aid” video to the packing slip (the result of a court decision in 1997).
        [TestMethod]
        public void SkiVideo_AddFirstAidVideo()
        {
            Assert.Inconclusive();
        }
        //If the payment is for a physical product or a book, generate a commission payment to the agent.
        [TestMethod]
        public void PhysicalProduct_CommissionPaymentCreated()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void Book_CommissionPaymentCreated()
        {
            Assert.Inconclusive();
        }
    }
}
