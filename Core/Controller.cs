using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ChristmasPastryShop.Core.Contracts;
using ChristmasPastryShop.Models.Booths;
using ChristmasPastryShop.Models.Booths.Contracts;
using ChristmasPastryShop.Models.Cocktails;
using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Models.Delicacies;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using ChristmasPastryShop.Utilities.Messages;

namespace ChristmasPastryShop.Core
{
    public class Controller : IController
    {
        private BoothRepository booths;
        public Controller()
        {
            booths = new BoothRepository();
        }

        public string AddBooth(int capacity)
        {
            int count = booths.Models.Count;
            var booth = new Booth(count + 1, capacity);
            booths.AddModel(booth);
            return string.Format(OutputMessages.NewBoothAdded, count + 1, capacity);
        }

        public string AddCocktail(int boothId, string cocktailTypeName, string cocktailName, string size)
        {
            IBooth booth = booths.Models.FirstOrDefault(b => b.BoothId == boothId);
            if (cocktailTypeName != "Hibernation" && cocktailTypeName != "MullenWine")
            {
                return string.Format(OutputMessages.InvalidCocktailType, cocktailTypeName);
            }

            if (size != "Large" && size != "Small" && size != "Middle")
            {
                return string.Format(OutputMessages.InvalidCocktailSize, size);
            }

            if (booth.CocktailMenu.Models.Any(c => c.Name == cocktailName && c.Size == size))
            {
                return string.Format(OutputMessages.CocktailAlreadyAdded, size, cocktailName);
            }

            ICocktail cocktail = null;

            if (cocktailTypeName == "Hibernation")
            {
                cocktail = new Hibernation(cocktailName, size);
            }
            else if (cocktailTypeName == "MulledWine")
            {
                cocktail = new MulledWine(cocktailName, size);
            }

            booth.CocktailMenu.AddModel(cocktail);

            return string.Format(OutputMessages.NewCocktailAdded, size, cocktailName, cocktailTypeName);
        }

        public string AddDelicacy(int boothId, string delicacyTypeName, string delicacyName)
        {
            IBooth booth = booths.Models.FirstOrDefault(b => b.BoothId == boothId);

            if (delicacyTypeName != "Stolen" && delicacyTypeName != "Gingerbread")
            {
                return string.Format(OutputMessages.InvalidDelicacyType, delicacyTypeName);
            }

            if (booth.DelicacyMenu.Models.Any(d => d.Name == delicacyName))
            {
                return string.Format(OutputMessages.DelicacyAlreadyAdded, delicacyName);
            }

            IDelicacy delicacy = null;

            if (delicacyTypeName == "Stolen")
            {
                delicacy = new Stolen(delicacyName);
            }
            else if (delicacyTypeName == "Gingerbread")
            {
                delicacy = new Gingerbread(delicacyName);
            }

            booth.DelicacyMenu.AddModel(delicacy);

            return string.Format(OutputMessages.NewDelicacyAdded, delicacyTypeName, delicacyName);
        }

        public string BoothReport(int boothId)
        {
            IBooth booth = booths.Models.FirstOrDefault(b => b.BoothId == boothId);
           return booth.ToString().TrimEnd();
        }

        public string LeaveBooth(int boothId)
        {
            IBooth booth = booths.Models.FirstOrDefault(b => b.BoothId == boothId);
            booth.Charge();
            booth.ChangeStatus();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Bill {booth.CurrentBill:f2} lv");
            sb.AppendLine($"Booth {boothId} is now available!");

            return sb.ToString().TrimEnd();
        }

        public string ReserveBooth(int countOfPeople)
        {
            var orderderBooths = booths.Models
                  .Where(x => x.IsReserved == false && x.Capacity >= countOfPeople)
                  .OrderBy(x => x.Capacity)
                  .OrderByDescending(x => x.BoothId)
                  .ToList();

            if (orderderBooths.Count == 0)
            {
                return string.Format(OutputMessages.NoAvailableBooth, countOfPeople);
            }

            var firstBooth = orderderBooths[0];
            firstBooth.ChangeStatus();

            return string.Format(OutputMessages.BoothReservedSuccessfully, firstBooth.BoothId, countOfPeople);
        }

        public string TryOrder(int boothId, string order)
        {
            string[] args = order.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string itemTypeName = args[0];
            string itemName = args[1];
            int countOfOrderedPieces = int.Parse(args[2]);

            if (itemTypeName.GetType().Name == "ICocktail")
            {
                string size = args[3];
            }

            IBooth booth = booths.Models.FirstOrDefault(b => b.BoothId == boothId);

            if (itemTypeName == "Gingerbread" || itemTypeName == "Stolen")
            {
                if (!booth.DelicacyMenu.Models.Any(d => d.Name == itemName))
                {
                    return string.Format(OutputMessages.DelicacyStillNotAdded, itemTypeName, itemName);
                }

                if (!booth.DelicacyMenu.Models.Any(c => c.GetType().Name == itemTypeName && c.Name == itemName))
                {
                    return string.Format(OutputMessages.DelicacyStillNotAdded, itemTypeName, itemName);
                }

                var delicacy = booth.DelicacyMenu.Models.FirstOrDefault(c => c.Name == itemName);

                booth.UpdateCurrentBill(delicacy.Price * countOfOrderedPieces);

            }
            else if (itemTypeName == "Hibernation" || itemTypeName == "MulledWine")
            {
                if (!booth.CocktailMenu.Models.Any(c => c.Name == itemName))
                {
                    return string.Format(OutputMessages.DelicacyStillNotAdded, itemTypeName, itemName);
                }

                string size = args[3];

                if (!booth.CocktailMenu.Models.Any(c => c.GetType().Name == itemTypeName && c.Name == itemName && c.Size == size))
                {
                    return string.Format(OutputMessages.CocktailStillNotAdded, size, itemName);
                }

                var cocktail = booth.CocktailMenu.Models.FirstOrDefault(c => c.Name == itemName && c.Size == size);

                booth.UpdateCurrentBill(cocktail.Price * countOfOrderedPieces);
            }
            else
            {
                return string.Format(OutputMessages.NotRecognizedType, itemTypeName);
            }

            return string.Format(OutputMessages.SuccessfullyOrdered, boothId, countOfOrderedPieces, itemName);
        }
    }
}
