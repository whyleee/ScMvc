using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ScMvc.Aids;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace ScMvc.Models.Mappers
{
    public class LinkFieldToModelMapper
    {
        public virtual Link ToModel(LinkField field)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                return null;
            }

            var link = new Link
            {
                Url = field.Url,
                Text = field.Text,
                Title = field.Title,
                Target = field.Target,
                Class = field.Class
            };

            var linkType = field.LinkType;
            if (linkType == "anchor")
            {
                link.Url = "#" + link.Url;
            }
            else if (linkType == "media")
            {
                var targetItem = field.TargetItem;
                if (targetItem != null)
                {
                    link.Url = MediaManager.GetMediaUrl(new MediaItem(targetItem));
                }
                else
                {
                    link.Url = "";
                }
            }
            else if (linkType == "internal")
            {
                var targetItem = field.TargetItem;
                if (targetItem != null)
                {
                    link.Url = targetItem.GetFriendlyUrl();
                    link.Text = link.Text.IfNotNullOrEmpty() ?? targetItem.DisplayName.IfNotNullOrEmpty() ?? targetItem.Name;
                }
                else
                {
                    link.Url = "";
                }
            }

            var queryString = field.QueryString;
            if (!string.IsNullOrEmpty(queryString))
            {
                link.Url += (link.Url.Contains('?') ? ':' : '?') + WebUtility.UrlDecode(queryString);
            }

            var anchor = field.Anchor;
            if (!string.IsNullOrEmpty(anchor))
            {
                link.Url += '#' + anchor;
            }

            return link;
        }
    }
}
