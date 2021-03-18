using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Helpers
{
    public class SessionHelper
    {
        /// <summary>
        /// Add a new Item to session
        /// </summary>
        /// <param name="objectToSession"></param>
        /// <param name="sessionName"></param>
        public static void AddItem(object objectToSession, string sessionName)
        {
            HttpContext.Current.Session[sessionName] = objectToSession;
        }        /// <summary>
        /// Retrive cached item
        /// </summary>
        /// <typeparam name="T">Type of Session Item</typeparam>
        /// <param name="sessionName">Name of Session Item</param>
        /// <returns>Session Item as type</returns>
        public static T GetItem<T>(string sessionName) where T : class
        {
            try
            {
                return (T)HttpContext.Current.Session[sessionName];
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Remove Item from the Session.
        /// </summary>       
        /// <param name="sessionName"></param>
        public static void RemoveItem(string sessionName)
        {
            HttpContext.Current.Session.Remove(sessionName);
        }
        /// <summary>
        /// Check for the session existance.
        /// </summary>
        /// <param name="sessionName"></param>      
        public static bool IsExisting(string sessionName)
        {
            if (HttpContext.Current.Session[sessionName] != null)
                return true;
            else
                return false;
        }
    }
}