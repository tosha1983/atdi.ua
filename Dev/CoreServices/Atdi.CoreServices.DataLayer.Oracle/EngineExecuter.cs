﻿using Atdi.Common;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Atdi.CoreServices.DataLayer.Oracle
{
    internal sealed class EngineExecuter : LoggedObject, IEngineExecuter
    {
        private readonly OracleQueryPatternFactory _patternFactory;
        private readonly IDataEngineConfig _config;
        private OracleConnection _connection;
        private OracleTransaction _transaction;
        private TransactionIsolationLevel _isolationLevel;

        public EngineExecuter(OracleQueryPatternFactory patternFactory, IDataEngineConfig config, ILogger logger) : base(logger)
        {
            this._patternFactory = patternFactory;
            this._config = config;
            this.TryOpenConnection();
            logger.Verbouse(Contexts.OracleEngine, Categories.Creation, Events.ObjectWasCreated.With("EngineExecuter"));
        }

        private void TryOpenConnection()
        {
            try
            {
                using (var trace = this.Logger.StartTrace(Contexts.OracleEngine, Categories.OpeningConnection, this))
                {
                    trace.SetData("Connection string", _config.ConnectionString);
                    _connection = new OracleConnection
                    {
                        ConnectionString = _config.ConnectionString
                    };
                    _connection.Open();
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.OracleEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToOpenConnection, e);
            }
        }

        private void ValidateTransaction()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException(Exceptions.TransactionHasNotBegan);
            }
        }

        private void ValidateConnection()
        {
            if (_connection == null )
            {
                throw new InvalidOperationException(Exceptions.ConnectionHasNotOpened);
            }
            if (_connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(Exceptions.ConnectionHasWrongState.With(_connection.State));
            }
        }

        public TransactionIsolationLevel IsolationLevel
        {
            get
            {
                this.ValidateTransaction();
                return _isolationLevel;
            }
        }

        public bool HasTransaction => (_transaction != null);

        public void BeginTran(TransactionIsolationLevel isoLevel = TransactionIsolationLevel.Default)
        {
            if (this.HasTransaction)
            {
                throw new InvalidOperationException(Exceptions.TransactionHasBegan);
            }
            this.ValidateConnection();
            try
            {
                if (isoLevel == TransactionIsolationLevel.Default)
                {
                    this._transaction = this._connection.BeginTransaction();
                    this._isolationLevel = TransactionIsolationLevel.Default;
                }
                else
                {
                    var sqlIsolationLevel = isoLevel.CopyTo<IsolationLevel>();
                    this._transaction = this._connection.BeginTransaction(sqlIsolationLevel);
                    this._isolationLevel = isoLevel;
                }
            }
            catch (Exception e)
            {
                Logger.Exception(Contexts.OracleEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToBeginTransaction, e);
            }
        }

        public void Commit()
        {
            this.ValidateTransaction();
            try
            {
                this._transaction.Commit();
                this._transaction = null;
            }
            catch (Exception e)
            {
                Logger.Exception(Contexts.OracleEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToCommitTransaction, e);
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                    _transaction = null;
                }
                catch (Exception e)
                {
                    Logger.Exception(Contexts.OracleEngine, Categories.Disposing, e, this);
                }
            }
            if (_connection != null) 
            {
                try
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                    }
                    _connection = null;
                }
                catch (Exception e)
                {
                    Logger.Exception(Contexts.OracleEngine, Categories.Disposing, e, this);
                }
            }
            
        }

        private  OracleExecuter CreateInternalExecuter()
        {
            return new OracleExecuter(_connection, _transaction, Logger);
        }

        public void Execute<TPattern>(TPattern queryPattern)
            where TPattern : class, IEngineQueryPattern
        {
            this.ValidateConnection();
            try
            {
                var executer = this.CreateInternalExecuter();
                var patternHandler = this._patternFactory.GetHandler(typeof(TPattern));
                patternHandler.Handle(queryPattern, executer);
            }
            catch (Exception e)
            {
                Logger.Exception(Contexts.OracleEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToExecuteQueryPattern.With(typeof(TPattern).FullName), e);
            }
        }

        public void Rollback()
        {
            this.ValidateTransaction();
            try
            {
                this._transaction.Rollback();
                this._transaction = null;
            }
            catch (Exception e)
            {
                Logger.Exception(Contexts.OracleEngine, Categories.Processing, e, this);
                throw new InvalidOperationException(Exceptions.FailedToRollbackTransaction, e);
            }
        }

        public override string ToString()
        {
            return $"State = {_connection?.State}, HasTran = {HasTransaction}, ContextName = '{_config.ContextName}' ConnectionString = '{_config.ConnectionString}'";
        }

        
    }
}