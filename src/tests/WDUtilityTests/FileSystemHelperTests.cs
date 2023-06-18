using WDUtility;

namespace WDUtilityTests;

[TestFixture]
public class FileSystemHelperTests
{
    [SetUp]
    public void SetUp()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "WDUtilityTests");
        _testFile = Path.Combine(_testDirectory, "testFile.txt");

        Directory.CreateDirectory(_testDirectory);
        File.WriteAllText(_testFile, "This is a test file.");
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testDirectory)) Directory.Delete(_testDirectory, true);
    }

    private string _testDirectory;
    private string _testFile;

    [Test]
    public void Copy_File_Test()
    {
        var destinationFile = Path.Combine(_testDirectory, "copiedFile.txt");
        FileSystemHelper.Copy(_testFile, destinationFile, false);

        Assert.That(File.Exists(destinationFile), Is.True);
        Assert.That(File.ReadAllText(destinationFile), Is.EqualTo(File.ReadAllText(_testFile)));
    }

    [Test]
    public void Copy_Directory_Test()
    {
        var destinationDirectory = Path.Combine(Path.GetTempPath(), "WDUtilityTests_Copied");
        FileSystemHelper.Copy(_testDirectory, destinationDirectory, true);

        Assert.That(Directory.Exists(destinationDirectory), Is.True);
        Assert.That(File.Exists(Path.Combine(destinationDirectory, "testFile.txt")), Is.True);

        Directory.Delete(destinationDirectory, true);
    }

    [Test]
    public void Delete_File_Test()
    {
        FileSystemHelper.Delete(_testFile);
        Assert.IsFalse(File.Exists(_testFile));
    }

    [Test]
    public void Delete_Directory_Test()
    {
        FileSystemHelper.Delete(_testDirectory);
        Assert.IsFalse(Directory.Exists(_testDirectory));
    }

    [Test]
    public void IsFile_Test()
    {
        Assert.That(FileSystemHelper.IsFile(_testFile), Is.True);
        Assert.That(FileSystemHelper.IsFile(_testDirectory), Is.False);
    }

    [Test]
    public void IsDirectory_Test()
    {
        Assert.That(FileSystemHelper.IsDirectory(_testDirectory), Is.True);
        Assert.That(FileSystemHelper.IsDirectory(_testFile), Is.False);
    }

    [Test]
    public void FindFilesByName_Test()
    {
        var foundFiles = FileSystemHelper.FindFilesByName(_testDirectory, "testFile.txt");
        Assert.That(foundFiles.Count(), Is.EqualTo(1));
        Assert.That(foundFiles.First(), Is.EqualTo(_testFile));
    }

    [Test]
    public void FindDirectoriesByName_Test()
    {
        var subDirectory = Path.Combine(_testDirectory, "subDirectory");
        Directory.CreateDirectory(subDirectory);

        var foundDirectories = FileSystemHelper.FindDirectoriesByName(_testDirectory, "subDirectory");
        Assert.That(foundDirectories.Count(), Is.EqualTo(1));
        Assert.That(foundDirectories.First(), Is.EqualTo(subDirectory));
    }

    // TODO test for windows specific method. Move to separate project 
    // [Test]
    // public void CreateShortcut_Test()
    // {
    // 	var shortcutPath = Path.Combine(_testDirectory, "testShortcut.lnk");
    // 	FileSystemHelper.CreateShortcut("testShortcut", _testFile, _testDirectory);
    //
    // 	Assert.That(File.Exists(shortcutPath), Is.True);
    // 	File.Delete(shortcutPath);
    // }

    [Test]
    public void TestConcatenateFiles()
    {
        // Arrange
        var sourceDirectory = Path.Combine(_testDirectory, "Source");
        Directory.CreateDirectory(sourceDirectory);

        string[] fileContents = { "File1 content", "File2 content", "File3 content" };
        for (var i = 0; i < fileContents.Length; i++)
            File.WriteAllText(Path.Combine(sourceDirectory, $"file{i + 1}.txt"), fileContents[i]);

        var destinationFile = Path.Combine(_testDirectory, "Concatenated.txt");

        // Act
        FileSystemHelper.ConcatenateFiles(sourceDirectory, "txt", destinationFile);

        // Assert
        Assert.That(File.Exists(destinationFile), Is.True);
        var concatenatedContent = File.ReadAllText(destinationFile);
        Assert.That(concatenatedContent, Is.EqualTo(string.Concat(fileContents)));
    }
}