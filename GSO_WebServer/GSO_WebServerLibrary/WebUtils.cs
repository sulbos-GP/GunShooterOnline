using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary
{
    public static class WebUtils
    {
        /// <summary>
        /// Request된 모델의 값들이 null인지 확인
        /// </summary>
        public static bool IsValidModelState<TModel>(TModel model)
        {
            if (model == null)
            {
                return false;
            }

            foreach (var property in model.GetType().GetProperties())
            {
  
                var value = property.GetValue(model);

                //value가 null인지 확인
                if (value == null)
                {
                    return false;
                }

                //string의 경우 길이도 확인
                if (property.PropertyType == typeof(string) && string.IsNullOrEmpty(value as string))
                {
                    return false;
                }

            }

            return true;
        }

    }
}
