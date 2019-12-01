using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSim.Uwp.Models
{
    public class PageCardItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

        public string BadgeString
        {
            get
            {
                if (IsInBeta) return "BETA";
                return null;
            }
        }

        public bool IsInBeta { get; set; }

        public Type PageType { get; set; }

        public PageCardItem()
        {
            ImagePath = null;
            IsInBeta = false;
        }
    }
}
