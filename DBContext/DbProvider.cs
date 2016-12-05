using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
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

        public ICollection<T> GetByFields<T>(EntityStruct obj) where T : EntityBase, IEntity, new()
        {
            return ExecuteListReader<T>(obj);
        }


        public ICollection<T> GetByFields<T>(BinaryExpression exp) where T : EntityBase, IEntity, new()
        {
            Func<IDbCommand, BinaryExpression, string> cmdBuilder = SelectCommandBulder.Create<T>;
            ICollection<T> result;
            using (var conn = _connection)
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = cmdBuilder.Invoke(command, exp);
                    command.CommandType = CommandType.Text;
                    conn.Open();
                    result = command.ExecuteListReader<T>();
                }
            }
            State = State.Close;
            return result;
        }

       public void Update<T>(BinaryExpression exp, EntityStruct obj) where T : EntityBase, IEntity, new()
        {
            Func<IDbCommand, Dictionary<BinaryExpression, EntityStruct>, string> cmdBuilder = UpdateBySelectCommandBulder.Create;

            using (var conn = _connection)
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = cmdBuilder.Invoke(command, new Dictionary<BinaryExpression, EntityStruct>
                        {
                            {exp, obj}
                        });
                    command.CommandType = CommandType.Text;
                    conn.Open();
                    if (command.ExecuteNonQuery() < 1)
                        throw new ExecuteQueryException(command);
                }
            }
            State = State.Close;
        }



        public void Add(EntityStruct obj)
        {
            if (obj.Value == null)
            {
                throw new ArgumentException();
            }
           
            Func<IDbCommand, ICollection<EntityStruct>, string> cmdBuilder = InsertCommandBuilder.Create;
            ExecuteNonQuery(
                new Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>>
                {
                    {
                        cmdBuilder, new List<EntityStruct>{obj}
                    }
                });
        }


        public void Remove(EntityStruct obj)
        {
            if (obj.Value == null)
            {
                throw new ArgumentException();
            }
            Func<IDbCommand, ICollection<EntityStruct>, string> cmdBuilder = DeleteCommandBuilder.Create;
            ExecuteNonQuery(
                new Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>>
                {
                    {
                        cmdBuilder, new List<EntityStruct>{obj}
                    }
                });
        }

        public void RemoveWhere(BinaryExpression exp, Type type)
        {
            throw new NotImplementedException();
        }

        public void Commit(
            ICollection<EntityStruct> updObjs,
            ICollection<EntityStruct> delObjs,
            ICollection<EntityStruct> addObjs,
           Dictionary<BinaryExpression, EntityStruct> packUpdObjs,
            Dictionary<BinaryExpression, Type> deleteExp
            )
        {
            if (updObjs.Count == 0 && delObjs.Count == 0 && addObjs.Count == 0 && packUpdObjs.Count == 0 && deleteExp.Count == 0)
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
            var packUpdDict =
                new Dictionary<Func<IDbCommand, Dictionary<BinaryExpression, EntityStruct>, string>, Dictionary<BinaryExpression, EntityStruct>>
                    {
                        {UpdateBySelectCommandBulder.Create, packUpdObjs}
                    };

            var packDeleteDict =
              new Dictionary<Func<IDbCommand, Dictionary<BinaryExpression, Type>, string>, Dictionary<BinaryExpression, Type>>
                    {
                        {DeleteWhereCommandBulder.Create, deleteExp}
                    };

            ExecuteNonQuery(cmdBuilder, packUpdDict, packDeleteDict);
        }

        private void ExecuteNonQuery(Dictionary<Func<IDbCommand, ICollection<EntityStruct>, string>, ICollection<EntityStruct>> cmdBuilder,

            Dictionary<Func<IDbCommand, Dictionary<BinaryExpression, EntityStruct>, string>, Dictionary<BinaryExpression, EntityStruct>> updObjs = null,
             Dictionary<Func<IDbCommand, Dictionary<BinaryExpression, Type>, string>, Dictionary<BinaryExpression, Type>> delObjs = null
            )
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
                    if (updObjs != null)
                    {
                        foreach (var updObj in updObjs)
                        {
                            cmdTxtBuilder.Append(updObj.Key.Invoke(command, updObj.Value));
                        }
                    }
                    if (delObjs != null)
                    {
                        foreach (var delObj in delObjs)
                        {
                            cmdTxtBuilder.Append(delObj.Key.Invoke(command, delObj.Value));
                        }
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




        private ICollection<T> ExecuteListReader<T>(EntityStruct objs) where T : EntityBase, IEntity, new()
        {
            Func<IDbCommand, EntityStruct, string> cmdBuilder = SelectCommandBulder.Create;
            ICollection<T> result;
            using (var conn = _connection)
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = cmdBuilder.Invoke(command, objs);
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
