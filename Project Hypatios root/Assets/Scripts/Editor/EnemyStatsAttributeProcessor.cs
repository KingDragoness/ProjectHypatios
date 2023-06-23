
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using System.Reflection;
using Kryz.CharacterStats;

public class EnemyStatsAttributeProcessor : OdinAttributeProcessor<EnemyStats>
{
    public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
    {
        // attributes.Add(new InfoBoxAttribute("Dynamically added attributes!"));
        attributes.Add(new HideReferenceObjectPickerAttribute());
        attributes.Add(new InlinePropertyAttribute());
        attributes.Add(new HideLabelAttribute());
    }

    public override void ProcessChildMemberAttributes(
        InspectorProperty parentProperty,
        MemberInfo member,
        List<Attribute> attributes)
    {
        attributes.Add(new SpaceAttribute(1f));
        // These attributes will be added to all of the child elements.
        //attributes.Add(new LabelWidthAttribute(20));
    }
}


public class CharacterStatAttributeProcessor : OdinAttributeProcessor<CharacterStat>
{
    public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
    {
        attributes.Add(new InlinePropertyAttribute());
        attributes.Add(new HideReferenceObjectPickerAttribute());
    }

    public override void ProcessChildMemberAttributes(
        InspectorProperty parentProperty,
        MemberInfo member,
        List<Attribute> attributes)
    {

        attributes.Add(new HideLabelAttribute());

        if (member.Name == "StatModifiers")
        {
            attributes.Add(new HideInPlayModeAttribute());
        }

    }
}

public class BaseEnemyStatsAttributeProcessor : OdinAttributeProcessor<BaseEnemyStats>
{
    public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
    {
        attributes.Add(new InlinePropertyAttribute());
        attributes.Add(new HideLabelAttribute());
    }

    public override void ProcessChildMemberAttributes(
        InspectorProperty parentProperty,
        MemberInfo member,
        List<Attribute> attributes)
    {
    }
}
