using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Hosting;

namespace Momo.Common.DataAccess
{
    public abstract class FileRepository : IRepository
    {
        public T Load<T>(object id) where T : IEntity
        {
            return Get<T>(id);
        }

        public T Get<T>(object id) where T : IEntity
        {
            var path = GetPath<T>(id);
            return !File.Exists(path) ? default(T) : Read<T>(path);
        }

        public T Get<T>(Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return Find<T>().SingleOrDefault(predicate);
        }

        public IQueryable<T> Find<T>() where T : IEntity
        {
            return new DirectoryInfo(GetDirectoryName<T>())
                .GetFiles()
                .OrderBy(x => x.CreationTime)
                .Select(x => Read<T>(x.FullName))
                .ToList()
                .AsReadOnly()
                .AsQueryable();
        }

        public IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IEntity
        {
            return Find<T>().Where(predicate);
        }

        public T Add<T>(T entity) where T : IEntity
        {
            IdGenerator.SetId(entity);
            Write(GetPath<T>(entity.Id), entity);
            return entity;
        }

        public T Remove<T>(T entity) where T : IEntity
        {
            Remove<T>(entity.Id);
            return entity;
        }

        public void Remove<T>(object id) where T : IEntity
        {
            var path = GetPath<T>(id);
            if (File.Exists(path)) File.Delete(path);
        }

        protected abstract void Write<T>(string path, T entity);
        protected abstract T Read<T>(string path);

        private string GetPath<T>(object id)
        {
            var directoryName = GetDirectoryName<T>();
            var fileName = id.ToString();

            return directoryName + Path.DirectorySeparatorChar + fileName + ".xml";
        }

        private string GetDirectoryName<T>()
        {
            var directoryName = Path.Combine(HostingEnvironment.MapPath("~/App_Data"), typeof(T).Name);

            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            return directoryName;
        }
    }
}