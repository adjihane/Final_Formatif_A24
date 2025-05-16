using System;
using Microsoft.EntityFrameworkCore;
using mock.depart.Data;

namespace mock.depart.Services
{
	public abstract class BaseService<T> where T:class
    {
        protected readonly ApplicationDbContext db;
        protected DbSet<T> set;

        public BaseService()
        {

        }

        public BaseService(ApplicationDbContext context)
        {
            this.db = context;
            this.set = db.Set<T>();
        }

		public virtual T Add(T item)
		{
            db.Add(item!);
            db.SaveChanges();

            return item;
        }

        public virtual T Update(T item)
        {
            db.Update(item!);
            db.SaveChanges();

            return item;
        }

        public virtual T? Delete(int id)
        {

            T? item = Get(id);
            if (item != null)
            {
                set.Remove(item);
            }
            db.SaveChanges();

            return item;
        }

        public virtual IQueryable<T> GetAll()
        {
            return set;
        }

        public virtual T? Get(int id)
        {
            T? item = set.Find(id);
            return item;
        }

        public bool Exists(int id)
        {
            return set.Find(id) != null;
        }
    }
}

