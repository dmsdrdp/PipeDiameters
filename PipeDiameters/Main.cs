using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;


//Занятие 3.4 Параметр проекта

namespace PipeDiameters
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectedRef = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите элемент");

            foreach (var selectedElement in selectedRef)
            {
                var element = doc.GetElement(selectedElement);

                if (element is Pipe)
                {
                    using (Transaction ts = new Transaction(doc, "Set parameter"))
                    {
                        ts.Start();
                        var familyInstance = element as Pipe;
                        Parameter outerDiameterText = element.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
                        Parameter innerDiameterText = element.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);
                        Parameter reservLenght = familyInstance.LookupParameter("Наименование");    //параметр проекта

                        double outerDiameter = UnitUtils.ConvertFromInternalUnits(outerDiameterText.AsDouble(), UnitTypeId.Millimeters);  // преобразование в миллиметры
                        double innerDiameter = UnitUtils.ConvertFromInternalUnits(innerDiameterText.AsDouble(), UnitTypeId.Millimeters);

                        reservLenght.Set($" Труба {Math.Round(outerDiameter, 1)}/{Math.Round(innerDiameter, 1)}");

                        ts.Commit();
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}
