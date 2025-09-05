using MECWeb.Components.Shared;
using MudBlazor;

namespace MECWeb.Services
{
    public class PurchaseCardService
    {
        public PurchaseCardService() { }

        public List<PurchaseCardViewModel> GetPurchaseCards()
        {
            List<PurchaseCardViewModel> purchaseCards = new List<PurchaseCardViewModel>()
            {
                new PurchaseCardViewModel
                {
                    Icon = Icons.Material.Filled.Computer,
                    HeaderText = "Rechner Bestellungen",
                    Category = "Rechner Bestellung",
                    HeaderDescription = "Bestellwesen",
                    Image = "/assets/homePicture.png",
                    Description = "Übersicht aller BDR & BV Rechner Bestellungen ",
                    Href = "/purchase/orders"
                },

                // Weitere Einkaufs-Karten können hier hinzugefügt werden:
                // new PurchaseCardViewModel
                // {
                //     Icon = Icons.Material.Filled.Inventory,
                //     HeaderText = "Lager Verwaltung", 
                //     Category = "Lager",
                //     HeaderDescription = "Bestandsführung",
                //     Image = "/assets/homePicture.png",
                //     Description = "Verwaltung des Hardware-Lagers und Bestandsübersicht",
                //     Href = "/purchase/inventory"
                // }
            };

            return purchaseCards;
        }

        public Dictionary<string, List<PurchaseCardViewModel>> GetPurchaseCardsCategorized()
        {
            Dictionary<string, List<PurchaseCardViewModel>> cards = new Dictionary<string, List<PurchaseCardViewModel>>();

            foreach (var item in this.GetPurchaseCards())
            {
                if (cards.TryGetValue(item.Category, out List<PurchaseCardViewModel>? cardList))
                {
                    if (cardList == null)
                        cards[item.Category] = new List<PurchaseCardViewModel>() { item };
                    else
                        cards[item.Category].Add(item);
                }
                else
                {
                    cards.Add(item.Category, new List<PurchaseCardViewModel>() { item });
                }
            }

            return cards;
        }
    }
}