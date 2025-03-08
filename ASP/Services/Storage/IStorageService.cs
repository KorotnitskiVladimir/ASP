namespace ASP.Services.Storage;

public interface IStorageService
{
    string SaveFile(IFormFile formFile);
    string GetRealPath(string name);
}

/* Служба сохранения файлов
Особенность работы с файлами в том, что их желательно выводить за «пространство»  сайта. После обновления сайта (redeploy) 
возможны ситуации с утратой наработанных сайтом файлов
*/