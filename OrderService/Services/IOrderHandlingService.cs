﻿using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Services
{
    public interface IOrderHandlingService
    {
        void PlaceOrder(Order order);
    }
}