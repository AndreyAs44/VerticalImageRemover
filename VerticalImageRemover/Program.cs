using System.Drawing;

while (true)
{
	Console.WriteLine("Введите путь к папке с изображениями:");
	var directoryPath = Console.ReadLine();

	if (!Directory.Exists(directoryPath))
	{
		Console.WriteLine("Указанная папка не существует.");
		continue;
	}

	int minHeight;
	while (true)
	{
		Console.WriteLine("Введите минимальную высоту изображения в пикселях:");
		if (int.TryParse(Console.ReadLine(), out minHeight))
		{
			break;
		}
		Console.WriteLine("Вы ввели некорректное значение высоты. Пожалуйста, попробуйте снова.");
	}

	Console.WriteLine("Введите формат файлов изображений (например, jpg, png):");
	var imageFormat = Console.ReadLine();
	var searchPattern = $"*.{imageFormat}";

	try
	{
		var directoryInfo = new DirectoryInfo(directoryPath);
		var files = directoryInfo.GetFiles(searchPattern); // Получаем все файлы указанного формата в папке

		foreach (var file in files)
		{
			using var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
			using var image = Image.FromFile(file.FullName);
			if (image.Height < minHeight)
			{
				Console.WriteLine($"Удаление файла: {file.Name}, высота: {image.Height}px");
				stream.Close(); // Закрываем поток перед удалением файла
				image.Dispose();
				TryToDeleteFile(file);
			}
		}
		Console.WriteLine("Обработка файлов завершена.");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Произошла ошибка: {ex.Message}");
	}

	Console.WriteLine("Нажмите Enter для новой операции или введите 'exit' для выхода.");
	if (Console.ReadLine()!.Equals("exit", StringComparison.OrdinalIgnoreCase))
	{
		break;
	}
}

static void TryToDeleteFile(FileInfo file)
{
	int attempts = 0;
	while (attempts < 3)
	{
		try
		{
			file.Delete();
			Console.WriteLine($"Файл удален: {file.Name}");
			break;
		}
		catch (IOException)
		{
			attempts++;
			Console.WriteLine($"Не удалось удалить файл {file.Name}. Попытка {attempts}...");
			Thread.Sleep(1000); // Задержка перед следующей попыткой
		}
	}
}