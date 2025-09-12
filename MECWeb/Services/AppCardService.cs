using MECWeb.Components.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using MudBlazor;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace MECWeb.Services
{

    public class AppCardService
    {

        public AppCardService() { }


        public List<AppCardViewModel> GetAppCards(Guid projectId)
        {
            if (projectId.Equals(Guid.Empty))
                return new List<AppCardViewModel>();

            List<AppCardViewModel> appCards = new()
            {

                new AppCardViewModel
                {
                    Icon = @Icons.Material.Filled.FormatAlignJustify,
                    HeaderText = "BDR & BV",
                    Category = "Formulare",
                    HeaderDescription = "Formular",
                    Image = "/assets/homePicture.png",
                    Description = "Bedienungs & Bildverarbeitungs Rechner Formular ",
                    Href = $"project/{projectId}/forms"
                },

                new AppCardViewModel
                {
                    Icon = Icons.Material.Filled.FormatAlignJustify,
                    HeaderText = "Einkauf",
                    Category = "Einkauf",
                    HeaderDescription = "Freigabe der BDR & BV Formulare",
                    Image = "/assets/homePicture.png",
                    Description = "Weiterleitung freigabe an die Installation",
                    Href = $"/purchase/orders"
                },

               new AppCardViewModel
                {
                    Icon = Icons.Material.Filled.FormatAlignCenter,
                    HeaderText = "Installation",
                    Category = "Installation",
                    HeaderDescription = "Installation",
                    Image = "/assets/homePicture.png",
                    Description = "Einrichtung der BDR & BV Rechner",
                    Href = "/installation/orders"
                },

                new AppCardViewModel
                {
                    Icon = Icons.Material.Filled.Translate,
                    HeaderText = "Übersetzung",
                    Category = "Inbetriebnahme",
                    HeaderDescription = "Inbetriebnahme",
                    Image = "/assets/homePicture.png",
                    Description = "Übersetzung von TIA / STEP7 / EPlan Projekten. Sowie von eigenerstellte Excel Dateien",
                    Href = $"project/{projectId}/translate"
                },

                // ✅ NEU: SOFTWARE ARCHIV APP CARD
                new AppCardViewModel
                {
                    Icon = Icons.Material.Filled.Archive,
                    HeaderText = "Software Archiv",
                    Category = "Repository",
                    HeaderDescription = "Git Repository Management",
                    Image = "/assets/homePicture.png",
                    Description = "Verwaltung von Software-Projekten, Dokumenten und Versionskontrolle .",
                    Href = $"project/{projectId}/repository"
                },

                //new AppCardViewModel
                //{
                //    Icon = Icons.Material.Filled.Translate,
                //    HeaderText = "Übersetzung",
                //    Category = "Inbetriebnahme",
                //    HeaderDescription = "Inbetriebnahme",
                //    Image = "/assets/homePicture.png",
                //    Description = "Übersetzung von TIA / STEP7 / EPlan Projekten. Sowie von eigenerstellte Excel Dateien",
                //    Href = $"project/{projectId}/translate"
                //},

                                //new AppCardViewModel
                //{
                //    Icon = Icons.Material.Filled.Translate,
                //    HeaderText = "Übersetzung",
                //    Category = "Inbetriebnahme",
                //    HeaderDescription = "Inbetriebnahme",
                //    Image = "/assets/homePicture.png",
                //    Description = "Übersetzung von TIA / STEP7 / EPlan Projekten. Sowie von eigenerstellte Excel Dateien",
                //    Href = $"project/{projectId}/translate"
                //},

            };

            return appCards;
        }

        public Dictionary<string, List<AppCardViewModel>> GetAppCardsCategorized(Guid projectId)
        {
            Dictionary<string, List<AppCardViewModel>> cards = new();

            foreach (var item in this.GetAppCards(projectId))
            {
                if (cards.TryGetValue(item.Category, out List<AppCardViewModel>? cardList))
                {
                    if (cardList == null)
                        cards[item.Category] = new List<AppCardViewModel>() { item };
                    else
                        cards[item.Category].Add(item);
                }
                else
                {
                    cards.Add(item.Category, new List<AppCardViewModel>() { item });
                }
            }

            return cards;
        }
    }
}