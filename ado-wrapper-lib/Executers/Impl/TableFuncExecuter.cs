﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ado_wrapper_lib.Executers.Impl
{
    internal class FuncExecuter<ParamsType, ResultType> : AdoExecuter<ParamsType, ResultType>
    {
        public FuncExecuter(string connectionStr) => _connectionString = connectionStr;

        protected override IList<T> RunExecuter<T>(string name, 
                                                   SqlParameter[]? paramsProc, 
                                                   SqlConnection connection, 
                                                   SqlTransaction? transaction, 
                                                   int timeout = 480)
        {
            throw new NotImplementedException();
        }

        protected override void RunExecuterWithoutResult(string name, 
                                                         SqlParameter[]? paramsProc, 
                                                         SqlConnection connection, 
                                                         SqlTransaction? transaction, 
                                                         int timeout)
        {
            throw new NotImplementedException();
        }

        protected override T RunScalarExecuter<T>(string name, 
                                                  SqlParameter[]? parameters, 
                                                  SqlConnection connection, 
                                                  SqlTransaction? transaction, 
                                                  int timeout)
        {
            T resultValue;
            string paramsStr = String.Empty;
            // Список параметров в виде строки
            foreach(var param in parameters)
            {
                if (paramsStr != String.Empty)
                    paramsStr += ", ";
                
                paramsStr += param.ParameterName;
            }

            using SqlCommand command = new($"SELECT {name}({paramsStr})", connection);
            //command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
                command.Parameters.AddRange(parameters);
            command.Transaction = transaction;
            command.CommandTimeout = timeout;

            var dbValue = command.ExecuteScalar();
            resultValue = Convert.IsDBNull(dbValue) ? default : (T)dbValue;
            return resultValue;
        }
    }
}
