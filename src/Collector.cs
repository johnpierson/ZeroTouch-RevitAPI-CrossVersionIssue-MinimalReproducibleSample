using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Revit.Elements;
using RevitServices.Persistence;
using Element = Revit.Elements.Element;

namespace RevitZeroTouchMultiVersionExample
{
    public class Collector
    {
        /// <summary>
        /// Hide the collector class from import
        /// </summary>
        private Collector() { }
        /// <summary>
        /// Simple collector for categories in a specific view.
        /// </summary>
        /// <param name="category">A Dynamo Revit category. (Revit.Elements.Category)</param>
        /// <param name="view">A Dynamo Revit View. (Revit.Elements.Views.View)</param>
        /// <returns name="dynamoElements">The elements cast as a Dynamo Revit element.</returns>
        public static List<global::Revit.Elements.Element> OfCategoryInView(Revit.Elements.Category category, Revit.Elements.Views.View view)
        {
            //access our current document
            var doc = DocumentManager.Instance.CurrentDBDocument;

            //obtain our `Autodesk.Revit.DB.Category` with the element id (long) from Dynamo.
            //Revit.Elements.Category don't have InternalElement unwrappers in Dynamo.
            var revitDbCategory = Autodesk.Revit.DB.Category.GetCategory(doc, new ElementId(category.Id));
            
            //convert our Revit.Elements.Views.View to Autodesk.Revit.DB.View.
            //Thankfully Dynamo has converters for this.
            var revitDbView = view.InternalElement as Autodesk.Revit.DB.View;

            //collect our list of `Autodesk.Revit.DB.Element` elements, (not types), and return those.
            var revitDbElements = new FilteredElementCollector(doc,revitDbView.Id).OfCategory(revitDbCategory.BuiltInCategory).WhereElementIsNotElementType().ToList();

            //last we have to convert these to "Dynamo" types.
            //This is achieved using 'ToDSType(true)' where true means that the element exists in Revit.

            return new List<Element>(revitDbElements.Select(e => e.ToDSType(true))).ToList();
        }
    }
}
