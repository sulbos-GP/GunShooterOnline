using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ExternalMethod
{

    public static System.Type GetReturnType(this MemberInfo memberInfo)
    {
        switch (memberInfo)
        {
            case FieldInfo fieldInfo:
                return fieldInfo.FieldType;
            case PropertyInfo propertyInfo:
                return propertyInfo.PropertyType;
            case MethodInfo methodInfo:
                return methodInfo.ReturnType;
            case EventInfo eventInfo:
                return eventInfo.EventHandlerType;
            default:
                return null;
        }
    }

    public static void SetMemberValue(this MemberInfo member, object obj, object value)
    {
        switch (member)
        {
            case FieldInfo fieldInfo:
                fieldInfo.SetValue(obj, value);
                break;
            case PropertyInfo propertyInfo:
                MethodInfo setMethod = propertyInfo.GetSetMethod(true);
                if (setMethod == null)
                    throw new ArgumentException("Property " + member.Name + " has no setter");
                setMethod.Invoke(obj, new object[1] { value });
                break;
            default:
                throw new ArgumentException("Can't set the value of a " + member.GetType().Name);
        }
    }

    public static object GetMemberValue(this MemberInfo member, object obj)
    {
        switch (member)
        {
            case FieldInfo fieldInfo:
                return fieldInfo.GetValue(obj);
            case PropertyInfo propertyInfo:
                return propertyInfo.GetGetMethod(true).Invoke(obj, null);
            default:
                throw new ArgumentException("Can't get the value of a " + member.GetType().Name);
        }
    }
}
