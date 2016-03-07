using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using OrmExample.Entities;

namespace OrmExample.Mapping
{
    public class DiscountPoliciesMapper : IMapper<DiscountPolicyBase>
    {
        private readonly EntityMapper entityMapper;

        public DiscountPoliciesMapper(string connectionString)
        {
            entityMapper = new EntityMapper(connectionString, new DiscountPolicyMapping());
        }

        public DiscountPolicyBase GetById(int id)
        {
            return (DiscountPolicyBase)entityMapper.GetById(id);
        }

        public IEnumerable<DiscountPolicyBase> GetAll()
        {
            return entityMapper.GetAll().Cast<DiscountPolicyBase>();
        }

        public void Insert(DiscountPolicyBase entity)
        {
            entityMapper.Insert(entity);
        }

        public void Update(DiscountPolicyBase entity)
        {
            entityMapper.Update(entity);
        }

        public void DeleteById(int id)
        {
            entityMapper.DeleteById(id);
        }
    }
}