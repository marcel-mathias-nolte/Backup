//-----------------------------------------------------------------------
// <copyright file="SessionBasedRequest.cs" company="RAN COMMUNITY SERVER">
//     (C)2010-2013 Marcel Nolte
// </copyright>
//-----------------------------------------------------------------------

namespace Nolte.Net
{
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    /// A web query service which keeps cookies (aka session based)
    /// </summary>
    public class SessionBasedRequest
    {
        /// <summary>
        /// A collection of all current cookies.
        /// </summary>
        private CookieCollection cookies = new CookieCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionBasedRequest" /> class.
        /// </summary>
        public SessionBasedRequest()
        {
            this.Proxy = null;
            this.LastReferrer = string.Empty;
        }

        /// <summary>
        /// Fired if a new request was finished.
        /// </summary>
        public event SessionBasedAsyncRequestHandler SessionBasedAsyncRequestFinished;

        /// <summary>
        /// Gets or sets the current proxy to use
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets the cookie collection
        /// </summary>
        public CookieCollection Cookies
        {
            get
            {
                return this.cookies;
            }

            set
            {
                if (value != null)
                {
                    this.cookies = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the last referrer url
        /// </summary>
        public string LastReferrer { get; protected set; }

        /// <summary>
        /// Send a post request asynchronously
        /// </summary>
        /// <param name="url">The target url</param>
        /// <param name="postData">The data to submit</param>
        /// <param name="tag">A user-defined tag</param>
        /// <param name="callback">A callback to call when finished (otherwise the event will be fired)</param>
        public void PostAsync(string url, PostArguments postData, object tag = null, SessionBasedAsyncRequestHandler callback = null)
        {
            (new System.Threading.Thread(() =>
            {
                if (callback != null)
                {
                    callback(new SessionBasedAsyncRequestArgs(this.Post(url, postData), tag));
                }
                else
                {
                    this.OnSessionBasedAsyncRequestFinished(new SessionBasedAsyncRequestArgs(this.Post(url, postData), tag));
                }
            })).Start();
        }

        /// <summary>
        /// Send a post request synchronously and returns the result
        /// </summary>
        /// <param name="url">The target url</param>
        /// <param name="postData">The data to submit</param>
        /// <returns>The request's result</returns>
        public string Post(string url, PostArguments postData)
        {
            var webRequestObject = (HttpWebRequest)WebRequest.Create(url);
            webRequestObject.Timeout = 30000;
            webRequestObject.Proxy = this.Proxy;

            webRequestObject.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; de; rv:1.9.2.17) Gecko/20110420 Firefox/3.6.17";
            if (!string.IsNullOrWhiteSpace(this.LastReferrer))
            {
                webRequestObject.Referer = this.LastReferrer;
            }

            webRequestObject.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequestObject.Headers.Add("Accept-Language: en-us,de-de;q=0.8,de;q=0.5,en;q=0.3");
            webRequestObject.CookieContainer = new CookieContainer();
            lock (this.cookies)
            {
                foreach (Cookie c in this.cookies)
                {
                    webRequestObject.CookieContainer.Add(c);
                }
            }

            webRequestObject.AllowAutoRedirect = false;
            webRequestObject.Method = "POST";
            webRequestObject.ContentType = "application/x-www-form-urlencoded";
            webRequestObject.ContentLength = postData.ContentLength;

            var requestStream = webRequestObject.GetRequestStream();
            requestStream.Write(postData.Content, 0, postData.ContentLength);
            requestStream.Close();

            var content = string.Empty;

            try
            {
                using (var response = (HttpWebResponse)webRequestObject.GetResponse())
                {
                    using (var webStream = response.GetResponseStream())
                    {
                        if (webStream != null)
                        {
                            using (var reader = new StreamReader(webStream))
                            {
                                content = reader.ReadToEnd();
                                lock (this.cookies)
                                {
                                    foreach (Cookie cook in response.Cookies)
                                    {
                                        this.cookies.Add(cook);
                                    }
                                }

                                reader.Close();
                            }

                            webStream.Close();
                        }
                    }

                    response.Close();
                    this.LastReferrer = url;
                }
            }
            catch (Exception)
            {
            }

            return content;
        }

        /// <summary>
        /// Send a get request asynchronously
        /// </summary>
        /// <param name="url">The target url</param>
        /// <param name="tag">A user-defined tag</param>
        /// <param name="callback">A callback to call when finished (otherwise the event will be fired)</param>
        public void GetAsync(string url, object tag = null, SessionBasedAsyncRequestHandler callback = null)
        {
            (new System.Threading.Thread(() =>
            {
                if (callback != null)
                {
                    callback(new SessionBasedAsyncRequestArgs(this.Get(url), tag));
                }
                else
                {
                    this.OnSessionBasedAsyncRequestFinished(new SessionBasedAsyncRequestArgs(this.Get(url), tag));
                }
            })).Start();
        }

        /// <summary>
        /// Send a get request synchronously and returns the result
        /// </summary>
        /// <param name="url">The target url</param>
        /// <returns>The request's result</returns>
        public string Get(string url)
        {
            var webRequestObject = (HttpWebRequest)WebRequest.Create(url);
            webRequestObject.Timeout = 30000;
            webRequestObject.Proxy = this.Proxy;
            webRequestObject.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; de; rv:1.9.2.17) Gecko/20110420 Firefox/3.6.17";
            if (!string.IsNullOrWhiteSpace(this.LastReferrer))
            {
                webRequestObject.Referer = this.LastReferrer;
            }

            webRequestObject.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequestObject.Headers.Add("Accept-Language: en-us,de-de;q=0.8,de;q=0.5,en;q=0.3");
            webRequestObject.CookieContainer = new CookieContainer();
            lock (this.cookies)
            {
                foreach (Cookie c in this.cookies)
                {
                    webRequestObject.CookieContainer.Add(c);
                }
            }

            webRequestObject.AllowAutoRedirect = false;

            var content = string.Empty;

            try
            {
                using (var response = (HttpWebResponse)webRequestObject.GetResponse())
                {
                    using (var webStream = response.GetResponseStream())
                    {
                        if (webStream != null)
                        {
                            using (var reader = new StreamReader(webStream))
                            {
                                content = reader.ReadToEnd();
                                lock (this.cookies)
                                {
                                    foreach (Cookie cook in response.Cookies)
                                    {
                                        this.cookies.Add(cook);
                                    }
                                }

                                reader.Close();
                            }

                            webStream.Close();
                        }
                    }

                    response.Close();
                    this.LastReferrer = url;
                }
            }
            catch
            {
            }

            return content;
        }

        /// <summary>
        /// Throw SessionBasedAsyncRequestFinished safely
        /// </summary>
        /// <param name="e">The request's result set</param>
        protected void OnSessionBasedAsyncRequestFinished(SessionBasedAsyncRequestArgs e)
        {
            if (this.SessionBasedAsyncRequestFinished != null)
            {
                this.SessionBasedAsyncRequestFinished(e);
            }
        }
    }
}
