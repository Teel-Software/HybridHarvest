/// <summary>
/// Интерфейс для скриптов, содержащих данные, которые стоит сохранять квиксейвом
/// </summary>
internal interface ISaveable
{
    void Save();
    void Load();
}
