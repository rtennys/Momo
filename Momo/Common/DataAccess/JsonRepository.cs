using System;
using System.IO;

namespace Momo.Common.DataAccess
{
    public class JsonRepository : FileRepository
    {
        protected override void Write<T>(string path, T entity)
        {
            File.WriteAllText(path, entity.SerializeJson());
        }

        protected override T Read<T>(string path)
        {
            return File.ReadAllText(path).DeserializeJson<T>();
        }
    }
}