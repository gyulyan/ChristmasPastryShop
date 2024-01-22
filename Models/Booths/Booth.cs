using ChristmasPastryShop.Models.Booths.Contracts;
using ChristmasPastryShop.Models.Cocktails;
using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using ChristmasPastryShop.Repositories.Contracts;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Booths
{
    public class Booth : IBooth
    {
        private int capacity;
        private DelicacyRepository delicacyMenu;
        private CocktailRepository cocktailMenu;
        private double currentBill;
        private double turnover;
        private bool isReserved;

        public Booth(int boothId, int capacity)
        {
            BoothId = boothId;
            Capacity = capacity;
            CurrentBill = 0;
            Turnover = 0;
            IsReserved = false;
            delicacyMenu = new DelicacyRepository();
            cocktailMenu = new CocktailRepository();
        }

        public int BoothId
        {
            get;
            private set;
        }

        public int Capacity
        {
            get => capacity;
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentException(ExceptionMessages.CapacityLessThanOne);
                }
                capacity = value;
            }
        }

        public IRepository<IDelicacy> DelicacyMenu => delicacyMenu;

        public IRepository<ICocktail> CocktailMenu => cocktailMenu;


        public double CurrentBill
        {
            get => currentBill;
            set
            {
                currentBill = value;
            }
        }

        public double Turnover
        {
            get => turnover;
            set
            {
                turnover = value;
            }
        }

        public bool IsReserved
        {
            get => isReserved;
            set
            {
                isReserved = value;
            }
        }

        public void ChangeStatus()
        {
            isReserved = !isReserved;
        }

        public void Charge()
        {
            turnover += currentBill;
            currentBill = 0;
        }

        public void UpdateCurrentBill(double amount)
        {
            currentBill += amount;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Booth: {BoothId}");
            sb.AppendLine($"Capacity: {Capacity}");
            sb.AppendLine($"Turnover: {this.turnover:f2} lv");
            sb.AppendLine("- Cocktail menu:");
            
            foreach (var cocktail in cocktailMenu.Models)
            {
                sb.AppendLine(cocktail.ToString());
            }
            sb.AppendLine("-Delicacy menu: ");

            foreach (var delicacy in delicacyMenu.Models)
            {
                sb.AppendLine(delicacy.ToString());
            }

            return sb.ToString().TrimEnd();
        }
    }
}
