using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderService.Models;
using OrderService.Services;
using Moq;
using System;

namespace OrderServiceTest
{
    [TestClass]
    public class OrderHandlingServiceTest
    {
        //If the payment is for a physical product, generate a packing slip for shipping.
        [TestMethod]
        public void PhysicalProduct_PackingSlipCreated()
        {
            string originalAddress = Guid.NewGuid().ToString();
            Order order = new Order { ProductType = ProductType.Unspecified, ShippingAddress = originalAddress };
            string address = null;
            Mock<IPackingSlipService> packingSlipServiceMock = new Mock<IPackingSlipService>(MockBehavior.Strict);
            packingSlipServiceMock.Setup(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>())).Callback<Order, string>((o, a) => address = a);

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object);
            sut.PlaceOrder(order);
            // TODO: we have a similar pattern in the following tests - refactor the above lines into a test helper class

            // verify we created a packing slip
            packingSlipServiceMock.Verify(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>()), Times.Once);
            
            // verify the correct address was passed through...we might do that in a separate test but here for now
            Assert.AreEqual(originalAddress, address, "address");
        }

        [TestMethod]
        public void NotPhysicalProduct_NoPackingSlipCreated()
        {
            Order order = new Order { ProductType = ProductType.Membership };
            Mock<IPackingSlipService> packingSlipServiceMock = new Mock<IPackingSlipService>(MockBehavior.Strict);

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we did not create a packing slip
            packingSlipServiceMock.Verify(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>()), Times.Never);

        }
        //If the payment is for a book, create a duplicate packing slip for the royalty department.
        [TestMethod]
        public void Book_RoyaltyPackingSlipCreated()
        {
            string originalAddress = Guid.NewGuid().ToString();
            Order order = new Order { ProductType = ProductType.Book, ShippingAddress = originalAddress };
            string address = null;
            Mock<IPackingSlipService> packingSlipServiceMock = new Mock<IPackingSlipService>(MockBehavior.Strict);
            packingSlipServiceMock.Setup(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>())).Callback<Order, string>((o, a) => address = a);

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we created 2 packing slips
            packingSlipServiceMock.Verify(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>()), Times.Exactly(2));

            // verify the royalty dept address
            Assert.AreEqual(OrderHandlingService.RoyaltyDepartment, address, "address");
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
