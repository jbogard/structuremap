﻿using System;
using System.Linq;
using System.Linq.Expressions;
using StructureMap.TypeRules;

namespace StructureMap.Building.Interception
{
    public class FuncInterceptor<T> : IInterceptor
    {
        private readonly LambdaExpression _expression;
        private readonly string _description;

        public FuncInterceptor(Expression<Func<T, T>> expression, string description = null)
        {
            _expression = expression;
            _description = description;
        }

        public FuncInterceptor(Expression<Func<IBuildSession, T, T>> expression, string description = null)
        {
            _expression = expression;
            _description = description;
        }

        public string Description
        {
            get
            {
                return _description ?? _expression
                    .ReplaceParameter(Accepts, Expression.Parameter(Accepts, Accepts.Name))
                    .ReplaceParameter(typeof(IBuildSession), Expression.Parameter(typeof(IBuildSession), "IBuildSession"))
                    .Body.ToString();

            }
        }
        public InterceptorRole Role { get { return InterceptorRole.Decorates; } }

        public Expression ToExpression(ParameterExpression session, ParameterExpression variable)
        {
            var body = _expression.ReplaceParameter(Accepts, variable)
                .ReplaceParameter(typeof (IBuildSession), session).Body;

            return Expression.Convert(body, typeof (T));
        }

        public IInterceptorPolicy ToPolicy()
        {
            return new InterceptionPolicy<T>(this);
        }

        public Type Accepts { get { return typeof (T); } }
        public Type Returns { get { return typeof (T); } }

        protected bool Equals(FuncInterceptor<T> other)
        {
            return Equals(_expression, other._expression);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FuncInterceptor<T>) obj);
        }

        public override int GetHashCode()
        {
            return (_expression != null ? _expression.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return "Interceptor of {0}: {1}".ToFormat(typeof(T).GetFullName(), Description);
        }
    }
}