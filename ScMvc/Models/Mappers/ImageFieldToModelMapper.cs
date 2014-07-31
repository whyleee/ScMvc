using System;
using System.Collections.Generic;
using System.Linq;
using Perks;
using Sitecore.Data.Fields;
using Sitecore.Resources.Media;

namespace ScMvc.Models.Mappers
{
    public class ImageFieldToModelMapper
    {
        public virtual Image ToModel(ImageField field)
        {
            if (field.Value.IsNullOrEmpty())
            {
                return null;
            }

            var img = new Image();
            var mediaItem = field.MediaItem;
            if (mediaItem == null)
            {
                return null;
            }

            img.Src = MediaManager.GetMediaUrl(field.MediaItem, new MediaUrlOptions()
            {
                VirtualFolder = Sitecore.Context.Site.VirtualFolder
            });
            img.Alt = field.Alt;

            int origWidth, origHeight;
            int.TryParse(mediaItem["Width"], out origWidth);
            int.TryParse(mediaItem["Height"], out origHeight);
            img.OrigWidth = origWidth;
            img.OrigHeight = origHeight;

            int width, height;
            int.TryParse(field.Width, out width);
            int.TryParse(field.Height, out height);

            if (width != origWidth)
            {
                img.Width = width;
            }
            if (height != origHeight)
            {
                img.Height = height;
            }

            return img;
        }
    }
}
