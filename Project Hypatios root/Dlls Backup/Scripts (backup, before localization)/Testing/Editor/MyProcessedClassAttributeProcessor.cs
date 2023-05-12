using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using System.Reflection;
using TestingPurposes;

public class MyProcessedClassAttributeProcessor : OdinAttributeProcessor<MyProcessedClass>
{
    public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
    {
        attributes.Add(new InfoBoxAttribute("Dynamically added attributes!"));
        attributes.Add(new InlinePropertyAttribute());
    }

    public override void ProcessChildMemberAttributes(
        InspectorProperty parentProperty,
        MemberInfo member,
        List<Attribute> attributes)
    {
        // These attributes will be added to all of the child elements.
        attributes.Add(new HideLabelAttribute());
        attributes.Add(new BoxGroupAttribute("Box", showLabel: false));

        // Here we add attributes to child properties respectively.
        if (member.Name == "Mode")
        {
            attributes.Add(new EnumToggleButtonsAttribute());
        }
        else if (member.Name == "Size")
        {
            attributes.Add(new RangeAttribute(0, 5));
        }
    }
}