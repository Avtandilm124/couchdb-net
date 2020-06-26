﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CouchDB.Driver.Types;

namespace CouchDB.Driver
{
    internal class CouchQueryable<TResult> : IOrderedQueryable<TResult>, IListSource
    {
        private readonly Expression _expression;
        private readonly IAsyncQueryProvider _queryProvider;

        public CouchQueryable(IAsyncQueryProvider queryProvider)
        { 
            _queryProvider = queryProvider;
            _expression = Expression.Constant(this);
        }
               
        public CouchQueryable(IAsyncQueryProvider queryProvider, Expression expression)
        {
            _queryProvider = queryProvider;
            _expression = expression;
        }

        Expression IQueryable.Expression
        {
            get { return _expression; }
        }
               
        Type IQueryable.ElementType
        { 
            get { return typeof(TResult); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _queryProvider; }
        }

        public bool ContainsListCollection => false;

        public IEnumerator<TResult> GetEnumerator()
            => _queryProvider.Execute<IEnumerable<TResult>>(_expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _queryProvider.Execute<IEnumerable>(_expression).GetEnumerator();

        public Task<CouchList<TResult>> ToListAsync(CancellationToken cancellationToken = default)
            => _queryProvider.ExecuteAsync<Task<CouchList<TResult>>>(_expression, cancellationToken);

        public IList GetList()
        {
            throw new NotSupportedException();
        }
    }
}
