namespace Sitecore.Support.XA.Feature.Forms.Pipelines.GetPlaceholderRenderings
{
    using System;
    using System.Linq;
    using System.Web;
    using Sitecore.Data;
    using Sitecore.Data.Comparers;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines.GetPlaceholderRenderings;
    using Sitecore.XA.Feature.Forms;
    using Sitecore.XA.Foundation.IoC;
    using Sitecore.XA.Foundation.Multisite;

    public class FilterRenderings
    {
        private Item GetCurrentItem(GetPlaceholderRenderingsArgs args) =>
            (args.ContentDatabase.GetItem(ID.Parse(HttpContext.Current.Request["scItemId"], ID.Null)) ?? Context.Item);

        private static bool IsValid(Item rendering, string placeholderKey)
        {
            ID[] source = new ID[] { Renderings.FormWrapper.ID, Renderings.MvcForm.ID };
            if (Sitecore.Support.XA.Feature.Forms.Constants.FormWrapperPlaceholders.Any<string>(new Func<string, bool>(placeholderKey.Contains)) && source.All<ID>(fw => (fw != rendering.ID)))
            {
                return false;
            }
            return true;
        }

        public void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull(args, "args");
            Item currentItem = this.GetCurrentItem(args);
            if ((currentItem != null) && (ServiceLocator.Current.Resolve<IMultisiteContext>().GetSiteItem(currentItem) != null))
            {
                args.PlaceholderRenderings = (from r in args.PlaceholderRenderings.Distinct<Item>(new ItemIdComparer())
                                              where IsValid(r, args.PlaceholderKey)
                                              select r).ToList<Item>();
            }
        }
    }
}