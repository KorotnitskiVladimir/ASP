namespace ASP.Services.Storage;

public class FileStorageService : IStorageService
{
    private const string storagePath = "C:\\storage\\";

    public string GetRealPath(string name)
    {
        return storagePath + name;
    }

    public string SaveFile(IFormFile formFile)
    {
        // 1. Из имени файла определить расширение
        // 2. Сгенерировать новое имя сохранив расширение, убедиться в его уникальности
        // 3. Скопировать formFile в хранилище под новым именем
        var ext = Path.GetExtension(formFile.FileName);
        string savedName;
        string fullName;
        do
        {
            savedName = Guid.NewGuid() + ext;
            fullName = storagePath + savedName;
        } while (File.Exists(fullName));
        formFile.CopyTo(new FileStream(fullName, FileMode.CreateNew));

        return savedName;
    }
}