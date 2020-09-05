using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRiak.EjCustomer
{
    public class Item
    {
        private readonly string id;
        private readonly string title;
        private readonly decimal price;

        public Item(string id, string title, decimal price)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }
            this.id = id;

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException("title");
            }
            this.title = title;

            this.price = price;
        }

        public string Id
        {
            get { return id; }
        }

        public string Title
        {
            get { return title; }
        }

        public decimal Price
        {
            get { return price; }
        }

    }
}
