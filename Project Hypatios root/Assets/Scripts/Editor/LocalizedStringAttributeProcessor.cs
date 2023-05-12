using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine.Localization;

namespace UnityEditor.Localization
{
    public class LocalizedStringAttributeProcessor<TLocalizedString> : OdinAttributeProcessor<TLocalizedString>
        where TLocalizedString : LocalizedString
    {
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            attributes.Add(new DrawWithUnityAttribute());
        }
    }
}