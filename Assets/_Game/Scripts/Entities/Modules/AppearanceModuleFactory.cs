namespace Assets._Game.Scripts.Entities.Modules
{
    public sealed class AppearanceModuleFactory
    {
        public AppearanceModule Create()
        {
            var appearanceModel = new AppearanceModule();
            return appearanceModel;
        }
    }
}
