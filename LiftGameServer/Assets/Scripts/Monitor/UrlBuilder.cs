using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lift
{
    [CreateAssetMenu(menuName = "Lift/UrlBuilder", fileName = "UrlBuilder")]
    public class UrlBuilder : ScriptableObject
    {
        [SerializeField]
        string address;
        [SerializeField]
        string port;
        [SerializeField]
        List<string> routeList = new();

        public void OnInitialAssertion()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(address));
            Assert.IsFalse(string.IsNullOrWhiteSpace(port));
        }

        public bool BuildWebSocketUrl(int routeIndex, out string url)
        {
            if (routeIndex < 0 || routeIndex >= routeList.Count)
            {
                url = null;
                return false;
            }

            var route = routeList[routeIndex] switch
            {
                null => "",
                _ => routeList[routeIndex]
            };
            url = "ws://" + address + ":" + port + route;
            return true;
        }
    }
}
