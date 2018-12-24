﻿namespace PressCenters.Services.Sources.BgInstitutions
{
    using System;
    using System.Linq;

    using AngleSharp;
    using AngleSharp.Dom;

    public abstract class NsiBgBaseSource : BaseSource
    {
        public override RemoteDataResult GetLatestPublications()
        {
            var address = this.GetNewsListUrl();
            var document = this.BrowsingContext.OpenAsync(address).Result;
            var links = document.QuerySelectorAll(".view-content .views-field-title a")
                .Select(x => x.Attributes["href"].Value).Select(x => this.NormalizeUrl(x, "http://www.nsi.bg"))
                .ToList();
            var news = links.Select(this.ParseRemoteNews).ToList();
            return new RemoteDataResult { News = news, };
        }

        internal RemoteNews ParseRemoteNews(string url)
        {
            var document = this.BrowsingContext.OpenAsync(url).Result;
            var title = document.QuerySelector("h1.page__title").TextContent.Trim();
            var imageAndContent = document.QuerySelectorAll("article .field-items .field-item");
            var imageUrl = this.GetImageUrl(imageAndContent);
            var content = this.GetContent(imageAndContent);

            var timeElement = document.QuerySelector(".submitted span");
            var time = DateTime.Parse(timeElement.Attributes["content"].Value);
            var news = new RemoteNews
                           {
                               Title = title,
                               OriginalUrl = url,
                               ShortContent = null,
                               Content = content,
                               ImageUrl = imageUrl,
                               PostDate = time,
                               RemoteId = this.ExtractIdFromUrl(url),
                           };

            return news;
        }

        internal string ExtractIdFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            var uri = new Uri(url);
            var id = this.ChooseUrlSegmentForId(uri);
            id = id.Replace("/", string.Empty);
            return id;
        }

        protected abstract string GetContent(IHtmlCollection<IElement> imageAndContent);

        protected abstract string GetImageUrl(IHtmlCollection<IElement> imageAndContent);

        protected abstract string ChooseUrlSegmentForId(Uri uri);

        protected abstract string GetNewsListUrl();
    }
}
