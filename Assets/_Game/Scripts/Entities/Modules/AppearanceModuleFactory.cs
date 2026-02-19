using Assets._Game.Scripts.Entities.Units;

namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AppearanceModuleFactory
    {
        public AppearanceModule Create(EntityVisualModel visualModel)
        {
            var appearanceModel = new AppearanceModule(visualModel);
            return appearanceModel;
        }
    }
}
