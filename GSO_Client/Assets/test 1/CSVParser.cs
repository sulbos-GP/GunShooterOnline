using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class CSVParser : MonoBehaviour
{
    // �Ľ̽� �ʿ䰴ü ĳ��.
    private static List<MemberInfo> _fileFieldInfoList = new List<MemberInfo>();
    private static List<string> _objFieldNameList = new List<string>();
    private static Type _strType = typeof(string);

    // �Ľ� ����.
    private const char COMMA = ',';
    private const char LINE_FEED1 = '\n';
    private const char LINE_FEED2 = '\r';
    private const char DOUBLE_QUOTE = '"';
    private const string EMPTY_STR = "";

    public static List<T> LoadData<T>(string csvText) where T : new()
    {
        // ���� ������ List.
        List<T> resultList = new List<T>();

        if (string.IsNullOrEmpty(csvText))
            return resultList;

        // �ʵ� ����.
        int cursor = ParseHeaderByFile<T>(csvText);

        // ���� ���� ����.
        Type fieldType;
        MemberInfo fInfo;
        int columnCount = 0;
        T datum = default(T);
        bool isDoubleQuote = false;
        char prevChar = LINE_FEED1;
        string targetValue;

        // ���� ����.
        for (int i = cursor; i < csvText.Length; i++)
        {
            char oneChar = csvText[i];

            switch (oneChar)
            {
                case LINE_FEED1:
                case LINE_FEED2:
                    // �������ΰ��� ����.
                    if (prevChar == LINE_FEED1 || prevChar == LINE_FEED2)
                        break;

                    // �����÷��� ������ �ش��÷��� �� �߰�.
                    if (columnCount < _fileFieldInfoList.Count)
                    {
                        // �÷��� 0�̰� �����Ͱ� ������ ��ü ����.
                        if (columnCount == 0)
                            datum = new T();

                        fInfo = _fileFieldInfoList[columnCount];

                        // ���빰�� ������ ������.
                        if (i - cursor <= 1)
                            targetValue = EMPTY_STR;
                        else
                            targetValue = csvText.Substring(cursor + 1, i - cursor - 1);

                        // �ֵ���ǥ�� �ִٸ� �߰��۾�.
                        if (targetValue.IndexOf(DOUBLE_QUOTE) > -1)
                            targetValue = LoadParse(targetValue);

                        // ���ڿ��� ���̸� ���� �ʿ䰡 ����.
                        if (!string.IsNullOrEmpty(targetValue))
                        {
                            if (_fileFieldInfoList[columnCount] != null)
                            {
                                if (targetValue.IndexOf("\\n") >= 0)
                                    targetValue = targetValue.Replace("\\n", "\n");

                                // ������Ƽ�� �� �Ҵ�.	
                                fieldType = _fileFieldInfoList[columnCount].GetReturnType();

                                if (fieldType.Equals(_strType))
                                {
                                    // stringŸ���� �ڽ��� �ؾ� ��.. classŸ���� �ƴ϶� struct�� string ������ �׳� ��������.
                                    object boxDatum = datum;
                                    fInfo.SetMemberValue(boxDatum, targetValue);
                                    datum = (T)boxDatum;
                                }
                                else
                                {
                                    fInfo.SetMemberValue(datum, GetTypeValue(fieldType, targetValue));
                                }
                            }
                        }
                    }

                    // ���������� ������ ��ü�� ��Ͽ� �߰�.
                    resultList.Add(datum);

                    // ���� �ʱ�ȭ.
                    cursor = i;
                    columnCount = 0;
                    isDoubleQuote = false;

                    break;
                case DOUBLE_QUOTE:
                    // �ֵ���ǥ ���۰� �� ������ ������ ������ ���ڷ� ó���ȴ�.
                    isDoubleQuote = !isDoubleQuote;

                    break;
                case COMMA:
                    // �ֵ���ǥ ���ο��� ����.
                    if (isDoubleQuote)
                        break;

                    // �����÷��� ������ ����.
                    if (columnCount >= _fileFieldInfoList.Count)
                        break;

                    // �÷��� 0�̸� ��ü ����.
                    if (columnCount == 0)
                        datum = new T();

                    // �÷��� �߰�.
                    fInfo = _fileFieldInfoList[columnCount];

                    // ���빰�� ������ ������.
                    if (i - cursor <= 1)
                        targetValue = EMPTY_STR;
                    else
                        targetValue = csvText.Substring(cursor + 1, i - cursor - 1);

                    // �ֵ���ǥ�� �ִٸ� �߰��۾�.
                    if (targetValue.IndexOf(DOUBLE_QUOTE) > -1)
                        targetValue = LoadParse(targetValue);

                    // ���ڿ��� ���̸� ���� �ʿ䰡 ����.
                    if (!string.IsNullOrEmpty(targetValue))
                    {
                        if (_fileFieldInfoList[columnCount] != null)
                        {
                            if (targetValue.IndexOf("\\n") >= 0)
                                targetValue = targetValue.Replace("\\n", "\n");

                            // ������Ƽ�� �� �Ҵ�.    
                            fieldType = _fileFieldInfoList[columnCount].GetReturnType();
                            if (fieldType.Equals(_strType))
                            {
                                // stringŸ���� �ڽ��� �ؾ� ��.. classŸ���� �ƴ϶� struct�� string ������ �׳� ��������.
                                object boxDatum = datum;
                                fInfo.SetMemberValue(boxDatum, targetValue);
                                datum = (T)boxDatum;
                            }
                            else
                            {
                                // ��� T��ü�� �÷� �ֱ�.
                                fInfo.SetMemberValue(datum, GetTypeValue(fieldType, targetValue));
                            }
                        }
                    }

                    cursor = i;
                    columnCount++;
                    break;
            }

            prevChar = oneChar;
        }

        return resultList;
    }

    // Ÿ�Ժ�ȯ.
    private static object GetTypeValue(Type fieldType, string targetValue)
    {
        if (fieldType == typeof(long))
        {
            long resultValue = 0;
            if (long.TryParse(targetValue, out resultValue))
                return resultValue;
        }
        else if (fieldType == typeof(int))
        {
            int resultValue = 0;
            if (int.TryParse(targetValue, out resultValue))
                return resultValue;
        }
        else if (fieldType == typeof(float))
        {
            float resultValue = 0;
            if (float.TryParse(targetValue, out resultValue))
                return resultValue;
        }
        else if (fieldType == typeof(bool))
        {
            bool resultValue = false;
            if (bool.TryParse(targetValue, out resultValue))
                return resultValue;
        }
        else if (fieldType.IsEnum)
        {
            return Enum.Parse(fieldType, targetValue);
        }

        Debug.LogErrorFormat("{0} : {1}", fieldType.Name, targetValue);
        return 0;
    }

    // ������ ��� ����.
    private static int ParseHeaderByFile<T>(string csvText)
    {
        // ���ϳ� Ŀ����ġ.
        int cursor = -1;

        // ���ü�� ��� ����.
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var type = typeof(T);
        MemberInfo[] objProperties = type.GetFields(bindingFlags).Cast<MemberInfo>()
            .Concat(type.GetProperties(bindingFlags)).ToArray();
        ParseHeaderByObj(objProperties);
        string typeName = csvText.Split(',')[0];

        // ������ ��� ����.
        _fileFieldInfoList.Clear();
        for (int i = 0; i < csvText.Length; i++)
        {
            char oneChar = csvText[i];

            if (oneChar == COMMA || oneChar == LINE_FEED1 || oneChar == LINE_FEED2)
            {
                // ���빰�� ������ ����� ����.
                if (i > cursor + 1)
                {
                    string targetFieldName = csvText.Substring(cursor + 1, i - cursor - 1);
                    if (targetFieldName.Contains(typeName))
                        targetFieldName = "Key";
                    // �ʵ�� �ش��ϴ� �÷���ġ�� ã�� �ٸ� ���.
                    //int targetIndex = mObjFieldNameList.FindIndex((string x) => { return x.Equals(targetFieldName, StringComparison.InvariantCultureIgnoreCase); });
                    int targetIndex = _objFieldNameList.IndexOf(targetFieldName);
                    _fileFieldInfoList.Add(targetIndex < 0 ? null : objProperties[targetIndex]);
                }

                cursor = i;
                if (oneChar == LINE_FEED1 || oneChar == LINE_FEED2)
                    break;
            }
        }
        return cursor;
    }

    // ��ü�� ��� ����.
    private static void ParseHeaderByObj(MemberInfo[] objProperties)
    {
        _objFieldNameList.Clear();
        for (int i = 0; i < objProperties.Length; i++)
            _objFieldNameList.Add(objProperties[i].Name);
    }

    // ��üȭ�� ���ڿ� �Ľ�.
    private static string LoadParse(string target)
    {
        if (target.IndexOf("\"\"") > -1)
            target = target.Replace("\"\"", "#@quote#");
        target = target.Replace("\"", "");
        target = target.Replace("#@quote#", "\"");

        if (target.IndexOf(",") > -1 && target.IndexOf('"') == 0 && target.LastIndexOf('"') == 0)
            target = target.Substring(1, target.Length - 2);

        return target;
    }

    // ����ȭ�� ���ڿ� �Ľ�.
    private static string SaveParse(string target)
    {
        // ��ǥ, �ֵ���ǥ, ������ �����ϸ� ����ó��.
        bool hasCRLF = target.IndexOf("\n") >= 0;
        bool hasDoubleQuotes = target.IndexOf(DOUBLE_QUOTE) >= 0;
        bool hasComma = target.IndexOf(',') >= 0;

        if (hasDoubleQuotes)
            target = target.Replace("\"", "\"\"");

        if (hasCRLF)
            target = target.Replace("\n", "\\n");

        if (hasCRLF || hasDoubleQuotes || hasComma)
            target = "\"" + target + "\"";

        return target;
    }

    // csv�� �����ϱ�.
    public static string Parse<T>(List<T> data, int idCount = 1) where T : new()
    {
        // ��� �ؽ�Ʈ���� ���.
        StringBuilder sb = new StringBuilder();

        // ��ü�� �ʵ带 ��� ����� �����Ѵ�.
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var type = typeof(T);
        MemberInfo[] objProperties = type.GetFields(bindingFlags).Cast<MemberInfo>().Concat(type.GetProperties(bindingFlags)).ToArray();

        // ID �÷��� ���� ������ ������ ����.
        for (int i = 0; i < idCount; i++)
        {
            MemberInfo lastField = objProperties[objProperties.Length - 1];
            for (int j = objProperties.Length - 2; j >= 0; j--)
                objProperties[j + 1] = objProperties[j];
            objProperties[0] = lastField;
        }

        ParseHeaderByObj(objProperties);
        _fileFieldInfoList.Clear();
        for (int i = 0; i < objProperties.Length; i++)
        {
            _fileFieldInfoList.Add(objProperties[i]);
            sb.Append(_objFieldNameList[i]);
            sb.Append(',');
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append('\n');

        // ��ü �ʵ帶�� ������� ������ ���� ���ڿ�ȭ.
        MemberInfo fInfo;
        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < _fileFieldInfoList.Count; j++)
            {
                fInfo = _fileFieldInfoList[j];
                object target = fInfo.GetMemberValue(data[i]);
                if (target != null)
                    sb.Append(SaveParse(target.ToString()));
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append('\n');
        }

        return sb.ToString();
    }
}
