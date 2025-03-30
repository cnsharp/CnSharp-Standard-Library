﻿using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CnSharp.Expressions
{
    internal class QueryableParser : IQueryableParser
    {
        public IQueryable DataSource { get; set; }

        public string FinalMethodName { get; set; }

        public Func<Expression, Expression> Converter { get; set; }

        public object Value { get; set; }

        private void Execute(MethodInfo method, params object[] @parameters)
        {
            var data = new ArrayList();
            data.Add(DataSource);
            if (@parameters.Any())
                data.AddRange(@parameters);

            Value = method.MakeGenericMethod(DataSource.ElementType).Invoke(null, data.ToArray());
            FinalMethodName = method.Name;
        }

        private void Attach(MethodInfo method, params object[] @parameters)
        {
            var data = new ArrayList();
            data.Add(DataSource);
            if (@parameters.Any())
                data.AddRange(@parameters);

            DataSource = method.MakeGenericMethod(DataSource.ElementType).Invoke(null, data.ToArray()) as IQueryable;
        }

        private void Attach(MethodInfo method, Func<Type[]> methodMaker, params object[] @parameters)
        {
            var data = new ArrayList();
            data.Add(DataSource);
            data.AddRange(parameters);

            var parameterTypes = methodMaker().ToList();
            parameterTypes.Insert(0, DataSource.ElementType);

            method = method.MakeGenericMethod(parameterTypes.ToArray());
            DataSource = method.Invoke(null, data.ToArray()) as IQueryable;
        }

        public void Build(MethodCallExpression methodCall)
        {
            if (methodCall == null)
                return;

            var methodName = methodCall.Method.Name;
            switch (methodName)
            {
                case "First":
                case "FirstOrDefault":
                case "Count":
                case "LongCount":
                case "Any":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var data = typeof(Queryable).GetMethods().Where(p => p.Name == methodName).Select(p => Tuple.Create(p, p.GetParameters())).ToArray();

                        var method = typeof(Queryable).GetMethods().First(p => p.Name == methodName && p.GetParameters().Length == methodCall.Arguments.Count);
                        Execute(method, methodCall.Arguments.Count > 1 ? new object[] { Converter(methodCall.Arguments[1]) } : new object[0]);
                    }
                    break;

                case "Single":
                case "SingleOrDefault":
                    throw new NotSupportedException("Please use Count and First/FirstOrDefault instead of this functionality!");

                case "Last":
                case "LastOrDefault":
                    throw new NotSupportedException("Please use OrderByDescending and First/FirstOrDefault instead of this functionality!");

                case "Where":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var method = typeof(Queryable).GetMethods().First(p => p.Name == methodName && p.GetParameters().Length == methodCall.Arguments.Count);
                        Attach(method, Converter(methodCall.Arguments[1]));
                    }
                    break;

                case "ToArray":
                case "ToList":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var method = typeof(Enumerable).GetMethods().First(p => p.Name == methodName && p.GetParameters().Length == methodCall.Arguments.Count);
                        Execute(method);
                    }
                    break;

                case "Select":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var unaryExpression = methodCall.Arguments[1] as UnaryExpression;

                        // Does not support Select extension method with index
                        var method = typeof(Queryable).GetMethods().First(p => p.Name == methodName && p.GetParameters().Length == methodCall.Arguments.Count);
                        var expr = Converter(methodCall.Arguments[1]);
                        Attach(method, () => new Type[] { (expr as LambdaExpression).ReturnType }, expr);
                    }
                    break;

                case "OrderBy":
                case "ThenBy":
                case "OrderByDescending":
                case "ThenByDescending":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var method = typeof(Queryable).GetMethods().First(p => p.Name == methodName && p.GetParameters().Length == methodCall.Arguments.Count);
                        var expr = Converter(methodCall.Arguments[1]);
                        Attach(method, () => new Type[] { (expr as LambdaExpression).ReturnType }, expr);
                    }
                    break;

                case "Take":
                case "Skip":
                    {
                        Build(methodCall.Arguments[0] as MethodCallExpression);

                        var method = typeof(Queryable).GetMethods().First(p => p.Name == methodName && p.GetParameters().Length == methodCall.Arguments.Count);
                        Attach(method, Converter(methodCall.Arguments[1]));
                    }
                    break;

                default:
                    throw new NotSupportedException("OData protocol does not support LINQ extension method: " + methodName);
            }
        }
    }
}
