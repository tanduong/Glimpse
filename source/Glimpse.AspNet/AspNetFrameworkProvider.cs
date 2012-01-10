﻿using System.Web;
using Glimpse.Core2;
using Glimpse.Core2.Extensibility;
using Glimpse.Core2.Framework;

namespace Glimpse.AspNet
{
    public class AspNetFrameworkProvider:IFrameworkProvider
    {
        /// <summary>
        /// Wrapper around HttpContext.Current for testing purposes. Not for public use.
        /// </summary>
        private HttpContextBase context;
        internal HttpContextBase Context
        {
            get { return context ?? new HttpContextWrapper(HttpContext.Current); }
            set { context = value; }
        }

        public IDataStore HttpRequestStore
        {
            get { return new DictionaryDataStoreAdapter(Context.Items);}
        }

        public IDataStore HttpServerStore
        {
            get { return new HttpApplicationStateBaseDataStoreAdapter(Context.Application); }
        }

        public object RuntimeContext
        {
            get { return Context; }
        }

        public IRequestMetadata RequestMetadata
        {
            get
            {
                var request = Context.Request;
                var response = Context.Response;
                return new RequestMetadata
                             {
                                 RequestHttpMethod = request.HttpMethod,
                                 GlimpseClientName = "ACCESS FROM COOKIE", //TODO: FIX ME
                                 IpAddress = request.UserHostAddress, //TODO: FIX ME WHEN BEHIND PROXIES
                                 ResponseContentType = response.ContentType,
                                 ResponseStatusCode = response.StatusCode,
                                 RequestUri = request.Url.AbsoluteUri,
                             };
            }
        }

        public void SetHttpResponseHeader(string name, string value)
        {
            var headers = Context.Response.Headers;
            headers.Set(name, value);
        }

        public void SetHttpResponseStatusCode(int statusCode)
        {
            Context.Response.StatusCode = statusCode;
            Context.Response.StatusDescription = null;
        }

        public void InjectHttpResponseBody(string htmlSnippet)
        {
            var response = Context.Response;

            response.Filter = new PreBodyTagFilter(htmlSnippet, response.Filter, Context.Response.ContentEncoding);
        }
    }
}