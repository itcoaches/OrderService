﻿using OrderService.Models;
using System;
using System.Collections.Generic;

namespace OrderService.Services
{
    public class OrderHandlingService : IOrderHandlingService
    {
        public static string RoyaltyDepartment = "Royalty Department";    // TODO: make this a config setting
        private readonly IPackingSlipService _packingSlipService;
        private readonly IMembershipService _membershipService;
        private readonly IEmailService _emailService;

        private List<Action<Order>> _rules = new List<Action<Order>>();

        public OrderHandlingService(IPackingSlipService packingSlipService, IMembershipService membershipService, IEmailService emailService)
        {
            _packingSlipService = packingSlipService;
            _membershipService = membershipService;
            _emailService = emailService;

            AddRules();
        }

        public void PlaceOrder(Order order)
        {
            foreach (Action<Order> rule in _rules)
                rule.Invoke(order);
        }

        private void AddRules()
        {
            _rules.Add((Order o) => { if (o.IsPhysical) _packingSlipService.GeneratePackingSlip(o, o.ShippingAddress); });
            _rules.Add((Order o) => { if (o.ProductType == ProductType.Book) _packingSlipService.GeneratePackingSlip(o, RoyaltyDepartment); });
            _rules.Add((Order o) => { if (o.ProductType == ProductType.Membership) _membershipService.ActivateMembership(o); });
            _rules.Add((Order o) => { if (o.ProductType == ProductType.Membership) _emailService.SendEmail(o); });
            _rules.Add((Order o) => { if (o.ProductType == ProductType.MembershipUpgrade) _membershipService.UpgradeMembership(o); });
            _rules.Add((Order o) => { if (o.ProductType == ProductType.MembershipUpgrade) _emailService.SendEmail(o); });
        }
    }
}
