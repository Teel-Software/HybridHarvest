/// <summary>
/// Интерфейс для скриптов объектов, Update которых нужно вызвать даже, когда объект неактивен.
/// </summary>
public interface IUpdateable
{
    void Update();
}
