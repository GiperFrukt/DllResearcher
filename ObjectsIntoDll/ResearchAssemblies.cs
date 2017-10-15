using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsIntoDll
{
    public static class ResearchAssemblies
    {
        /// <summary>
        /// Перебор всех dll в указанной папке для последующего разбора на классы и методы
        /// </summary>
        /// <param name="pathToFolder"> Путь до папки</param>
        /// <returns>Список классов и методов</returns>
        public static IEnumerable<Dictionary<string, List<string>>> FindClassAndMethods(string pathToFolder)
        {
            List<Dictionary<string, List<string>>> dataIntoAllDll = new List<Dictionary<string, List<string>>>();
            if (Directory.Exists(pathToFolder))
            {
                string[] dirs = Directory.GetFiles(pathToFolder, "*.dll");
                dirs.ToList().ForEach(dll => dataIntoAllDll.Add(ResearchDll(dll)));
            }
            else
            {
                dataIntoAllDll.Add(new Dictionary<string, List<string>>() { { "Ошибка при указании пути. Данной папки не существует", new List<string> { } } });
            }
            return dataIntoAllDll;
        }

        /// <summary>
        /// Поиск всех входящих публичных и защищённых классов и методов внутри них
        /// </summary>
        /// <param name="pathToDll"> Путь до конкретной dll</param>
        /// <returns>Публичные и защищённый классы и методы</returns>
        private static Dictionary<string, List<string>> ResearchDll(string pathToDll)
        {
            Assembly assembly = null; // Сборка
            IEnumerable<TypeInfo> classList = null; // Перечень классов в сборке
            // Словарь "класс - публичные/защищённые методы"
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            try
            {
                assembly = Assembly.ReflectionOnlyLoadFrom(pathToDll); // подгрузка сборки без права использовать её методы
                result.Add(pathToDll, new List<string> { });
            }
            catch (BadImageFormatException badImage)
            {
                result.Add(badImage.FileName, new List<string> { "Отсутствует манифест сборки. Анализ невозможен" });
            }
            catch (Exception ex)
            {
                result.Add(pathToDll, new List<string> { ex.Message });
            }

            // assembly будет null, если сработали блоки catch
            if (assembly != null)
            {
                // втрой блок try-catch нужен для однозначности определения ReflectionTypeLoadException
                try
                {
                    // Типы, определённые внутри данной сборки
                    classList = assembly.DefinedTypes;
                }
                catch (ReflectionTypeLoadException ex)
                {
                    result.Add(assembly.GetName().ToString(), new List<string> { "В сборке обнаружены зависимости от недоступных сборок. Анализ не возможен" });
                }
                catch (Exception ex)
                {
                    result.Add(assembly.GetName().ToString(), new List<string> { ex.Message });
                }

                // ClassList будет null, если сработали блоки catch
                if (classList != null)
                {
                    // обход всех классов и методов для составления списка публичных и защищённых
                    classList.ToList().ForEach(className => className.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToList().
                        ForEach(methodName =>
                        {
                            // Добавление метода в словарь, если он не закрытый
                            if (!methodName.IsPrivate)
                            {
                                string key = methodName.ReflectedType.Name.ToString(); // Имя класс
                                string value = methodName.Name.ToString(); // Имя метода
                                if (!result.ContainsKey(key))
                                {
                                    result.Add(key, new List<string> { });
                                }
                                result[key].Add(value);
                            }
                        }));
                }
            }
            return result;


            #region Много ретурнов
            //Assembly assembly = null; // Сборка
            //IEnumerable<TypeInfo> ClassList = null; // Перечень классов в сборке
            //// Словарь "класс - публичные/защищённые методы"
            //Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            //try
            //{
            //    assembly = Assembly.ReflectionOnlyLoadFrom(pathToDll); // подгрузка сборки без права использовать её методы
            //}
            //catch (BadImageFormatException badImage)
            //{
            //    result.Add(pathToDll, new List<string> { "Отсутствует манифест сборки. Анализ невозможен" });
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    result.Add(pathToDll, new List<string> { ex.Message });
            //    return result;
            //}



            //try
            //{
            //    // Типы, определённые внутри данной сборки
            //    ClassList = assembly.DefinedTypes;
            //}
            //catch (ReflectionTypeLoadException ex)
            //{
            //    result.Add(assembly.GetName().ToString(), new List<string> { "В сборке обнаружены зависимости от недоступных сборок. Анализ не возможен" });
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    result.Add(assembly.GetName().ToString(), new List<string> { ex.Message });
            //    return result;
            //}

            //// обход всех классов и методов для составления списка публичных и защищённых
            //ClassList.ToList().ForEach(className => className.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToList().
            //    ForEach(methodName =>
            //    {
            //        // Добавление метода в словарь, если он не закрытый
            //        if (!methodName.IsPrivate)
            //        {
            //            string key = methodName.ReflectedType.Name.ToString(); // Имя класс
            //            string value = methodName.Name.ToString(); // Имя метода
            //            if (!result.ContainsKey(key))
            //            {
            //                result.Add(key, new List<string> { });
            //            }
            //            result[key].Add(value);
            //        }
            //    }));
            //return result; 
            #endregion
        }
    }


}
