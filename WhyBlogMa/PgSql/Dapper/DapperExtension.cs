using Infrastructure.Dapper.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Dapper;
using System.Data.Common;
using System.Linq.Expressions;
using System.Linq;

namespace Infrastructure.Dapper
{
    public class DapperExtension<T> where T : class
    {
        public DapperExtension()
        {
            var obj = typeof(T);
            GetField(obj);
            GetTableName(obj);
        }

        public string TableName { get; set; }
        /// <summary>
        /// 查询字段
        /// </summary>
        public string Fields { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<string> AllFields { get; set; } = new List<string>();

        public string GroupFields { get; set; } = "";

        public string HavingFields { get; set; } = "";

        public string InsertFields { get; set; } = "";

        public string WhereOpt { get; set; }

        public void GetField(Type obj)
        {
            
            var fieldList = obj.GetProperties();

            for (int i = 0; i < fieldList.Length; i++)
            {
                var property = fieldList[i];
                AllFields.Add(property.Name);
                Fields += property.Name;
                if (i < fieldList.Length - 1)
                {
                    Fields += ",";
                }

            }

        }

        public IEnumerable<T> ToList(DbConnection con, IDbTransaction tran = null)
        {
 
            string sqlTemplate = "select {0} from {1} where {2} {3} {4}";
            string sql = string.Format(sqlTemplate, Fields, TableName, WhereOpt, GroupFields, HavingFields);
            return con.Query<T>(sql, tran).ToList();
        }

        public DapperExtension<T> Where(Expression<Func<T, bool>> exp)
        {
            WhereOpt = VisitExpression(exp);
            return this;
        }

        /// <summary>
        /// 考虑到group having操作复杂字段直接传字符串
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public DapperExtension<T> Select(string fields)
        {
            Fields = fields;
            return this;
        }

        public DapperExtension<T> Having(string having)
        {
            HavingFields = having;
            return this;
        }

        /// <summary>
        /// 取部分字段
        /// </summary>
        /// <param name="expression"></param>
        public DapperExtension<T> Select(Expression<Func<T, Object>> expression)
        {
            Fields = VisitExpression(expression);
            return this;
        }

        public DapperExtension<T> GroupBy(Expression<Func<T, Object>> expression)
        {
            GroupFields = "group by " + VisitExpression(expression);
            return this;
        }

        public int Insert(T obj, DbConnection con, IDbTransaction tran=null)
        {
            string sql = "insert into {1} ({0}) values ({2}) returning id";
            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                property.GetValue(obj);
            }
            return 1;
        }



        public string GetTableName(Type obj)
        {
            string result = "";
            string tableNmae = obj.FullName;

            if (CacheHelper.CachePool.Exists(tableNmae))
            {
                result = CacheHelper.CachePool.Get(tableNmae) as string;
            }
            else
            {
                TableNameAttribute attribute = obj.GetCustomAttribute(typeof(TableNameAttribute)) as TableNameAttribute;
                CacheHelper.CachePool.Set(tableNmae, attribute.Name, TimeSpan.MaxValue, true);
                result = CacheHelper.CachePool.Get(tableNmae) as string;
            }
            this.TableName = result;
            return result;
        }

        public string VisitExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                var method = expression as MethodCallExpression;
                var parms = method.Arguments;
                var objParm = new Expression[parms.Count];
                if (parms.Count != 0)
                {
                    var i = 0;
                    foreach (var parm in parms)
                    {
                        objParm[i] = parm;
                        i++;
                    }
                }

            }
            if (expression.NodeType == ExpressionType.Lambda)
            {
                var lambda = expression as LambdaExpression;
                return VisitExpression(lambda.Body);
            }
            if (expression.NodeType == ExpressionType.Equal
                || expression.NodeType == ExpressionType.AndAlso
                || expression.NodeType == ExpressionType.OrElse
                || expression.NodeType == ExpressionType.GreaterThan
                || expression.NodeType == ExpressionType.GreaterThanOrEqual
                || expression.NodeType == ExpressionType.LessThan
                || expression.NodeType == ExpressionType.LessThanOrEqual
                || expression.NodeType == ExpressionType.NotEqual)
            {
                var exp = expression as BinaryExpression;
                return BinaryExpressionAnalysis(exp);
            }


            if (expression.NodeType == ExpressionType.Constant)
            {
                var exp = expression as ConstantExpression;
                return ConstantExpressionAnalysis(exp);
            }

            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var exp = expression as MemberExpression;
                return exp.Member.Name;
            }

            if (expression.NodeType == ExpressionType.New)
            {
                var exp = expression as NewExpression;
                var memberList = new List<string>();
                foreach (var member in exp.Members)
                {
                    memberList.Add(member.Name);
                }

                return string.Join(',', memberList);
            }
            return "";
        }

        private string ConstantExpressionAnalysis(ConstantExpression exp, string field = "", string key = "")
        {
            var value = exp.Value;
            return GetStrByType(value);

        }
        private string GetStrByType(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            if (value is string)
            {
                return string.Format("'{0}'", value);
            }
            if (value is int || value is decimal || value is long)
            {
                return string.Format("{0}", value);
            }

            if (value is DateTime)
            {
                return string.Format("'{0}'", ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
            }

            var ints = value as List<int>;
            if (ints != null)
            {
                return string.Format("',{0},'", string.Join(",", ints));
            }

            var intsnull = value as List<int?>;
            if (intsnull != null)
            {
                return string.Format("',{0},'", string.Join(",", intsnull));
            }

            var values = value as int[];
            if (values != null)
            {
                return string.Format("',{0},'", string.Join(",", values));
            }

            var valuesnull = value as int?[];
            if (valuesnull != null)
            {
                return string.Format("',{0},'", string.Join(",", valuesnull));
            }


            var longList = value as List<long>;
            if (longList != null)
            {
                return string.Format("',{0},'", string.Join(",", longList));
            }

            var longListnull = value as List<long?>;
            if (longListnull != null)
            {
                return string.Format("',{0},'", string.Join(",", longListnull));
            }

            var longArr = value as long[];
            if (longArr != null)
            {
                return string.Format("',{0},'", string.Join(",", longArr));
            }

            var longArrnull = value as long?[];
            if (longArrnull != null)
            {
                return string.Format("',{0},'", string.Join(",", longArrnull));
            }

            var list = value as List<string>;
            if (list != null)
            {
                return string.Format("',{0},'", string.Join(",", list));
            }

            var strings = value as string[];
            if (strings != null)
            {
                return string.Format("',{0},'", string.Join(",", strings));
            }
            return "";
        }
        private string BinaryExpressionAnalysis(BinaryExpression exp)
        {
            var left = VisitExpression(exp.Left);
            var oper = GetOperStr(exp.NodeType);
            var right = VisitExpression(exp.Right);
            if (right == "NULL")
            {
                oper = oper == "=" ? " IS " : " IS NOT ";
            }
            return string.Format("({0} {1} {2})", left, oper, right);
        }



        private string GetOperStr(ExpressionType eType)
        {
            switch (eType)
            {
                case ExpressionType.OrElse: return "OR";
                case ExpressionType.AndAlso: return "AND";
                case ExpressionType.Or: return "|";
                case ExpressionType.And: return "&";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
                case ExpressionType.Not: return "not";
            }
            return "";
        }





    }
}
