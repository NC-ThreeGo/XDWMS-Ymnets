
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace Apps.Core.PageControl
{
    public static class PagerHelper
    {
        #region Html Pager
        public static MvcHtmlString Pager(this HtmlHelper helper, int totalItemCount, int pageSize, int pageIndex, string actionName, string controllerName,
            PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    new RouteValueDictionary(routeValues),
                    new RouteValueDictionary(htmlAttributes)
                );
            return builder.RenderPager();
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, int totalItemCount, int pageSize, int pageIndex, string actionName, string controllerName,
            PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            var builder = new PagerBuilder
                (
                    helper,
                    actionName,
                    controllerName,
                    totalPageCount,
                    pageIndex,
                    pagerOptions,
                    routeName,
                    routeValues,
                    htmlAttributes
                );
            return builder.RenderPager();
        }

        private static MvcHtmlString Pager(HtmlHelper helper, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(helper, null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList)
        {
            if (pagedList == null)
                return Pager(helper, (PagerOptions)null, null);
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, null, null, null);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, PagerOptions pagerOptions)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, null);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, PagerOptions pagerOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, htmlAttributes);
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, pagerOptions, null, null, htmlAttributes);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, PagerOptions pagerOptions, string routeName, object routeValues)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, PagerOptions pagerOptions, string routeName, RouteValueDictionary routeValues)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, null);
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, null);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, PagerOptions pagerOptions, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, PagerOptions pagerOptions, string routeName,
            RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, pagerOptions, htmlAttributes);
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, pagerOptions, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, string routeName, object routeValues, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, null, new RouteValueDictionary(htmlAttributes));
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        public static MvcHtmlString Pager(this HtmlHelper helper, IPagedList pagedList, string routeName, RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(helper, null, htmlAttributes);
            return Pager(helper, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, routeName, routeValues, htmlAttributes);
        }

        #endregion

        #region jQuery Ajax Pager

        private static MvcHtmlString AjaxPager(HtmlHelper html, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(html, null, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, int totalItemCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var totalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes));
            return builder.RenderPager();
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, int totalItemCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagerOptions == null)
                pagerOptions = new PagerOptions();
            pagerOptions.UseJqueryAjax = true;

            var totalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            var builder = new PagerBuilder(html, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes);
            return builder.RenderPager();
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, (PagerOptions)null, null);
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, string routeName, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, (PagerOptions)null, null);
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, null, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions,
            IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, htmlAttributes);
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null,
                             ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, string routeName, object routeValues, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions,
                             null);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, string routeName, object routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, string routeName, RouteValueDictionary routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, htmlAttributes);
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions,
                             routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString AjaxPager(this HtmlHelper html, IPagedList pagedList, string actionName, string controllerName,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            if (pagedList == null)
                return AjaxPager(html, pagerOptions, null);
            return AjaxPager(html, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, actionName, controllerName, null, pagerOptions, null, ajaxOptions,
                             null);
        }

        #endregion

        #region Microsoft Ajax Pager

        public static MvcHtmlString Pager(this AjaxHelper ajax, int totalItemCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, new RouteValueDictionary(routeValues), ajaxOptions, new RouteValueDictionary(htmlAttributes));
            return builder.RenderPager();
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, int totalItemCount, int pageSize, int pageIndex, string actionName, string controllerName,
            string routeName, PagerOptions pagerOptions, RouteValueDictionary routeValues, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            var totalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            var builder = new PagerBuilder(ajax, actionName, controllerName, totalPageCount, pageIndex, pagerOptions,
                                           routeName, routeValues, ajaxOptions, htmlAttributes);
            return builder.RenderPager();
        }

        private static MvcHtmlString Pager(AjaxHelper ajax, PagerOptions pagerOptions, IDictionary<string, object> htmlAttributes)
        {
            return new PagerBuilder(null, ajax, pagerOptions, htmlAttributes).RenderPager();
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, AjaxOptions ajaxOptions)
        {
            return pagedList == null ? Pager(ajax, (PagerOptions)null, null) : Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, null, null, ajaxOptions, null);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions)
        {
            return pagedList == null ? Pager(ajax, pagerOptions, null) : Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex,
                null, null, null, pagerOptions, null, ajaxOptions, null);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, htmlAttributes);
            return Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, null, pagerOptions, null, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, string routeName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, null, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, string routeName, RouteValueDictionary routeValues,
            AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, null, htmlAttributes);
            return Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, null, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, string routeName, object routeValues, PagerOptions pagerOptions,
            AjaxOptions ajaxOptions, object htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, new RouteValueDictionary(htmlAttributes));
            return Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }

        public static MvcHtmlString Pager(this AjaxHelper ajax, IPagedList pagedList, string routeName, RouteValueDictionary routeValues,
            PagerOptions pagerOptions, AjaxOptions ajaxOptions, IDictionary<string, object> htmlAttributes)
        {
            if (pagedList == null)
                return Pager(ajax, pagerOptions, htmlAttributes);
            return Pager(ajax, pagedList.TotalItemCount, pagedList.PageSize, pagedList.CurrentPageIndex, null, null, routeName, pagerOptions, routeValues, ajaxOptions, htmlAttributes);
        }
        #endregion
    }
}