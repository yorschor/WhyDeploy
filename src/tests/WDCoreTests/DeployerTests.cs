// using Newtonsoft.Json.Linq;
// using NLog;
// using WDBase;
// using WDCore;
// using WDCore.Models;
// using WDCore.Operations.FileSystem;
// using WDCore.Serialisation;
//
// namespace WDCoreTests;
//
// public class DeployerTests
// {
//     private const string TempTestFolderName = "WDCoreTests";
//     private const string TestJobName = "TestJob1";
//     private const string TestJobShortName = "TJ1";
//     private const string TestFileName = "testFile1.txt";
//     private const string TestDirName = "testFolder1";
//     private string _appConfigPath;
//     private string _cacheFolderPath;
//     private Deployer _deployer;
//     private DeployJob _deployJob;
//     private string _targetDirectoryPath;
//     private string _tempTestDirectory;
//     private string _testJobDirPath;
//
//
//     [SetUp]
//     public void Setup()
//     {
//         // Create a DeployJob object
//         _deployJob = new DeployJob(
//             TestJobName,
//             TestJobShortName,
//             1,
//             "source",
//             "",
//             new BaseOperation[]
//             {
//                 new CopyOperation { From = TestFileName, To = TestFileName },
//                 new CopyOperation { From = TestDirName, To = TestDirName }
//             },
//             new BaseOperation[]
//             {
//                 new DeleteOperation { Target = TestFileName },
//                 new DeleteOperation { Target = TestDirName }
//             }
//         );
//
//         // Create a temporary directory for the test
//         _tempTestDirectory = Path.Combine(Path.GetTempPath(), TempTestFolderName);
//         Directory.CreateDirectory(_tempTestDirectory);
//
//         // Set up the cache folder and target directory paths
//         _cacheFolderPath = Path.Combine(_tempTestDirectory, "deployCache");
//         _targetDirectoryPath = Path.Combine(_tempTestDirectory, "deploymentTargetFolder");
//         _testJobDirPath = Path.Combine(_cacheFolderPath, TestJobName);
//         _appConfigPath = Path.Combine(_tempTestDirectory, "DeployAppConfig.json");
//
//         Directory.CreateDirectory(_cacheFolderPath);
//         Directory.CreateDirectory(_targetDirectoryPath);
//         Directory.CreateDirectory(_testJobDirPath);
//
//         // Create a test DeployAppConfig.json file
//         var appConfig = DeployAppConfig.CreateNew(_targetDirectoryPath, "NewApp", LogManager.GetCurrentClassLogger());
//
//         var appConfigJson = JObject.FromObject(appConfig).ToString();
//         File.WriteAllText(_appConfigPath, appConfigJson);
//
//         // Initialize the Deployer class with the test DeployAppConfig.json file
//         _deployer = new Deployer(_appConfigPath);
//
//         // Serialize and write the DeployJob object to a file
//         var deployJobPath = Path.Combine(_testJobDirPath, "deploy.json");
//
//         var deployJobJson = WDSerializer.Serialize(_deployJob);
//         File.WriteAllText(deployJobPath, deployJobJson);
//
//         // Create test files in the deploy directory
//         Directory.CreateDirectory(Path.Combine(_testJobDirPath, "source", TestDirName));
//         File.WriteAllText(Path.Combine(_testJobDirPath, "source", TestFileName), "Test file content");
//         File.WriteAllText(Path.Combine(_testJobDirPath, "source", TestDirName, "testFile2.txt"), "Test file content");
//     }
//
//
//     [TearDown]
//     public void TearDown()
//     {
//         Directory.Delete(_tempTestDirectory, true);
//         // Directory.Delete(_cacheFolderPath, true);
//         // Directory.Delete(_targetDirectoryPath, true);
//         // File.Delete("DeployAppConfig.json");
//     }
//
//     [Test]
//     public void TestFindAllJobs()
//     {
//         // Act
//         _deployer.FindAllJobs();
//
//         // Assert
//         // Assert that the TestJob was found by the Deployer
//         Assert.That(_deployer.Jobs.ContainsJob(_deployJob.Name), Is.True);
//     }
//
//     [Test]
//     public void TestDeploy()
//     {
//         _deployer.FindAllJobs();
//         // Deploy the job and assert that it was successful
//         var result = _deployer.Deploy(TestJobName);
//         Assert.That(result, Is.True);
//
//         // Check that the file was copied to the target directory
//         var targetFilePath = Path.Combine(_targetDirectoryPath, TestJobName, TestFileName);
//         Assert.That(File.Exists(targetFilePath), Is.True);
//
//         // Purge the job and assert that it was successful
//         result = _deployer.Purge(_deployJob.Name);
//         Assert.That(result, Is.True);
//
//         // Check that the copied file was deleted from the target directory
//         Assert.That(File.Exists(targetFilePath), Is.False);
//     }
//
//     [Test]
//     public void TestPurge()
//     {
//         // Arrange
//         _deployer.FindAllJobs();
//         // Perform the Deploy operation first
//         _deployer.Deploy(_deployJob.Name);
//
//         // Act
//         var purgeResult = _deployer.Purge(_deployJob.Name);
//
//         // Assert
//         Assert.That(purgeResult, Is.True);
//         Assert.That(File.Exists(Path.Combine(_targetDirectoryPath, TestDirName)), Is.False);
//     }
// }

