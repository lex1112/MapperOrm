using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MapperOrm.CommandBuilders;
using MapperOrm.Exceptions;
using MapperOrm.Helpers;
using MapperOrm.Models;
using MapperOrm.Repository;

namespace MapperOrm.DBContext
{
    internal class DbProvider : IDataSourceProvider
    {
        private IDbConnection _connection;

        internal DbProvider(IDbConnection connection)
        {
            _connection = connection;
            State = State.Open;
        }

        public State State { get; private set; }

        public ICollection<T> ExecuteByField<T>(Dictionary<string, object> keyValue) where T : class, IEntity, new()
        {
            if (keyValue.Count == 0)
            {
                throw new ArgumentException(string.Format("Argument keyValue is empty."));
            }
            return ExecuteListReader<T>(keyValue);
        }

        public void Add(ICollection<EntityStruct> objs)
        {
            if (objs.Count == 0)
            {
                return;
            }
            Func<IDbCommand, ICollection<EntityStruct>, string> cmdBuilder = InsertCommandBuilder.Create;
            ExecuteNonQuery(
                new Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>>
                {
                    {
                        cmdBuilder, objs
                    }
                });
        }



        public void Update(ICollection<EntityStruct> objs)
        {
            if (objs.Count == 0)
            {
                return;
            }
            Func<IDbCommand, ICollection<EntityStruct>, string> cmdBuilder = UpdateCommandBuilder.Create;
            ExecuteNonQuery(
               new Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>>
                {
                    {
                        cmdBuilder, objs
                    }
                });
        }

        public void Remove(ICollection<EntityStruct> objs)
        {
            if (objs.Count == 0)
            {
                return;
            }
            Func<IDbCommand, ICollection<EntityStruct>, string> cmdBuilder = DeleteCommandBuilder.Create;
            ExecuteNonQuery(
                new Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>>
                {
                    {
                        cmdBuilder, objs
                    }
                });
        }

        public void Commit(ICollection<EntityStruct> updObjs, ICollection<EntityStruct> delObjs, ICollection<EntityStruct> addObjs)
        {
            if (updObjs.Count == 0 && delObjs.Count == 0 && addObjs.Count == 0)
            {
                return;
            }
            var cmdBuilder = new Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>>();
            if (addObjs.Count > 0)
            {
                cmdBuilder.Add(InsertCommandBuilder.Create, addObjs);
            }
            if (updObjs.Count > 0)
            {
                cmdBuilder.Add(UpdateCommandBuilder.Create, updObjs);
            }
            if (delObjs.Count > 0)
            {
                cmdBuilder.Add(DeleteCommandBuilder.Create, delObjs);
            }

            ExecuteNonQuery(cmdBuilder);
        }

        private void ExecuteNonQuery(Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>> cmdBuilder)
        {
            using (var conn = _connection)
            {
                using (var command = conn.CreateCommand())
                {
                    var cmdTxtBuilder = new StringBuilder();
                    foreach (var builder in cmdBuilder)
                    {
                        cmdTxtBuilder.Append(builder.Key.Invoke(command, builder.Value));
                    }

                    command.CommandText = cmdTxtBuilder.ToString();
                    command.CommandType = CommandType.Text;
                    conn.Open();
                    if (command.ExecuteNonQuery() < 1)
                        throw new ExecuteQueryException(command);
                }
            }
            State = State.Close;
        }




        private ICollection<T> ExecuteListReader<T>(Dictionary<string, object> keyValue) where T : class, IEntity, new()
        {
            Func<IDbCommand, Dictionary<string, object>, string> cmdBuilder = SelectCommandBulder.Create<T>;
            ICollection<T> result;
            using (var conn = _connection)
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = cmdBuilder.Invoke(command, keyValue);
                    command.CommandType = CommandType.Text;
                    conn.Open();
                    result = command.ExecuteListReader<T>();
                }
            }
            State = State.Close;
            return result;
        }

        private void Dispose()
        {
            if (State == State.Open)
            {
                _connection.Close();
                State = State.Close;
            }
            _connection = null;
            GC.SuppressFinalize(this);
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        ~DbProvider()
        {
            Dispose();
        }
    }
}
