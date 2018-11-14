using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Apps.Models.Sys
{
    public partial class permModel
    {
        public string KeyCode { get; set; }
        public bool IsValid { get; set; }
        /// <summary>
        /// 类别 1:按钮权限 2：数据权限
        /// </summary>
        public int Category { get; set; }


       /// <summary>
       /// 列表数据过滤
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="list">列表</param>
       /// <param name="filePath">权限</param>
       /// <returns></returns>
        public static List<T> SetDataTransparent<T>(List<T> list,string filePath)
        {

            List<permModel> perm =HttpContext.Current.Session[filePath] as List<permModel>;
            //获得被禁用的列
            perm = perm.Where(a => a.Category == 2).ToList();
            if (perm.Count() > 0)
            {
                foreach (var m in list)
                {
                    Type type = m.GetType(); //获取类型
                    foreach (var r in perm)
                    {
                        System.Reflection.PropertyInfo propertyInfo = type.GetProperty(r.KeyCode); //获取指定名称的属性
                        //被禁用列会被处理掉
                        propertyInfo.SetValue(m, SetDefaultValue(propertyInfo.PropertyType.FullName), null);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 单个模型数据过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">列表</param>
        /// <param name="filePath">权限</param>
        /// <returns></returns>
        public static T SetSingleDataTransparent<T>(T model, string filePath)
        {
            List<permModel> perm = HttpContext.Current.Session[filePath] as List<permModel>;
            //获得被禁用的列
            perm = perm.Where(a => a.Category == 2).ToList();
            if (perm.Count() > 0)
            {
                    Type type = model.GetType(); //获取类型
 
                    foreach (var r in perm)
                    {
                        System.Reflection.PropertyInfo propertyInfo = type.GetProperty(r.KeyCode); 


                        //被禁用列会被处理掉
                        propertyInfo.SetValue(model, SetDefaultValue(propertyInfo.PropertyType.FullName), null);
                    }
            }
            return model;
        }


        /// <summary>
        /// 修改模型禁用修改(把原始的值赋回修改模型，让其保存BLL层不变！)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">修改的模型</param>
        /// <param name="dataModel">原来的模型</param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T SetSingleDataTransparent<T>(T model,T dataModel, string filePath)
        {
            List<permModel> perm = HttpContext.Current.Session[filePath] as List<permModel>;
            //获得被禁用的列
            perm = perm.Where(a => a.Category == 2).ToList();
            if (perm.Count() > 0)
            {
                Type type = model.GetType(); //获取类型
                Type type2 = dataModel.GetType();
                foreach (var r in perm)
                {
                    //修改的模型
                    System.Reflection.PropertyInfo propertyInfo = type.GetProperty(r.KeyCode);
                    //原来的模型
                    System.Reflection.PropertyInfo propertyInfo2 = type2.GetProperty(r.KeyCode);
                    //将原来模型的值赋值给修改的模型，让其维持原有的值
                    propertyInfo.SetValue(model, propertyInfo2.GetValue(r.KeyCode), null);
                }
            }
            return model;
        }

        public static object SetDefaultValue(string fullName)
        {
            if (fullName.Contains("System.Int") || fullName.Contains("System.Decimal") || fullName.Contains("System.Boolean") || fullName.Contains("System.Single"))
            {
                return 0;
            }
            else
            {
                return "";
            }
            
        }



    }
}
