
public interface ISaveable
{
    string UniqueID { get; set; }

    GameObjectSave GameObjectSave { get; set; }

    void Register();
    void Deregister();
    void StoreScene(string sceneName);
    void RestoreScene(string sceneName);
}
