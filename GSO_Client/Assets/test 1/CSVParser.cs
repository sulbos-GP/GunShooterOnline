using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class CSVParser : MonoBehaviour
{
    // 파싱시 필요객체 캐싱.
    private static List<MemberInfo> _fileFieldInfoList = new List<MemberInfo>();
    private static List<string> _objFieldNameList = new List<string>();
    private static Type _strType = typeof(string);

    // 파싱 문자.
    private const char COMMA = ',';
    private const char LINE_FEED1 = '\n';
    private const char LINE_FEED2 = '\r';
    private const char DOUBLE_QUOTE = '"';
    private const string EMPTY_STR = "";

    public static List<T> LoadData<T>(string csvText) where T : new()
    {
        // 최종 리턴할 List.
        List<T> resultList = new List<T>();

        if (string.IsNullOrEmpty(csvText))
            return resultList;

        // 필드 추출.
        int cursor = ParseHeaderByFile<T>(csvText);

        // 내용 추출 변수.
        Type fieldType;
        MemberInfo fInfo;
        int columnCount = 0;
        T datum = default(T);
        bool isDoubleQuote = false;
        char prevChar = LINE_FEED1;
        string targetValue;

        // 내용 추출.
        for (int i = cursor; i < csvText.Length; i++)
        {
            char oneChar = csvText[i];

            switch (oneChar)
            {
                case LINE_FEED1:
                case LINE_FEED2:
                    // 개행중인경우는 무시.
                    if (prevChar == LINE_FEED1 || prevChar == LINE_FEED2)
                        break;

                    // 남은컬럼이 있으면 해당컬럼에 값 추가.
                    if (columnCount < _fileFieldInfoList.Count)
                    {
                        // 컬럼이 0이고 데이터가 있으면 객체 생성.
                        if (columnCount == 0)
                            datum = new T();

                        fInfo = _fileFieldInfoList[columnCount];

                        // 내용물이 없으면 빈값으로.
                        if (i - cursor <= 1)
                            targetValue = EMPTY_STR;
                        else
                            targetValue = csvText.Substring(cursor + 1, i - cursor - 1);

                        // 쌍따옴표가 있다면 추가작업.
                        if (targetValue.IndexOf(DOUBLE_QUOTE) > -1)
                            targetValue = LoadParse(targetValue);

                        // 문자열이 빈값이면 넣을 필요가 없음.
                        if (!string.IsNullOrEmpty(targetValue))
                        {
                            if (_fileFieldInfoList[columnCount] != null)
                            {
                                if (targetValue.IndexOf("\\n") >= 0)
                                    targetValue = targetValue.Replace("\\n", "\n");

                                // 프로퍼티에 값 할당.	
                                fieldType = _fileFieldInfoList[columnCount].GetReturnType();

                                if (fieldType.Equals(_strType))
                                {
                                    // string타입은 박싱을 해야 들어감.. class타입이 아니라 struct에 string 있으면 그냥 하지마라.
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

                    // 최종적으로 정제된 객체를 목록에 추가.
                    resultList.Add(datum);

                    // 전부 초기화.
                    cursor = i;
                    columnCount = 0;
                    isDoubleQuote = false;

                    break;
                case DOUBLE_QUOTE:
                    // 쌍따옴표 시작과 끝 사이의 모든것은 무조건 문자로 처리된다.
                    isDoubleQuote = !isDoubleQuote;

                    break;
                case COMMA:
                    // 쌍따옴표 내부에선 무시.
                    if (isDoubleQuote)
                        break;

                    // 남은컬럼이 없으면 무시.
                    if (columnCount >= _fileFieldInfoList.Count)
                        break;

                    // 컬럼이 0이면 객체 생성.
                    if (columnCount == 0)
                        datum = new T();

                    // 컬럼에 추가.
                    fInfo = _fileFieldInfoList[columnCount];

                    // 내용물이 없으면 빈값으로.
                    if (i - cursor <= 1)
                        targetValue = EMPTY_STR;
                    else
                        targetValue = csvText.Substring(cursor + 1, i - cursor - 1);

                    // 쌍따옴표가 있다면 추가작업.
                    if (targetValue.IndexOf(DOUBLE_QUOTE) > -1)
                        targetValue = LoadParse(targetValue);

                    // 문자열이 빈값이면 넣을 필요가 없음.
                    if (!string.IsNullOrEmpty(targetValue))
                    {
                        if (_fileFieldInfoList[columnCount] != null)
                        {
                            if (targetValue.IndexOf("\\n") >= 0)
                                targetValue = targetValue.Replace("\\n", "\n");

                            // 프로퍼티에 값 할당.    
                            fieldType = _fileFieldInfoList[columnCount].GetReturnType();
                            if (fieldType.Equals(_strType))
                            {
                                // string타입은 박싱을 해야 들어감.. class타입이 아니라 struct에 string 있으면 그냥 하지마라.
                                object boxDatum = datum;
                                fInfo.SetMemberValue(boxDatum, targetValue);
                                datum = (T)boxDatum;
                            }
                            else
                            {
                                // 대상 T객체에 컬럼 넣기.
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

    // 타입변환.
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

    // 파일의 헤더 추출.
    private static int ParseHeaderByFile<T>(string csvText)
    {
        // 파일내 커서위치.
        int cursor = -1;

        // 대상객체의 헤더 추출.
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var type = typeof(T);
        MemberInfo[] objProperties = type.GetFields(bindingFlags).Cast<MemberInfo>()
            .Concat(type.GetProperties(bindingFlags)).ToArray();
        ParseHeaderByObj(objProperties);
        string typeName = csvText.Split(',')[0];

        // 파일의 헤더 추출.
        _fileFieldInfoList.Clear();
        for (int i = 0; i < csvText.Length; i++)
        {
            char oneChar = csvText[i];

            if (oneChar == COMMA || oneChar == LINE_FEED1 || oneChar == LINE_FEED2)
            {
                // 내용물이 있으면 헤더에 저장.
                if (i > cursor + 1)
                {
                    string targetFieldName = csvText.Substring(cursor + 1, i - cursor - 1);
                    if (targetFieldName.Contains(typeName))
                        targetFieldName = "Key";
                    // 필드명에 해당하는 컬럼위치를 찾는 다른 방법.
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

    // 객체의 헤더 추출.
    private static void ParseHeaderByObj(MemberInfo[] objProperties)
    {
        _objFieldNameList.Clear();
        for (int i = 0; i < objProperties.Length; i++)
            _objFieldNameList.Add(objProperties[i].Name);
    }

    // 객체화시 문자열 파싱.
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

    // 파일화시 문자열 파싱.
    private static string SaveParse(string target)
    {
        // 쉼표, 쌍따옴표, 개행이 존재하면 예외처리.
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

    // csv로 저장하기.
    public static string Parse<T>(List<T> data, int idCount = 1) where T : new()
    {
        // 대상 텍스트파일 얻기.
        StringBuilder sb = new StringBuilder();

        // 객체의 필드를 모두 헤더로 저장한다.
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var type = typeof(T);
        MemberInfo[] objProperties = type.GetFields(bindingFlags).Cast<MemberInfo>().Concat(type.GetProperties(bindingFlags)).ToArray();

        // ID 컬럼이 제일 앞으로 오도록 조정.
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

        // 객체 필드마다 파일헤더 순서에 맞춰 문자열화.
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
