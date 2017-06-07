using System.Collections.Generic;
using System.Reflection;
using IBot;
using TicTacToe;

namespace WpfGame.Model
{
    public interface IBotSelector
    {
        IEnumerable<Assembly> GetBotAssemblies(string folderPath);
        IEnumerable<Assembly> GetBotAssemblies();

        AbstractBot GetBotFromAssembly(Assembly botFrom, Field field, CellState cellState);
    }
}