using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class DiscountPolicyMapping : IMapping
    {
        private readonly PromoDayMapping promoDayMapping = new PromoDayMapping();
        private readonly DiscountUntilExpiredMapping discountUntilExpiredMapping = new DiscountUntilExpiredMapping();

        public IEntity Load(int id, SqlDataReader dataReader)
        {
            string typeName = (string)dataReader["Discriminator"];
            if (typeName == typeof(PromoDay).Name)
            {
                return promoDayMapping.Load(id, dataReader);
            }
            if (typeName == typeof(DiscountUntilExpired).Name)
            {
                return discountUntilExpiredMapping.Load(id, dataReader);
            }
            throw new NotSupportedException();
        }

        public SqlParameter[] ModifyParameters(IEntity entity)
        {
            SqlParameter[] specificParameters = new SqlParameter[0];
            SqlParameter[] nullParameters = new SqlParameter[0];
            string typeName = "";

            if (entity is PromoDay)
            {
                specificParameters = promoDayMapping.ModifyParameters(entity);
                nullParameters = NullParametersFor(discountUntilExpiredMapping.Columns);
                typeName = typeof(PromoDay).Name;
            }
            else if (entity is DiscountUntilExpired)
            {
                specificParameters = discountUntilExpiredMapping.ModifyParameters(entity);
                nullParameters = NullParametersFor(promoDayMapping.Columns);
                typeName = typeof(DiscountUntilExpired).Name;
            }

            return specificParameters.Concat(nullParameters)
                                     .Concat(new[]
                                         {
                                             new SqlParameter("DiscountPercentage", 
                                                              ((DiscountPolicyBase) entity).DiscountPercentage.Value),
                                             new SqlParameter("Discriminator", typeName)
                                         })
                                     .ToArray();
        }

        private SqlParameter[] NullParametersFor(IEnumerable<string> columnNames)
        {
            return columnNames.Select(cn => new SqlParameter(cn, DBNull.Value)).ToArray();
        }

        public string[] Columns
        {
            get
            {
                return new[] { "DiscountPercentage", "Discriminator" }
                    .Concat(promoDayMapping.Columns)
                    .Concat(discountUntilExpiredMapping.Columns)
                    .ToArray();
            }
        }

        public string TableName
        {
            get { return "DiscountPolicies"; }
        }
    }

    internal class DiscountUntilExpiredMapping
    {
        public IEntity Load(int id, SqlDataReader dataReader)
        {
            DiscountUntilExpired discountUntilExpired = new DiscountUntilExpired();
            discountUntilExpired.Id = id;
            // DateSpan mapping - Embedded Value
            DateTime dateSpanStart = (DateTime)dataReader["FromTo_Start"];
            DateTime dateSpanEnd = (DateTime)dataReader["FromTo_End"];
            discountUntilExpired.FromTo = new DateSpan { StartDate = dateSpanStart, EndDate = dateSpanEnd };
            return discountUntilExpired;
        }

        public SqlParameter[] ModifyParameters(IEntity entity)
        {
            DiscountUntilExpired discountUntilExpired = (DiscountUntilExpired)entity;
            return new[]
                {
                    new SqlParameter("FromTo_Start", discountUntilExpired.FromTo.StartDate),
                    new SqlParameter("FromTo_End", discountUntilExpired.FromTo.EndDate)
                };
        }

        public string[] Columns
        {
            get { return new[] { "FromTo_Start", "FromTo_End" }; }
        }
    }

    internal class PromoDayMapping
    {
        public IEntity Load(int id, SqlDataReader dataReader)
        {
            PromoDay promoDay = new PromoDay();
            promoDay.Id = id;
            promoDay.DayDate = (DateTime)dataReader["DayDate"];
            return promoDay;
        }

        public SqlParameter[] ModifyParameters(IEntity entity)
        {
            PromoDay promoDay = (PromoDay)entity;
            return new[]
                {
                    new SqlParameter("DayDate", promoDay.DayDate)
                };
        }

        public string[] Columns { get { return new[] { "DayDate" }; } }
    }
}