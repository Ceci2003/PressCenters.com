﻿namespace PressCenters.Data.Models
{
    using System.Collections.Generic;

    using PressCenters.Data.Common.Models;

    public class Source : BaseDeletableModel<int>
    {
        public Source()
        {
            this.News = new HashSet<News>();
        }

        public string TypeName { get; set; }

        public string ShortName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string DefaultImageUrl { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
