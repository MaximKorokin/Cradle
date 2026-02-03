using Assets._Game.Scripts.Infrastructure.Game;

namespace Assets._Game.Scripts.Infrastructure.Persistence
{
    public sealed class SaveService
    {
        private readonly GameContext _gameContext;

        public SaveService(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        public void Save()
        {

        }
    }
}
