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
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object, null, null, commissionServiceMock.Object);
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
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
            membershipServiceMock.Setup(m => m.ActivateMembership(It.IsAny<Order>()));
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
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
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object, null, null, commissionServiceMock.Object);
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
            Order order = new Order { ProductType = ProductType.Membership };
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
            membershipServiceMock.Setup(m => m.ActivateMembership(It.IsAny<Order>()));
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(null, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we activated a membership
            membershipServiceMock.Verify(m => m.ActivateMembership(It.IsAny<Order>()), Times.Once);
        }
        //If the payment is an upgrade to a membership, apply the upgrade.
        [TestMethod]
        public void MembershipUpgrade_UpgradeApplied()
        {
            Order order = new Order { ProductType = ProductType.MembershipUpgrade };
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
            membershipServiceMock.Setup(m => m.UpgradeMembership(It.IsAny<Order>()));
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(null, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we activated a membership
            membershipServiceMock.Verify(m => m.UpgradeMembership(It.IsAny<Order>()), Times.Once);
        }
        //If the payment is for a membership or upgrade, e-mail the owner and inform them of the activation/upgrade.
        [TestMethod]
        public void MembershipNew_EmailOwner()
        {
            Order order = new Order { ProductType = ProductType.Membership };
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
            membershipServiceMock.Setup(m => m.ActivateMembership(It.IsAny<Order>()));
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(null, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we sent an email
            emailServiceMock.Verify(m => m.SendEmail(It.IsAny<Order>()), Times.Once);
        }
        [TestMethod]
        public void MembershipUpgrade_EmailOwner()
        {
            Order order = new Order { ProductType = ProductType.MembershipUpgrade };
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
            membershipServiceMock.Setup(m => m.UpgradeMembership(It.IsAny<Order>()));
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(null, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we sent an email
            emailServiceMock.Verify(m => m.SendEmail(It.IsAny<Order>()), Times.Once);
        }
        //If the payment is for the video “Learning to Ski,” add a free “First Aid” video to the packing slip (the result of a court decision in 1997).
        [TestMethod]
        public void SkiVideo_AddFirstAidVideo()
        {
            Order order = new Order { ProductType = ProductType.Video, ProductName = "Learning to Ski", SpecialInstructions = string.Empty };
            Mock<IPackingSlipService> packingSlipServiceMock = new Mock<IPackingSlipService>(MockBehavior.Strict);
            string specialInstructions = "Remember to add the First Aid video";
            packingSlipServiceMock.Setup(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>()));
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
            membershipServiceMock.Setup(m => m.UpgradeMembership(It.IsAny<Order>()));
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we added special instructions to add the first aid video
            Assert.IsTrue(order.SpecialInstructions.Contains(specialInstructions));
        }
        //If the payment is for a physical product or a book, generate a commission payment to the agent.
        [TestMethod]
        public void PhysicalProduct_CommissionPaymentCreated()
        {
            Order order = new Order { ProductType = ProductType.Unspecified };
            Mock<IPackingSlipService> packingSlipServiceMock = new Mock<IPackingSlipService>(MockBehavior.Strict);
            packingSlipServiceMock.Setup(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>()));
            Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
            membershipServiceMock.Setup(m => m.UpgradeMembership(It.IsAny<Order>()));
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
            emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
            Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
            commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

            OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
            sut.PlaceOrder(order);

            // verify we created a commission payment
            commissionServiceMock.Verify(m => m.CreateCommissionPayment(It.IsAny<Order>()), Times.Once);
        }
        //[TestMethod]
        //public void Book_CommissionPaymentCreated()
        //{
        //    Order order = new Order { ProductType = ProductType.Book};
        //    Mock<IPackingSlipService> packingSlipServiceMock = new Mock<IPackingSlipService>(MockBehavior.Strict);
        //    packingSlipServiceMock.Setup(m => m.GeneratePackingSlip(It.IsAny<Order>(), It.IsAny<string>()));
        //    Mock<IMembershipService> membershipServiceMock = new Mock<IMembershipService>(MockBehavior.Strict);
        //    membershipServiceMock.Setup(m => m.UpgradeMembership(It.IsAny<Order>()));
        //    Mock<IEmailService> emailServiceMock = new Mock<IEmailService>(MockBehavior.Strict);
        //    emailServiceMock.Setup(m => m.SendEmail(It.IsAny<Order>()));
        //    Mock<ICommissionService> commissionServiceMock = new Mock<ICommissionService>(MockBehavior.Strict);
        //    commissionServiceMock.Setup(m => m.CreateCommissionPayment(It.IsAny<Order>()));

        //    OrderHandlingService sut = new OrderHandlingService(packingSlipServiceMock.Object, membershipServiceMock.Object, emailServiceMock.Object, commissionServiceMock.Object);
        //    sut.PlaceOrder(order);

        //    // verify we created a commission payment
        //    commissionServiceMock.Verify(m => m.CreateCommissionPayment(It.IsAny<Order>()), Times.Once);
        //}

        // TODO: what about Video? maybe needs commission payment too
    }
}
