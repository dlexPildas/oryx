using Dapper;
using System;
using System.Data;

namespace OryxDomain.Models
{
    public class MySqlGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object value)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new Guid();
            }
            return new Guid(value.ToString());
        }
    }
}
