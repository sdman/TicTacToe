using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using IBot;
using TicTacToe;

namespace WpfGame.Model
{
    public class BotSelector : IBotSelector
    {
        private static readonly Type _abstractBotType = typeof(AbstractBot);

        public IEnumerable<Assembly> GetBotAssemblies(string folderPath)
        {
            string[] assemblies = Directory.GetFiles(folderPath, "*.dll", SearchOption.TopDirectoryOnly);

            IList<Assembly> loadedAssemblies = new Collection<Assembly>();

            foreach (string assemblyPath in assemblies)
            {
                try
                {
                    Assembly a = Assembly.LoadFile(assemblyPath);

                    if (HasBot(a))
                    {
                        loadedAssemblies.Add(a);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            return loadedAssemblies;
        }

        public IEnumerable<Assembly> GetBotAssemblies()
        {
            string directory = Directory.GetCurrentDirectory();
            return GetBotAssemblies(directory);
        }

        public AbstractBot GetBotFromAssembly(Assembly botFrom, Field field, CellState cellState)
        {
            if (!HasBot(botFrom))
            {
                throw new InvalidOperationException("Сборка не содержит бота.");
            }

            Type botType = botFrom.GetTypes().First(x => x.IsSubclassOf(_abstractBotType));
            return (AbstractBot)Activator.CreateInstance(botType, field, cellState);
        }

        private static bool HasBot(Assembly assembly)
        {
            Type botBaseType = _abstractBotType;
            return assembly.GetTypes().Any(x => x.IsSubclassOf(botBaseType));
        }
    }
}