// using WDBase;
// using WDCore;
// using WDCore.Models;
// using WDCore.Operations;
//
// namespace WDCoreTests;
//
// [TestFixture]
// public class JobCollectionTests
// {
// 	private JobCollection _jobCollection;
//
// 	private DeployJob CreateDeployJob(string name, string shortName, short id) =>
// 		new(
// 			name,
// 			shortName,
// 			id,
// 			"DefaultSourceDirectory",
// 			"DefaultCustomWorkingDirectory",
// 			Array.Empty<BaseOperation>(),
// 			Array.Empty<BaseOperation>()
// 		);
//
// 	[SetUp]
// 	public void SetUp()
// 	{
// 		_jobCollection = new JobCollection();
// 	}
//
// 	[Test]
// 	public void Test_AddJob_Success()
// 	{
// 		var job = CreateDeployJob("Job1", "J1", 1);
// 		Assert.That(_jobCollection.Add(job), Is.True);
// 		Assert.That(_jobCollection.Get("Job1"), Is.EqualTo(job));
// 		Assert.That(_jobCollection.Get("J1"), Is.EqualTo(job));
// 		Assert.That(_jobCollection.Get(1), Is.EqualTo(job));
// 	}
//
// 	[Test]
// 	public void Test_AddJob_DuplicateName_Failure()
// 	{
// 		var job1 = CreateDeployJob("Job1", "J1", 1);
// 		var job2 = CreateDeployJob("Job1", "J2", 2);
// 		Assert.IsTrue(_jobCollection.Add(job1));
// 		Assert.IsFalse(_jobCollection.Add(job2));
// 	}
//
// 	[Test]
// 	public void Test_AddJob_DuplicateShortName_Failure()
// 	{
// 		var job1 = CreateDeployJob("Job1", "J1", 1);
// 		var job2 = CreateDeployJob("Job2", "J1", 2);
// 		Assert.IsTrue(_jobCollection.Add(job1));
// 		Assert.IsFalse(_jobCollection.Add(job2));
// 	}
//
// 	[Test]
// 	public void Test_AddJob_DuplicateId_Failure()
// 	{
// 		var job1 = CreateDeployJob("Job1", "J1", 1);
// 		var job2 = CreateDeployJob("Job2", "J2", 1);
// 		Assert.IsTrue(_jobCollection.Add(job1));
// 		Assert.IsFalse(_jobCollection.Add(job2));
// 	}
//
// 	[Test]
// 	public void Test_RemoveJob()
// 	{
// 		var job = CreateDeployJob("Job1", "J1", 1);
// 		Assert.IsTrue(_jobCollection.Add(job));
// 		_jobCollection.Remove(job);
//
// 		Assert.IsNull(_jobCollection.Get("Job1"));
// 		Assert.IsNull(_jobCollection.Get("J1"));
// 		Assert.IsNull(_jobCollection.Get(1));
// 	}
//
// 	[Test]
// 	public void Test_ClearJobs()
// 	{
// 		var job1 = CreateDeployJob("Job1", "J1", 1);
// 		var job2 = CreateDeployJob("Job2", "J2", 2);
// 		_jobCollection.Add(job1);
// 		_jobCollection.Add(job2);
// 		_jobCollection.Clear();
//
// 		Assert.IsNull(_jobCollection.Get("Job1"));
// 		Assert.IsNull(_jobCollection.Get("J1"));
// 		Assert.IsNull(_jobCollection.Get(1));
// 		Assert.IsNull(_jobCollection.Get("Job2"));
// 		Assert.IsNull(_jobCollection.Get("J2"));
// 		Assert.IsNull(_jobCollection.Get(2));
// 	}
//
// 	[Test]
// 	public void Test_GetNonExistentJob_ReturnsNull()
// 	{
// 		Assert.IsNull(_jobCollection.Get("NonExistentJob"));
// 		Assert.IsNull(_jobCollection.Get("NonExistentShortName"));
// 		Assert.IsNull(_jobCollection.Get(999));
// 	}
// }

