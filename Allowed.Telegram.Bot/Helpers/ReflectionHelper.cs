namespace Allowed.Telegram.Bot.Helpers
{
    public static class ReflectionHelper
    {
        public static object GetProperty(this object entity, string propertyName)
        {
            return entity.GetType().GetProperty(propertyName).GetValue(entity);
        }

        public static void SetProperty(this object entity, string propertyName, object propertyValue)
        {
            entity.GetType().GetProperty(propertyName).SetValue(propertyName, propertyValue);
        }
    }
}
