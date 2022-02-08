using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace BugTest
{
    public class ExtensionClass : IExtensionApplication
    {
        public void Initialize()
        {
            //Overrule.AddOverrule(RXObject.GetClass(typeof(Polyline)), BaseTransformOverrule.Instance, true);
            Overrule.AddOverrule(RXObject.GetClass(typeof(Polyline)), TestGripOverrule.Instance, true);
            //Overrule.AddOverrule(RXObject.GetClass(typeof(Polyline)), BaseObjectOverrule.Instance, true);
            Overrule.AddOverrule(RXObject.GetClass(typeof(Polyline)), TestDrawOverrule.Instance, true);
        }

        public void Terminate()
        {
        }
    }
}
