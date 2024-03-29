﻿using MyEvernote.Common;
using MyEvernote.Core.DataAccess;  
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class Repository<T> : RepositoryBase,IDataAccess<T> where T : class
    {
      
        private DbSet<T> _objectSet;

        public Repository()
        {
            _objectSet = context.Set<T>();
        }

        public List<T> List()
        {
            return _objectSet.ToList();
        }

        public IQueryable<T> ListQueryable()
        {
            return _objectSet.AsQueryable<T>();
        }

        public int Insert(T obj)
        {
            _objectSet.Add(obj);

            if (obj is MyEntityBase) // obj MyEntityBase'den miras aldıysa eğer burdada otomatik ekleyebilirisn
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime now = DateTime.Now;
                o.CreatedOn = now;
                o.ModifieOn = now;
                o.ModifiedUsername = App.Common.GetCurrentUsername() ;
            }

            return Save();
        }

        public List<T> List(Expression<Func<T,bool>> where)
        {
            return _objectSet.Where(where).ToList();
        }

        public int Update(T obj)
        {
            if (obj is MyEntityBase) // obj MyEntityBase'den miras aldıysa eğer burdada ekleyebilirsin
            {
                MyEntityBase o = obj as MyEntityBase;
                o.ModifieOn = DateTime.Now;
                o.ModifiedUsername = App.Common.GetCurrentUsername();
            }
            return Save();
        }

        public int Delete(T obj)
        {
            //eğer silme işlemi değilde IsRemoved = true yada false yaparsan alttaki şartı da kullanabilirsin
            //if (obj is MyEntityBase) // obj MyEntityBase'den miras aldıysa eğer
            //{
            //    MyEntityBase o = obj as MyEntityBase;
            //    o.ModifieOn = DateTime.Now;
            //    o.ModifiedUsername = "system";
            //}
             _objectSet.Remove(obj);
            return Save();
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public T Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.FirstOrDefault(where);
        }
    }
}
