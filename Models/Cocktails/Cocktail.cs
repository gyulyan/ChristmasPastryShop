using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Cocktails
{
    public abstract class Cocktail : ICocktail
    {
        private string name;
        private string size;
        private double price;

        protected Cocktail(string name, string size, double price)
        {
            Name = name;
            Size = size;
            Price = price;
        }

        public string Name
        {
            get => name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(ExceptionMessages.NameNullOrWhitespace);
                }
                name = value;
            }
        }

        public string Size
        {
            get => size;
            private set
            {
                size = value;
            }
        }

        public double Price
        {
            get => price;
            private set
            {
                if (this.Size == "Large")
                {
                    price = value;
                }
                else if (this.Size == "Middle")
                {
                    price = 2 / 3 * value;
                }
                else if (this.Size == "Small")
                {
                    price = 1 / 3 * value;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{this.Name} ({this.Size}) - {this.Price:f2} lv");

            return sb.ToString().TrimEnd();
        }
    }
}
