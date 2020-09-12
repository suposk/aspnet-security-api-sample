using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.Identity.Client;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.TokenStorage
{
    public class SessionTokenCache
    {
        private static ReaderWriterLockSlim sessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private string userId = string.Empty;
        private string cacheId = string.Empty;
        private HttpContextBase httpContext = null;

        private TokenCache cache = new TokenCache();

        public SessionTokenCache(string userId, HttpContextBase httpcontext)
        {
            // not object, we want the SUB
            this.userId = userId;
            this.cacheId = this.userId + "_TokenCache";
            this.httpContext = httpcontext;
            this.Load();
        }

        public TokenCache GetMsalCacheInstance()
        {
            this.cache.SetBeforeAccess(this.BeforeAccessNotification);
            this.cache.SetAfterAccess(this.AfterAccessNotification);
            this.Load();
            return this.cache;
        }

        public void SaveUserStateValue(string state)
        {
            sessionLock.EnterWriteLock();
            this.httpContext.Session[this.cacheId + "_state"] = state;
            sessionLock.ExitWriteLock();
        }

        public string ReadUserStateValue()
        {
            string state = string.Empty;
            sessionLock.EnterReadLock();
            state = (string)this.httpContext.Session[this.cacheId + "_state"];
            sessionLock.ExitReadLock();
            return state;
        }

        public void Load()
        {
            sessionLock.EnterReadLock();
            this.cache.Deserialize((byte[])this.httpContext.Session[this.cacheId]);
            sessionLock.ExitReadLock();
        }

        public void Persist()
        {
            sessionLock.EnterWriteLock();

            // Optimistically set HasStateChanged to false. We need to do it early to avoid losing changes made by a concurrent thread.
            this.cache.HasStateChanged = false;

            // Reflect changes in the persistent store
            this.httpContext.Session[this.cacheId] = this.cache.Serialize();
            sessionLock.ExitWriteLock();
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            this.Load();
        }

        // Triggered right after MSAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (this.cache.HasStateChanged)
            {
                this.Persist();
            }
        }
    }
}