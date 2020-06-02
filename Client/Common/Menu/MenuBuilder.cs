using Microsoft.AspNetCore.Components.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SectorModel.Client.Common.Menu
{
    public class MenuBuilder
    {
        private readonly List<MenuItem> _menuItems;

        public MenuBuilder()
        {
            _menuItems = new List<MenuItem>();
        }

        public MenuBuilder AddItem(int position, string title, string link, NavLinkMatch match = NavLinkMatch.Prefix, bool isVisible = true, bool isEnabled = true)
        {
            var menuItem = new MenuItem
            {
                Position = position,
                Title = title,
                Link = link,
                Match = match,
                IsVisible = isVisible,
                IsEnabled = isEnabled
            };

            _menuItems.Add(menuItem);

            return this;
        }

        public List<MenuItem> Build(Func<MenuItem, int> orderBy)
        {
            var menuItems = _menuItems.OrderBy(orderBy);

            return menuItems.ToList();
        }
    }

    public class MenuItem
    {
        public int Position { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public NavLinkMatch Match { get; set; }
        public MenuBuilder MenuItems { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }
    }
}