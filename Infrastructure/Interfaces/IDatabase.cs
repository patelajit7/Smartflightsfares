using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IDatabase
    {
        int Create<T>(T obj) where T : ContentBase;
        void Update<T>(T obj) where T : ContentBase;
        List<T> List<T>() where T : ContentBase;
        List<T> Where<T>(Expression<Func<T, bool>> expression) where T : ContentBase;
        List<T> ExecuteQuery<T>(string query, List<SqlParameter> param);

        //  List<T> ExecuteQuery<T>(string query, List<SqlParameter> param, EnumSqlConnection EnumSqlConnection);
    }
}
