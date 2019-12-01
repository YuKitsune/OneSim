using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneSim.Uwp.Pages;

namespace OneSim.Uwp.Models.ViewModels
{
    /// <summary>
    ///     The Default Page View Model.
    /// </summary>
    public class DefaultViewModel
    {
        public ObservableCollection<PageCardItem> Cards { get; set; }

        public DefaultViewModel()
        {
            Cards = new ObservableCollection<PageCardItem>()
            {
                new PageCardItem
                {
                    Name = "Career",
                    Description = "View your current career progression, achievements, and more..."
                },
                new PageCardItem
                {
                    Name = "My Fleet",
                    Description = "Manage your fleet or Aircraft."
                },
                new PageCardItem
                {
                    Name = "Current Jobs",
                    Description = "See currently available jobs."
                },
                new PageCardItem
                {
                    Name = "Social",
                    Description = "Engage with other users.",
                    IsInBeta = true
                },
                new PageCardItem
                {
                    Name = "Map",
                    Description = "See who's currently flying on online networks.",
                    PageType = typeof(MapPage)
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                },
                new PageCardItem
                {
                    Name = "Dummy",
                    Description = "blah.",
                }
            };
        }
    }
}
