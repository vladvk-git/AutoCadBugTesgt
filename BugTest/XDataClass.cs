using Autodesk.AutoCAD.DatabaseServices;

namespace BugTest
{
    internal static class XDataClass
    {
        internal static void WriteRegAppName(Entity entity)
        {

            if (entity.ExtensionDictionary.IsNull)
            {
                if (!entity.IsWriteEnabled) entity.UpgradeOpen();
                entity.CreateExtensionDictionary();
            }

            var dictid = entity.ExtensionDictionary;

            using (Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (var trans = entity.Database.TransactionManager.StartOpenCloseTransaction())
                {
                    var rat = (RegAppTable)trans.GetObject(entity.Database.RegAppTableId, OpenMode.ForRead);

                    if (!rat.Has("BugTest"))
                    {
                        rat.UpgradeOpen();

                        var ratr = new RegAppTableRecord();

                        ratr.Name = "BugTest";
                        rat.Add(ratr);

                        trans.AddNewlyCreatedDBObject(ratr, true);
                    }


                    //var dict = (DBDictionary)trans.GetObject(dictid, OpenMode.ForRead, false, true);
                    if (!entity.IsWriteEnabled) entity.UpgradeOpen();
                    entity.XData = new ResultBuffer(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "BugTest"),
                                new TypedValue((int)DxfCode.ExtendedDataAsciiString, "BugTest"));

                    trans.Commit();
                }
            }
        }
    }
}
