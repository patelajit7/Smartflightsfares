using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;

namespace Presentation.Models
{
        public static class MVCHelperExtensionMethods
        {
            public static MvcHtmlString AddHtmlAttributes(this MvcHtmlString input, object htmlAttributes)
            {
                // WE WANT TO INJECT INTO AN EXISTING ELEMENT.  IF THE ATTRIBUTE ALREADY EXISTS, ADD TO IT, OTHERWISE
                // CREATE THE ATTRIBUTE WITH DATA VALUES.

                // USE XML PARSER TO PARSE HTML ELEMENT
                var xdoc = XDocument.Parse(input.ToHtmlString());
                var rootElement = (from e in xdoc.Elements() select e).FirstOrDefault();

                // IF WE CANNOT PARSE THE INPUT USING XDocument THEN RETURN THE ORIGINAL UNMODIFIED.
                if (rootElement == null)
                {
                    return input;
                }

                // USE RouteValueDictionary TO PARSE THE NEW HTML ATTRIBUTES
                var routeValueDictionary = new RouteValueDictionary(htmlAttributes);

                foreach (var routeValue in routeValueDictionary)
                {
                    var attribute = rootElement.Attribute(routeValue.Key);

                    if (attribute == null)
                    {
                        attribute = new XAttribute(name: routeValue.Key, value: routeValue.Value);
                        rootElement.Add(attribute);
                    }
                    else
                    {
                        attribute.Value = string.Format("{0} {1}", attribute.Value, routeValue.Value).Trim();
                    }
                }

                var elementString = rootElement.ToString();
                var response = new MvcHtmlString(elementString);
                return response;
            }
        }
    
}