﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRiak.EjCustomer
{
    public class OrderSummary
    {
        private readonly IList<OrderSummaryItem> summaries = new List<OrderSummaryItem>();
        private uint customerId;

        public OrderSummary(uint customerId)
        {
            if (customerId == default(uint))
            {
                throw new ArgumentOutOfRangeException("customerId", "customerId must be greater than 0");
            }
            this.customerId = customerId;
        }

        public uint CustomerId
        {
            get { return customerId; }
        }

        public IEnumerable<OrderSummaryItem> Summaries
        {
            get { return summaries; }
        }

        public void AddSummaryItem(OrderSummaryItem orderSummaryItem)
        {
            if (orderSummaryItem == null)
            {
                throw new ArgumentNullException("orderSummaryItem");
            }
            this.summaries.Add(orderSummaryItem);
        }

    }
}
