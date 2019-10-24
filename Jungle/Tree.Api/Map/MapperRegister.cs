using ExpressMapper;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tree.Api.Map {
    public static class MapperRegister {
        public static void RegisterMaps() {
            RegisterAllCustomMapsFromExecutingAssembly();
        }

        private static void RegisterAllCustomMapsFromExecutingAssembly() {
            var registerMapperMethod = typeof(Mapper).GetMethods()
                     .Where(x => x.Name == "RegisterCustom" && x.GetGenericArguments().Count() == 3).First();
            var iCustomTypeMapper = typeof(ICustomTypeMapper);
            var customTypeMappers = Assembly.GetExecutingAssembly().GetTypes().Where(x => iCustomTypeMapper.IsAssignableFrom(x));

            foreach (var mapperClass in customTypeMappers) {
                var genericCustomTypeMapperInterface =
                    mapperClass.GetInterfaces().Where(x => x.IsGenericType && iCustomTypeMapper.IsAssignableFrom(x)).First();

                if (genericCustomTypeMapperInterface == null) {
                    continue;
                }

                var genericArguments = genericCustomTypeMapperInterface.GetGenericArguments();
                if (genericArguments.Count() != 2) {
                    continue;
                }
                var mapFromType = genericArguments.First();
                var mapToType = genericArguments.Last();

                var genericRegisterMapperMethod = registerMapperMethod.MakeGenericMethod(mapFromType, mapToType, mapperClass);
                genericRegisterMapperMethod.Invoke(null, null);
            }
        }
    }
}