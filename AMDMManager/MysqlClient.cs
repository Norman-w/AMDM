//using System;
//using System.Collections.Generic;
//using System.Text;
//using MySql.Data.MySqlClient;
//using System.Collections;
//using System.Reflection;
//using System.Data;
//using System.Drawing;
//using System.IO;
//using Newtonsoft.Json;

//    #region 参数
//    public abstract class SqlParameters
//    {
//        public string TableName { get; set; }
//        public string ResultType { get; set; }
//        public string GetAllFields(Type type)
//        {
//            return GetAllFields(type, true);
//        }
//        public string GetAllFields(Type type, bool skipListField)
//        {
//            System.Text.StringBuilder fields = new System.Text.StringBuilder();
//            System.Reflection.PropertyInfo[] infos = type.GetProperties();
//            bool hasvalue = false;
//            for (int i = 0; i < infos.Length; i++)
//            {
//                if (infos[i].PropertyType.IsGenericType && skipListField)
//                {
//                    continue;
//                }
//                if (hasvalue)
//                {
//                    fields.Append(",");
//                }
//                fields.Append(infos[i].Name);
//                hasvalue = true;
//            }
//            return fields.ToString();
//        }

//    }
//    #region 获取时
//    public abstract class SqlGeterParameters : SqlParameters
//    {
//        Dictionary<string, object> whereequals = new Dictionary<string, object>();
//        public Dictionary<string, object> WhereEquals { get { return whereequals; } set { whereequals = value; } }
//        Dictionary<string, IList> whereininfos = new Dictionary<string, IList>();
//        public Dictionary<string, IList> WhereInInfos { get { return whereininfos; } set { whereininfos = value; } }
//        //Dictionary<string, string> wherelikeparams = new Dictionary<string, string>();
//        //public Dictionary<string, string> WhereLickParams { get { return wherelikeparams; } set { wherelikeparams = value; } }
//        System.Collections.Specialized.NameValueCollection wherelikeparams = new System.Collections.Specialized.NameValueCollection();
//        public System.Collections.Specialized.NameValueCollection WhereLickParams { get { return wherelikeparams; } set { wherelikeparams = value; } }
//        Dictionary<string, object> wheremorethan = new Dictionary<string, object>();
//        /// <summary>
//        /// 重复的行的过滤分组,也就是GroupBy
//        /// </summary>
//        public string GroupBy { get; set; }
//        /// <summary>
//        /// 哪个列 大于那个值 比如你要构造 WHERE ID>0 入参为 "ID", 0;
//        /// </summary>
//        public Dictionary<string, object> WhereMoreThan { get { return this.wheremorethan; } set { this.wheremorethan = value; } }
//        Dictionary<string, object> wherelessthan = new Dictionary<string, object>();
//        /// <summary>
//        /// 哪个列 小于哪个值 比如你要构造 WHERE Id 小于 10  那就是入参为 "ID", 10;
//        /// </summary>
//        public Dictionary<string, object> WhereLessThan { get { return this.wherelessthan; } set { this.wherelessthan = value; } }
//        public Nullable<int> StartIndex { get; set; }
//        public Nullable<int> MaxRecordCount { get; set; }
//        public string OrderByColumn { get; set; }
//        public string Sort { get; set; }
//        List<string> FieldsStr2Arr(string fields)
//        {
//            string[] rets = null;
//            if (string.IsNullOrEmpty(fields))
//            {
//                //System.Collections.Specialized.NameValueCollection nv = new System.Collections.Specialized.NameValueCollection();
//                return null;
//            }
//            rets = fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
//            if (rets == null)
//            {
//                return null;
//            }
//            if (rets.Length > 0)
//            {
//                List<string> retList = new List<string>();
//                for (int i = 0; i < rets.Length; i++)
//                {
//                    retList.Add(rets[i].ToLower().Trim());
//                }
//                return retList;
//            }
//            else
//            {
//                return null;
//            }
//        }
//    }
//    public class SqlObjectGeterParameters : SqlGeterParameters
//    {
//        List<string> FieldsStr2Arr(string fields)
//        {
//            string[] rets = null;
//            if (string.IsNullOrEmpty(fields))
//            {
//                return null;
//            }
//            rets = fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
//            if (rets == null)
//            {
//                return null;
//            }
//            if (rets.Length > 0)
//            {
//                List<string> retList = new List<string>();
//                for (int i = 0; i < rets.Length; i++)
//                {
//                    retList.Add(rets[i].ToLower().Trim());
//                }
//                return retList;
//            }
//            else
//            {
//                return null;
//            }
//        }
//        List<string> fields = new List<string>();
//        public List<string> Fields { get { return fields; } set { fields = value; } }
        
//        public void SetFields(string fields)
//        {
//            this.fields.Clear();
//            if (fields.Equals("*"))
//            {
//                this.fields.Add(fields);
//            }
//            else
//            {
//                this.fields.AddRange(FieldsStr2Arr(fields));
//            }
//        }
//        /// <summary>
//        /// 设定将要获取的字段,为指定类型的所有默认类型
//        /// </summary>
//        /// <param name="type"></param>
//        public void SetFieldsAllDefault(Type type)
//        {
//            List<string> ret = new List<string>();
//            System.Reflection.PropertyInfo[] infos = type.GetProperties();
//            for (int i = 0; i < infos.Length; i++)
//            {
//                if (IsCustomType(infos[i].PropertyType))
//                {
//                    continue;
//                }
//                ret.Add(infos[i].Name);
//            }
//            this.fields.Clear();
//            this.fields.AddRange(ret);
//        }
//        /// <summary>
//        /// 根据你所感兴趣的类型和对象的值,设定要返回的字段,返回的是非常规类型被过滤出的字段,返回的结果都是小写的
//        /// </summary>
//        /// <param name="interestedFields">感兴趣的字段</param>
//        /// <param name="type">目标数据类型</param>
//        /// <returns></returns>
//        public List<string> SetFieldsAndFilteOutCustomTypeFilds(string interestedFieldsStr, Type type)
//        {
//            if (string.IsNullOrEmpty(interestedFieldsStr) == false)
//            {
//                this.fields.Clear();
//                string allfieldsStr = GetAllFields(type, false);
//                List<string> allfields = FieldsStr2Arr(allfieldsStr.ToLower());//该类型的全部字段
//                List<string> allExtFields = FilteFieldsNotDefault(type, allfields);//该类型的非默认字段
//                if (interestedFieldsStr.Equals("*"))
//                {
//                    this.fields.Add("*");
//                    //当要获取*的时候 返回的拓展字段还要加上list这样的字段
//                    return allExtFields;
//                }
//                List<string> defaultFields = FilteFieldsAllDefault(type, allfields);//该类型的默认字段
//                List<string> interestedFields = FieldsStr2Arr(interestedFieldsStr);//用户感兴趣的字段,可以包含反射内没有的
//                List<string> needSetFields = jiaoji(interestedFields, defaultFields);//感兴趣的也有 默认字段里面也有的 是本类型下可以获取的字段
//                this.fields.AddRange(needSetFields);
//                List<string> extFields = minus(interestedFields, needSetFields);
//                return extFields;
//            }
//            return null;
//        }
//        #region list string minus
//        List<string> minus(List<string> big, List<string> small)
//        {
//            List<string> bigcopy = new List<string>();
//            bigcopy.AddRange(big);
//            for (int i = 0; i < big.Count; i++)
//            {
//                if (small.Contains(big[i]))
//                {
//                    bigcopy.Remove(big[i]);
//                }
//            }
//            return bigcopy;
//        }
//        /// <summary>
//        /// 求两个list的交集,并且过滤重复项,lista 里面有 1 2 3 4 5 5 ,listb里面有 1 2 3 5 的时候,返回结果为 1,2,3,5
//        /// </summary>
//        /// <param name="lista"></param>
//        /// <param name="listb"></param>
//        /// <returns></returns>
//        List<string> jiaoji(List<string> lista, List<string> listb)
//        {
//            List<string> ret = new List<string>();
//            if (lista == null  && listb == null)
//            {
//                return ret;
//            }
//            if (listb.Count == 0 && lista.Count == 0)
//            {
//                return ret;
//            }
//            for (int i = 0; i < lista.Count; i++)
//            {
//                if (listb.Contains(lista[i]) && ret.Contains(lista[i]) == false)
//                {
//                    ret.Add(lista[i]);
//                }
//            }
//            return ret;
//        }
//        #endregion
//        public void SetFields(List<string> fields)
//        {
//            if (fields != null)
//            {
//                this.fields.Clear();
//                this.fields.AddRange(fields);
//            }
//        }
//        public string GetSetedFieldsStr()
//        {
//            StringBuilder sb = new StringBuilder();
//            bool hasvalue = false;
//            for (int i = 0; i < fields.Count; i++)
//            {
//                if (fields[i].Equals("*"))
//                {
//                    return fields[i];
//                }
//                if (hasvalue)
//                {
//                    sb.Append(",");
//                }
//                sb.AppendFormat("`{0}`", fields[i]);
//                hasvalue = true;
//            }
//            return sb.ToString();
//        }
//        public List<string> GetSetedFieldsList()
//        {
//            return fields;
//        }
//        public string[] GetSetedFieldsArr()
//        {
//            return fields.ToArray();
//        }

//        //public List<string> GetFieldsWhithoutGeneric(Type type, List<string> allFields)
//        //{
//        //    List<string> ret = new List<string>();
//        //    System.Reflection.PropertyInfo[] infos = type.GetProperties();
//        //    for (int i = 0; i < infos.Length; i++)
//        //    {
//        //        if (infos[i].PropertyType.IsGenericType)
//        //        {
//        //            continue;
//        //        }
//        //        if (allFields.Contains(infos[i].Name.ToLower()) == false)
//        //        {
//        //            continue;
//        //        }
//        //        ret.Add(infos[i].Name);
//        //    }
//        //    return ret;
//        //}
//        public List<string> FilteFieldsAllDefault(Type type, List<string> allFields)
//        {
//            List<string> ret = new List<string>();
//            System.Reflection.PropertyInfo[] infos = type.GetProperties();
//            for (int i = 0; i < infos.Length; i++)
//            {
//                Type currentFieldType = infos[i].PropertyType;
//                if (currentFieldType.IsGenericType && currentFieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
//                {
//                    Type[] tList = currentFieldType.GetGenericArguments();
//                    if (tList != null && tList.Length == 1)
//                    {
//                        Type tType = tList[0];
//                        ret.Add(infos[i].Name.ToLower());
//                        continue;
//                        //如果这种情况 那就有可能是nullable<datetime>这样的nullable类型的字段 也属于default字段
//                    }
//                }
//                else if (IsCustomType(infos[i].PropertyType))
//                {
//                    //nullable<datetime> 也属于这个自定义类型中.但是这样的字段不能忽略
//                    continue;
//                }
//                if (allFields.Contains(infos[i].Name.ToLower()) == false)
//                {
//                    continue;
//                }
//                ret.Add(infos[i].Name.ToLower());
//            }
//            return ret;
//        }
//        public List<string> FilteFieldsNotDefault(Type type, List<string> allFields)
//        {
//            List<string> ret = new List<string>();
//            System.Reflection.PropertyInfo[] infos = type.GetProperties();
//            for (int i = 0; i < infos.Length; i++)
//            {
//                if (IsCustomType(infos[i].PropertyType) == false)
//                {
//                    continue;
//                }
//                if (allFields.Contains(infos[i].Name.ToLower()) == false)
//                {
//                    continue;
//                }
//                ret.Add(infos[i].Name.ToLower());
//            }
//            return ret;
//        }
//        bool IsCustomType(Type type)
//        {
//            return (type != typeof(object) && Type.GetTypeCode(type) == TypeCode.Object);
//        }
//        #region 构造函数
//        public SqlObjectGeterParameters(string fields, string whereFieldName, object whereFieldValue)
//        {
//            this.SetFields(fields);
//            this.WhereEquals.Add(whereFieldName.Trim(), whereFieldValue);
//        }
//        public SqlObjectGeterParameters()
//        {
//        }
//        #endregion
//    }
//    public class SqlValueGeterParameters : SqlGeterParameters
//    {
//        public string Field { get; set; }
//        /// <summary>
//        /// 要获取的值是不是函数字段 比如 是否是 max(price), count(*)这样的字段,如果是的话  在获取数据的时候  对field不进行``符号的引用
//        /// </summary>
//        public bool IsFuncField { get; set; }
//        public SqlValueGeterParameters(Type parentObjType, string field, string whereFieldName, object whereFieldValue)
//        {
//            this.Field = field.ToLower().Trim();
//            this.WhereEquals.Add(whereFieldName, whereFieldValue);
//            this.TableName = parentObjType.Name;
//        }
//        public SqlValueGeterParameters(Type parentObjType)
//        {
//            this.TableName = parentObjType.Name;
//        }
//        public SqlValueGeterParameters()
//        {
//        }
//    }
//    #endregion

//    #region 更新时

//    public class SqlObjectUpdaterParamters : SqlParameters
//    {
//        Dictionary<string, object> whereequals = new Dictionary<string, object>();
//        public Dictionary<string, object> WhereEquals { get { return whereequals; } set { whereequals = value; } }
//        Dictionary<string, IList> whereininfos = new Dictionary<string, IList>();
//        public Dictionary<string, IList> WhereInInfos { get { return whereininfos; } set { whereininfos = value; } }
//        Dictionary<string, string> wherelikeparams = new Dictionary<string, string>();
//        public Dictionary<string, string> WhereLickParams { get { return wherelikeparams; } set { wherelikeparams = value; } }

//        Dictionary<string, object> updatefieldnameandvalues = new Dictionary<string, object>();

//        /// <summary>
//        /// 要更新的字段和对应的值,当要增加某个字段的数量的时候 如Count=Count+1 这里面的Key 是 Count Value是1 程序处理的时候要处理对应的加操作而不是更新操作
//        /// </summary>
//        public Dictionary<string, object> UpdateFieldNameAndValues { get { return updatefieldnameandvalues; } set { updatefieldnameandvalues = value; } }
//        bool IsCustomType(Type type)
//        {
//            return (type != typeof(object) && Type.GetTypeCode(type) == TypeCode.Object);
//        }
//        public object DataObject
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {

//                try
//                {
//                    this.updatefieldnameandvalues.Clear();
//                    Type type = value.GetType();
//                    PropertyInfo[] infos = type.GetProperties();
//                    //Dictionary<string, object> line = new Dictionary<string, object>();
//                    for (int i = 0; i < infos.Length; i++)
//                    {
//                        //if (infos[i].PropertyType.IsGenericType && infos[i].PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
//                        //{
//                        //    Console.WriteLine("没有处理的Inser信息里面包含了列表字段,这个字段将为你跳过");
//                        //    continue;
//                        //}
//                        if (IsCustomType(infos[i].PropertyType))
//                        {
//                            Type currentColType = infos[i].PropertyType;
//                            if (currentColType.IsGenericType && currentColType.
//              GetGenericTypeDefinition().Equals
//              (typeof(Nullable<>)))
//                            {
//                                //nullable 对"Nullable" 类型的处理
//                                Type[] ts = infos[i].PropertyType.GetGenericArguments();
//                                Type elemType = ts[0];
//                                //destType = elemType;
//                            }
//                            else
//                            {
//                                continue;
//                            }
//                        }
//                        object val = infos[i].GetValue(value, null);
//                        if (val == null)
//                        {
//                            //为空的字段可能是没有赋值,就不需要加到参数里面了
//                        }
//                        else
//                        {
//                            updatefieldnameandvalues.Add(infos[i].Name.ToLower(), val);
//                        }
//                    }
//                }
//                catch (Exception)
//                {

//                    throw;
//                }

//            }
//        }
//        public string Fields { get; set; }
//    }
//    public class SqlValueUpdaterParamters : SqlParameters
//    {
//        Dictionary<string, object> whereequals = new Dictionary<string, object>();
//        public Dictionary<string, object> WhereEquals { get { return whereequals; } set { whereequals = value; } }
//        Dictionary<string, IList> whereininfos = new Dictionary<string, IList>();
//        public Dictionary<string, IList> WhereInInfos { get { return whereininfos; } set { whereininfos = value; } }
//        Dictionary<string, string> wherelikeparams = new Dictionary<string, string>();
//        public Dictionary<string, string> WhereLickParams { get { return wherelikeparams; } set { wherelikeparams = value; } }

//        public string Field { get; set; }
//        public object Value { get; set; }
//        public Nullable<bool> SaveByJsonFormat { get; set; }
//    }
//    #endregion

//    #region 删除时
//    public class SqlDeleteRecordParams : SqlParameters
//    {
//        Dictionary<string, object> whereequals = new Dictionary<string, object>();
//        public Dictionary<string, object> WhereEquals { get { return whereequals; } set { whereequals = value; } }
//        Dictionary<string, IList> whereininfos = new Dictionary<string, IList>();
//        public Dictionary<string, IList> WhereInInfos { get { return whereininfos; } set { whereininfos = value; } }
//        Dictionary<string, string> wherelikeparams = new Dictionary<string, string>();
//        public Dictionary<string, string> WhereLickParams { get { return wherelikeparams; } set { wherelikeparams = value; } }

//        public SqlDeleteRecordParams(string fieldName, object indexValue)
//        {
//            this.WhereEquals.Add(fieldName, indexValue);
//        }
//        public SqlDeleteRecordParams(string filedName, IList indexCollection)
//        {
//            if (indexCollection != null && string.IsNullOrEmpty(filedName) == false)
//            {
//                List<object> values = new List<object>();
//                for (int i = 0; i < indexCollection.Count; i++)
//                {
//                    values.Add(indexCollection[i]);
//                }
//                this.WhereInInfos.Add(filedName, values);
//            }
//        }
//        public SqlDeleteRecordParams()
//        { }
//    }
//    #endregion

//    #region 增加时
//    public class SqlInsertRecordParams : SqlParameters
//    {
//        /// <summary>
//        /// 如果给定一个实例,自动用实例的类型作为表名,自动过滤掉id字段
//        /// </summary>
//        /// <param name="item"></param>
//        public SqlInsertRecordParams(object item)
//        {
//            this.DataObject = item;
//            this.RemoveField("id");
//            Type t = item.GetType();
//            int pos = t.Name.LastIndexOf(".");
//            if (pos >= 0)
//            {
//                this.TableName = t.Name.Substring(pos);
//            }
//            else
//            {
//                this.TableName = t.Name;
//            }
//        }
//        public SqlInsertRecordParams()
//        { 
//        }
//        bool IsCustomType(Type type)
//        {
//            return (type != typeof(object) && Type.GetTypeCode(type) == TypeCode.Object);
//        }
//        public object DataObject
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {

//                try
//                {
//                    this.insertfieldnameandvalues.Clear();
//                    Type type = value.GetType();
//                    PropertyInfo[] infos = type.GetProperties();
//                    Dictionary<string, object> line = new Dictionary<string, object>();
//                    for (int i = 0; i < infos.Length; i++)
//                    {
//                        //if (infos[i].PropertyType.IsGenericType && infos[i].PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
//                        //{
//                        //    Console.WriteLine("没有处理的Inser信息里面包含了列表字段,这个字段将为你跳过");
//                        //    continue;
//                        //}
//                        if (IsCustomType(infos[i].PropertyType))
//                        {
//                            Type currentColType = infos[i].PropertyType;
//                            if (currentColType.IsGenericType && currentColType.
//              GetGenericTypeDefinition().Equals
//              (typeof(Nullable<>)))
//                            {
//                                //nullable 对"Nullable" 类型的处理
//                                Type[] ts = infos[i].PropertyType.GetGenericArguments();
//                                Type elemType = ts[0];
//                                //destType = elemType;
//                            }
//                            else if (currentColType.Equals(typeof(Byte[])))//longblob类型的直接存进去
//                            { 
//                            }
//                            else if(currentColType.Equals(typeof(Image)))
//                            {

//                            }
//                            else
//                            {
//                                continue;
//                            }
//                        }
//                        object val = infos[i].GetValue(value, null);

//                        if (val == null)
//                        {
//                            //为空的字段可能是没有赋值,就不需要加到参数里面了
//                        }
//                        else
//                        {
//                            line.Add(infos[i].Name.ToLower(), val);
//                        }
//                    }
//                    this.insertfieldnameandvalues.Add(line);
//                }
//                catch (Exception)
//                {

//                    throw;
//                }

//            }
//        }
//        public List<object> DataObjects
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {

//                try
//                {
//                    this.insertfieldnameandvalues.Clear();
//                    for (int t = 0; t < value.Count; t++)
//                    {
//                        Type type = value[t].GetType();
//                        PropertyInfo[] infos = type.GetProperties();
//                        Dictionary<string, object> line = new Dictionary<string, object>();
//                        for (int i = 0; i < infos.Length; i++)
//                        {
//                            //if (infos[i].PropertyType.IsGenericType && infos[i].PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
//                            //{
//                            //    Console.WriteLine("没有处理的Inser信息里面包含了列表字段,这个字段将为你跳过");
//                            //    continue;
//                            //}
//                            if (IsCustomType(infos[i].PropertyType))
//                            {
//                                Type currentColType = infos[i].PropertyType;
//                                if (currentColType.IsGenericType && currentColType.
//                  GetGenericTypeDefinition().Equals
//                  (typeof(Nullable<>)))
//                                {
//                                    //nullable 对"Nullable" 类型的处理
//                                    Type[] ts = infos[i].PropertyType.GetGenericArguments();
//                                    Type elemType = ts[0];
//                                    //destType = elemType;
//                                }
//                                else
//                                {
//                                    continue;
//                                }
//                                continue;
//                            }
//                            object val = infos[i].GetValue(value[t], null);
//                            if (val == null)
//                            {
//                                //为空的字段可能是没有赋值,就不需要加到参数里面了
//                                line.Add(infos[i].Name.ToLower(), DBNull.Value);
//                            }
//                            else
//                            {
//                                line.Add(infos[i].Name.ToLower(), val);
//                            }
//                        }
//                        this.insertfieldnameandvalues.Add(line);
//                    }
//                }
//                catch (Exception)
//                {
//                    throw;
//                }

//            }
//        }
//        public void SetDataObjects(IList list)
//        {
//            List<object> objs = new List<object>();
//            if(list!= null)
//            {
//                for (int i = 0; i < list.Count; i++)
//                {
//                    objs.Add(list[i]);
//                }
//            }
//            this.DataObjects = objs;
//        }
//        public object RequestObject
//        {
//            get { throw new NoNullAllowedException(); }
//            set
//            {
//                //Type type = value.GetType();
//                //PropertyInfo[] infos = type.GetProperties();
//                //Dictionary<string, object> line = new Dictionary<string, object>();
//                //for (int i = 0; i < infos.Length; i++)
//                //{
//                //    if (infos[i].PropertyType.IsGenericType)
//                //    {
//                //        Console.WriteLine("没有处理的Inser信息里面包含了列表字段,这个字段将为你跳过");
//                //        continue;
//                //    }
//                //    object val = infos[i].GetValue(value, null);
//                //    if (val == null)
//                //    {
//                //        //为空的字段可能是没有赋值,就不需要加到参数里面了
//                //    }
//                //    else
//                //    {
//                //        line.Add(infos[i].Name, val);
//                //    }
//                //}
//                //this.insertfieldnameandvalues.Add(line);
//                this.DataObject = value;
//            }
//        }
//        //insert into (a,b,c,d,e) values(1,2,3,4,5);
//        List<Dictionary<string, object>> insertfieldnameandvalues = new List<Dictionary<string, object>>();
//        public List<Dictionary<string, object>> InsertFieldNameAndValues { get { return insertfieldnameandvalues; } set { insertfieldnameandvalues = value; } }
//        /// <summary>
//        /// 删除不必要的插入的字段,比如Id字段,不区分大小写
//        /// </summary>
//        /// <param name="fieldName"></param>
//        public void RemoveField(string fieldName)
//        {
//            for (int i = 0; i < insertfieldnameandvalues.Count; i++)
//            {
//                Dictionary<string, object> current = insertfieldnameandvalues[i];
//                current.Remove(fieldName.ToLower());
//            }
//        }
//    }
//    #endregion


//    #endregion

//    #region 纯正mysqlClient
//    public class MysqlClient_Bak
//    {
//        public string ip, user, pass, database; 
//        public int port;
//        public MysqlClient_Bak(string ip, string user, string pass, string database, int port)
//        {
//            this.ip = ip;
//            this.user = user;
//            this.pass = pass;
//            this.database = database;
//            this.port = port;
//        }
//        #region 增删改查
//        public List<T> GetDatas<T>(SqlObjectGeterParameters pr)
//        {
//            StringBuilder sqlSB = new StringBuilder();
//            Type resultType = typeof(T);
//            List<T> rets = new List<T>();
//            List<string> fields = pr.GetSetedFieldsList();
//            PropertyInfo[] fieldsinfo = resultType.GetProperties();
//            Dictionary<string, PropertyInfo> fieldInfoDic = new Dictionary<string, PropertyInfo>();
//            if (fieldsinfo != null)
//            {
//                for (int i = 0; i < fieldsinfo.Length; i++)
//                {
//                    PropertyInfo current = fieldsinfo[i];
//                    if (fieldInfoDic.ContainsKey(current.Name))
//                    {
//                        PropertyInfo otherField = fieldInfoDic[current.Name];
//                        if (current.DeclaringType == null || current.DeclaringType.BaseType == null)
//                        {
//                            fieldInfoDic.Add(current.Name, current);
//                        }
//                        else
//                        {
//                            if (current.DeclaringType.BaseType == otherField.PropertyType)
//                            {//如果当前的爸爸是已经存在了的,那孩子就是有新功能的,让孩子替换爸爸
//                                fieldInfoDic[current.Name] = current;
//                                continue;
//                            }
//                            else
//                            {
//                                //否则里面的已经是儿子了爸爸就不用添加了
//                            }
//                        }
//                    }
//                    else
//                    {
//                        fieldInfoDic.Add(current.Name, current);
//                    }
//                }
//            }
//            if (fields == null)
//            {
//                return null;
//            }
//            //else if (fields.Count == 1)
//            //{
//            //}
//            else
//            {
//                //SqlObjectGeterParameters pr = JSON.ToObject<SqlObjectGeterParameters>(req.ParamsJsonContant);

//                //StringBuilder sqlSB = new StringBuilder();
//                StringBuilder fieldsStr = new StringBuilder(); List<string> hasChildFields = new List<string>();
//                StringBuilder whereStr = new StringBuilder();
//                StringBuilder limitStr = new StringBuilder();
//                StringBuilder orderStr = new StringBuilder();
//                #region fields
//                fieldsStr.Append(pr.GetSetedFieldsStr());
//                #endregion
//                #region where
//                if (pr.WhereEquals.Count + pr.WhereInInfos.Count + pr.WhereLickParams.Count > 0)
//                {
//                    whereStr.Append("WHERE ");
//                    bool hasWhereParam1 = false;
//                    foreach (KeyValuePair<string, object> pair in pr.WhereEquals)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        object value = pair.Value;
//                        if (pair.Value is string)
//                        {
//                            whereStr.AppendFormat("`{0}`='{1}' ", key, ToMysqlString(value));
//                        }
//                        else
//                        {
//                            whereStr.AppendFormat("`{0}`={1} ", key, value);
//                        }
//                        hasWhereParam1 = true;
//                    }
//                    foreach (KeyValuePair<string, IList> pair in pr.WhereInInfos)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        IList value = pair.Value;
//                        if (value != null)
//                        {
//                            if (value.Count > 0)
//                            {
//                                whereStr.AppendFormat("`{0}` IN (", key);
//                                bool hasInValue = false;
//                                for (int i = 0; i < value.Count; i++)
//                                {
//                                    if (hasInValue)
//                                    {
//                                        whereStr.Append(",");
//                                    }
//                                    if (value[i] is string)
//                                    {
//                                        whereStr.AppendFormat("'{0}'", ToMysqlString(value[i]));
//                                    }
//                                    else
//                                    {
//                                        whereStr.Append(value[i]);
//                                    }
//                                    hasInValue = true;
//                                }
//                                whereStr.Append(") ");
//                            }
//                        }
//                        else
//                        {
//                            continue;
//                        }

//                        hasWhereParam1 = true;
//                    }
//                    //foreach (KeyValuePair<string, string> pair in pr.WhereLickParams)
//                    //{
//                    //    if (hasWhereParam1)
//                    //    {
//                    //        whereStr.Append("AND ");
//                    //    }
//                    //    string key = pair.Key;
//                    //    string value = pair.Value;
//                    //    whereStr.AppendFormat("`{0}` LIKE '%{1}%'", key, ToMysqlString(value));
//                    //    hasWhereParam1 = true;
//                    //}
//                    for (int i = 0; i < pr.WhereLickParams.Count; i++)
//                    {
//                        string key = pr.WhereLickParams.GetKey(i);
//                        string[] values = pr.WhereLickParams.GetValues(key);
//                        for (int j = 0; j < values.Length; j++)
//                        {
//                            if (hasWhereParam1)
//                            {
//                                whereStr.Append("AND ");
//                            }
//                            string value = values[j];
//                            whereStr.AppendFormat("`{0}` LIKE '%{1}%'", key, ToMysqlString(value));
//                            hasWhereParam1 = true;
//                        }
//                    }
//                    foreach (KeyValuePair<string, object> pair in pr.WhereLessThan)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        object value = pair.Value;
//                        if (value is string || value is DateTime)
//                        {
//                            whereStr.AppendFormat("`{0}`<'{1}' ", key, ToMysqlString(value));
//                        }
//                        else
//                        {
//                            whereStr.AppendFormat("`{0}`<{1} ", key, value);
//                        }
//                        hasWhereParam1 = true;
//                    }
//                    foreach (KeyValuePair<string, object> pair in pr.WhereMoreThan)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        object value = pair.Value;
//                        if (value is string || value is DateTime)
//                        {
//                            whereStr.AppendFormat("`{0}`>'{1}' ", key, ToMysqlString(value));
//                        }
//                        else
//                        {
//                            whereStr.AppendFormat("`{0}`>{1} ", key, value);
//                        }
//                        hasWhereParam1 = true;
//                    }
//                }
//                #endregion
//                #region order
//                if (string.IsNullOrEmpty(pr.OrderByColumn) == false)
//                {
//                    orderStr.AppendFormat("ORDER BY `{0}` {1} ", pr.OrderByColumn, pr.Sort);
//                }
//                #endregion
//                #region limit
//                if (pr.MaxRecordCount > 0)
//                {
//                    limitStr.AppendFormat("LIMIT {0},{1} ", pr.StartIndex, pr.MaxRecordCount);
//                }
//                #endregion
//                #region select


//                sqlSB.AppendFormat("SELECT {0} FROM `{1}`{2}{3}{4}", fieldsStr, pr.TableName, whereStr, orderStr, limitStr);
//                ////debug 时候打开可以查看mysql查询语句
//                //Console.WriteLine("执行mysql请求:{0}", sqlSB);
//                rets = GetDatas<T>(sqlSB.ToString());
//                #endregion
//            }
//            return rets;
//        }
//        public List<T> GetDatas<T>(string cmdStr)
//        {
//            List<T> rets = new List<T>();
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(cmdStr);
//                    cmd.Connection = conn;
//                    MySqlDataReader reader = cmd.ExecuteReader();
//                    List<T> lines = Reader2Obj<T>(reader, cmd);
//                    rets.AddRange(lines);
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(mysqlErr.Message + "QP.SqlClient.MysqlClient");
//                                     throw;
//                }
//            }
//            return rets;
//        }
//        /// <summary>
//        /// 使用mysql语句,获取多行数据,每行数据都是key-value的方式保存,返回的dictionary的字段名字索引都是小写的
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="cmdStr"></param>
//        /// <returns></returns>
//        public List<Dictionary<string,object>> GetDatas(string cmdStr)
//        {
//            List<Dictionary<string, object>> rets = new List<Dictionary<string,object>>();
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(cmdStr);
//                    cmd.Connection = conn;
//                    MySqlDataReader reader = cmd.ExecuteReader();
//                    while(reader.Read())
//                    {
//                        Dictionary<string, object> line = new Dictionary<string, object>();
//                        int colCount = reader.FieldCount;
//                        for (int i = 0; i < colCount; i++)
//                        {
//                            line.Add(reader.GetName(i).ToLower(), reader[i]);
//                        }
//                        rets.Add(line);
//                    }
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(mysqlErr.Message + "QP.SqlClient.MysqlClient");
//                    throw;
//                }
//            }
//            return rets;
//        }
//        public List<T> GetValues<T>(SqlValueGeterParameters pr)
//        {
//            List<T> rets = new List<T>();
//            if (true)
//            {
//                //SqlObjectGeterParameters pr = JSON.ToObject<SqlObjectGeterParameters>(req.ParamsJsonContant);

//                StringBuilder sqlSB = new StringBuilder();
//                StringBuilder fieldsStr = new StringBuilder(pr.Field);
//                StringBuilder whereStr = new StringBuilder();
//                StringBuilder limitStr = new StringBuilder();
//                StringBuilder groupByStr = new StringBuilder();
//                StringBuilder orderStr = new StringBuilder();

//                #region where
//                if (pr.WhereEquals.Count + pr.WhereInInfos.Count + pr.WhereLickParams.Count + pr.WhereLessThan.Count + pr.WhereMoreThan.Count > 0)
//                {
//                    whereStr.Append("WHERE ");
//                    bool hasWhereParam1 = false;
//                    foreach (KeyValuePair<string, object> pair in pr.WhereEquals)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        object value = pair.Value;
//                        if (pair.Value is string)
//                        {
//                            whereStr.AppendFormat("`{0}`='{1}' ", key, ToMysqlString(value));
//                        }
//                        else
//                        {
//                            whereStr.AppendFormat("`{0}`={1} ", key, value);
//                        }
//                        hasWhereParam1 = true;
//                    }
//                    foreach (KeyValuePair<string, IList> pair in pr.WhereInInfos)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        IList value = pair.Value;
//                        if (value != null)
//                        {
//                            if (value.Count > 0)
//                            {
//                                whereStr.AppendFormat("`{0}` IN (",key);
//                                bool hasInValue = false;
//                                for (int i = 0; i < value.Count; i++)
//                                {
//                                    if (hasInValue)
//                                    {
//                                        whereStr.Append(",");
//                                    }
//                                    if (value[i] is string)
//                                    {
//                                        whereStr.AppendFormat("'{0}'", value[i]);
//                                    }
//                                    else
//                                    {
//                                        whereStr.Append(value[i]);
//                                    }
//                                    hasInValue = true;
//                                }
//                                whereStr.Append(") ");
//                            }
//                        }
//                        else
//                        {
//                            continue;
//                        }

//                        hasWhereParam1 = true;
//                    }
//                    foreach (KeyValuePair<string, string> pair in pr.WhereLickParams)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        string value = pair.Value;
//                        whereStr.AppendFormat("`{0}` LIKE '%{1}%'", key, value);
//                        hasWhereParam1 = true;
//                    }
//                    foreach (KeyValuePair<string, object> pair in pr.WhereLessThan)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        object value = pair.Value;
//                        if (value is string || value is DateTime)
//                        {
//                            whereStr.AppendFormat("`{0}`<'{1}' ", key, ToMysqlString(value));
//                        }
//                        else
//                        {
//                            whereStr.AppendFormat("`{0}`<{1} ", key, value);
//                        }
//                        hasWhereParam1 = true;
//                    }
//                    foreach (KeyValuePair<string, object> pair in pr.WhereMoreThan)
//                    {
//                        if (hasWhereParam1)
//                        {
//                            whereStr.Append("AND ");
//                        }
//                        string key = pair.Key;
//                        object value = pair.Value;
//                        if (value is string || value is DateTime)
//                        {
//                            whereStr.AppendFormat("`{0}`>'{1}' ", key, ToMysqlString(value));
//                        }
//                        else
//                        {
//                            whereStr.AppendFormat("`{0}`>{1} ", key, value);
//                        }
//                        hasWhereParam1 = true;
//                    }
//                }
//                #endregion
//                #region order
//                if (string.IsNullOrEmpty(pr.OrderByColumn) == false)
//                {
//                    orderStr.AppendFormat("ORDER BY `{0}` {1} ", pr.OrderByColumn, pr.Sort);
//                }
//                #endregion
//                #region limit
//                if (pr.MaxRecordCount > 0)
//                {
//                    limitStr.AppendFormat("LIMIT {0},{1} ", pr.StartIndex, pr.MaxRecordCount);
//                }
//                #endregion
//                #region GroupBy
//                if (string.IsNullOrEmpty(pr.GroupBy) == false)
//                {
//                    groupByStr.AppendFormat(" GROUP BY {0}", pr.GroupBy);
//                }
//                #endregion
//                #region select
//                string fieldRealStr = pr.IsFuncField ? fieldsStr.ToString() : string.Format("`{0}`", fieldsStr.ToString());
//                sqlSB.AppendFormat("SELECT {0} FROM `{1}`{2}{3}{4}{5}", fieldRealStr, pr.TableName, whereStr, orderStr, limitStr, groupByStr);

//                using (MySqlConnection conn = new MySqlConnection())
//                {
//                    conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                    try
//                    {
//                        conn.Open();
//                        MySqlCommand cmd = new MySqlCommand(sqlSB.ToString());
//                        cmd.Connection = conn;
//                        MySqlDataReader reader = cmd.ExecuteReader();
//                        while (reader.Read())
//                        {
//                            if (reader[0] is DBNull)
//                            {
//                                //rets.Add((T)Activator.CreateInstance(typeof(T)));
//                            }
//                            else
//                            {
//                                rets.Add((T)reader[0]);
//                            }
//                        }
//                    }
//                    catch (Exception mysqlErr)
//                    {
//                        Console.WriteLine(mysqlErr.Message, "主窗体.cs SearchMysql");
//                        throw;
//                    }
//                }
//                #endregion
//            }
//            return rets;
//        }
//        public List<T> GetValues<T>(string cmdStr)
//        {
//            List<T> rets = new List<T>();
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(cmdStr);
//                    cmd.Connection = conn;
//                    MySqlDataReader reader = cmd.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        if (reader[0] is DBNull)
//                        {
//                            //rets.Add((T)Activator.CreateInstance(typeof(T)));
//                        }
//                        else
//                        {
//                            rets.Add((T)reader[0]);
//                        }
//                    }
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(mysqlErr.Message, "主窗体.cs SearchMysql");
//                    throw;
//                }
//            }
//            return rets;
//        }
//        public int GetSum(SqlGeterParameters pr, string field)
//        {
//            return GetValue<int>(SQLFuncName.SUM,pr, field);
//        }
//        public long GetCount(SqlGeterParameters pr)
//        {
//            return GetValue<long>(SQLFuncName.COUNT, pr, "*");
//        }
//        public enum SQLFuncName { SUM, COUNT };
//        public int GetValue<T>(SQLFuncName func, SqlGeterParameters pr, string field)
//        {
//            int retSum = 0;
//            StringBuilder sqlSB = new StringBuilder();
//            StringBuilder whereStr = new StringBuilder();
//            #region where
//            if (pr.WhereEquals.Count + pr.WhereInInfos.Count + pr.WhereLickParams.Count > 0)
//            {
//                whereStr.Append("WHERE ");
//                bool hasWhereParam1 = false;
//                foreach (KeyValuePair<string, object> pair in pr.WhereEquals)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    object value = pair.Value;
//                    if (pair.Value is string)
//                    {
//                        whereStr.AppendFormat("`{0}`='{1}' ", key, ToMysqlString(value));
//                    }
//                    else
//                    {
//                        whereStr.AppendFormat("`{0}`={1} ", key, value);
//                    }
//                    hasWhereParam1 = true;
//                }
//                foreach (KeyValuePair<string, IList> pair in pr.WhereInInfos)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    IList value = pair.Value;
//                    if (value != null)
//                    {
//                        if (value.Count > 0)
//                        {
//                            whereStr.AppendFormat("`{0}` IN (", key);
//                            bool hasInValue = false;
//                            for (int i = 0; i < value.Count; i++)
//                            {
//                                if (hasInValue)
//                                {
//                                    whereStr.Append(",");
//                                }
//                                if (value[i] is string)
//                                {
//                                    whereStr.AppendFormat("'{0}'", ToMysqlString(value[i]));
//                                }
//                                else
//                                {
//                                    whereStr.Append(value[i]);
//                                }
//                                hasInValue = true;
//                            }
//                            whereStr.Append(") ");
//                        }
//                    }
//                    else
//                    {
//                        continue;
//                    }

//                    hasWhereParam1 = true;
//                }
//                List<string> keys = new List<string>(pr.WhereLickParams.AllKeys);
//                foreach (var key in keys)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string val = string.Format("{0}",pr.WhereLickParams[key]);
//                    whereStr.AppendFormat("`{0}` LIKE '%{1}%'", key, ToMysqlString(val));
//                    hasWhereParam1 = true;
//                }
//                //foreach (var pair in pr.WhereLickParams)
//                //{
//                //    if (hasWhereParam1)
//                //    {
//                //        whereStr.Append("AND ");
//                //    }
//                //    //System.Collections.Specialized.NameValueCollection
//                //    string key = pair.Key;
//                //    string value = pair.Value;
//                //    whereStr.AppendFormat("`{0}` LIKE '%{1}%'", key, ToMysqlString(value));
//                //    hasWhereParam1 = true;
//                //}
//                foreach (KeyValuePair<string, object> pair in pr.WhereLessThan)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    object value = pair.Value;
//                    if (value is string || value is DateTime)
//                    {
//                        whereStr.AppendFormat("`{0}`<'{1}' ", key, ToMysqlString(value));
//                    }
//                    else
//                    {
//                        whereStr.AppendFormat("`{0}`<{1} ", key, value);
//                    }
//                    hasWhereParam1 = true;
//                }
//                foreach (KeyValuePair<string, object> pair in pr.WhereMoreThan)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    object value = pair.Value;
//                    if (value is string || value is DateTime)
//                    {
//                        whereStr.AppendFormat("`{0}`>'{1}' ", key, ToMysqlString(value));
//                    }
//                    else
//                    {
//                        whereStr.AppendFormat("`{0}`>{1} ", key, value);
//                    }
//                    hasWhereParam1 = true;
//                }
//            }
//            #endregion

//            #region select
//            sqlSB.AppendFormat("SELECT {0}({1}) FROM `{2}`{3}",func.ToString(), field, pr.TableName, whereStr);

//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(sqlSB.ToString());
//                    cmd.Connection = conn;
//                    object ret = cmd.ExecuteScalar();
//                    if (ret is DBNull)
//                    {
//                        retSum = 0;
//                    }
//                    else
//                    {
//                        retSum = Convert.ToInt32(ret);
//                    }
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(mysqlErr.Message, "主窗体.cs SearchMysql");
//                    throw;
//                }
//            }
//            #endregion
//            return retSum;
//        }
//        public long InsertData<T>(SqlInsertRecordParams pr)
//        {
//            List<MysqlDic> dics = new List<MysqlDic>();
//            for (int i = 0; i < pr.InsertFieldNameAndValues.Count; i++)
//            {
//                MysqlDic dic = new MysqlDic();
//                #region fields
//                ushort currentFieldIndex = 0;
//                foreach (KeyValuePair<string, object> pair in pr.InsertFieldNameAndValues[i])
//                {
//                    currentFieldIndex++;
//                    #region 插入数据时,如果值是默认值 就不需要赋值和插入.
//                    object value = pair.Value;
//                    Type valueType = value.GetType();
//                    if (currentFieldIndex == 6)
//                    {
                        
//                    }
//                    if (valueType.IsValueType)
//                    {
//                        if (value == Activator.CreateInstance(valueType))
//                        {
//                            continue;
//                        }
//                    }
//                    else if(value == null)
//                    {
//                        continue;
//                    }
//                    #endregion
//                    //_1是标记的哪一行
//                    string key = string.Format("{0}", pair.Key);
                    
                    
//                    dic.Add(key, value);
//                }
//                #endregion
//                dics.Add(dic);
//            }
            
//            return InsertGetId(GetInsertCommand(dics, pr.TableName));
//        }
//        /// <summary>
//        /// 通过简单的代码插入数据,自动通过实例的类型指定表名,自动去掉id字段,自动把id字段(如果存在)赋值为插入后的结果
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public long EasyInsertData(object item)
//        {
//            SqlInsertRecordParams pr = new SqlInsertRecordParams(item);
//            List<MysqlDic> dics = new List<MysqlDic>();
//            for (int i = 0; i < pr.InsertFieldNameAndValues.Count; i++)
//            {
//                MysqlDic dic = new MysqlDic();
//                #region fields
//                foreach (KeyValuePair<string, object> pair in pr.InsertFieldNameAndValues[i])
//                {
//                    //_1是标记的哪一行
//                    string key = string.Format("{0}", pair.Key);
//                    object value = pair.Value;
//                    dic.Add(key, value);
//                }
//                #endregion
//                dics.Add(dic);
//            }
//            long id  = InsertGetId(GetInsertCommand(dics, pr.TableName));
//            PropertyInfo idInfo = null;
//            idInfo = item.GetType().GetProperty("id");
//            if (idInfo == null)
//            {
//                idInfo = item.GetType().GetProperty("Id");
//            }
//            if (idInfo == null)
//            {
//                idInfo = item.GetType().GetProperty("ID");
//            }
//            if (idInfo == null)
//            {
//                idInfo = item.GetType().GetProperty("iD");
//            }
//            if (idInfo!= null)
//            {
//                idInfo.SetValue(item, id, null);
//            }
//            return id;
//        }
//        /// <summary>
//        /// 插入数据,批量的,不返回ID,只返回插入的数量
//        /// </summary>
//        /// <param name="pr"></param>
//        /// <returns></returns>
//        public int InsertDatas(SqlInsertRecordParams pr)
//        {
//            List<MysqlDic> dics = new List<MysqlDic>();
//            for (int i = 0; i < pr.InsertFieldNameAndValues.Count; i++)
//            {
//                MysqlDic dic = new MysqlDic();
//                #region fields
//                foreach (KeyValuePair<string, object> pair in pr.InsertFieldNameAndValues[i])
//                {
//                    //_1是标记的哪一行
//                    string key = string.Format("{0}", pair.Key, i);
//                    object value = pair.Value;
//                    dic.Add(key, value);
//                }
//                #endregion
//                dics.Add(dic);
//            }
//            return Insert(GetInsertCommand(dics, pr.TableName));
//        }

//        public int DeleteData(SqlDeleteRecordParams pr)
//        {
//            int deletedRowCount = 0;
//            StringBuilder sqlSB = new StringBuilder();
//            StringBuilder whereStr = new StringBuilder();
//            #region where
//            if (pr.WhereEquals.Count + pr.WhereInInfos.Count + pr.WhereLickParams.Count > 0)
//            {
//                whereStr.Append("WHERE ");
//                bool hasWhereParam1 = false;
//                foreach (KeyValuePair<string, object> pair in pr.WhereEquals)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    object value = pair.Value;
//                    if (pair.Value is string)
//                    {
//                        whereStr.AppendFormat("`{0}`='{1}' ", key, ToMysqlString(value));
//                    }
//                    else
//                    {
//                        whereStr.AppendFormat("`{0}`={1} ", key, value);
//                    }
//                    hasWhereParam1 = true;
//                }
//                foreach (KeyValuePair<string, IList> pair in pr.WhereInInfos)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    IList value = pair.Value;
//                    if (value != null)
//                    {
//                        if (value.Count > 0)
//                        {
//                            whereStr.AppendFormat("`{0}` IN (",key);
//                            bool hasInValue = false;
//                            for (int i = 0; i < value.Count; i++)
//                            {
//                                if (hasInValue)
//                                {
//                                    whereStr.Append(",");
//                                }
//                                if (value[i] is string)
//                                {
//                                    whereStr.AppendFormat("'{0}'", ToMysqlString(value[i]));
//                                }
//                                else
//                                {
//                                    whereStr.Append(value[i]);
//                                }
//                                hasInValue = true;
//                            }
//                            whereStr.Append(") ");
//                        }
//                    }
//                    else
//                    {
//                        continue;
//                    }

//                    hasWhereParam1 = true;
//                }
//                foreach (KeyValuePair<string, string> pair in pr.WhereLickParams)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    string value = pair.Value;
//                    whereStr.AppendFormat("`{0}` LIKE '%{1}%'", key, value);
//                    hasWhereParam1 = true;
//                }
//            }
//            #endregion
//            #region delete from
//            sqlSB.AppendFormat("DELETE FROM `{0}` {1}", pr.TableName, whereStr);
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(sqlSB.ToString());
//                    cmd.Connection = conn;
//                    deletedRowCount = cmd.ExecuteNonQuery();
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(mysqlErr.Message, "主窗体.cs SearchMysql");
//                    throw;
//                }
//            }
//            #endregion
//            return deletedRowCount;
//        }
//        /// <summary>
//        /// UpdateDatas和UpdateData直接方法一样 只是返回的结果不一样
//        /// </summary>
//        /// <param name="pr"></param>
//        /// <returns></returns>
//        public int UpdateDatas(SqlObjectUpdaterParamters pr)
//        {
//            MysqlDic dic = new MysqlDic();
//            if (pr.UpdateFieldNameAndValues.Count > 0)
//            {
//                foreach (KeyValuePair<string, object> pair in pr.UpdateFieldNameAndValues)
//                {
//                    string key = pair.Key;
//                    object value = pair.Value;
//                    dic.Add(key, value);
//                }
//            }
//            #region 由于设置dataobject的时候UpdateFieldNameAndValues会自动设置  所以下面这个判断没有用了
//            //else
//            //{
//            //    if (pr.DataObject != null)//而且这里判断的时候由于DataObject只可以设置 所以这样使用是错误的 后续考虑把这个字段直接变成函数为SetDataObject
//            //    {
//            //        Type objType = pr.DataObject.GetType();
//            //        PropertyInfo[] infos = objType.GetProperties();
//            //        if (infos != null)
//            //        {
//            //            for (int i = 0; i < infos.Length; i++)
//            //            {
//            //                PropertyInfo info = infos[i];
//            //                if (IsCustomType(info.PropertyType))
//            //                {
//            //                    continue;
//            //                }
//            //                string key = info.Name.ToLower();
//            //                if (string.IsNullOrEmpty(pr.Fields) == false)
//            //                {
//            //                    if (pr.Fields.ToLower().Contains(key) == false)//虽然设置了整个DataObject去给更新时使用,但是如果用户还设置了要更新哪些字段的话,就只更新哪些字段.
//            //                    //防止更新多余的字段把一些为空或者是默认值的字段也都给更新掉.也影响了数据库的性能
//            //                    {
//            //                        continue;
//            //                    }
//            //                }
//            //                object value = info.GetValue(pr.DataObject, null);
//            //                dic.Add(key, value);
//            //            }
//            //        }
//            //    }
//            //}
//            #endregion
//            if (dic.Count == 0)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                string prJson = null;
//                try
//                {
//                    prJson = JsonConvert.SerializeObject(pr);
//                }
//                catch (Exception convertPr2JsonErr)
//                {
//                    Console.WriteLine("MysqlClient.cs updatedatas 给定dic为空,且序列化SqlObjectUpdaterParamters失败:{0}", convertPr2JsonErr.Message);
//                }
//                Console.WriteLine("MysqlClient.cs 发生错误 SqlClient UpdateData Failed : EmptyDic, tableName:{0} prJson:{1}",pr.TableName, prJson);
//                Console.ResetColor();
//                return -1;
//                //throw new NotImplementedException();
//                //return false;
//            }
//            StringBuilder whereStr = new StringBuilder();
//            #region where
//            if (pr.WhereEquals.Count + pr.WhereInInfos.Count + pr.WhereLickParams.Count > 0)
//            {
//                whereStr.Append("WHERE ");
//                bool hasWhereParam1 = false;
//                foreach (KeyValuePair<string, object> pair in pr.WhereEquals)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    object value = pair.Value;
//                    if (pair.Value is string)
//                    {
//                        whereStr.AppendFormat("`{0}`='{1}' ", key, ToMysqlString(value));
//                        if (dic.ContainsKey(key.ToLower()))
//                        {
//                            dic.Remove(key.ToLower());
//                        }
//                    }
//                    else
//                    {
//                        whereStr.AppendFormat("`{0}`={1} ", key, value);
//                        if (dic.ContainsKey(key.ToLower()))
//                        {
//                            dic.Remove(key.ToLower());
//                        }
//                    }
//                    hasWhereParam1 = true;
//                }
//                foreach (KeyValuePair<string, IList> pair in pr.WhereInInfos)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    IList value = pair.Value;
//                    if (value != null)
//                    {
//                        if (value.Count > 0)
//                        {
//                            whereStr.AppendFormat("`{0}` IN (", key);
//                            bool hasInValue = false;
//                            for (int i = 0; i < value.Count; i++)
//                            {
//                                if (hasInValue)
//                                {
//                                    whereStr.Append(",");
//                                }
//                                if (value[i] is string)
//                                {
//                                    whereStr.AppendFormat("'{0}'", ToMysqlString(value[i]));
//                                }
//                                else
//                                {
//                                    whereStr.Append(value[i]);
//                                }
//                                hasInValue = true;
//                            }
//                            whereStr.Append(") ");
//                        }
//                    }
//                    else
//                    {
//                        continue;
//                    }

//                    hasWhereParam1 = true;
//                }
//                foreach (KeyValuePair<string, string> pair in pr.WhereLickParams)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    string value = pair.Value;
//                    whereStr.AppendFormat("`{0}` LIKE '%{1}%'", key, ToMysqlString(value));
//                    hasWhereParam1 = true;
//                }
//            }
//            #endregion
//            MySqlCommand updateCmd = GetUpdateCommand(dic, whereStr.ToString(), pr.TableName);
//            int ret = ExecuteNonQuery(updateCmd);
//            return ret;
//        }
//        /// <summary>
//        /// UpdateDatas和UpdateData直接方法一样 只是返回的结果不一样
//        /// </summary>
//        /// <param name="pr"></param>
//        /// <returns></returns>
//        public bool UpdateData(SqlObjectUpdaterParamters pr)
//        {
//            return UpdateDatas(pr) > 0 ? true : false;
//        }
//        public bool UpdateValue(SqlValueUpdaterParamters pr)
//        {
//            StringBuilder sb = new StringBuilder();
//            StringBuilder whereStr = new StringBuilder();
//            #region where
//            if (pr.WhereEquals.Count + pr.WhereInInfos.Count + pr.WhereLickParams.Count > 0)
//            {
//                whereStr.Append("WHERE ");
//                bool hasWhereParam1 = false;
//                foreach (KeyValuePair<string, object> pair in pr.WhereEquals)
//                {
//                    if (hasWhereParam1)
//                    {
//                        whereStr.Append("AND ");
//                    }
//                    string key = pair.Key;
//                    object value = pair.Value;
//                    if (pair.Value is string)
//                    {
//                        whereStr.AppendFormat("`{0}`='{1}' ", key, ToMysqlString(value));
//                    }
//                    else
//                    {
//                        whereStr.AppendFormat("`{0}`={1} ", key, value);

//                    }
//                    hasWhereParam1 = true;
//                }
//            }
//            #endregion
//            string valueSqlStr = ToMysqlString(pr.Value);
//            sb.AppendFormat("UPDATE `{0}` SET `{1}`='{2}' {3}", pr.TableName, pr.Field, valueSqlStr, whereStr);
//            int ret = ExecuteNonQuery(new MySqlCommand(sb.ToString()));
//            if (ret>0)
//            {
//                return true;
//            }
//            return false;
//        }

//        #endregion

//        #region MYSQL方法
//        public List<T> MysqlSelect<T>(string cmdstr)
//        {
//            List<T> retLines = new List<T>();
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4}", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(cmdstr, conn);

//                    MySqlDataReader reader = cmd.ExecuteReader();
//                    List<T> lines = Reader2Obj<T>(reader, cmd);
//                    retLines.AddRange(lines);
//                    //cmdstr = null;
//                }
//                catch (Exception mysqlErr)
//                {
//                    throw mysqlErr;
//                }
//            }
//            cmdstr = null;
//            return retLines;
//        }
//        public List<T> MysqlSelectValue<T>(string cmdstr)
//        {
//            List<T> retLines = new List<T>();
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4}", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(cmdstr, conn);
//                    cmdstr = null;
//                    MySqlDataReader reader = cmd.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        if (reader[0].GetType() == typeof(DBNull))
//                        {
//                            continue;
//                        }
//                        //object val = reader[0];
//                        //Type tp = val.GetType();
//                        retLines.Add((T)(reader[0]));
//                    }
//                }
//                catch (Exception mysqlErr)
//                {
//                    throw mysqlErr;
//                }
//            }
//            cmdstr = null;
//            return retLines;
//        }
//        public int MysqlExecute(string cmdstr)
//        {
//            int ret = -1;
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4}", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(cmdstr, conn);

//                    ret = cmd.ExecuteNonQuery();
//                    cmdstr = null;
//                }
//                catch (Exception mysqlErr)
//                {
//                    throw mysqlErr;
//                }
//            }
//            return ret;
//        }

//        //List<T> Reader2Obj<T>(MySqlDataReader reader, MySqlCommand cmd)
//        //{
//        //    IList retSelects = null;
//        //    Type nodeType = typeof(T);
//        //    Dictionary<string, System.Reflection.PropertyInfo> colnamesAndProps = new Dictionary<string, System.Reflection.PropertyInfo>();

//        //    #region 全表
//        //    retSelects = CreateIList(nodeType);
//        //    System.Reflection.PropertyInfo[] nodeTypes = nodeType.GetProperties();
//        //    for (int i = 0; i < nodeTypes.Length; i++)
//        //    {
//        //        colnamesAndProps.Add(nodeTypes[i].Name, nodeTypes[i]);
//        //    }
//        //    int linePos = 0;
//        //    int colCount = colnamesAndProps.Count;
//        //    while (reader.Read())
//        //    {
//        //        object lineObj = Activator.CreateInstance(nodeType);
//        //        object r = reader[0];
//        //        DataTable datat = reader.GetSchemaTable();
//        //        #region 根据返回值的结果填充
//        //        int retColCount = reader.FieldCount;

//        //        for (int i = 0; i < retColCount; i++)
//        //        {
//        //            try
//        //            {
//        //                object currentField = reader[i];
//        //                string currentFieldColName = reader.GetName(i);
//        //                if (colnamesAndProps.ContainsKey(currentFieldColName))
//        //                {
//        //                    object o = reader[i];
//        //                    System.Reflection.PropertyInfo currentColPropInfo = colnamesAndProps[currentFieldColName];
//        //                    Type currentColType = currentColPropInfo.PropertyType;
//        //                    if (o.GetType() == typeof(byte[]))//如果是blob类型的话,就是用blob保存的某种结构,转换后赋值
//        //                    {
//        //                        byte[] bytes = o as byte[];
//        //                        if (bytes.Length > 0)
//        //                        {
//        //                            o = Deserialize((byte[])o);
//        //                            if (o != null)
//        //                            {
//        //                                currentColPropInfo.SetValue(lineObj, o, null);
//        //                            }
//        //                        }

//        //                    }
//        //                    #region 图片格式

//        //                    else if (currentColPropInfo.PropertyType == typeof(Image))
//        //                    {
//        //                        Stream s = new MemoryStream();
//        //                        byte[] bs = o as byte[];

//        //                        if (bs != null)
//        //                        {
//        //                            if (bs.Length > 0)
//        //                            {
//        //                                try
//        //                                {
//        //                                    s.Write(bs, 0, bs.Length);
//        //                                    System.Drawing.Image img = System.Drawing.Image.FromStream(s);
//        //                                    s.Dispose();
//        //                                    s = null;
//        //                                    currentColPropInfo.SetValue(lineObj, img, null);
//        //                                }
//        //                                catch (Exception imgRedErr)
//        //                                {
//        //                                    //ErrMsg = imgRedErr.Message;
//        //                                    throw imgRedErr;
//        //                                }
//        //                            }
//        //                        }

//        //                    }
//        //                    #endregion
//        //                    else if (o.GetType() != typeof(DBNull))
//        //                    {
//        //                        Type destType = currentColPropInfo.PropertyType;
//        //                        if (currentColType.IsGenericType && currentColType.
//        //  GetGenericTypeDefinition().Equals
//        //  (typeof(Nullable<>)))
//        //                        {
//        //                            //nullable
//        //                            Type[] ts = currentColPropInfo.PropertyType.GetGenericArguments();
//        //                            Type elemType = ts[0];
//        //                            destType = elemType;
//        //                        }
//        //                        else if (destType.BaseType != null && destType.BaseType == typeof(Enum))
//        //                        {
//        //                            o = Enum.Parse(destType, o.ToString());
//        //                        }
//        //                        else
//        //                        {
//        //                            o = Convert.ChangeType(o, destType);
//        //                        }
//        //                        currentColPropInfo.SetValue(lineObj, o, null);
//        //                    }
//        //                    else
//        //                    {
//        //                        //o = null;
//        //                    }
//        //                }
//        //            }
//        //            catch (Exception mysqlErr)
//        //            {
//        //                throw mysqlErr;
//        //            }
//        //        }
//        //        #endregion


//        //        linePos++;
//        //        retSelects.Add(lineObj);
//        //    }
//        //    #endregion

//        //    List<T> ret = new List<T>();
//        //    for (int i = 0; i < retSelects.Count; i++)
//        //    {
//        //        ret.Add((T)retSelects[i]);
//        //    }
//        //    retSelects.Clear();
//        //    retSelects = null;
//        //    return ret;
//        //}
//        IList CreateIList(Type listtype)
//        {
//            if (listtype == null) return null;
//            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listtype), new object[] { });
//        }
//        object Deserialize(byte[] data)
//        {
//            object obj = null;
//            try
//            {
//                if (null == data) return null;
//                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
//                MemoryStream rems = new MemoryStream(data, 0, data.Length);
//                obj = formatter.Deserialize(rems);
//            }
//            catch (Exception e)
//            {
//                throw e;
//                //MessageBox.Show(e.Message);
//                //throw;
//            }
//            return obj;
//        }
//        #endregion

//        #region 工具函数
//        bool IsCustomType(Type type)
//        {
//            return (type != typeof(object) && Type.GetTypeCode(type) == TypeCode.Object);
//        }
//        List<string> GetFieldsArr(string fields)
//        {
//            string[] rets = null;
//            if (string.IsNullOrEmpty(fields))
//            {
//                return null;
//            }
//            rets = fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
//            if (rets == null)
//            {
//                return null;
//            }
//            if (rets.Length > 0)
//            {
//                List<string> retList = new List<string>();
//                for (int i = 0; i < rets.Length; i++)
//                {
//                    retList.Add(rets[i].ToLower().Trim());
//                }
//                return retList;
//            }
//            else
//            {
//                return null;
//            }
//        }
//        List<T> Reader2Obj<T>(MySqlDataReader reader, MySqlCommand cmd)
//        {
//            //string ErrMsg = null;
//            IList retSelects = null;
//            Type nodeType = typeof(T);
//            Dictionary<string, System.Reflection.PropertyInfo> colnamesAndProps = new Dictionary<string, System.Reflection.PropertyInfo>();

//            #region 全表
//            retSelects = CreateIList(nodeType);
//            System.Reflection.PropertyInfo[] nodeTypes = nodeType.GetProperties();
//            for (int i = 0; i < nodeTypes.Length; i++)
//            {
//                colnamesAndProps.Add(nodeTypes[i].Name.ToLower(), nodeTypes[i]);
//            }

//            int colCount = colnamesAndProps.Count;

//            object currentLineObj = null;//用来记录下面的while里面读取每一条数据的时候发现哪一条出现错误
//            string currentColName = null;//用来记录下面的while中的for循环字段的时候哪个字段出了问题
//            int linePos = 0;//用来记录下面的while中的数据读取到多少的时候出现了错误
//            #region 处理读取到的全部行
//            try
//            {
//                while (reader.Read())
//                {
//                    object lineObj = Activator.CreateInstance(nodeType); currentLineObj = lineObj;
//                    object r = reader[0];
//                    DataTable datat = reader.GetSchemaTable();
//                    #region 根据返回值的结果填充
//                    int retColCount = reader.FieldCount;

//                    for (int i = 0; i < retColCount; i++)
//                    {
//                        #region 每个字段的处理
//                        try
//                        {
//                            if (i== 5&&linePos == 4)
//                            {

//                            }
//                            object currentField = reader[i];
//                            string currentFieldColName = reader.GetName(i).ToLower(); currentColName = currentFieldColName;
//                            #region 如果反射类型集合中包含给定的类型

//                            if (colnamesAndProps.ContainsKey(currentFieldColName))
//                            {
//                                object o = reader[i];
//                                System.Reflection.PropertyInfo currentColPropInfo = colnamesAndProps[currentFieldColName];
//                                Type currentColType = currentColPropInfo.PropertyType;
//                                #region 如果是blob类型
//                                if (o.GetType() == typeof(byte[]))//如果是blob类型的话,就是用blob保存的某种结构,转换后赋值
//                                {
//                                    byte[] bytes = o as byte[];
//                                    if (bytes.Length > 0)
//                                    {
//                                        if (currentColType == typeof(string))
//                                        {
//                                            o = Encoding.UTF8.GetString(bytes);
//                                        }
//                                        else
//                                        {
//                                            o = Deserialize((byte[])o);
//                                        }
//                                        if (o != null)
//                                        {
//                                            currentColPropInfo.SetValue(lineObj, o, null);
//                                        }
//                                    }

//                                }
//                                #endregion
                                
//                                #region 图片格式

//                                else if (currentColPropInfo.PropertyType == typeof(Image))
//                                {
//                                    Stream s = new MemoryStream();
//                                    byte[] bs = o as byte[];

//                                    if (bs != null)
//                                    {
//                                        if (bs.Length > 0)
//                                        {
//                                            try
//                                            {
//                                                s.Write(bs, 0, bs.Length);
//                                                System.Drawing.Image img = System.Drawing.Image.FromStream(s);
//                                                s.Dispose();
//                                                s = null;
//                                                currentColPropInfo.SetValue(lineObj, img, null);
//                                            }
//                                            catch (Exception imgRedErr)
//                                            {
//                                                //ErrMsg = imgRedErr.Message;
//                                                throw imgRedErr;
//                                            }
//                                        }
//                                    }

//                                }
//                                #endregion
//                                #region 如果从数据库读取到的内容是dbnull
//                                else if (o.GetType() != typeof(DBNull))
//                                {
//                                    Type destType = currentColPropInfo.PropertyType;
//                                    //if (destType.BaseType != null && destType.BaseType == typeof(Enum))//ruguo shi meijude 
//                                    //{
//                                    //    o = Enum.Parse(destType, o.ToString());
//                                    //    currentColPropInfo.SetValue(lineObj, o, null);
//                                    //}
//                                    //else //qita de 
//                                    //{
//                                        if (currentColType.IsGenericType && currentColType.
//                                            GetGenericTypeDefinition().Equals
//                                            (typeof(Nullable<>)))
//                                        {
//                                            //nullable 对"Nullable" 类型的处理
//                                            Type[] ts = currentColPropInfo.PropertyType.GetGenericArguments();
//                                            Type elemType = ts[0];
//                                            destType = elemType;
//                                        }
//                                        #region 如果nullable检测完了并且还是枚举类型的
//                                        if (destType.BaseType != null && destType.BaseType == typeof(Enum))//ruguo shi meijude 
//                                        {
//                                            o = Enum.Parse(destType, o.ToString());
//                                        }
//                                        #endregion
//                                        #region 普通
//                                        else
//                                        {
//                                            o = Convert.ChangeType(o, destType);
//                                        }
//                                        #endregion
                                        
//                                        currentColPropInfo.SetValue(lineObj, o, null);
//                                    //}
//                                }
//                                #endregion
//                                #region 如果从数据库读取到的类型是string 但是 实体类的类型是enum的
//                                #endregion
//                                else
//                                {
//                                    #region 如果从数据库返回的数据是dbnull 并且 C#定义的类字段也支持nullable
//                                    if (currentColType.IsGenericType && currentColType.
//                                            GetGenericTypeDefinition().Equals
//                                            (typeof(Nullable<>)))
//                                    {
//                                        o = null;
//                                    }
//                                    #endregion
//                                    //o = null;
//                                }
//                            }
//                            #endregion
//                            #region 否则就不能处理,比如说mysql里面返回的很多的字段  c#代码中没有定义 那就不能设置代码创建的不存在的字段
                            
//                            #endregion
//                        }
//                        catch (Exception mysqlErr)
//                        {
//                            throw mysqlErr;
//                        }
//                        #endregion
//                    }
//                    #endregion
//                    linePos++;
//                    retSelects.Add(lineObj);
//                }
//            }
//            catch (Exception err)
//            {
//                throw err;
//            }
//            #endregion


//            #endregion

//            List<T> ret = new List<T>();
//            for (int i = 0; i < retSelects.Count; i++)
//            {
//                ret.Add((T)retSelects[i]);
//            }
//            return ret;
//        }
//        static public string ToMysqlString(object val)
//        {
//            if (val == null)
//            {
//                return null;
//            }
//            string str = null;
//            if (val is DateTime)
//            {
//                str = ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss");
//            }
//            else if (val is string)
//            {
//                str = val.ToString();
//                str = str.Replace("\\", "\\\\");
//                str = str.Replace("`", "\\`");
//                str = str.Replace("'", "\\'");
//                str = str.Replace("\"", "\\\"");
//            }
//            else
//            {
//                if (val.GetType() == typeof(bool))
//                {
//                    str = Convert.ToBoolean(val.ToString()) == true ? "1" : "0";
//                }
//                else
//                {
//                    str = val.ToString();
//                }
//            }
//            return str;
//        }
//        #endregion

//        #region paramDic
//        class MysqlDic : Dictionary<string, MySqlParameter>
//        {
//            byte[] Serialize(object data)
//            {
//                if (null == data) return null;
//                MemoryStream rems = null;
//                try
//                {
//                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
//                    rems = new MemoryStream();
//                    formatter.Serialize(rems, data);
//                }
//                catch (Exception)
//                {

//                    throw;
//                }

//                return rems.GetBuffer();
//            }
//            public MysqlDic() { }

//            public MysqlDic(IDictionary<string, MySqlParameter> dictionary)
//                : base(dictionary)
//            { }
//            public void Add(string key, object value)
//            {
//                MySqlParameter mysqlParameterValue = new MySqlParameter(string.Format("@{0}", key), value);

//                if (value == null)
//                {
//                    mysqlParameterValue.Value = null;
//                }
//                else if (value is string)
//                {
//                    //mysqlParameterValue.Value = MyCode.BASEFunc.Serialize(value.ToString()); //闹着玩儿测试的 哈哈
//                    string valueStr = (string)value;
//                    mysqlParameterValue.Value = valueStr;
//                }
//                else if (value is DateTime)
//                {
//                    DateTime dateTime = (DateTime)value;
//                    mysqlParameterValue.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
//                }
//                else if (value is int)
//                {
//                    mysqlParameterValue.Value = (int)value;
//                }
//                else if (value is long)
//                {
//                    mysqlParameterValue.Value = (long)value;
//                }
//                else if (value is double)
//                {
//                    mysqlParameterValue.Value = (double)value;
//                }
//                else if (value is bool)
//                {
//                    mysqlParameterValue.Value = (bool)value;
//                }
//                else if (value is float)
//                {
//                    mysqlParameterValue.Value = (float)value;
//                }
//                else if (value is decimal)
//                {
//                    mysqlParameterValue.Value = (decimal)value;
//                }
//                else if (value.GetType().BaseType == typeof(Enum))
//                {
//                    mysqlParameterValue.Value = value.ToString();
//                    mysqlParameterValue.DbType = DbType.String;
//                }
//                else
//                {
//                    mysqlParameterValue.Value = Serialize(value);
//                }

//                this.Add(key, mysqlParameterValue);
//            }

//            public new void Add(string key, MySqlParameter value)
//            {
//                if (!string.IsNullOrEmpty(key) && value != null && value.Value != null)
//                {
//                    base[key] = value;
//                }
//            }

//            public void AddAll(IDictionary<string, string> dict)
//            {
//                if (dict != null && dict.Count > 0)
//                {
//                    IEnumerator<KeyValuePair<string, string>> kvps = dict.GetEnumerator();
//                    while (kvps.MoveNext())
//                    {
//                        KeyValuePair<string, string> kvp = kvps.Current;
//                        Add(kvp.Key, kvp.Value);
//                    }
//                }
//            }
//        }
//        MySqlCommand GetInsertCommand(List<MysqlDic> dics, string tablename)
//        {
//            string insertParamsStr = BuildInsertMarkValueStr(dics);
//            StringBuilder retsb = new StringBuilder();
//            //#region 2021年6月10日11:07:42  检查一下表名是不是包含.  如果包含点,那就是指定了要用哪个数据库的某个表,就是不用切换当前客户端的要使用的database了
//            //if (tablename.Contains("."))
//            //{
//            //    string database, realTableName;
                
//            //}
//            //#endregion
//            //2021年6月10日11:20:50  现在不使用了,因为加不加`` 没有意义
//            //retsb.AppendFormat("INSERT INTO `{0}` {1}", tablename, insertParamsStr);
//            retsb.AppendFormat("INSERT INTO {0} {1}", tablename, insertParamsStr);
//            MySqlCommand cmd = new MySqlCommand(retsb.ToString());
//            for (int i = 0; i < dics.Count; i++)
//            {
//                foreach (KeyValuePair<string, MySqlParameter> pair in dics[i])
//                {
//                    string key = pair.Key;
//                    MySqlParameter value = pair.Value;
//                    value.ParameterName = string.Format("{0}_{1}",value.ParameterName,i);
//                    cmd.Parameters.Add(value);
//                }
//            }
            
//            return cmd;
//        }
//        MySqlCommand GetUpdateCommand(MysqlDic updateFieldDic, string whereParam, string tablename)
//        {
//            string updateParamsStr = BuildUpdateMarkValueStr(updateFieldDic);
//            string whereParamsStr = whereParam;//索引就用正常的文字方式记录就可以了.更新的参数才需要用@ parameter方式
//            StringBuilder retsb = new StringBuilder();
//            retsb.AppendFormat("UPDATE `{0}` SET {1} {2}", tablename, updateParamsStr, whereParamsStr);
//            MySqlCommand cmd = new MySqlCommand(retsb.ToString());
//            foreach (KeyValuePair<string, MySqlParameter> pair in updateFieldDic)
//            {
//                string key = pair.Key;
//                MySqlParameter value = pair.Value;
//                cmd.Parameters.Add(value);
//            }
//            return cmd;
//        }
//        long InsertGetId(MySqlCommand cmd)
//        {
//            long id = 0;
//            StringBuilder lastIDSB = new StringBuilder("SELECT LAST_INSERT_ID()");
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    cmd.Connection = conn;
//                    int insertRet = cmd.ExecuteNonQuery();
//                    if (insertRet == 1)
//                    {
//                        MySqlCommand lastIdCmd = new MySqlCommand(lastIDSB.ToString(), conn);
//                        object lastRetObj = lastIdCmd.ExecuteScalar();
//                        if (lastRetObj != null)
//                        {
//                            if (long.TryParse(lastRetObj.ToString(), out id))
//                            {
//                                //成功
//                                return id;
//                            }
//                        }
//                    }
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(string.Format("{0} ************ {1}", mysqlErr.Message, "ExecuteGetId"));
//                    throw;
//                }
//            }
//            return id;
//        }
//        int Insert(MySqlCommand cmd)
//        {
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    cmd.Connection = conn;
//                    int insertRet = cmd.ExecuteNonQuery();
//                    if (insertRet >= 1)
//                    {
//                        return insertRet;
//                    }
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(string.Format("{0} ************ {1}", mysqlErr.Message, "ExecuteGetId"));
//                    throw;
//                }
//            }
//            return -1;
//        }
//        public long InsertGetId(string cmd)
//        {
//            return InsertGetId(new MySqlCommand(cmd));
//        }
//        object ExecuteScalar(MySqlCommand cmd)
//        {
//            object ret = null;
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    cmd.Connection = conn;
//                    ret = cmd.ExecuteScalar();
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(string.Format("{0} ************ {1}", mysqlErr.Message, "void Execute(MySqlCommand cmd)"));
//                    throw;
//                }
//            }
//            return ret;
//        }
//        int ExecuteNonQuery(MySqlCommand cmd)
//        {
//            int ret = 0;
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    cmd.Connection = conn;
//                    ret = cmd.ExecuteNonQuery();
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(string.Format("{0} ************ {1}", mysqlErr.Message, "void Execute(MySqlCommand cmd)"));
//                    throw;
//                }
//            }
//            return ret;
//        }

//        string BuildInsertMarkValueStr(List<MysqlDic> parametersList)
//        {
//            List<StringBuilder> linesValues = new List<StringBuilder>();
//            int rowPos = 0;
//            StringBuilder columnNameSB = new StringBuilder();
//            StringBuilder firstRowValuesSB = new StringBuilder();
//            foreach (KeyValuePair<string, MySqlParameter> pair in parametersList[0])
//            {
//                string key = pair.Key;
//                MySqlParameter value = pair.Value;
//                if (!string.IsNullOrEmpty(key) && value != null && value.Value != null)
//                {
//                    if (columnNameSB.Length>0)
//                    {
//                        columnNameSB.Append(',');
//                        firstRowValuesSB.Append(',');
//                    }
//                    columnNameSB.AppendFormat("`{0}`", key);
//                    firstRowValuesSB.AppendFormat("@{0}_{1}", key,rowPos);
//                }
//            }
//            rowPos++;
//            linesValues.Add(firstRowValuesSB);
//            //从第二行开始的数据集合
            
//            for (int i = 1; i < parametersList.Count; i++)
//            {
//                StringBuilder currentLineValuesSB = new StringBuilder();
//                Dictionary<string, MySqlParameter> lineValues = parametersList[i];
//                foreach (KeyValuePair<string, MySqlParameter> pair in parametersList[0])
//                {
//                    string key = pair.Key;
//                    MySqlParameter value = pair.Value;
//                    if (value != null && value.Value != null)
//                    {
//                        if (currentLineValuesSB.Length>0)
//                        {
//                            currentLineValuesSB.Append(',');
//                        }
//                        currentLineValuesSB.AppendFormat("@{0}_{1}", key,rowPos);
//                    }
//                }
//                linesValues.Add(currentLineValuesSB);
//                rowPos++;
//            }

//            StringBuilder stringbuilderRet = new StringBuilder();

//            stringbuilderRet.AppendFormat("({0}) VALUES ({1})", columnNameSB, firstRowValuesSB);
//            for (int i = 1; i < linesValues.Count; i++)
//            {
//                stringbuilderRet.AppendFormat(",\r\n({0})", linesValues[i]);
//            }
//            return stringbuilderRet.ToString();
//        }
//        string BuildUpdateMarkValueStr(MysqlDic parameters)
//        {
//            StringBuilder stringbuilder = new StringBuilder();
//            bool hasdParam = false;
//            foreach (KeyValuePair<string, MySqlParameter> pair in parameters)
//            {
//                string key = pair.Key;
//                MySqlParameter value = pair.Value;
//                if (!string.IsNullOrEmpty(key) && value != null && value.Value != null)
//                {
//                    if (hasdParam)
//                    {
//                        stringbuilder.Append(',');
//                    }
//                    else
//                    {
//                        hasdParam = true;
//                    }
//                    stringbuilder.AppendFormat("`{0}`=@{1}", key, key);
//                }
//            }
//            return stringbuilder.ToString();
//        }
//        #endregion

//        #region 不使用泛型
//        IList Reader2Obj(MySqlDataReader reader, MySqlCommand cmd, Type resultType)
//        {
//            //string ErrMsg = null;
//            IList retSelects = null;
//            Type nodeType = resultType;
//            Dictionary<string, System.Reflection.PropertyInfo> colnamesAndProps = new Dictionary<string, System.Reflection.PropertyInfo>();

//            #region 全表
//            retSelects = CreateIList(nodeType);
//            System.Reflection.PropertyInfo[] nodeTypes = nodeType.GetProperties();
//            for (int i = 0; i < nodeTypes.Length; i++)
//            {
//                colnamesAndProps.Add(nodeTypes[i].Name.ToLower(), nodeTypes[i]);
//            }

//            int colCount = colnamesAndProps.Count;

//            object currentLineObj = null;//用来记录下面的while里面读取每一条数据的时候发现哪一条出现错误
//            string currentColName = null;//用来记录下面的while中的for循环字段的时候哪个字段出了问题
//            int linePos = 0;//用来记录下面的while中的数据读取到多少的时候出现了错误
//            #region 处理读取到的全部行
//            try
//            {
//                while (reader.Read())
//                {
//                    object lineObj = Activator.CreateInstance(nodeType); currentLineObj = lineObj;
//                    object r = reader[0];
//                    DataTable datat = reader.GetSchemaTable();
//                    #region 根据返回值的结果填充
//                    int retColCount = reader.FieldCount;

//                    for (int i = 0; i < retColCount; i++)
//                    {
//                        #region 每个字段的处理
//                        try
//                        {
//                            object currentField = reader[i];
//                            string currentFieldColName = reader.GetName(i).ToLower(); currentColName = currentFieldColName;
//                            if (colnamesAndProps.ContainsKey(currentFieldColName))
//                            {
//                                object o = reader[i];
//                                System.Reflection.PropertyInfo currentColPropInfo = colnamesAndProps[currentFieldColName];
//                                Type currentColType = currentColPropInfo.PropertyType;
//                                if (o.GetType() == typeof(byte[]))//如果是blob类型的话,就是用blob保存的某种结构,转换后赋值
//                                {
//                                    byte[] bytes = o as byte[];
//                                    if (bytes.Length > 0)
//                                    {
//                                        o = Deserialize((byte[])o);
//                                        if (o != null)
//                                        {
//                                            currentColPropInfo.SetValue(lineObj, o, null);
//                                        }
//                                    }

//                                }
//                                #region 图片格式

//                                else if (currentColPropInfo.PropertyType == typeof(Image))
//                                {
//                                    Stream s = new MemoryStream();
//                                    byte[] bs = o as byte[];

//                                    if (bs != null)
//                                    {
//                                        if (bs.Length > 0)
//                                        {
//                                            try
//                                            {
//                                                s.Write(bs, 0, bs.Length);
//                                                System.Drawing.Image img = System.Drawing.Image.FromStream(s);
//                                                s.Dispose();
//                                                s = null;
//                                                currentColPropInfo.SetValue(lineObj, img, null);
//                                            }
//                                            catch (Exception imgRedErr)
//                                            {
//                                                //ErrMsg = imgRedErr.Message;
//                                                throw imgRedErr;
//                                            }
//                                        }
//                                    }

//                                }
//                                #endregion
//                                else if (o.GetType() != typeof(DBNull))
//                                {
//                                    Type destType = currentColPropInfo.PropertyType;
//                                    if (destType.BaseType != null && destType.BaseType == typeof(Enum))//ruguo shi meijude 
//                                    {
//                                        o = Enum.Parse(destType, o.ToString());
//                                    }
//                                    else //qita de 
//                                    {
//                                        if (currentColType.IsGenericType && currentColType.
//                                            GetGenericTypeDefinition().Equals
//                                            (typeof(Nullable<>)))
//                                        {
//                                            //nullable 对"Nullable" 类型的处理
//                                            Type[] ts = currentColPropInfo.PropertyType.GetGenericArguments();
//                                            Type elemType = ts[0];
//                                            destType = elemType;
//                                        }
//                                        o = Convert.ChangeType(o, destType);
//                                        currentColPropInfo.SetValue(lineObj, o, null);
//                                    }
//                                }
//                                else
//                                {
//                                    if (currentColType.IsGenericType && currentColType.
//                                            GetGenericTypeDefinition().Equals
//                                            (typeof(Nullable<>)))
//                                    {
//                                        o = null;
//                                    }
//                                    //o = null;
//                                }
//                            }
//                        }
//                        catch (Exception mysqlErr)
//                        {
//                            throw mysqlErr;
//                        }
//                        #endregion
//                    }
//                    #endregion
//                    linePos++;
//                    retSelects.Add(lineObj);
//                }
//            }
//            catch (Exception err)
//            {
//                throw err;
//            }
//            #endregion


//            #endregion

//            return retSelects;
//        }
//        public IList GetDatas(string cmdStr, Type resultType)
//        {
//            IList list = CreateIList(resultType);
//            using (MySqlConnection conn = new MySqlConnection())
//            {
//                conn.ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Charset=utf8", ip, port, user, pass, database);
//                try
//                {
//                    conn.Open();
//                    MySqlCommand cmd = new MySqlCommand(cmdStr);
//                    cmd.Connection = conn;
//                    MySqlDataReader reader = cmd.ExecuteReader();
//                    list = Reader2Obj(reader, cmd, resultType);
//                }
//                catch (Exception mysqlErr)
//                {
//                    Console.WriteLine(mysqlErr.Message + "QP.SqlClient.MysqlClient");
//                    throw;
//                }
//            }
//            return list;
//        }
//        #endregion

//        #region 工具方法 把字符串转换成List<T>
//        public List<T> ToList<T>(string strValue)
//        {
//            Type ttype = typeof(T);
//            List<T> rets = new List<T>();
//            if (strValue == null)
//            {
//                return rets;
//            }
//            string[] arr = strValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
//            for (int i = 0; i < arr.Length; i++)
//            {
//                T t = (T)Convert.ChangeType(arr[i], ttype);
//                rets.Add(t);
//            }
//            return rets;
//        }
//        #endregion
//    }
//    #endregion